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
using BattlestarGalacticaFightersShared;

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
        RenderingData renderingData = new RenderingData();
        SpriteBatch spriteBatch;
        int vertical_background_replication, horizontal_background_replication;
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            renderingData.gameInterface = Game.Content.Load<SpriteFont>("interface");

            renderingData.explosion = Game.Content.Load<Texture2D>("explosion_animation");
            renderingData.laser = Game.Content.Load<Texture2D>("laser");

            // Initialize scrolling background
            renderingData.space = Game.Content.Load<Texture2D>("space");
            renderingData.space_dust = Game.Content.Load<Texture2D>("space_dust");

            // Initialize 3d models
            renderingData.viperMarkII = Game.Content.Load<Model>("Viper_Mk_II");
            BoundingSphere boundingSphere = new BoundingSphere();
            foreach (var mesh in renderingData.viperMarkII.Meshes)
            {
                foreach (BasicEffect fx in mesh.Effects)
                {
                    fx.EnableDefaultLighting();
                    fx.SpecularColor = Color.White.ToVector3();
                    fx.DiffuseColor = Color.Gray.ToVector3();
                    fx.AmbientLightColor = Color.White.ToVector3();
                }
                boundingSphere = BoundingSphere.CreateMerged(boundingSphere, mesh.BoundingSphere);
            }
            RenderingData.viperMarkIIBoundingRadius = boundingSphere.Radius;

            renderingData.raider = Game.Content.Load<Model>("Cylon_Raider");
            boundingSphere = new BoundingSphere();
            foreach (var mesh in renderingData.raider.Meshes)
            {
                foreach (BasicEffect fx in mesh.Effects)
                {
                    fx.EnableDefaultLighting();
                    fx.SpecularColor = Color.Gray.ToVector3();
                    fx.DiffuseColor = Color.Gray.ToVector3();
                    fx.AmbientLightColor = Color.White.ToVector3();
                }
                boundingSphere = BoundingSphere.CreateMerged(boundingSphere, mesh.BoundingSphere);
            }
            RenderingData.raiderBoundingRadius = boundingSphere.Radius;

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
            this.DrawBackground(gameTime);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            this.DrawInterface(gameTime);
            
            IList<Entities.Projectile> projectiles = GameState.state.projectiles.Value.ToList();
            foreach (Entities.Projectile projectile in projectiles)
                spriteBatch.Draw(renderingData.laser, Conversions.toSpriteBatchCoords(projectile.Position.Value, GraphicsDevice.Viewport) - new Vector2(renderingData.laser.Width / 2, 0.0f), Color.White);

            IList<Entities.Explosion> explosions = GameState.state.explosions.Value.ToList();
            foreach (Entities.Explosion explosion in explosions)
                spriteBatch.Draw(renderingData.explosion, Conversions.toSpriteBatchCoords(explosion.Position.Value, GraphicsDevice.Viewport) - new Vector2(50.0f,70.0f), new Rectangle(( 10 - ((int)(explosion.Time.Value * 10))) * 120, 0, 120, 120), Color.White);

            spriteBatch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            Entities.Viper viper = GameState.state.viper;

            renderingData.viperMarkII.Draw(
                basic_effect.World
                * Matrix.CreateFromQuaternion(Quaternion.CreateFromYawPitchRoll(viper.Roll.Value, 0.0f, 0.0f))
                * Matrix.CreateTranslation(Conversions.toVector3(viper.Position.Value)),
                basic_effect.View,
                basic_effect.Projection
            );

            IList<Entities.Cylon> cylons = GameState.state.cylons.Value.ToList();

            foreach (Entities.Cylon cylon in cylons)
            {
                if ( cylon.Shields.Value <= 3 && ((int) gameTime.TotalGameTime.TotalMilliseconds / 150) % 2 == 0 )
                 foreach (var mesh in renderingData.raider.Meshes)
                  {
                    foreach (BasicEffect fx in mesh.Effects)
                    {
                      fx.AmbientLightColor = Color.Red.ToVector3();
                    }
                  }
                else if ( cylon.Hit.Value )
                    foreach (var mesh in renderingData.raider.Meshes)
                    {
                        foreach (BasicEffect fx in mesh.Effects)
                        {
                            fx.AmbientLightColor = Color.Cyan.ToVector3();
                            fx.SpecularColor = Color.Cyan.ToVector3();
                            fx.DiffuseColor = Color.Cyan.ToVector3();
                        }
                    }
                else
                    foreach (var mesh in renderingData.raider.Meshes)
                    {
                        foreach (BasicEffect fx in mesh.Effects)
                        {
                            fx.SpecularColor = Color.Gray.ToVector3();
                            fx.DiffuseColor = Color.Gray.ToVector3();
                            fx.AmbientLightColor = Color.Gray.ToVector3();
                        }
                    }

                renderingData.raider.Draw(
                    basic_effect.World
                    * Matrix.CreateFromQuaternion(Quaternion.CreateFromYawPitchRoll(0.0f, 0.0f, cylon.Yaw.Value))
                    * Matrix.CreateTranslation(Conversions.toVector3(cylon.Position.Value)),
                    basic_effect.View,
                    basic_effect.Projection
                );
            }

            base.Draw(gameTime);
        }

        Vector2 backgroundPosition, cloudsPosition;
        int horizontalScrollDirection = 0;
        public void DrawBackground(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null);

            if (BattlestarGalacticaFightersShared.InputState.MoveLeft == BattlestarGalacticaFightersShared.InputState.MoveRight)
                horizontalScrollDirection = 0;
            if (BattlestarGalacticaFightersShared.InputState.MoveLeft)
                horizontalScrollDirection = 1;
            if (BattlestarGalacticaFightersShared.InputState.MoveRight)
                horizontalScrollDirection = -1;

            // Update background position based on player's ship movement
            backgroundPosition.X = backgroundPosition.X + horizontalScrollDirection * (float)gameTime.ElapsedGameTime.TotalSeconds * renderingData.backgroundScrollSpeed;
            //cloudsPosition.X = cloudsPosition.X + horizontalScrollDirection * (float)gameTime.ElapsedGameTime.TotalSeconds * renderingData.cloudsScrollSpeed;
            backgroundPosition.Y += (float)gameTime.ElapsedGameTime.TotalSeconds * renderingData.backgroundScrollSpeed;
            cloudsPosition.Y += (float)gameTime.ElapsedGameTime.TotalSeconds * renderingData.cloudsScrollSpeed;

            // Clamp position to avoid overflow
            backgroundPosition.X = backgroundPosition.X % renderingData.space.Width;
            backgroundPosition.Y = backgroundPosition.Y % renderingData.space.Height;
            cloudsPosition.Y = cloudsPosition.Y % renderingData.space_dust.Height;

            spriteBatch.Draw(
                renderingData.space,
                Vector2.Zero,
                new Rectangle((int)backgroundPosition.X, -(int)backgroundPosition.Y, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
                Color.White
            );

            spriteBatch.Draw(
                renderingData.space_dust,
                Vector2.Zero,
                new Rectangle((int)cloudsPosition.X, -(int)cloudsPosition.Y, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
                Color.White
            );

            spriteBatch.End();
        }

        public void DrawInterface(GameTime gameTime)
        {
            string msg;

            Color guncolor = Color.Cyan;
            if (GameState.state.viper.CannonTemperature.Value > 25)
                guncolor = Color.LimeGreen;
            if (GameState.state.viper.CannonTemperature.Value > 50)
                guncolor = Color.Yellow;
            if (GameState.state.viper.CannonTemperature.Value > 80)
                guncolor = Color.Orange;
            if (GameState.state.viper.CannonTemperature.Value > 95)
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
            if ((int)GameState.state.galacticaShields.Value < 25)
                galacticashield = Color.Orange;
            if ((int)GameState.state.galacticaShields.Value < 10)
                galacticashield = Color.Red;

            spriteBatch.DrawString(renderingData.gameInterface, "Cannon temp: " + (int)GameState.state.viper.CannonTemperature.Value + "%", new Vector2(10, GraphicsDevice.Viewport.Height - 40), guncolor);
            spriteBatch.DrawString(renderingData.gameInterface, "Viper shields: " + (int)GameState.state.viper.Shields.Value, new Vector2(10, GraphicsDevice.Viewport.Height - 20), vipershield);

            msg = "FTL online in: " + Math.Round(GameState.state.gameTime.Value) + "\"";
            spriteBatch.DrawString(renderingData.gameInterface, msg, new Vector2(GraphicsDevice.Viewport.Width - renderingData.gameInterface.MeasureString("FTL online in: 300\"").X - 10, GraphicsDevice.Viewport.Height - 40), Color.LimeGreen);
            msg = "Galactica shields: " + (int)GameState.state.galacticaShields.Value + "%";
            spriteBatch.DrawString(renderingData.gameInterface, msg, new Vector2(GraphicsDevice.Viewport.Width - renderingData.gameInterface.MeasureString(msg).X - 10, GraphicsDevice.Viewport.Height - 20), galacticashield);

            spriteBatch.DrawString(renderingData.gameInterface, "FPS: " + (int)( 1.0f / gameTime.ElapsedGameTime.TotalSeconds), new Vector2(10, 10), Color.LimeGreen);
        }

    }
}
