// Ulteriori informazioni su F# all'indirizzo http://fsharp.net

module GameWorld
open Measures
open Vector2

type Aircraft = 
 {
  Position : Vector2<m>
  Yaw : float<d>
  Roll : float<d>
  Shields : int
 }

type Viper = 
 {
  Body : Aircraft
  Missiles : int
  CannonTemperature : int<f>
 }

type Raider =
 {
  Body : Aircraft
 }

type Projectile = 
 {
  Position : Vector2<m>
  Velocity : Vector2<m>
 }

type ViperProjectile = 
 {
  Body : Projectile
 }

type RaiderProjectile = 
 {
  Body : Projectile
 }

type Missile = 
 {
  Position : Vector2<m>
  Velocity : Vector2<m/s>
  Acceleration : Vector2<m/s^2>
 }

type ViperMissile = 
 {
  Body : Missile
  Target : Raider
 }

// Per limitare la frustrazione dell'utente, in questo gioco la tecnologia silone
// è meno avanzata di quella umana
type RaiderMissile = 
 {
  Body : Missile
 }