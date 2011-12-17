using System;
using Microsoft.Xna.Framework.Graphics;

namespace BattlestarGalacticaFightersShared
{

    /// This is here so it can be referenced by both Logic and Rendering components
    /// avoiding circular references between projects
    public class RenderingData
    {
        public int backgroundScrollSpeed = 48;
        public int cloudsScrollSpeed = 256;
        public Model viperMarkII, raider;
        public Texture2D space, space_dust, explosion, laser;
        public SpriteFont gameInterface;

        // This members are static for quick access from Logic
        // Initialized properly after LoadContent
        public static float raiderBoundingRadius = 0.0f, viperMarkIIBoundingRadius = 0.0f;
    }

}