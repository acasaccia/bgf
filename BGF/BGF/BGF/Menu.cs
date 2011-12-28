using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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
  public class Menu : Microsoft.Xna.Framework.DrawableGameComponent
  {
    public Menu(Game game)
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
    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        base.LoadContent();
    }

    KeyboardState previousKeyboard;
    GamePadState previousGamepad;
    Stream stream;
    BinaryFormatter binaryFormatter;
    bool displayHelp;
    public override void Update(GameTime gameTime)
    {
        //
        // UPDATE MENU ENTRIES BASED ON GAMESTATE
        //
        menuEntries[RESUME_GAME].enabled = GameState.state.elapsedTime.Value > 0 && !GameState.state.gameOver.Value;
        menuEntries[SAVE_GAME].enabled = GameState.state.elapsedTime.Value > 0 && !GameState.state.gameOver.Value;
        // check that savegame exists
        menuEntries[LOAD_GAME].enabled = File.Exists("savegame");


        //
        // MENU INPUT
        //

        KeyboardState keyboard = Keyboard.GetState();
        GamePadState gamepad = GamePad.GetState(PlayerIndex.One);

        if ((gamepad.ThumbSticks.Left.Y > 0 && !(previousGamepad.ThumbSticks.Left.Y > 0)) || (keyboard.IsKeyDown(Keys.Up) && !previousKeyboard.IsKeyDown(Keys.Up)))
            do {
                selectedEntry = (selectedEntry - 1 + menuEntries.Length) % menuEntries.Length;
            } while (!menuEntries[selectedEntry].enabled);

        if ((gamepad.ThumbSticks.Left.Y < 0 && !(previousGamepad.ThumbSticks.Left.Y < 0)) || (keyboard.IsKeyDown(Keys.Down) && !previousKeyboard.IsKeyDown(Keys.Down)))
            do {
                selectedEntry = (selectedEntry + 1 + menuEntries.Length) % menuEntries.Length;
            } while (!menuEntries[selectedEntry].enabled);

        if ((gamepad.Buttons.A == ButtonState.Pressed && !(previousGamepad.Buttons.A == ButtonState.Pressed)) || (keyboard.IsKeyDown(Keys.Enter) && !previousKeyboard.IsKeyDown(Keys.Enter)))
        {

            //
            // MENU ACTIONS
            //

            switch (selectedEntry) {
                case NEW_GAME:
                    GameState.reset();
                    Casanova.commit_variable_updates();
                    this.ResumeGameComponents();
                    break;
                case RESUME_GAME:
                    this.ResumeGameComponents();
                    break;
                case LOAD_GAME:
                    Entities.GameState state;
                    stream = File.Open("savegame", FileMode.Open);
                    binaryFormatter = new BinaryFormatter();
                    state = (Entities.GameState) binaryFormatter.Deserialize(stream);
                    stream.Close();
                    GameState.set(state);
                    Casanova.commit_variable_updates();
                    this.ResumeGameComponents();
                    break;
                case SAVE_GAME:
                    stream = File.Open("savegame", FileMode.Create);
                    binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(stream, GameState.state);
                    stream.Close();
                    break;
                case HELP:
                    displayHelp = !displayHelp;
                    break;
                case EXIT_GAME:
                    Game.Exit();
                    break;
            }

        }

        previousKeyboard = keyboard;
        previousGamepad = gamepad;

        base.Update(gameTime);
    }

    public struct MenuEntry
    {
        public string label;
        public bool enabled;
        public bool selected;
        public MenuEntry(string p1, bool p2, bool p3 )
        {
            label = p1;
            enabled = p2;
            selected= p3;
        }
    }

    const int NEW_GAME = 0;
    const int RESUME_GAME = 1;
    const int LOAD_GAME = 2;
    const int SAVE_GAME = 3;
    const int HELP = 4;
    const int EXIT_GAME = 5;

    MenuEntry[] menuEntries = new MenuEntry[6] {
        new MenuEntry("New game", true, true),
        new MenuEntry("Resume game", false, false),
        new MenuEntry("Load game", false, false),
        new MenuEntry("Save game", false, false),
        new MenuEntry("Help", true, false),
        new MenuEntry("Exit", true, false)
    };

    int selectedEntry = 0;
    public override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.Black);
      spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
      spriteBatch.Draw(contentData.logo, new Rectangle((GraphicsDevice.Viewport.Width - contentData.logo.Width) / 2, 0, contentData.logo.Width, contentData.logo.Height), Color.White);

      if (displayHelp)
      {
          string[] helpMsg = { "CONTROLS (ENTER to return to Main menu)",
                               "Menu:",
                               "Select      Gamepad A           Keyboard ENTER",
                               "Move        Gamepad Left Stick  Keyboard UP,DOWN",
                               "In game:",
                               "Move        Gamepad A           Keyboard ENTER",
                               "Select      Gamepad Left Stick  Keyboard Arrow UP,RIGHT,DOWN,LEFT",
                               "Menu        Gamepad START       Keyboard ESC" };
          
          int helpMsgWidth = (int)(contentData.menuHelpFont.MeasureString(helpMsg[6]).X);

          for (int i = 0; i < helpMsg.Length; i++)
          {
              spriteBatch.DrawString(contentData.menuHelpFont, helpMsg[i], new Vector2((GraphicsDevice.Viewport.Width - helpMsgWidth ) / 2, contentData.logo.Height + contentData.menuFont.MeasureString("X").Y * i), Color.White);
          }
      }
      else
      {
          Color entryColor;
          for (int i = 0; i < menuEntries.Length; i++)
          {
              if (!menuEntries[i].enabled)
                  entryColor = Color.Gray;
              else
                  entryColor = Color.White;
              spriteBatch.DrawString(contentData.menuFont, menuEntries[i].label, new Vector2((GraphicsDevice.Viewport.Width - contentData.menuFont.MeasureString(menuEntries[i].label).X) / 2, contentData.logo.Height + contentData.menuFont.MeasureString("O").Y * i), entryColor);
          }
          spriteBatch.DrawString(contentData.menuFont, "->", new Vector2((GraphicsDevice.Viewport.Width - contentData.menuFont.MeasureString(menuEntries[selectedEntry].label).X) / 2 - 50, contentData.logo.Height + contentData.menuFont.MeasureString("O").Y * selectedEntry), Color.White);
      }

      spriteBatch.End();
      base.Draw(gameTime);
    }

    public void ResumeGameComponents()
    {
        while (Game.Components.Count > 0)
        {
            ((GameComponent)Game.Components[0]).Dispose();
        }
        Game.Components.Clear();
        Game.Components.Add(new LogicBridge(Game));
        Game.Components.Add(new Rendering(Game));
        Game.Components.Add(new Audio(Game));
        Game.Components.Add(new Shared.InputState(Game));
        MediaPlayer.Volume = 0.3f;
        MediaPlayer.Play(contentData.inGame);
        #if DEBUG
                Game.Components.Add(new FrameRateCounter(Game));
        #endif
    }

  }
}
