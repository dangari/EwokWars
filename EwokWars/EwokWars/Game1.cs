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

namespace EwokWars
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Levels.LevelParser levelParser;
        Levels.Level currentLevel;
        Miscellaneous.Menu menu = new Miscellaneous.Menu();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            menu.Initialize();
            levelParser = new Levels.LevelParser();

            currentLevel = levelParser.Parse("level1.xml");
            currentLevel.Initialize(GraphicsDevice.Viewport);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {        
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            menu.LoadContent(this.Content);
            currentLevel.LoadContent(this.Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (currentLevel.Hero.IsAlive == false)
                menu.GameOver = true;
            else if (currentLevel.Cleared == true)
                menu.LevelSolved = true;

            menu.Update(gameTime);

            switch (menu.CurrentScreen)
            {
                case Miscellaneous.Menu.Screen.STARTGAME:
                    currentLevel.Update(gameTime);
                    break;
                case Miscellaneous.Menu.Screen.EXITGAME:
                    Exit();
                    break;
                case Miscellaneous.Menu.Screen.RESTART:
                    currentLevel.Reset();
                    menu.GameOver = false;
                    break;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            if (menu.CurrentScreen == EwokWars.Miscellaneous.Menu.Screen.STARTGAME)
                currentLevel.Draw(spriteBatch, GraphicsDevice);
            else
                menu.Draw(spriteBatch, GraphicsDevice);

            base.Draw(gameTime);
        }
    }
}
