module GameState

open System
open Measures
open Vector2D
open Casanova
open Entities
open Coroutines
open Constants
open BattlestarGalacticaFightersInput

let state : GameState =
 {
  viper = 
    { 
     Position = Variable( fun () -> { X = 0.0f<m>; Y = 0.0f<m> } )
     Speed = Variable( fun () -> { X = 0.0f<m/s>; Y = 0.0f<m/s> } )
     Roll = Variable( fun () -> 0.0f<rad> )
     RollSpeed = Variable( fun () -> 0.0f<rad/s> )
     CannonTemperature = Variable( fun () -> 0.0f<f> )
     IsShooting = Variable( fun () -> false )
     OverHeated = Variable( fun () -> false )
     Shields = Variable( fun () -> vipershields )
    }
  projectiles = Variable(fun () -> [])
  cylons = Variable(fun () -> [])
  explosions = Variable(fun () -> [])
  galacticaShields = Variable(fun () -> galacticashields)
  gameTime = Variable(fun () -> timetoFTLonline)
  escapedCylons = Variable( fun () -> false )
 }

let rec update_state(dt:float32<s>) = 
 
 state.gameTime := !state.gameTime - dt

 update_viper state.viper dt

 for c in !state.cylons do
  update_cylon c dt

 for p in !state.projectiles do
  update_projectile p dt

 for e in !state.explosions do
  update_explosion e dt

 let escapedCylons =
  [for c in !state.cylons do
    if (!c.Position).Y > -entitiesRemovalClamp.Y then
     yield c]
 
 if escapedCylons.Length < (!state.cylons).Length then
  state.galacticaShields := !state.galacticaShields - ( (!state.cylons).Length + escapedCylons.Length ) * 5
  state.escapedCylons := true
 else
  state.galacticaShields := !state.galacticaShields
  state.escapedCylons := false

 state.cylons :=
  [for c in !state.cylons do
     if (!c.Position).Y > -entitiesRemovalClamp.Y && !c.Shields > 0 then
      yield c]

 state.projectiles := 
  [for p in !state.projectiles do
    if (!p.Position).Y < entitiesRemovalClamp.Y && (!p.Colliders).Length < 1 then
     yield p]

 state.explosions :=
  [for e in !state.explosions do
    if !e.Time > 0.0f<s> then
     yield e]

and private update_viper (viper:Viper) (dt:float32<s>) = 
 viper.Position := !viper.Position + !viper.Speed * dt
 viper.Roll := !viper.Roll + !viper.RollSpeed * dt

 // these two flags will be eventually set by coroutines
 viper.OverHeated := false
 viper.IsShooting := false

 if (not InputState.FireCannon && !viper.CannonTemperature > 0.0f<f>) then
    viper.CannonTemperature := !viper.CannonTemperature - cannonCooldownRate * dt

and private update_cylon (c:Cylon) (dt:float32<s>) =
 c.Position := !c.Position + !c.Speed * dt
 let colliders = [for p in !state.projectiles do
                    if Vector2D.Distance(!c.Position, !p.Position) < collisionDistance then
                     yield p] 
 c.Shields := !c.Shields - colliders.Length
 if colliders.Length > 0 then
    c.Hit := true
 else
    c.Hit := false
 c.Colliders := colliders

and private update_projectile (p:Projectile) (dt:float32<s>) =
 p.Position := !p.Position + p.Speed * dt
 p.Colliders := [for c in !state.cylons do
                    if Vector2D.Distance(!c.Position, !p.Position) < collisionDistance then
                     yield c]

and private update_explosion (e:Explosion) (dt:float32<s>) =
 e.Time := !e.Time - dt
 e.Position := !e.Position + e.Speed * dt

let private (!) = immediate_read
let private (:=) = immediate_write

let private main =

  let rec update_input() =
   co {
    // some shortcuts
    let speed = state.viper.Speed
    let rollSpeed = state.viper.RollSpeed
    let currentPosition = !state.viper.Position
    let currentRoll = !state.viper.Roll

    speed := { X = 0.0f<m/s>; Y = 0.0f<m/s> }
    rollSpeed := 0.0f<rad/s>

    if InputState.MoveLeft && currentPosition.X < viperPositionClamp.X then
     speed := !speed + { X = maxSpeed; Y = 0.0f<m/s> }
    if InputState.MoveRight && currentPosition.X > -viperPositionClamp.X then
     speed := !speed - { X = maxSpeed; Y = 0.0f<m/s> }
    if InputState.MoveUp && currentPosition.Y < viperPositionClamp.Y then
     speed := !speed + { X = 0.0f<m/s>; Y = maxSpeed }
    if InputState.MoveDown && currentPosition.Y > -viperPositionClamp.Y then
     speed := !speed - { X = 0.0f<m/s>; Y = maxSpeed }

    if InputState.MoveLeft && currentRoll < maxRoll then
     rollSpeed := !rollSpeed + maxRollSpeed
    if InputState.MoveRight && currentRoll > -maxRoll then 
     rollSpeed := !rollSpeed - maxRollSpeed
    
    if InputState.MoveRight = InputState.MoveLeft then
     if currentRoll < 0.1f<rad> && currentRoll > - 0.1f<rad>  then
      rollSpeed := 0.0f<rad/s>
     else if currentRoll > 0.1f<rad> then
      rollSpeed := !rollSpeed - maxRollSpeed
     else if currentRoll < 0.1f<rad> then
       rollSpeed := !rollSpeed + maxRollSpeed

    do! yield_
    return! update_input()
   }

  // Generate a projectile if trigger is pulled, then wait the right amount before we can shoot again.
  // When shooting cannon heats up. If max temperature is reached we can't shoot until cooldown time has passed.
  let rec shoot_projectiles() = 
   co {
    do! yield_
    if InputState.FireCannon then
     // shoot a projectile
     if !state.viper.CannonTemperature < cannonMaxTemperature then
      state.projectiles :=
       {
        Position = Variable( fun () -> !state.viper.Position )
        Speed = viperProjectilesSpeed
        Colliders = Variable(fun () -> [])
       } :: !state.projectiles
       state.viper.CannonTemperature := !state.viper.CannonTemperature + cannonShootHeating
       state.viper.IsShooting := true
       if ( !state.viper.CannonTemperature > cannonMaxTemperature ) then
        state.viper.CannonTemperature := cannonMaxTemperature
        state.viper.OverHeated := true
       do! wait ( (float) cannonShootingTime )
     else
       do! wait ( (float) cannonCooldownTime )
    return! shoot_projectiles() 
   }

  let rec spawn_cylons() = 
   let random = System.Random()
   co{
    do! wait (System.Random().Next(5,10) |> float)
    let enterPosition = System.Random().Next(-2,2)
    let m : float32<m> = 1.0f<m>
    let convertToM (x : float32) = x * m
    let pos = convertToM ( (float32) enterPosition / 10.0f )
    state.cylons :=
     { 
      Position = Variable( fun() -> { X = pos; Y = entitiesRemovalClamp.Y } )
      Speed = Variable( fun() -> { X = 0.0f<m/s>; Y = -cylonSpeed } )
      Yaw = Variable(fun() -> 3.14f<rad> )
      Colliders = Variable(fun () -> [])
      Shields = Variable(fun() -> cylonShields)
      Hit = Variable(fun() -> false)
     } :: !state.cylons
    return! spawn_cylons () 
   }

//  let rec generate_explosions() = 
//   co{
//    do! yield_
//    for c in !state.cylons do
//     if c.Shields <= 0 then
//      printf "ui"
//    return! generate_explosions () 
//   }

//  in (generate_explosions() .||> spawn_cylons() .||> update_input() .||> shoot_projectiles()) |> ref

  in (spawn_cylons() .||> update_input() .||> shoot_projectiles()) |> ref

let update_script() = main.Value <- update_ai main.Value
