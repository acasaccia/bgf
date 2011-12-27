using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace BattlestarGalacticaFighters
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Audio : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public Audio(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        SoundEffect viperGun, raiderGun, explosion, overHeated, hit;
        protected override void LoadContent()
        {
            viperGun = Game.Content.Load<SoundEffect>("viper_gun");
            raiderGun = Game.Content.Load<SoundEffect>("raider_gun");
            explosion = Game.Content.Load<SoundEffect>("explosion");
            overHeated = Game.Content.Load<SoundEffect>("overheated");
            hit = Game.Content.Load<SoundEffect>("hit");
        }

        double previousCollisionTime = -10.0;
        int previousShieldValue = 0;
        public override void Update(GameTime gameTime)
        {
            if (GameState.state.viper.IsShooting.Value) viperGun.Play(0.3f, 0.0f, 0.0f);
            if (GameState.state.viper.OverHeated.Value) overHeated.Play(0.6f, 0.0f, 0.0f);

            IList<Entities.Cylon> cylons = GameState.state.cylons.Value.ToList();
            bool cylonExplodes = false, cylonShoots = false, cylonHit = false;
            foreach (Entities.Cylon cylon in cylons)
            {
                IList<Entities.Projectile> cylonColliders = cylon.Colliders.Value.ToList();
                if (cylon.Shields.Value == 0)
                    cylonExplodes = true;
                if (cylon.IsShooting.Value)
                    cylonShoots = true;
                if (cylonColliders.Count > 0)
                    cylonHit = true;
            }
            bool viperExplodes = GameState.state.viper.Shields.Value == 0 && previousShieldValue > 0;
            if (cylonExplodes || viperExplodes) explosion.Play();
            if (cylonShoots) raiderGun.Play(0.3f, 0.0f, 0.0f);
            if (GameState.state.viper.LastCollisionTime.Value > previousCollisionTime || cylonHit) hit.Play(0.3f, 0.0f, 0.0f);
            previousCollisionTime = GameState.state.viper.LastCollisionTime.Value;
            previousShieldValue = GameState.state.viper.Shields.Value;
            
            base.Update(gameTime);
        }
    }
}
