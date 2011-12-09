module Entities
open Measures
open Vector2D
open Casanova

type Viper = 
 {
  Position : Variable<Vector2D<m>>
  Speed : Variable<Vector2D<m/s>>
  Roll : Variable<float32<rad>>
  RollSpeed : Variable<float32<rad/s>>
  CannonTemperature : Variable<float32<f>>
  
  // Missiles : Variable<int<1>>
  
  MaxSpeed : float32<m/s>
  MaxRollSpeed : float32<rad/s>
  MaxRoll :  float32<rad>

  CannonShootInterval : float32<s>
  CannonShootHeating : float32<f>
  CannonCooldownRate : float32<f/s>
  CannonMaxTemperature : float32<f>
 }

and Projectile = 
 {
  Position : Variable<Vector2D<m>>
  Speed : Vector2D<m/s>
  // Colliders : Variable<List<Raider>>
 }

and Cylon =
 {
  Position : Variable<Vector2D<m>>
  Speed : Variable<Vector2D<m/s>>
 }