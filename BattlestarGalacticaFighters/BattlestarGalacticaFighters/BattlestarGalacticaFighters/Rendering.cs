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
        Model player_ship, raider;
        RenderingData rendering_data = new RenderingData();
        SpriteBatch sprite_batch;
        SoundEffect soundEffect;
        int vertical_background_replication, horizontal_background_replication;
        protected override void LoadContent()
        {
            sprite_batch = new SpriteBatch(GraphicsDevice);

            soundEffect = Game.Content.Load<SoundEffect>("viper_gun");

            var input_service = Game.Services.GetService(typeof(IInputService)) as IInputService;
            input_service.Shoot += () => soundEffect.Play();

            // Initialize scrolling background
            rendering_data.space = Game.Content.Load<Texture2D>("space");
            // Calculate how many times we should replicate texture based on viewport and texture size
            vertical_background_replication = GraphicsDevice.Viewport.Height / rendering_data.space.Height;
            horizontal_background_replication = GraphicsDevice.Viewport.Width / rendering_data.space.Width;
            // Initial position of background
            backgroundPosition = new Vector2(0, GraphicsDevice.Viewport.Height - rendering_data.space.Height);

            // Initialize 3d models
            player_ship = Game.Content.Load<Model>("Viper_Mk_II");
            raider = Game.Content.Load<Model>("Cylon_raider");

            // Initialize basic_effect
            basic_effect = new BasicEffect(GraphicsDevice);
            basic_effect.World = Matrix.Identity;
            basic_effect.View = Matrix.CreateLookAt(Vector3.Up * 1.0f, Vector3.Zero, Vector3.Forward);
            basic_effect.Projection = Matrix.CreateOrthographic((float)GraphicsDevice.Viewport.Width/GraphicsDevice.Viewport.Height, 1.0f, 0.1f, 10000.0f);
            //basic_effect.View = Matrix.CreateLookAt(Vector3.Up * 0.5f, Vector3.Zero, Vector3.Forward);
            //basic_effect.Projection = Matrix.CreatePerspectiveFieldOfView(1.5f, GraphicsDevice.Viewport.AspectRatio, 0.1f, 10.0f);
            base.LoadContent();
        }


        Vector2 backgroundPosition;
        public override void Update(GameTime gameTime)
        {
            //GameWorld.updateBackground();

            backgroundPosition.Y += (float)gameTime.ElapsedGameTime.TotalSeconds * rendering_data.backgroundPixelPerSecond;
            backgroundPosition.Y = backgroundPosition.Y % rendering_data.space.Height;

            var direction = 0;

            KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Right))
                direction = -1;

            if (keyboard.IsKeyDown(Keys.Left))
                direction = +1;

            backgroundPosition.X = backgroundPosition.X + direction * (float)gameTime.ElapsedGameTime.TotalSeconds * rendering_data.backgroundPixelPerSecond;
            backgroundPosition.X = backgroundPosition.X % rendering_data.space.Width;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            sprite_batch.Begin();

            this.DrawBackground();
            sprite_batch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            Quaternion rotation = Quaternion.CreateFromYawPitchRoll((float)gameTime.TotalGameTime.TotalSeconds, 0.0f, 0.0f);

            player_ship.Draw(
                basic_effect.World * Matrix.CreateTranslation(0.5f, 0.0f, 0.0f)  * Matrix.CreateFromQuaternion(rotation),
                basic_effect.View,
                basic_effect.Projection
            );

            raider.Draw(
                basic_effect.World * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(0.2f * (float)Math.Sin((float)gameTime.TotalGameTime.TotalSeconds), 0.0f, -1.0f + 0.1f * (float)gameTime.TotalGameTime.TotalSeconds),
                basic_effect.View,
                basic_effect.Projection
            );
    
            base.Draw(gameTime);
        }

        public void DrawBackground()
        {
            // Draw the texture as many times as needed to fill a grid that covers whole viewport
            for (int i = -this.vertical_background_replication; i <= this.vertical_background_replication+1; i++)
            {
                for (int j = -this.horizontal_background_replication; j <= this.horizontal_background_replication; j++)
                {
                    sprite_batch.Draw(
                        rendering_data.space,
                        new Vector2(backgroundPosition.X + rendering_data.space.Width * j, backgroundPosition.Y - rendering_data.space.Height * i),
                        null, Color.White, 0, new Vector2(0.0f, 0.0f), 1, SpriteEffects.None, 0f
                    );
                }
            }
        }

    }
}
