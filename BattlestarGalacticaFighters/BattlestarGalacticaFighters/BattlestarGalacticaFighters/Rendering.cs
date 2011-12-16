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
        SpriteFont font;
        int vertical_background_replication, horizontal_background_replication;
        Vector3 player_position = Vector3.Zero;
        protected override void LoadContent()
        {
            sprite_batch = new SpriteBatch(GraphicsDevice);
            font = Game.Content.Load<SpriteFont>("interface");

            rendering_data.explosion = Game.Content.Load<Texture2D>("red-explosion");
            rendering_data.laser = Game.Content.Load<Texture2D>("laser");

            // Initialize scrolling background
            rendering_data.space = Game.Content.Load<Texture2D>("space");

            // Calculate how many times we should replicate texture based on viewport and texture size
            vertical_background_replication = (int)Math.Ceiling((double)GraphicsDevice.Viewport.Height / rendering_data.space.Height);
            horizontal_background_replication = (int)Math.Ceiling((double)GraphicsDevice.Viewport.Width / rendering_data.space.Width);

            // Initial position of background
            backgroundPosition = new Vector2(0.0f, GraphicsDevice.Viewport.Height - rendering_data.space.Height);

            // Initialize 3d models
            viper_mk_2 = Game.Content.Load<Model>("Viper_Mk_II");

            foreach (var mesh in viper_mk_2.Meshes)
            {
                foreach (BasicEffect fx in mesh.Effects)
                {
                    fx.EnableDefaultLighting();
                    fx.SpecularColor = Color.White.ToVector3();
                    fx.DiffuseColor = Color.Gray.ToVector3();
                    fx.AmbientLightColor = Color.White.ToVector3();
                }

            }

            raider = Game.Content.Load<Model>("Cylon_Raider");

            foreach (var mesh in raider.Meshes)
            {
                foreach (BasicEffect fx in mesh.Effects)
                {
                    fx.EnableDefaultLighting();
                    fx.SpecularColor = Color.Gray.ToVector3();
                    fx.DiffuseColor = Color.Gray.ToVector3();
                    fx.AmbientLightColor = Color.White.ToVector3();
                }
            }

            // heavy_raider = Game.Content.Load<Model>("Cylon_Heavy_Raider");

            // Initialize basic_effect
            basic_effect = new BasicEffect(GraphicsDevice);
            basic_effect.World = Matrix.Identity;
            basic_effect.View = Matrix.CreateLookAt(Vector3.Backward * 1.0f, Vector3.Zero, Vector3.Up);
            basic_effect.Projection = Matrix.CreateOrthographic((float)GraphicsDevice.Viewport.Width / GraphicsDevice.Viewport.Height, 1.0f, 0.1f, 10000.0f);

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            IList<Entities.Projectile> projectiles = GameState.state.projectiles.Value.ToList();

            sprite_batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            this.DrawBackground(gameTime);
            this.DrawInterface(gameTime);
            foreach (Entities.Projectile projectile in projectiles)
            {
                Vector2 position = Utilities.toVector2(projectile.Position.Value);
                position.X = (position.X + GraphicsDevice.Viewport.AspectRatio / 2) * GraphicsDevice.Viewport.Width / GraphicsDevice.Viewport.AspectRatio;
                position.Y = GraphicsDevice.Viewport.Height - ((position.Y + 0.5f) * GraphicsDevice.Viewport.Height);
                sprite_batch.Draw(rendering_data.laser, new Vector2(position.X - 13, position.Y), Color.White);
            }
         
            sprite_batch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            Entities.Viper viper = GameState.state.viper;

            viper_mk_2.Draw(
                basic_effect.World
                * Matrix.CreateFromQuaternion(Quaternion.CreateFromYawPitchRoll(viper.Roll.Value, 0.0f, 0.0f))
                * Matrix.CreateTranslation(Utilities.toVector3(viper.Position.Value)),
                basic_effect.View,
                basic_effect.Projection
            );

            IList<Entities.Cylon> cylons = GameState.state.cylons.Value.ToList();

            foreach (Entities.Cylon cylon in cylons)
            {
                if ( cylon.Shields.Value <= 3 && ((int) gameTime.TotalGameTime.TotalMilliseconds / 150) % 2 == 0 )
                 foreach (var mesh in raider.Meshes)
                  {
                    foreach (BasicEffect fx in mesh.Effects)
                    {
                      fx.AmbientLightColor = Color.Red.ToVector3();
                    }
                  }
                else if ( cylon.Hit.Value )
                    foreach (var mesh in raider.Meshes)
                    {
                        foreach (BasicEffect fx in mesh.Effects)
                        {
                            fx.AmbientLightColor = Color.Cyan.ToVector3();
                            fx.SpecularColor = Color.Cyan.ToVector3();
                            fx.DiffuseColor = Color.Cyan.ToVector3();
                        }
                    }
                else
                    foreach (var mesh in raider.Meshes)
                    {
                        foreach (BasicEffect fx in mesh.Effects)
                        {
                            fx.SpecularColor = Color.Gray.ToVector3();
                            fx.DiffuseColor = Color.Gray.ToVector3();
                            fx.AmbientLightColor = Color.Gray.ToVector3();
                        }
                    }

                raider.Draw(
                    basic_effect.World
                    * Matrix.CreateFromQuaternion(Quaternion.CreateFromYawPitchRoll(0.0f, 0.0f, cylon.Yaw.Value))
                    * Matrix.CreateTranslation(Utilities.toVector3(cylon.Position.Value)),
                    basic_effect.View,
                    basic_effect.Projection
                );
            }

            base.Draw(gameTime);
        }

        public void DrawBackground(GameTime gameTime)
        {
            float scroll_direction = 0.0f;
            if (BattlestarGalacticaFightersInput.InputState.MoveLeft)
            {
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

        public void DrawInterface(GameTime gameTime)
        {
            string msg;

            Color guncolor = Color.Cyan;
            if (GameState.state.viper.CannonTemperature.Value > 25)
                guncolor = Color.LimeGreen;
            if (GameState.state.viper.CannonTemperature.Value > 50)
                guncolor = Color.Yellow;
            if ( GameState.state.viper.CannonTemperature.Value > 80 )
                guncolor = Color.Orange;
            if (GameState.state.viper.CannonTemperature.Value > 95 )
                guncolor = Color.Red;

            Color vipershield = Color.Cyan;
            switch ((int)GameState.state.viper.Shields.Value) {
                case 2:
                    vipershield = Color.Yellow;
                    break;
                case 1:
                    vipershield = Color.Red;
                    break;
            }

            Color galacticashield = Color.Cyan;
            if ((int)GameState.state.galacticaShields.Value < 75)
                galacticashield = Color.LimeGreen;
            if ((int)GameState.state.galacticaShields.Value < 50)
                galacticashield = Color.Yellow;
            if ( (int)GameState.state.galacticaShields.Value < 25 )
                galacticashield = Color.Orange;
            if ((int)GameState.state.galacticaShields.Value < 10 )
                galacticashield = Color.Red;

            sprite_batch.DrawString(font, "Cannon temp: " + (int)GameState.state.viper.CannonTemperature.Value + "%", new Vector2(10, GraphicsDevice.Viewport.Height - 40), guncolor);
            sprite_batch.DrawString(font, "Viper shields: " + (int)GameState.state.viper.Shields.Value, new Vector2(10, GraphicsDevice.Viewport.Height - 20), vipershield);

            msg = "FTL online in: " + Math.Round(GameState.state.gameTime.Value) + "\"";
            sprite_batch.DrawString(font, msg, new Vector2(GraphicsDevice.Viewport.Width - font.MeasureString("FTL online in: 300\"").X - 10, GraphicsDevice.Viewport.Height - 40), Color.LimeGreen);
            msg = "Galactica shields: " + (int)GameState.state.galacticaShields.Value + "%";
            sprite_batch.DrawString(font, msg, new Vector2(GraphicsDevice.Viewport.Width - font.MeasureString(msg).X - 10, GraphicsDevice.Viewport.Height - 20), galacticashield);
        }

    }
}
