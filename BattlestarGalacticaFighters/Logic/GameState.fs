module GameState

open System
open Measures
open Utilities
open Casanova
open Entities
open Coroutines
open Constants
open Shared

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
     Colliders = Variable( fun () -> [] )
    }
  projectiles = Variable(fun () -> [])
  cylons = Variable(fun () -> [])
  explosions = Variable(fun () -> [])
  galacticaShields = Variable(fun () -> galacticashields)
  gameTime = Variable(fun () -> gameDuration)
  escapedCylons = Variable( fun () -> [] )
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

 // enemies who reached bottom of the screen will damage galactica
 state.escapedCylons :=
 [for c in !state.cylons do
  if (!c.Position).Y < -entitiesRemovalClamp.Y then
   yield c]

 state.galacticaShields := !state.galacticaShields - (!state.escapedCylons).Length * cylonDamage

 state.cylons :=
  [for c in !state.cylons do
    if (!c.Position).Y > -entitiesRemovalClamp.Y && !c.Shields > 0 then
     yield c]

 state.projectiles := 
  [for p in !state.projectiles do
    if (!p.Position).Y < entitiesRemovalClamp.Y && (!p.Colliders).Length < 1 then
     yield p]

 state.explosions := 
  List.concat [
   // remove old explosions
   [for e in !state.explosions do
    if !e.Time > 0.0f<s> then
     yield e];
   // generate new ones
   [for c in !state.cylons do
     if !c.Shields < 1 then
      yield {
        Position = Variable( fun () -> !c.Position )
        Speed = !c.Speed / 2.0f
        Time = Variable( fun () -> explosionDuration )
       }]
  ]

and private update_viper (v:Viper) (dt:float32<s>) = 
 v.Position := !v.Position + !v.Speed * dt
 v.Roll := !v.Roll + !v.RollSpeed * dt
 if (not InputState.FireCannon && !v.CannonTemperature > 0.0f<f>) then
  v.CannonTemperature := !v.CannonTemperature - cannonCooldownRate * dt
 else
  v.CannonTemperature := !v.CannonTemperature
 v.Colliders := [for c in !state.cylons do
                  if Vector2D.Distance(!c.Position, !v.Position) < cylonBoundingRadius
                     && !c.Shields = 0 // Cylon has no more shields and will be removed next frame
                                       // this way we ensure we dont decrement shields more than once
                                       // for each cylon
                     then yield c]
 v.Shields := !v.Shields - (!v.Colliders).Length
 v.OverHeated := false // These two flags are used for audio events, they will
 v.IsShooting := false // eventually be raised for a single frame by coroutines

and private update_cylon (c:Cylon) (dt:float32<s>) =
 c.Position := !c.Position + !c.Speed * dt
 if Vector2D.Distance(!c.Position, !state.viper.Position) < viperBoundingRadius then
  c.Shields := 0 // Kamikazeeee
 else
  c.Shields := !c.Shields - (!c.Colliders).Length
 c.Colliders := [for p in !state.projectiles do
                  if Vector2D.Distance(!c.Position, !p.Position) < cylonBoundingRadius
                     && (!p.Colliders).Length > 0 // Projectile has some colliders and will be removed next frame
                                                  // this way we ensure we dont decrement shields more than once
                                                  // for each projectile
                     then yield p]

and private update_projectile (p:Projectile) (dt:float32<s>) =
 p.Position := !p.Position + p.Speed * dt
 p.Colliders := [for c in !state.cylons do
                    if Vector2D.Distance(!c.Position, !p.Position) < cylonBoundingRadius then
                     yield c]

and private update_explosion (e:Explosion) (dt:float32<s>) =
 e.Time := !e.Time - dt
 e.Position := !e.Position + e.Speed * dt

//
// Coroutines
//

let private (!) = immediate_read
let private (:=) = immediate_write

let private main =

  let process_input() =
   co {
    do! yield_
    // Some shortcuts
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
     if currentRoll > 0.1f<rad> then
      rollSpeed := !rollSpeed - maxRollSpeed
     else if currentRoll < -0.1f<rad> then
       rollSpeed := !rollSpeed + maxRollSpeed
     else
      state.viper.Roll := 0.0f<rad>
      rollSpeed := 0.0f<rad/s>

   } |> repeat_

  let shoot_projectiles() = 
   co {
    do! yield_
    // Trigger is pulled
    if InputState.FireCannon then
     // Cannon is not too hot
     if !state.viper.CannonTemperature < cannonMaxTemperature then
      // Ok to frag some toasters
      state.projectiles :=
       {
        Position = Variable( fun () -> !state.viper.Position )
        Speed = viperProjectilesSpeed
        Colliders = Variable(fun () -> [])
       } :: !state.projectiles
       // Update temperature
       state.viper.CannonTemperature := !state.viper.CannonTemperature + cannonShootHeating
       // Raise flag to play shooting sound
       state.viper.IsShooting := true
       if ( !state.viper.CannonTemperature > cannonMaxTemperature ) then
        // Cannon got too hot: temperature to max value and play annoying sound
        state.viper.CannonTemperature := cannonMaxTemperature
        state.viper.OverHeated := true
       do! wait ( (float) cannonShootingTime )
     else
      // Cannon too hot, let's wait
      do! wait ( (float) cannonCooldownTime )
   } |> repeat_
  
  let aimAndShoot (self:Cylon) (target:Viper) =
   co {
    do! yield_
    let direction = !target.Position - !self.Position
    state.projectiles :=
    {
     Position = Variable( fun () -> !self.Position )
     Speed = convertVectorToVectorMS ( Vector2D.Normalize(direction) ) / 2.0f
     Colliders = Variable(fun () -> [])
    } :: !state.projectiles
    do! wait 1.0
    return ()
   }

  let cylon_ai (self:Cylon) =
   co {
    do! yield_
    let random = System.Random()
    do! wait (System.Random().Next(0,1) |> float)
    //do! aimAndShoot (self) (state.viper)
    let speedChange = System.Random().Next(-5,5)
    let speed = convertFloat32ToMS ( (float32) speedChange / 10.0f )
    self.Speed := { X = 0.0f<m/s>; Y = 0.0f<m/s> }
   } |> repeat_

  let spawn_cylons () = 
   let random = System.Random()
   co{
    do! wait (System.Random().Next(3,10) |> float)
    let enterPosition = System.Random().Next(-4,4)
    let pos = convertFloat32ToM ( (float32) enterPosition / 10.0f )
    let newborn = { 
     Position = Variable( fun() -> { X = pos; Y = entitiesRemovalClamp.Y } )
     Speed = Variable( fun() -> { X = 0.0f<m/s>; Y = -cylonSpeed } )
     Yaw = Variable(fun() -> pi )
     Colliders = Variable(fun () -> [])
     Shields = Variable(fun() -> cylonShields)
     AI = Variable(fun() -> co{ return () })
    }
    newborn.AI := cylon_ai(newborn)
    state.cylons := newborn
      :: !state.cylons
   } |> repeat_

  in (spawn_cylons() .||> process_input() .||> shoot_projectiles()) |> ref

let update_script () =
 main.Value <- update_ai main.Value
 for c in !state.cylons
  do c.AI := update_ai !c.AI