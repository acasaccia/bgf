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

    // Masks different Input devices and makes input state available to
    // F# project
    public static class InputState
    {
        public static bool MoveTop;
        public static bool MoveRight;
        public static bool MoveBottom;
        public static bool MoveLeft;
    }

    public class Input : Microsoft.Xna.Framework.GameComponent
    {
        public Input(Game game)
            : base(game)
        {
        
        }

        protected override void Dispose(bool disposing)
        {
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

            prev_kb = keyboard;
            prev_gamepad = gamepad;

            base.Update(gameTime);
        }

        public static bool FireCannon;
        public event Action FireMissile;
        public event Action MoveLeft;
        public event Action MoveRight;
        public event Action MoveUp;
        public event Action MoveDown;
        public event Action CycleTargetRight;
        public event Action CycleTargetLeft;
    }
}
