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

        SoundEffect viperGun, explosion, overHeated, alert;
        protected override void LoadContent()
        {
            viperGun = Game.Content.Load<SoundEffect>("viper_gun");
            explosion = Game.Content.Load<SoundEffect>("explosion");
            overHeated = Game.Content.Load<SoundEffect>("overheated");
            alert = Game.Content.Load<SoundEffect>("alert");
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            if (GameState.state.viper.IsShooting.Value) viperGun.Play(0.3f, 0.0f, 0.0f);
            if (GameState.state.viper.OverHeated.Value) overHeated.Play(0.6f, 0.0f, 0.0f);
            IList<Entities.Cylon> escapedCylons = GameState.state.escapedCylons.Value.ToList();
            bool cylonEscapes = false;
            if(escapedCylons.Count > 0)
                cylonEscapes = true;
            if (cylonEscapes) alert.Play(0.6f, 0.0f, 0.0f);
            IList<Entities.Cylon> cylons = GameState.state.cylons.Value.ToList();
            bool cylonExplodes = false;
            foreach (Entities.Cylon cylon in cylons)
            {
                if (cylon.Shields.Value == 0)
                    cylonExplodes = true;
            }
            if (cylonExplodes) explosion.Play();
            base.Update(gameTime);
        }
    }
}
