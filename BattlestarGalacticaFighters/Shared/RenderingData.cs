using System;
using Microsoft.Xna.Framework.Graphics;

/// 
/// This is here so it can be referenced by both Logic and Rendering components
/// avoiding circular references between projects
/// 
namespace Shared
{
    public class RenderingData
    {
        public const int backgroundScrollSpeed = 48; // pixels per second
        public const int cloudsScrollSpeed = 256; // pixels per second
        public Model viperMarkII, raider;
        public Texture2D space, spaceDust, explosion, laser;
        public SpriteFont gameInterface;

        // This members are static for quick access from Logic
        // They will be initialized properly after LoadContent
        public static float raiderBoundingRadius = 0.0f;
        public static float viperMarkIIBoundingRadius = 0.0f;
        public static Viewport viewPort;
    }

}