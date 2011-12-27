module Entities
open Measures
open Utilities
open Casanova
open Coroutines

type GameState =
  {
    viper : Viper
    projectiles : Variable<List<Projectile>>
    cylons : Variable<List<Cylon>>
    explosions : Variable<List<Explosion>>
    cylonsFragged : Variable<int>
    elapsedTime : Variable<float>
    gameOver : Variable<bool>
  }

and Viper = 
  {
    Position : Variable<Vector2D<m>>
    Speed : Variable<Vector2D<m/s>>
    Roll : Variable<float32<rad>>
    CannonTemperature : Variable<float32<f>>
    IsShooting : Variable<bool>
    LastCollisionTime : Variable<float>
    OverHeated : Variable<bool>
    Shields : Variable<int>
    Colliders : Variable<List<Cylon>>
  }

and Projectile = 
  {
    Position : Variable<Vector2D<m>>
    Speed : Vector2D<m/s>
    HasColliders : Variable<bool>
    Owner : Factions
  }

and Cylon =
  {
    Position : Variable<Vector2D<m>>
    Speed : Variable<Vector2D<m/s>>
    Roll : Variable<float32<rad>>
    Colliders : Variable<List<Projectile>>
    Shields : Variable<int>
    IsShooting : Variable<bool>
    AI : Variable<Coroutine<Unit>>
  }

and Explosion =
  {
    Position : Variable<Vector2D<m>>
    Speed : Vector2D<m/s>
    Time : Variable<float32<s>>
  }

