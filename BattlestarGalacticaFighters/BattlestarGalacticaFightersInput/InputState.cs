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


namespace BattlestarGalacticaFightersInput
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class InputState : Microsoft.Xna.Framework.GameComponent
    {
        public InputState(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        KeyboardState prev_kb = new KeyboardState();
        GamePadState prev_gamepad = new GamePadState();

        public static bool FireCannon;
        public static bool MoveUp;
        public static bool MoveRight;
        public static bool MoveDown;
        public static bool MoveLeft;

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            GamePadState gamepad = GamePad.GetState(PlayerIndex.One);

            if (
                (gamepad.Buttons.A == ButtonState.Pressed && gamepad.Buttons.A != ButtonState.Pressed) ||
                (keyboard.IsKeyDown(Keys.Space) && prev_kb.IsKeyUp(Keys.Space))
            )
                FireCannon = true;

            if (
                (gamepad.Buttons.LeftStick == ButtonState.Pressed) ||
                (keyboard.IsKeyDown(Keys.Left))
            )
                MoveLeft = true;

            if (
                (gamepad.Buttons.RightStick == ButtonState.Pressed) ||
                (keyboard.IsKeyDown(Keys.Right))
            )
                MoveRight = true;

            if (
                (gamepad.Buttons.LeftStick == ButtonState.Pressed) ||
                (keyboard.IsKeyDown(Keys.Up))
            )
                MoveUp = true;

            if (
                (gamepad.Buttons.RightStick == ButtonState.Pressed) ||
                (keyboard.IsKeyDown(Keys.Down))
            )
                MoveDown = true;

            prev_kb = keyboard;
            prev_gamepad = gamepad;

            base.Update(gameTime);
        }

    }
}
