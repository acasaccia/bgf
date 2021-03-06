﻿module Utilities
open Measures

type Vector2D<[<Measure>] 'a> =
  {
    X : float32<'a>
    Y : float32<'a>
  }

  static member Zero : Vector2D<'a> =
    { X = 0.0f<_>; Y = 0.0f<_> }

  static member (+)
    (v1:Vector2D<'a>,v2:Vector2D<'a>):Vector2D<'a> =
      { X = v1.X + v2.X; Y = v1.Y + v2.Y }

  static member (~-)
    (v:Vector2D<'a>):Vector2D<'a> =
      { X = -v.X; Y = -v.Y }

  static member (-)
    (v1:Vector2D<'a>,v2:Vector2D<'a>):Vector2D<'a> =
      v1+(-v2)

  static member (*)
    (v1:Vector2D<'a>,v2:Vector2D<'b>):Vector2D<'a*'b> =
      { X = v1.X * v2.X; Y = v1.Y * v2.Y }

  static member (*)
    (v:Vector2D<'a>,k:float32<'b>):Vector2D<'a*'b> =
      { X = v.X * k; Y = v.Y * k }

  static member (*)
    (k:float32<'b>,v:Vector2D<'a>):Vector2D<'a*'b> =
      { X = v.X * k; Y = v.Y * k }

  static member (/)
    (v:Vector2D<'a>,f:float32<'b>):Vector2D<'a/'b> =
      v*(1.0f/f)

  member this.Length : float32<'a> =
    sqrt((this.X*this.X+this.Y*this.Y))

  static member Distance
    (v1:Vector2D<'a>,v2:Vector2D<'a>) =
      (v1-v2).Length

  static member Normalize(v:Vector2D<'a>):Vector2D<1> =
    v/v.Length

  static member Angle
    (v1:Vector2D<m>,v2:Vector2D<m>):float32<1> =
      acos(((v1*v2).X + (v1*v2).Y)/(v1.Length*v2.Length))

let s : float32<s> = 1.0f<s>
let m : float32<m> = 1.0f<m>
let ms : float32<m/s> = 1.0f<m/s>
let rad : float32<rad> = 1.0f<rad>

type Factions =
 | Colonies = 0
 | Cylons = 1

