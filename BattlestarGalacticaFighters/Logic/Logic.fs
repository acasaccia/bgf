// Ulteriori informazioni su F# all'indirizzo http://fsharp.net
namespace GameWorld
module Logic = 

    open Measures
    open Vector2D
    open Entities

    open Microsoft.Xna.Framework.Input
    open Microsoft.Xna.Framework

    type GameState =
        {
            Player : Viper
            Enemy : Raider
        }

    let state : GameState =
        {
            Player = 
                {
                    Body = 
                        {
                            Position = { X = 0.0<m>; Y = 0.0<m> };
                            Yaw = 0.0<d>
                            Roll = 0.0<d>
                            Shields = 3
                        }
                    Missiles = 3
                    LockedTarget = None
                }
            Enemy = 
                {
                    Body = 
                        {
                            Position = { X = 0.0<m>; Y = 0.0<m> };
                            Yaw = 180.0<d>
                            Roll = 0.0<d>
                            Shields = 3
                        }
                }
         
        }

    let updateGameState( gs:GameState, dt:float ) =
        let p =
            {
                gs.Player
                with Body =
                {
                    gs.Player.Body
                    with Position = 
                    {
                        X = gs.Player.Body.Position.X + 0.01<m>
                        Y = gs.Player.Body.Position.Y
                    }
                }
            }
        {
            gs with Player = p
        }