module Entities
open Measures
open Vector2D
open Casanova

type Viper = 
 {
  Position : Variable<Vector2D<m>>
  Yaw : Variable<float32<rad>>
  Speed : float32<m/s>
 }
