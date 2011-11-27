module GameState

open System
open Measures
open Vector2D
open Entities
open Casanova
open BattlestarGalacticaFightersInput

type GameState =
 {
  viper : Variable<Viper>
 }

let state : GameState =
 {
  viper = 
    Variable( fun () -> 
     { 
      Position = Variable( fun () -> { X = 0.0f<m>; Y = 0.0f<m> } )
      Yaw = Variable( fun () -> 0.0f<rad> )
      Speed = 0.3f<m/s>
     }
    )
 }

let rec update_state(dt:float32<s>) = 
 update_viper !state.viper dt
 state.viper := !state.viper
 
and private update_viper (p:Viper) (dt:float32<s>) = 
 p.Position := !p.Position +
  if InputState.MoveLeft then
   { X = p.Speed * dt; Y = 0.0f<m> }
  elif InputState.MoveRight then
   { X = -p.Speed * dt; Y = 0.0f<m> }
  elif InputState.MoveUp then
   { X = 0.0f<m>; Y = p.Speed * dt }
  elif InputState.MoveDown then
   { X = 0.0f<m>; Y = -p.Speed * dt }
  else
   { X = 0.0f<m>; Y = 0.0f<m> }
 
 p.Yaw := 
   if InputState.MoveLeft then
    0.5f<rad>
   elif InputState.MoveRight then
    -0.5f<rad>
   else
     0.0f<rad>

// Those should be used in scripts
// let private (!) = immediate_read
// let private (:=) = immediate_write