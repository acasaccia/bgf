module GameState

open System
open Measures
open Vector2D
open Casanova
open Entities
open Coroutines
open BattlestarGalacticaFightersInput

type GameState =
 {
  viper : Viper
  projectiles : Variable<List<Projectile>>
  cylons : Variable<List<Cylon>>
  borders : Vector2D<m>
 }

let state : GameState =
 {
  viper = 
    { 
     Position = Variable( fun () -> { X = 0.0f<m>; Y = 0.0f<m> } )
     Speed = Variable( fun () -> { X = 0.0f<m/s>; Y = 0.0f<m/s> } )
     Roll = Variable( fun () -> 0.0f<rad> )
     RollSpeed = Variable( fun () -> 0.0f<rad/s> )
     CannonTemperature = Variable( fun () -> 0.0f<f> )

     //Missiles = Variable( fun () -> 4 )

     MaxSpeed = 0.3f<m/s>
     MaxRollSpeed = 1.0f<rad/s>
     MaxRoll = 0.5f<rad>

     CannonShootInterval = 0.1f<s>
     CannonShootHeating = 10.0f<f>
     CannonCooldownRate = 50.0f<f/s>
     CannonMaxTemperature = 100.0f<f>
    }
  projectiles = 
   Variable(fun () -> [])
  cylons = 
   Variable(fun () -> [])
  borders = 
   { X = 0.8f<m>; Y = 0.45f<m> }
 }

let rec update_state(dt:float32<s>) = 

 update_viper state.viper dt

 for p in !state.projectiles do
  update_projectile p dt
 state.projectiles := 
  [for p in !state.projectiles do
    if (!p.Position).Y < state.borders.Y then
     yield p]

 for c in !state.cylons do
  update_cylon c dt
 state.cylons := 
  [for c in !state.cylons do
    if (!c.Position).Y > -state.borders.Y then
     yield c]

and private update_viper (viper:Viper) (dt:float32<s>) = 
 viper.Position := !viper.Position + !viper.Speed * dt
 viper.Roll := !viper.Roll + !viper.RollSpeed * dt

and private update_projectile (p:Projectile) (dt:float32<s>) =
  p.Position := !p.Position + p.Speed * dt

and private update_cylon (c:Cylon) (dt:float32<s>) =
  c.Position := !c.Position + !c.Speed * dt

let private (!) = immediate_read
let private (:=) = immediate_write

let private main =

  // Generate a projectile if trigger is pulled,
  // then wait the right amount before we can shoot again.
  // When shooting cannon heats up, otherwise it will cooldown.
  // If max temperature is reached we can't shoot
  let rec shoot_projectiles() = 
   co{
    do! yield_
    if InputState.FireCannon && !state.viper.CannonTemperature < state.viper.CannonMaxTemperature then
     state.projectiles :=
      { 
       Position = Variable( fun () -> !state.viper.Position )
       Speed = { X = 0.0f<m/s>; Y = 3.0f<m/s> }
      } :: !state.projectiles
      state.viper.CannonTemperature := !state.viper.CannonTemperature + state.viper.CannonShootHeating
    elif !state.viper.CannonTemperature > 0.0f<f> then
     state.viper.CannonTemperature := !state.viper.CannonTemperature - state.viper.CannonCooldownRate * state.viper.CannonShootInterval
    do! wait ( (float) state.viper.CannonShootInterval )
    return! shoot_projectiles() 
   }

  let rec update_input() =
   co {
    // some shortcuts
    let speed = state.viper.Speed
    let maxSpeed = state.viper.MaxSpeed
    let rollSpeed = state.viper.RollSpeed
    let maxRollSpeed = state.viper.MaxRollSpeed
    let maxRoll = state.viper.MaxRoll
    let currentPosition = !state.viper.Position
    let currentRoll = !state.viper.Roll

    speed := { X = 0.0f<m/s>; Y = 0.0f<m/s> }
    rollSpeed := 0.0f<rad/s>

    if InputState.MoveLeft && currentPosition.X < state.borders.X then
     speed := !speed + { X = maxSpeed; Y = 0.0f<m/s> }
    if InputState.MoveRight && currentPosition.X > -state.borders.X then
     speed := !speed - { X = maxSpeed; Y = 0.0f<m/s> }
    if InputState.MoveUp && currentPosition.Y < state.borders.Y then
     speed := !speed + { X = 0.0f<m/s>; Y = maxSpeed }
    if InputState.MoveDown && currentPosition.Y > -state.borders.Y then
     speed := !speed - { X = 0.0f<m/s>; Y = maxSpeed }

    if InputState.MoveLeft && currentRoll < maxRoll then
     rollSpeed := !rollSpeed + maxRollSpeed
    if InputState.MoveRight && currentRoll > -maxRoll then 
     rollSpeed := !rollSpeed - maxRollSpeed
    
    if InputState.MoveRight = InputState.MoveLeft then
     if currentRoll > 0.0f<rad> then
      rollSpeed := !rollSpeed - maxRollSpeed
     else
       rollSpeed := !rollSpeed + maxRollSpeed

    do! yield_
    return! update_input()
   }

  let rec spawn_cylons() = 
   let random = System.Random()
   co{
    do! wait (System.Random().Next(5,10) |> float)
    state.cylons :=
     { 
      Position = Variable( fun() -> { X = 0.2f<m>; Y = state.borders.Y } )
      Speed = Variable( fun() -> { X = 0.0f<m/s>; Y = -0.1f<m/s> } )
     } :: !state.cylons
    return! spawn_cylons () 
   }

  in (spawn_cylons() .||> update_input() .||> shoot_projectiles()) |> ref

let update_script() = main.Value <- update_ai main.Value
