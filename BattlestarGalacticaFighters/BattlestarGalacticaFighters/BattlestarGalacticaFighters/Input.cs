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
    public interface IInputService
    {
        event Action Shoot;
    }

    public class Input : Microsoft.Xna.Framework.GameComponent, IInputService
    {
        public Input(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IInputService), this);
        }

        protected override void Dispose(bool disposing)
        {
            Game.Services.RemoveService(typeof(IInputService));
            base.Dispose(disposing);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        KeyboardState prev_kb = new KeyboardState();
        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();

            if ( keyboard.IsKeyDown(Keys.Space) && prev_kb.IsKeyUp(Keys.Space) )
                if (Shoot != null) Shoot();

            prev_kb = keyboard;

            base.Update(gameTime);
        }

        public event Action Shoot;
    }
}
