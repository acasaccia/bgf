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
        Model viper_mk_2, raider, heavy_raider;
        Vector2 backgroundPosition;
        RenderingData rendering_data = new RenderingData();
        SpriteBatch sprite_batch;
        //SoundEffect soundEffect;
        int vertical_background_replication, horizontal_background_replication;
        Vector3 player_position = Vector3.Zero;
        protected override void LoadContent()
        {
            sprite_batch = new SpriteBatch(GraphicsDevice);

            //soundEffect = Game.Content.Load<SoundEffect>("viper_gun");

            //var input_service = Game.Services.GetService(typeof(IInputService)) as IInputService;
            //input_service.FireCannon += () => soundEffect.Play();

            // Initialize scrolling background
            rendering_data.space = Game.Content.Load<Texture2D>("space");
            // Calculate how many times we should replicate texture based on viewport and texture size
            vertical_background_replication = (int) Math.Ceiling( (double) GraphicsDevice.Viewport.Height / rendering_data.space.Height );
            horizontal_background_replication = (int) Math.Ceiling( (double) GraphicsDevice.Viewport.Width / rendering_data.space.Width );

            // Initial position of background
            backgroundPosition = new Vector2(0.0f, GraphicsDevice.Viewport.Height - rendering_data.space.Height);

            // Initialize 3d models
            viper_mk_2 = Game.Content.Load<Model>("Viper_Mk_II");
            raider = Game.Content.Load<Model>("Cylon_Raider");
            // heavy_raider = Game.Content.Load<Model>("Cylon_Heavy_Raider");

            // Initialize basic_effect
            basic_effect = new BasicEffect(GraphicsDevice);
            basic_effect.World = Matrix.Identity;
            basic_effect.View = Matrix.CreateLookAt(Vector3.Backward * 1.0f, Vector3.Zero, Vector3.Up);
            basic_effect.Projection = Matrix.CreateOrthographic((float)GraphicsDevice.Viewport.Width/GraphicsDevice.Viewport.Height, 1.0f, 0.1f, 10000.0f);
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Texture2D rect = new Texture2D(GraphicsDevice, 2, 2);

            Color[] data = new Color[2*2];
            for(int i=0; i < data.Length; ++i) data[i] = Color.White;
            rect.SetData(data);

            IList<Entities.Projectile> projectiles = GameState.state.projectiles.Value.ToList();

            sprite_batch.Begin();
                this.DrawBackground(gameTime);
                foreach (Entities.Projectile projectile in projectiles)
                {
                    Vector2 position =  Utilities.toVector2(projectile.Position.Value);
                    position.X = (position.X + GraphicsDevice.Viewport.AspectRatio / 2) * GraphicsDevice.Viewport.Width / GraphicsDevice.Viewport.AspectRatio;
                    position.Y = GraphicsDevice.Viewport.Height - ( (position.Y + 0.5f) * GraphicsDevice.Viewport.Height );
                    sprite_batch.Draw(rect, position, Color.White);
                }
            sprite_batch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            viper_mk_2.Draw(
                basic_effect.World
                * Matrix.CreateFromQuaternion(Quaternion.CreateFromYawPitchRoll(GameState.state.viper.Roll.Value, 0.0f, 0.0f))
                * Matrix.CreateTranslation(Utilities.toVector3(GameState.state.viper.Position.Value)),
                basic_effect.View,
                basic_effect.Projection
            );

            IList<Entities.Cylon> cylons = GameState.state.cylons.Value.ToList();

            foreach (Entities.Cylon cylon in cylons)
            {
                raider.Draw(
                    basic_effect.World
                    * Matrix.CreateTranslation(Utilities.toVector3(cylon.Position.Value)),
                    basic_effect.View,
                    basic_effect.Projection
                );
            }

            //raider.Draw(
            //    basic_effect.World * Matrix.CreateTranslation(0.5f,0.2f,0.0f),
            //    basic_effect.View,
            //    basic_effect.Projection
            //);

            //heavy_raider.Draw(
            //    basic_effect.World * Matrix.CreateTranslation(-0.5f, -0.2f, 0.0f),
            //    basic_effect.View,
            //    basic_effect.Projection
            //);

            base.Draw(gameTime);
        }

        public void DrawBackground(GameTime gameTime)
        {
            float scroll_direction = 0.0f;
            if (BattlestarGalacticaFightersInput.InputState.MoveLeft) {
                scroll_direction -= 1.0f;
            }
            if (BattlestarGalacticaFightersInput.InputState.MoveRight)
            {
                scroll_direction += 1.0f;
            }

            backgroundPosition.X = backgroundPosition.X + scroll_direction * (float)gameTime.ElapsedGameTime.TotalSeconds * rendering_data.backgroundPixelPerSecond * 1.5f;
            backgroundPosition.X = backgroundPosition.X % rendering_data.space.Width;

            backgroundPosition.Y += (float)gameTime.ElapsedGameTime.TotalSeconds * rendering_data.backgroundPixelPerSecond;
            backgroundPosition.Y = backgroundPosition.Y % rendering_data.space.Height;

            // Draw the texture as many times as needed to fill a grid that covers whole viewport
            for (int i = -1; i < vertical_background_replication + 1; i++)
            {
                for (int j = -1; j < horizontal_background_replication + 1; j++)
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
