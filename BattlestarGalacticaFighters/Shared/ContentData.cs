using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

/// 
/// This is here so it can be referenced by both Logic and Rendering components
/// avoiding circular references between projects
/// 
namespace Shared
{
    public class ContentData
    {
        public Model viperMarkII, raider;
        public Texture2D space, spaceDust, explosion, laser, energyCells;
        public SpriteFont menuFont, gameFont;
        public Song menu, inGame;
        public SpriteBatch spriteBatch;
        public Texture2D logo;

        // This members are static for quick access from Logic
        // Some of them are initialized properly after LoadContent
        public const int backgroundScrollSpeed = 48; // pixels per second
        public const int cloudsScrollSpeed = 256; // pixels per second
        public static float raiderBoundingRadius = 0.0f;
        public static float viperMarkIIBoundingRadius = 0.0f;
        public static Viewport viewPort;
    }

}