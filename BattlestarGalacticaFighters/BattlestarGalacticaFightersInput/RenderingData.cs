using System;
using Microsoft.Xna.Framework.Graphics;

namespace BattlestarGalacticaFightersShared
{
    public class RenderingData
    {
        public int backgroundScrollSpeed = 48;
        public int cloudsScrollSpeed = 256;
        public Model viperMarkII, raider;
        public Texture2D space, space_dust, explosion, laser;
        public SpriteFont gameInterface;
        
        public static float raiderBoundingRadius = 0.0f;
        public static float viperMarkIIsBoundingRadius = 0.0f, viperMarkIIBoundingRadius = 0.0f;
    }

}