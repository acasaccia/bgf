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
        RenderingData rendering_data = new RenderingData();
        SpriteBatch sprite_batch;
        SoundEffect soundEffect;
        protected override void LoadContent()
        {
            sprite_batch = new SpriteBatch(GraphicsDevice);

            soundEffect = Game.Content.Load<SoundEffect>("viper_gun");

            var input_service = Game.Services.GetService(typeof(IInputService)) as IInputService;
            input_service.Shoot += () => soundEffect.Play();

            // Initialize scrolling background
            rendering_data.space = Game.Content.Load<Texture2D>("space");

            // Initialize 3d models
            player_ship = Game.Content.Load<Model>("Viper_Mk_II");

            // Initialize basic_effect
            basic_effect = new BasicEffect(GraphicsDevice);
            basic_effect.World = Matrix.Identity;
            basic_effect.View = Matrix.CreateLookAt(
                Vector3.Up * 1.0f,
                Vector3.Zero,
                Vector3.Forward
            );
            basic_effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                1.5f,
                GraphicsDevice.Viewport.AspectRatio,
                0.01f, 100.0f
            );

            base.LoadContent();
        }


        Vector2 backgroundPosition = new Vector2(0.0f, 0.0f);
        public override void Update(GameTime gameTime)
        {
            backgroundPosition.Y += (float)gameTime.ElapsedGameTime.TotalSeconds * rendering_data.backgroundPixelPerSecond;
            backgroundPosition.Y = backgroundPosition.Y % rendering_data.space.Height;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            sprite_batch.Begin();
            // Draw the texture, if it is still onscreen.
            if (backgroundPosition.Y < GraphicsDevice.Viewport.Height)
            {
                sprite_batch.Draw(rendering_data.space, backgroundPosition, null, Color.White, 0, new Vector2(0.0f, 0.0f), 1, SpriteEffects.None, 0f);
            }
            // Draw the texture a second time, behind the first,
            // to create the scrolling illusion.
            sprite_batch.Draw(rendering_data.space, new Vector2(backgroundPosition.X, backgroundPosition.Y - rendering_data.space.Height), null,  Color.White, 0, new Vector2(0.0f, 0.0f), 1, SpriteEffects.None, 0f);
            sprite_batch.End();

            GraphicsDevice.SetRenderTarget(null);

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            player_ship.Draw(
                basic_effect.World,
                basic_effect.View,
                basic_effect.Projection
            );

            base.Draw(gameTime);
        }

    }
}
