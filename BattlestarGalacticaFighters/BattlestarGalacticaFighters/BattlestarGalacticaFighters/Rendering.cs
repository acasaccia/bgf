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
using Shared;

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

        ContentData contentData;
        public override void Initialize()
        {
            contentData = Game.Services.GetService(typeof(ContentData)) as ContentData;
            base.Initialize();
        }

        SpriteBatch spriteBatch;
        BasicEffect basicEffect;
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // Initialize basicEffect
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.World = Matrix.Identity;
            basicEffect.View = Matrix.CreateLookAt(Vector3.Backward * 1.0f, Vector3.Zero, Vector3.Up);
            basicEffect.Projection = Matrix.CreateOrthographic((float)GraphicsDevice.Viewport.Width / GraphicsDevice.Viewport.Height, 1.0f, 0.1f, 10000.0f);

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            this.DrawBackground(gameTime);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            Entities.Viper viper = GameState.state.viper;
            IList<Entities.Projectile> projectiles = GameState.state.projectiles.Value.ToList();
            IList<Entities.Explosion> explosions = GameState.state.explosions.Value.ToList();
            IList<Entities.Cylon> cylons = GameState.state.cylons.Value.ToList();

            //
            // PROJECTILES
            //

            if (viper.Shields.Value > 0)
            {
                foreach (Entities.Projectile projectile in projectiles)
                {
                    if (projectile.Owner == Utilities.Factions.Colonies)
                        spriteBatch.Draw(contentData.laser, Conversions.toSpriteBatchCoords(projectile.Position.Value, GraphicsDevice.Viewport) - new Vector2(contentData.laser.Width / 2, 0.0f), Color.White);
                    else
                        spriteBatch.Draw(contentData.energyCells, Conversions.toSpriteBatchCoords(projectile.Position.Value, GraphicsDevice.Viewport) - new Vector2(contentData.laser.Width / 2, 0.0f), Color.White);
                }
            }

            //
            // EXPLOSIONS
            //

            foreach (Entities.Explosion explosion in explosions)
                spriteBatch.Draw(contentData.explosion, Conversions.toSpriteBatchCoords(explosion.Position.Value, GraphicsDevice.Viewport) - new Vector2(50.0f,70.0f), new Rectangle(( 10 - ((int)(explosion.Time.Value * 10))) * 120, 0, 120, 120), Color.White);

            spriteBatch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

            //
            // PLAYER CONTROLLED SHIP
            //

            if (GameState.state.elapsedTime.Value - viper.LastCollisionTime.Value < 0.2)
                foreach (var mesh in contentData.viperMarkII.Meshes)
                {
                    foreach (BasicEffect fx in mesh.Effects)
                    {
                        fx.SpecularColor = Color.Orange.ToVector3();
                        fx.AmbientLightColor = Color.Red.ToVector3();
                    }
                }
            else
                foreach (var mesh in contentData.viperMarkII.Meshes)
                {
                    foreach (BasicEffect fx in mesh.Effects)
                    {
                        fx.SpecularColor = Color.White.ToVector3();
                        fx.DiffuseColor = Color.Gray.ToVector3();
                        fx.AmbientLightColor = Color.White.ToVector3();
                    }
                }

            if (viper.Shields.Value > 0)
            {
                contentData.viperMarkII.Draw(
                    basicEffect.World
                    * Matrix.CreateFromQuaternion(Quaternion.CreateFromYawPitchRoll(viper.Roll.Value, 0.0f, 0.0f))
                    * Matrix.CreateTranslation(Conversions.toVector3(viper.Position.Value)),
                    basicEffect.View,
                    basicEffect.Projection
                );
            }

            //
            // ENEMIES
            //

            foreach (Entities.Cylon cylon in cylons)
            {
                IList<Entities.Projectile> cylonColliders = cylon.Colliders.Value.ToList();
                if (cylon.Shields.Value < Constants.cylonShieldsWarning && ((int)gameTime.TotalGameTime.TotalMilliseconds / 150) % 2 == 0)
                 foreach (var mesh in contentData.raider.Meshes)
                  {
                    foreach (BasicEffect fx in mesh.Effects)
                    {
                        fx.SpecularColor = Color.White.ToVector3();
                        fx.AmbientLightColor = Color.Red.ToVector3();
                    }
                  }
                else if (cylonColliders.Count > 0)
                    foreach (var mesh in contentData.raider.Meshes)
                    {
                        foreach (BasicEffect fx in mesh.Effects)
                        {
                            fx.SpecularColor = Color.LightCyan.ToVector3();
                            fx.AmbientLightColor = Color.Cyan.ToVector3();
                        }
                    }
                else
                    foreach (var mesh in contentData.raider.Meshes)
                    {
                        foreach (BasicEffect fx in mesh.Effects)
                        {
                            fx.SpecularColor = Color.Gray.ToVector3();
                            fx.DiffuseColor = Color.Gray.ToVector3();
                            fx.AmbientLightColor = Color.Gray.ToVector3();
                        }
                    }

                contentData.raider.Draw(
                    basicEffect.World
                    * Matrix.CreateFromQuaternion(Quaternion.CreateFromYawPitchRoll(cylon.Roll.Value, 0.0f, 3.14f))
                    * Matrix.CreateTranslation(Conversions.toVector3(cylon.Position.Value)),
                    basicEffect.View,
                    basicEffect.Projection
                );
            }

            this.DrawInterface(gameTime);

            base.Draw(gameTime);
        }

        Vector2 backgroundPosition, cloudsPosition;
        int horizontalScrollDirection = 0;

        //
        // BACKGROUND
        //

        public void DrawBackground(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null);

            if (Shared.InputState.MoveLeft)
                horizontalScrollDirection = 1;
            if (Shared.InputState.MoveRight)
                horizontalScrollDirection = -1;
            if (Shared.InputState.MoveLeft == Shared.InputState.MoveRight || GameState.state.viper.Shields.Value < 1)
                horizontalScrollDirection = 0;

            // Update background position based on player's ship movement
            backgroundPosition.X = backgroundPosition.X + horizontalScrollDirection * (float)gameTime.ElapsedGameTime.TotalSeconds * ContentData.backgroundScrollSpeed;
            //cloudsPosition.X = cloudsPosition.X + horizontalScrollDirection * (float)gameTime.ElapsedGameTime.TotalSeconds * contentData.cloudsScrollSpeed;
            backgroundPosition.Y += (float)gameTime.ElapsedGameTime.TotalSeconds * ContentData.backgroundScrollSpeed;
            cloudsPosition.Y += (float)gameTime.ElapsedGameTime.TotalSeconds * ContentData.cloudsScrollSpeed;

            // Clamp position to avoid overflow
            backgroundPosition.X = backgroundPosition.X % contentData.space.Width;
            backgroundPosition.Y = backgroundPosition.Y % contentData.space.Height;
            cloudsPosition.Y = cloudsPosition.Y % contentData.spaceDust.Height;

            spriteBatch.Draw(
                contentData.space,
                Vector2.Zero,
                new Rectangle((int)backgroundPosition.X, -(int)backgroundPosition.Y, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
                Color.White
            );

            spriteBatch.Draw(
                contentData.spaceDust,
                Vector2.Zero,
                new Rectangle((int)cloudsPosition.X, -(int)cloudsPosition.Y, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height),
                Color.White
            );

            spriteBatch.End();
        }

        //
        // INTERFACE
        //

        public void DrawInterface(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            Color gunColor = Color.Cyan;
            Color cannonInterfaceColor = Color.LimeGreen;
            if (GameState.state.viper.CannonTemperature.Value > 25)
                gunColor = Color.LimeGreen;
            if (GameState.state.viper.CannonTemperature.Value > 50)
                gunColor = Color.Yellow;
            if (GameState.state.viper.CannonTemperature.Value > 80)
                gunColor = Color.Orange;
            if (GameState.state.viper.CannonTemperature.Value > 95)
            {
                gunColor = Color.Red;
                cannonInterfaceColor = Color.Red;
            }

            Color shieldColor = Color.Cyan;
            Color shieldInterfaceColor = Color.LimeGreen;
            switch (GameState.state.viper.Shields.Value) {
                case 4:
                case 3:
                    shieldColor = Color.LimeGreen;
                    break;
                case 2:
                    shieldColor = Color.Yellow;
                    break;
                case 1:
                    shieldColor = Color.Red;
                    shieldInterfaceColor = Color.Red;
                    break;
            }

            int numberOfBars = 30;
            int cannonTemperature = (int)(GameState.state.viper.CannonTemperature.Value / (Constants.cannonMaxTemperature / numberOfBars));
            int shields = numberOfBars - (Constants.vipershields - GameState.state.viper.Shields.Value) * (numberOfBars / Constants.vipershields);

            string gunBar = "";
            for (int c = 0; c < cannonTemperature; c++) gunBar += "|";
            
            string shieldBar = "";
            for (int c = 0; c < shields; c++) shieldBar += "|";

            string msg = "Score: " + ((int)GameState.state.elapsedTime.Value * 10 + GameState.state.cylonsFragged.Value * 100).ToString();

            // advanced sprites tecmology
            string bar = "||||||||||||||||||||||||||||||";

            int temperatureLabelOffset = (int)contentData.gameFont.MeasureString("Temperature: " + bar).X + 10;
            int shieldsLabelOffset = (int)contentData.gameFont.MeasureString("Shields: " + bar).X + 10;
            int rightBarsOffset = (int)contentData.gameFont.MeasureString(bar).X + 10;
            int charHeight = (int)(int)contentData.gameFont.MeasureString("|").Y;

            spriteBatch.DrawString(contentData.gameFont, "Temperature: ", new Vector2(GraphicsDevice.Viewport.Width - temperatureLabelOffset, GraphicsDevice.Viewport.Height - charHeight), cannonInterfaceColor);
            spriteBatch.DrawString(contentData.gameFont, gunBar, new Vector2(GraphicsDevice.Viewport.Width - rightBarsOffset, GraphicsDevice.Viewport.Height - charHeight), gunColor);
            spriteBatch.DrawString(contentData.gameFont, "Shields: ", new Vector2(GraphicsDevice.Viewport.Width - shieldsLabelOffset, GraphicsDevice.Viewport.Height - 2 * charHeight), shieldInterfaceColor);
            spriteBatch.DrawString(contentData.gameFont, shieldBar, new Vector2(GraphicsDevice.Viewport.Width - rightBarsOffset, GraphicsDevice.Viewport.Height - 2 * charHeight), shieldColor);

            int seconds = (int)GameState.state.elapsedTime.Value % 60;
            int minutes = (int)Math.Truncate(GameState.state.elapsedTime.Value/60);

            spriteBatch.DrawString(contentData.gameFont, msg, new Vector2(10, GraphicsDevice.Viewport.Height - 2 * charHeight), Color.LimeGreen);
            spriteBatch.DrawString(contentData.gameFont, "Time elapsed: " + String.Format("{0:00}", minutes) + ':' + String.Format("{0:00}", seconds), new Vector2(10, GraphicsDevice.Viewport.Height - charHeight), Color.LimeGreen);

            if (GameState.state.gameOver.Value) {
                MediaPlayer.Volume = 1.0f;
                spriteBatch.Draw(contentData.gameOver,new Vector2((int)(GraphicsDevice.Viewport.Width - contentData.gameOver.Width)/2, GraphicsDevice.Viewport.Height/2), Color.White);
            }

            spriteBatch.End();
        }

    }
}
