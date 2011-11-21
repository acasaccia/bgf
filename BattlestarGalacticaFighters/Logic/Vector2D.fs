module Vector2D

type Vector2D<[<Measure>] 'a> =
    {
        X : float<'a>
        Y : float<'a>
    }

    static member Zero : Vector2D<'a> =
        { X = 0.0<_>; Y = 0.0<_> }

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
        (v:Vector2D<'a>,k:float<'b>):Vector2D<'a*'b> =
            { X = v.X * k; Y = v.Y * k }

    static member (*)
        (k:float<'b>,v:Vector2D<'a>):Vector2D<'a*'b> =
            { X = v.X * k; Y = v.Y * k }

    static member (/)
        (v:Vector2D<'a>,f:float<'b>):Vector2D<'a/'b> =
            v*(1.0/f)