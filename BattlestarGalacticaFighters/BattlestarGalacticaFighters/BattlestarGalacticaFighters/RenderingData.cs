using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace BattlestarGalacticaFighters
{
    class RenderingData
    {
        public UInt16 backgroundPixelPerSecond = 64;
        public Texture2D space;
    }

    class Utilities
    {
        // Game Logic is 2D
        public static Vector3 toVector3( Vector2D.Vector2D vector2d )
        {
            return new Vector3((float)vector2d.X, (float)vector2d.Y, 0.0f);
        }

        public static Vector2 toVector2( Vector2D.Vector2D vector2d )
        {
            return new Vector2((float)vector2d.X, (float)vector2d.Y);
        }

    }

}