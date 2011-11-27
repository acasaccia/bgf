module GameState

open Measures
open Vector2D
open Entities
open Casanova
open BattlestarGalacticaFightersInput

type GameState =
 {
  player : Variable<Viper>
 }

let state : GameState =
 {
  player = 
    Variable( fun () -> { Position = Variable( fun () -> { X = 0.0<m>; Y = 0.0<m> } ) } )
 }

// Those should be used in scripts
// let private (!) = immediate_read
// let private (:=) = immediate_write

let rec update_state(dt:float32) = 
 update_player !state.player dt
 state.player := !state.player
 
and private update_player (p:Viper) (dt:float32) = 
 p.Position := !p.Position +
  if InputState.MoveLeft then
   { X = -0.001<m>; Y = 0.0<m> }
  elif InputState.MoveRight then
   { X = 0.001<m>; Y = 0.0<m> }
  else
   { X = 0.0<m>; Y = 0.0<m> }