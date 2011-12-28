using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattlestarGalacticaFighters
{
    class Conversions
    {
        // Game Logic is 2D
        public static Vector3 toVector3(Utilities.Vector2D vector2d)
        {
            return new Vector3((float)vector2d.X, (float)vector2d.Y, 0.0f);
        }

        public static Vector2 toVector2(Utilities.Vector2D vector2d)
        {
            return new Vector2((float)vector2d.X, (float)vector2d.Y);
        }

        public static Vector2 toSpriteBatchCoords(Utilities.Vector2D vector2d, Viewport Viewport)
        {
            Vector2 position;
            position.X = (vector2d.X + Viewport.AspectRatio / 2) * Viewport.Width / Viewport.AspectRatio;
            position.Y = Viewport.Height - ((vector2d.Y + 0.5f) * Viewport.Height);
            return position;
        }
    }
}
