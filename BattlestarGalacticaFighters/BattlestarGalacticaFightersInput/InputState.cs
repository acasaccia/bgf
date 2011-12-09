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

        // These will be accessed by logic and will mask different input methods
        public static bool FireCannon;
        public static bool MoveUp;
        public static bool MoveRight;
        public static bool MoveDown;
        public static bool MoveLeft;

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            GamePadState gamepad = GamePad.GetState(PlayerIndex.One);

            FireCannon = (gamepad.Buttons.A == ButtonState.Pressed) || (keyboard.IsKeyDown(Keys.LeftControl));
            MoveRight = (gamepad.ThumbSticks.Left.X < 0) || (keyboard.IsKeyDown(Keys.Left));
            MoveLeft = (gamepad.ThumbSticks.Left.X > 0) || (keyboard.IsKeyDown(Keys.Right));
            MoveUp = (gamepad.ThumbSticks.Left.Y > 0) || (keyboard.IsKeyDown(Keys.Up));
            MoveDown = (gamepad.ThumbSticks.Left.Y < 0) || (keyboard.IsKeyDown(Keys.Down));

            base.Update(gameTime);
        }

    }
}
