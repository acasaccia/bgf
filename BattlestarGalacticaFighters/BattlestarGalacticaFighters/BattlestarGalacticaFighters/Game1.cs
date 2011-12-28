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

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            #if DEBUG
                graphics.IsFullScreen = false;
            #endif
            graphics.PreferredBackBufferWidth = 1270;
            graphics.PreferredBackBufferHeight = 720;
            Content.RootDirectory = "Content";
            this.Services.AddService(typeof(ContentData), contentData);
        }

        protected override void Initialize()
        {
            Components.Add(new Menu(this));
            base.Initialize();
        }

        ContentData contentData = new ContentData();
        SpriteBatch spriteBatch;
        protected override void LoadContent()
        {
            ContentData.viewPort = GraphicsDevice.Viewport;

            spriteBatch = new SpriteBatch(GraphicsDevice);
            contentData.gameFont = Content.Load<SpriteFont>("interface");
            contentData.menuFont = Content.Load<SpriteFont>("menu");

            spriteBatch = new SpriteBatch(GraphicsDevice);
            contentData.logo = Content.Load<Texture2D>("logo");
            contentData.menu = Content.Load<Song>("Battlestar Galactica Opening");
            contentData.inGame = Content.Load<Song>("All Along The Watchtower");

            // Projectiles and fx
            contentData.explosion = Content.Load<Texture2D>("explosion_animation");
            contentData.laser = Content.Load<Texture2D>("laser");
            contentData.energyCells = Content.Load<Texture2D>("energy_cells");

            // Scrolling background
            contentData.space = Content.Load<Texture2D>("space");
            contentData.spaceDust = Content.Load<Texture2D>("space_dust");

            // 3d models
            contentData.viperMarkII = Content.Load<Model>("Viper_Mk_II");
            BoundingSphere boundingSphere = new BoundingSphere();
            foreach (var mesh in contentData.viperMarkII.Meshes)
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
            ContentData.viperMarkIIBoundingRadius = boundingSphere.Radius;

            contentData.raider = Content.Load<Model>("Cylon_Raider");
            boundingSphere = new BoundingSphere();
            foreach (var mesh in contentData.raider.Meshes)
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
            ContentData.raiderBoundingRadius = boundingSphere.Radius;

            // Initialize menu background music
            MediaPlayer.Volume = 1.0f;
            MediaPlayer.Play(contentData.menu);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                while (Components.Count > 0)
                {
                    ((GameComponent) Components[0]).Dispose();
                }
                Components.Clear();
                Components.Add(new Menu(this));
                MediaPlayer.Volume = 1.0f;
                MediaPlayer.Play(contentData.menu);
            }

            base.Update(gameTime);
        }

    }
}
