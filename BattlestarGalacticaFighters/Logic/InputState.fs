module InputState

open Casanova
open Microsoft.Xna.Framework.Input

type InputState =
 {
  move_right : Variable<bool>
  move_left : Variable<bool>
 }

let input_state : InputState =
 {
  move_right = Variable( fun () -> false )
  move_left = Variable( fun () -> false )
 }
