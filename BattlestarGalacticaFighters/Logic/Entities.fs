﻿module Entities
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
  galacticaShields : Variable<int>
  gameTime : Variable<float32<s>>
  escapedCylons : Variable<bool>
 }

and Viper = 
 {
  Position : Variable<Vector2D<m>>
  Speed : Variable<Vector2D<m/s>>
  Roll : Variable<float32<rad>>
  RollSpeed : Variable<float32<rad/s>>
  CannonTemperature : Variable<float32<f>>
  IsShooting : Variable<bool>
  OverHeated : Variable<bool>
  Shields : Variable<int>
 }

and Projectile = 
 {
  Position : Variable<Vector2D<m>>
  Speed : Vector2D<m/s>
  Colliders : Variable<List<Cylon>>
 }

and Cylon =
 {
  Position : Variable<Vector2D<m>>
  Speed : Variable<Vector2D<m/s>>
  Yaw : Variable<float32<rad>>
  Colliders : Variable<List<Projectile>>
  Shields : Variable<int>
  Hit : Variable<bool>
  AI : Variable<Coroutine<Unit>>
 }

and Explosion =
 {
  Position : Variable<Vector2D<m>>
  Speed : Vector2D<m/s>
  Time : Variable<float32<s>>
 }