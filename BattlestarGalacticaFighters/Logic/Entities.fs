module Entities
open Measures
open Vector2D

type Aircraft = 
    {
        Position : Vector2D<m>
        Yaw : float<d>
        Roll : float<d>
        Shields : int
    }

type Raider =
    {
        Body : Aircraft
    }

type Viper = 
    {
        Body : Aircraft
        Missiles : int
        LockedTarget : Option<Raider>
    }

type Projectile = 
    {
        Position : Vector2D<m>
        Velocity : Vector2D<m>
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
        Position : Vector2D<m>
        Velocity : Vector2D<m/s>
        Acceleration : Vector2D<m/s^2>
        Yaw : float<d>
    }

type ViperMissile = 
    {
        Body : Missile
        Target : Raider
    }

type RaiderMissile = 
    {
        Body : Missile
    }