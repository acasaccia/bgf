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
        event Action FireCannon;
        event Action FireMissile;
        event Action MoveLeft;
        event Action MoveRight;
        event Action MoveUp;
        event Action MoveDown;
        event Action CycleTargetRight;
        event Action CycleTargetLeft;

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
        GamePadState prev_gamepad = new GamePadState();
        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            GamePadState gamepad = GamePad.GetState(PlayerIndex.One);

            if (
                (gamepad.Buttons.A == ButtonState.Pressed && gamepad.Buttons.A != ButtonState.Pressed) ||
                (keyboard.IsKeyDown(Keys.Space) && prev_kb.IsKeyUp(Keys.Space) )
            )
                if (FireCannon != null) FireCannon();

            if (
                (gamepad.Buttons.LeftStick == ButtonState.Pressed ) ||
                (keyboard.IsKeyDown(Keys.Left))
            )
                if (MoveLeft != null) MoveLeft();

            if (
                (gamepad.Buttons.RightStick == ButtonState.Pressed) ||
                (keyboard.IsKeyDown(Keys.Right))
            )
                if (MoveRight != null) MoveRight();

            if (
                (gamepad.Buttons.LeftStick == ButtonState.Pressed) ||
                (keyboard.IsKeyDown(Keys.Up))
            )
                if (MoveUp != null) MoveUp();

            if (
                (gamepad.Buttons.RightStick == ButtonState.Pressed) ||
                (keyboard.IsKeyDown(Keys.Down))
            )
                if (MoveDown != null) MoveDown();



            /*
            if ( keyboard.IsKeyDown(Keys.Space) && prev_kb.IsKeyUp(Keys.Space) )
                if (Shoot != null) Shoot();

            if (keyboard.IsKeyDown(Keys.Space) && prev_kb.IsKeyUp(Keys.Space))
                if (Shoot != null) Shoot();

            if (keyboard.IsKeyDown(Keys.Space) && prev_kb.IsKeyUp(Keys.Space))
                if (Shoot != null) Shoot();

            if (keyboard.IsKeyDown(Keys.Space) && prev_kb.IsKeyUp(Keys.Space))
                if (Shoot != null) Shoot();
             */

            prev_kb = keyboard;
            prev_gamepad = gamepad;

            base.Update(gameTime);
        }

        public event Action FireCannon;
        public event Action FireMissile;
        public event Action MoveLeft;
        public event Action MoveRight;
        public event Action MoveUp;
        public event Action MoveDown;
        public event Action CycleTargetRight;
        public event Action CycleTargetLeft;
    }
}
