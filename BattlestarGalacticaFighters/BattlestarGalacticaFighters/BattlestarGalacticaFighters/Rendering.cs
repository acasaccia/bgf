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
    public class Rendering : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public Rendering(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }


        public override void Initialize()
        {
            base.Initialize();
        }


        BasicEffect basic_effect;
        Model player_ship;
        protected override void LoadContent()
        {
            player_ship = Game.Content.Load<Model>("Viper_Mk_II");
            basic_effect = new BasicEffect(GraphicsDevice);
            basic_effect.World = Matrix.Identity;
            basic_effect.View = Matrix.CreateLookAt(
                Vector3.Up * 10.0f,
                Vector3.Zero,
                Vector3.Forward
            );
            basic_effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                1.2f,
                GraphicsDevice.Viewport.AspectRatio,
                0.01f, 100.0f
            );
            base.LoadContent();
        }


        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            player_ship.Draw(
                basic_effect.World,
                basic_effect.View,
                basic_effect.Projection
            );

            base.Draw(gameTime);
        }

    }
}
