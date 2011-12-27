module GameState

open System
open Measures
open Utilities
open Casanova
open Entities
open Coroutines
open Constants
open Shared


//
//  INITIALIZE GAME STATE
//

let state : GameState =
  {
    viper = 
      { 
        Position = Variable( fun () -> { X = 0.0f<m>; Y = 0.0f<m> } )
        Speed = Variable( fun () -> { X = 0.0f<m/s>; Y = 0.0f<m/s> } )
        Roll = Variable( fun () -> 0.0f<rad> )
        CannonTemperature = Variable( fun () -> 0.0f<f> )
        IsShooting = Variable( fun () -> false )
        OverHeated = Variable( fun () -> false )
        Shields = Variable( fun () -> vipershields )
        Colliders = Variable( fun () -> [] )
        LastCollisionTime = Variable( fun () -> -10.0 )
      }
    projectiles = Variable(fun () -> [])
    cylons = Variable(fun () -> [])
    explosions = Variable(fun () -> [])
    cylonsFragged = Variable(fun () -> 0)
    elapsedTime = Variable(fun () -> 0.0)
    gameOver = Variable( fun () -> false )
  }


//
//  UPDATE GAME STATE
//

let rec update_state(dt:float32<s>) = 
  update_viper state.viper dt
  for c in !state.cylons do update_cylon c dt
  for p in !state.projectiles do update_projectile p dt
  for e in !state.explosions do update_explosion e dt

  // remove enemy ships which have been destroyed or went out of screen
  state.cylons := [for c in !state.cylons do
                     if (!c.Position).Y > -entitiesRemovalClamp.Y
                       && !c.Shields > 0
                     then yield c]

  // remove projectiles that hit something or went out of screen
  state.projectiles := [for p in !state.projectiles do
                          if (!p.Position).Y < entitiesRemovalClamp.Y
                            && (!p.Position).Y > -entitiesRemovalClamp.Y
                            && not !p.HasColliders
                          then yield p]

  // generate new explosions for each cylon
  let cylonEsplosions = [for c in !state.cylons do
                         if !c.Shields < 1
                         then yield {
                           Position = Variable( fun () -> !c.Position )
                           Speed = !c.Speed / 2.0f
                           Time = Variable( fun () -> explosionDuration )
                         }]

  // generate explosions for viper -> Game Over
  let viperExplosion = if !state.viper.Shields < 1 && not !state.gameOver
                       then [{
                               Position = Variable( fun () -> !state.viper.Position )
                               Speed = !state.viper.Speed / 2.0f
                               Time = Variable( fun () -> explosionDuration )
                             }]
                       else []

  // remove old explosions, add new ones
  state.explosions := List.concat [
                       [for e in !state.explosions do
                          if !e.Time > 0.0f<s>
                          then yield e];
                        cylonEsplosions;
                        viperExplosion
                       ]

  state.cylonsFragged := !state.cylonsFragged + (cylonEsplosions).Length
  state.elapsedTime := if not !state.gameOver then !state.elapsedTime + (float)dt else !state.elapsedTime
  state.gameOver := if !state.viper.Shields < 1 then true else false


//
// UPDATE PLAYER CONTROLLED SHIP
//

and private update_viper (v:Viper) (dt:float32<s>) = 
  v.Position := !v.Position + !v.Speed * dt
  v.Roll := if (!v.Speed).X > 0.0f<m/s> && !v.Roll < cylonMaxRoll then !v.Roll + cylonRollSpeed * dt
            else if ( v.Speed.Value.X < 0.0f<m/s> && !v.Roll > (-cylonMaxRoll) ) then !v.Roll - cylonRollSpeed * dt
            else if ( !v.Roll < 0.0f<rad> ) then !v.Roll + cylonRollSpeed * dt
            else !v.Roll - cylonRollSpeed * dt
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
  let projectileColliders = [for p in !state.projectiles do
                               if Vector2D.Distance(!p.Position, !v.Position) < viperBoundingRadius
                                 && p.Owner = Factions.Cylons
                                 && !p.HasColliders
                               then yield p]
  v.Shields := !v.Shields - (!v.Colliders).Length - projectileColliders.Length
  v.LastCollisionTime := if (!v.Colliders).Length + projectileColliders.Length > 0
                         then !state.elapsedTime
                         else !v.LastCollisionTime
                          
  // Following flags are used for audio events, will eventually be raised for a single frame by coroutines
  v.OverHeated := false 
  v.IsShooting := false


//
// UPDATE ENEMY SHIP
//

and private update_cylon (c:Cylon) (dt:float32<s>) =
  c.Position := !c.Position + !c.Speed * dt
  c.Speed := !c.Speed
  c.Roll := if (!c.Speed).X > 0.0f<m/s> && !c.Roll < cylonMaxRoll then !c.Roll + cylonRollSpeed * dt
            else if ( c.Speed.Value.X < 0.0f<m/s> && !c.Roll > (-cylonMaxRoll) ) then !c.Roll - cylonRollSpeed * dt
            else if ( !c.Roll < 0.0f<rad> ) then !c.Roll + cylonRollSpeed * dt
            else !c.Roll - cylonRollSpeed * dt
  if Vector2D.Distance(!c.Position, !state.viper.Position) < viperBoundingRadius && not !state.gameOver then
    c.Shields := 0 // Kamikazeeee
  else
    c.Shields := !c.Shields - (!c.Colliders).Length
  c.Colliders := [for p in !state.projectiles do
                    if Vector2D.Distance(!c.Position, !p.Position) < cylonBoundingRadius
                      && !p.HasColliders // Projectile has some colliders so it will be removed next frame
                                         // this way we ensure we dont decrement shields more than once
                                         // for each projectile
                    then yield p]
  // Following flags are used for audio events, will eventually be raised for a single frame by coroutines
  c.IsShooting := false


//
// UPDATE PROJECTILE
//

and private update_projectile (p:Projectile) (dt:float32<s>) = 
  p.Position := !p.Position + p.Speed * dt
  let cylonsColliders = [for c in !state.cylons do
                           if Vector2D.Distance(!c.Position, !p.Position) < cylonBoundingRadius
                             && p.Owner = Factions.Colonies
                           then yield c]
  let viperCollides = Vector2D.Distance(!state.viper.Position, !p.Position) < viperBoundingRadius
                      && p.Owner = Factions.Cylons
  p.HasColliders := cylonsColliders.Length > 0 || viperCollides


//
// UPDATE EXPLOSION
//

and private update_explosion (e:Explosion) (dt:float32<s>) =
  e.Time := !e.Time - dt
  e.Position := !e.Position + e.Speed * dt


//
// COROUTINES SECTION
//

let private (!) = immediate_read
let private (:=) = immediate_write

let private main =

  let random = System.Random((int)!state.elapsedTime)

  //
  // PROCESS INPUT
  //

  let process_input() =
    co {
      do! yield_
      if !state.viper.Shields > 0 then
        // Some shortcuts
        let speed = state.viper.Speed
        let currentPosition = !state.viper.Position
        let currentRoll = !state.viper.Roll
        speed := { X = 0.0f<m/s>; Y = 0.0f<m/s> }
        if InputState.MoveLeft && currentPosition.X < viperPositionClamp.X then
          speed := !speed + { X = viperSpeed; Y = 0.0f<m/s> }
        if InputState.MoveRight && currentPosition.X > -viperPositionClamp.X then
          speed := !speed - { X = viperSpeed; Y = 0.0f<m/s> }
        if InputState.MoveUp && currentPosition.Y < viperPositionClamp.Y then
          speed := !speed + { X = 0.0f<m/s>; Y = viperSpeed }
        if InputState.MoveDown && currentPosition.Y > -viperPositionClamp.Y then
          speed := !speed - { X = 0.0f<m/s>; Y = viperSpeed }
    } |> repeat_


  //
  // PLAYER CONTROLLED SHIP GUN LOGIC
  //

  let update_viper_cannon() = 
    co {
      do! yield_
      if !state.viper.Shields > 0 then
        if InputState.FireCannon then
          // Trigger is pulled
          if !state.viper.CannonTemperature < cannonMaxTemperature then
            // Cannon is not too hot
            state.projectiles :=
            {
              Position = Variable( fun () -> !state.viper.Position )
              Speed = viperProjectilesSpeed
              HasColliders = Variable(fun () -> false)
              Owner = Factions.Colonies
            } :: !state.projectiles
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


  //
  // CYLON SINGLE SHOT
  //

  let shoot (self:Cylon) (target:Viper) =
    co {
      do! wait cylonBurstWait
      if not !state.gameOver then
        let direction = !target.Position - !self.Position
        state.projectiles :=
        {
          Position = Variable( fun () -> !self.Position )
          Speed = ( Vector2D.Normalize(direction) * cylonProjectilesSpeedFactor ) * ms
          HasColliders = Variable(fun () -> false)
          Owner = Factions.Cylons
        } :: !state.projectiles
        self.IsShooting := true;
      return ()
    }


  //
  // CYLON BURST SHOT
  //

  let shootBurst (self:Cylon) (target:Viper) =
    co {
      do! yield_
      do! shoot (self) (target)
      do! shoot (self) (target)
      do! shoot (self) (target)
      return ()
    }


  //
  // CYLON AI SCRIPT
  //

  let cylon_ai (self:Cylon) =
    co {
      do! yield_
      do! wait 0.5
      let targetDirection = !state.viper.Position - !self.Position
      if Vector2D<m>.Angle ({ X = 0.0f<m>; Y = -1.0f<m> }, targetDirection) < cylonShootingAngle
         && random.Next(2) = 1 // if they win coin flip they can shoot
      then do! shootBurst (self) (state.viper)
      self.Speed := match random.Next(3) with
                    | 1 -> { X = cylonSpeed / 2.0f; Y = -cylonSpeed }
                    | 2 -> { X = -cylonSpeed / 2.0f; Y = -cylonSpeed }
                    | _ -> { X = 0.0f<m/s>; Y = -cylonSpeed }
    } |> repeat_


  //
  // CYLON SPAWN SCRIPT
  //

  let spawn_cylons () = 
    co {
      do! yield_
      // With time, we increase enemies spawn rate
      let advance = !state.elapsedTime / 60.0
      // Minimum spawn wait time is 0.7s
      do! wait ( max (((random.Next(7,35) |> float) / 10.0) - advance * minimumSpawnWait) (minimumSpawnWait) )
      let pos = ((random.Next(10) - 5) |> float32) / 10.0f * m
      let newborn = {
        Position = Variable( fun() -> { X = pos; Y = entitiesRemovalClamp.Y } )
        Speed = Variable( fun() -> { X = 0.0f<m/s>; Y = -cylonSpeed * 2.0f } )
        Roll = Variable(fun() -> 0.0f<rad> )
        Colliders = Variable(fun () -> [])
        Shields = Variable(fun() -> cylonShields)
        IsShooting = Variable(fun () -> false)
        AI = Variable(fun() -> co{ return () })
      }
      newborn.AI := cylon_ai(newborn)
      state.cylons := newborn :: !state.cylons
    } |> repeat_
   
  in (spawn_cylons() .||> process_input() .||> update_viper_cannon()) |> ref


//
// SCRIPT UPDATE
//

let update_script () =
  main.Value <- update_ai main.Value
  for c in !state.cylons
    do c.AI := update_ai !c.AI

