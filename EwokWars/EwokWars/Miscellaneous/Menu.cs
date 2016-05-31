using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EwokWars.Miscellaneous
{
    class Menu
    {
        public enum Screen { MAINMENU, STARTGAME, GAMEOVER, CONTROLLSPAGE, EXITGAME, PAUSEMENU, RESTART, WINNINGSCREEN };
        private enum ButtonStatus { BUTTON1, BUTTON2, BUTTON3, BUTTON4 };

        public Screen CurrentScreen { get; private set; }

        private bool bLevelSolved;
        public bool LevelSolved
        {
            get { return bLevelSolved;  }

            set
            {
                if (value == true)
                {
                    bLevelSolved = true;
                    CurrentScreen = Screen.WINNINGSCREEN;
                }
                else
                {
                    bLevelSolved = false;
                }
            }
        }

        private bool bGameOver;
        public bool GameOver
        {
            get { return bGameOver; }

            set
            {
                if (value == true)
                {
                    bGameOver = true;
                    CurrentScreen = Screen.GAMEOVER;
                }
                else
                {
                    bGameOver = false;
                    CurrentScreen = Screen.STARTGAME;
                }
            }
        }


        private Texture2D background;
        private Texture2D newGameText;
        private Texture2D exitText;
        private Texture2D controlsText;
        private Texture2D button1;
        private Texture2D button2;
        private Texture2D button3;
        private Texture2D button4;
        private Texture2D buttonNormal;
        private Texture2D buttonHighlight;
        private Texture2D controlsPage;
        private Texture2D pause;
        private Texture2D continueText;
        private Texture2D gameOver;
        private Texture2D restart;
        private Texture2D menuImage;
        private Texture2D nextLevel;
        private Texture2D winningScreen;
        
        private Vector2 backgroundPos;
        private Vector2 button1Pos;
        private Vector2 button2Pos;
        private Vector2 button3Pos;
        private Vector2 button4Pos;

        private ButtonStatus buttonStatus;

        private bool downPressed = false;
        private bool upPressed = false;
        private bool switchStatus = false;
        private bool gameStarted = false;

        public void Initialize()
        {
            backgroundPos = Vector2.Zero;
            CurrentScreen = Screen.MAINMENU;
            buttonStatus = ButtonStatus.BUTTON1;

            // u have to change only the value of the first button the rest will follow these changes
            button1Pos = new Vector2(225, 110);
            button2Pos = new Vector2(button1Pos.X, button1Pos.Y + 100);
            button3Pos = new Vector2(button1Pos.X, button1Pos.Y + 200);
            button4Pos = new Vector2(button1Pos.X, button1Pos.Y + 300);
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice gd, DebugKit debugKit = null)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(background, backgroundPos, Color.White);

            switch (CurrentScreen)
            {
                case Screen.MAINMENU:
                    drawStartmenu(spriteBatch);
                    break;
                case Screen.PAUSEMENU:
                    drawPausemenu(spriteBatch);
                    break;
                case Screen.CONTROLLSPAGE:
                    spriteBatch.Draw(controlsPage, backgroundPos, Color.White);
                    break;
                case Screen.GAMEOVER:
                    drawGameOverScreen(spriteBatch);
                    break;
                case Screen.WINNINGSCREEN:
                    drawWinningScreen(spriteBatch);
                    break;
                default:
                    break;
            }

            spriteBatch.End();
        }

        public void LoadContent(ContentManager content)
        {
            //Text
            newGameText = content.Load<Texture2D>("Images/Menu/NewGame");
            exitText = content.Load<Texture2D>("Images/Menu/Exit");
            controlsText = content.Load<Texture2D>("Images/Menu/Controls");
            continueText = content.Load<Texture2D>("Images/Menu/Continue");
            restart = content.Load<Texture2D>("Images/Menu/Restart");
            nextLevel = content.Load<Texture2D>("Images/Menu/NextLevel");

            //Buttons
            buttonNormal = content.Load<Texture2D>("Images/Menu/ButtonNormal");
            buttonHighlight = content.Load<Texture2D>("Images/Menu/ButtonHighlight");

            //Backgrounds
            controlsPage = content.Load<Texture2D>("Images/Menu/ControlsPage");
            menuImage = content.Load<Texture2D>("Images/Menu/Menu");
            gameOver = content.Load<Texture2D>("Images/Menu/GameOver");
            pause = content.Load<Texture2D>("Images/Menu/Pause");
            winningScreen = content.Load<Texture2D>("Images/Menu/YouWon");

            button1 = buttonNormal;
            button2 = buttonHighlight;
            button3 = buttonNormal;
            button4 = buttonNormal;
            background = menuImage;

        }

        public void Update(GameTime gameTime)
        {
            ChangeSwitchStatus();
            switch (CurrentScreen)
            {
                case Screen.MAINMENU:
                    background = menuImage;
                    updateStartmenu();
                    break;
                case Screen.GAMEOVER:               
                    background = gameOver;
                    updateGameOverScreen();
                    break;
                case Screen.PAUSEMENU:
                    background = pause;
                    updatePausemenu();
                    break;
                case Screen.WINNINGSCREEN:
                    background = winningScreen;
                    updateWinningScreen();
                    break;
            }

            HighlightButton();
        }
       
        // for ENTER AND ESC
        private void ChangeSwitchStatus()
        {
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Enter))
                switchStatus = true;
            else
                switchStatus = false;
            
            if (keyState.IsKeyDown(Keys.Escape) && CurrentScreen != Screen.GAMEOVER)
            {
                switchStatus = false;

                if (gameStarted == false)
                    CurrentScreen = Screen.MAINMENU;
                else
                    CurrentScreen = Screen.PAUSEMENU;
            }
        }

        // how the menus look like
        private void drawStartmenu(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(button1, button1Pos, Color.White);
            spriteBatch.Draw(newGameText, button1Pos, Color.White);

            spriteBatch.Draw(button2, button2Pos, Color.White);
            spriteBatch.Draw(controlsText, button2Pos, Color.White);

            spriteBatch.Draw(button3, button3Pos, Color.White);
            spriteBatch.Draw(exitText, button3Pos, Color.White);
        }

        private void drawPausemenu(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(button1, button1Pos, Color.White);
            spriteBatch.Draw(continueText, button1Pos, Color.White);

            spriteBatch.Draw(button2, button2Pos, Color.White);
            spriteBatch.Draw(newGameText, button2Pos, Color.White);

            spriteBatch.Draw(button3, button3Pos, Color.White);
            spriteBatch.Draw(controlsText, button3Pos, Color.White);

            spriteBatch.Draw(button4, button4Pos, Color.White);
            spriteBatch.Draw(exitText, button4Pos, Color.White);
        }

        private void drawGameOverScreen(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(button1, button2Pos, Color.White);
            spriteBatch.Draw(restart, button2Pos, Color.White);

            spriteBatch.Draw(button2, button3Pos, Color.White);
            spriteBatch.Draw(exitText, button3Pos, Color.White);
        }

        private void drawWinningScreen(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(button1, button1Pos, Color.White);
            spriteBatch.Draw(restart, button1Pos, Color.White);

            spriteBatch.Draw(button3, button2Pos, Color.White);
            spriteBatch.Draw(nextLevel, button2Pos, Color.White);

            spriteBatch.Draw(button2, button3Pos, Color.White);
            spriteBatch.Draw(exitText, button3Pos, Color.White);
        }

        //update menus
        private void updateStartmenu() 
        {
            ChangeButtonStatus3();

            if (switchStatus)
            {
                switch (buttonStatus)
                {
                    case ButtonStatus.BUTTON1:
                        CurrentScreen = Screen.STARTGAME;
                        gameStarted = true;
                        break;
                    case ButtonStatus.BUTTON2:
                        CurrentScreen = Screen.CONTROLLSPAGE;
                        break;
                    case ButtonStatus.BUTTON3:
                        CurrentScreen = Screen.EXITGAME;
                        break;
                    default:
                        break;
                }
            }
        }

        private void updatePausemenu() 
        {
            ChangeButtonStatus4();

            if (switchStatus)
            {
                switch (buttonStatus)
                {
                    case ButtonStatus.BUTTON1:
                        CurrentScreen = Screen.STARTGAME;
                        break;
                    case ButtonStatus.BUTTON2:
                        CurrentScreen = Screen.RESTART;
                        break;
                    case ButtonStatus.BUTTON3:
                        CurrentScreen = Screen.CONTROLLSPAGE;
                        break;
                    case ButtonStatus.BUTTON4:
                        CurrentScreen = Screen.EXITGAME;
                        break;
                    default:
                        break;
                }
            }
        }

        private void updateGameOverScreen()
        {
            ChangeButtonStatus2();
            
            if (switchStatus)
            {
                switch (buttonStatus)
                {
                    case ButtonStatus.BUTTON1:
                        CurrentScreen = Screen.RESTART;
                        //GameOver = false;
                        break;
                    case ButtonStatus.BUTTON2:
                        CurrentScreen = Screen.EXITGAME;
                        break;
                    default:
                        break;
                }
            }
        }

        private void updateWinningScreen() 
        {
            ChangeButtonStatus2();

            if (switchStatus)
            {
                switch (buttonStatus)
                {
                    case ButtonStatus.BUTTON1:
                        CurrentScreen = Screen.RESTART;
                        LevelSolved = false;
                        break;
                    case ButtonStatus.BUTTON2:
                        CurrentScreen = Screen.EXITGAME;
                        break;
                    default:
                        break;
                }
            }
        }

        private void HighlightButton()
        {
            switch (buttonStatus)
            {
                case ButtonStatus.BUTTON1:
                    button1 = buttonHighlight;
                    button2 = buttonNormal;
                    button3 = buttonNormal;
                    button4 = buttonNormal;
                    break;
                case ButtonStatus.BUTTON2:
                    button1 = buttonNormal;
                    button2 = buttonHighlight;
                    button3 = buttonNormal;
                    button4 = buttonNormal;
                    break;
                case ButtonStatus.BUTTON3:
                    button1 = buttonNormal;
                    button2 = buttonNormal;
                    button3 = buttonHighlight;
                    button4 = buttonNormal;
                    break;
                case ButtonStatus.BUTTON4:
                    button1 = buttonNormal;
                    button2 = buttonNormal;
                    button3 = buttonNormal;
                    button4 = buttonHighlight;
                    break;
                default:
                    break;
            }
        }

        // diffrent buttons counts
        private void ChangeButtonStatus2()
        {
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Down))
                downPressed = true;

            if (keyState.IsKeyDown(Keys.Up))
                upPressed = true;

            if (keyState.IsKeyUp(Keys.Down) && downPressed == true)
            {
                switch (buttonStatus)
                {
                    case ButtonStatus.BUTTON1:
                        buttonStatus = ButtonStatus.BUTTON2;
                        downPressed = false;
                        break;
                    case ButtonStatus.BUTTON2:
                        buttonStatus = ButtonStatus.BUTTON1;
                        downPressed = false;
                        break;
                    default:
                        break;
                }
            }

            if (keyState.IsKeyUp(Keys.Up) && upPressed == true)
            {
                switch (buttonStatus)
                {
                    case ButtonStatus.BUTTON1:
                        buttonStatus = ButtonStatus.BUTTON2;
                        upPressed = false;
                        break;
                    case ButtonStatus.BUTTON2:
                        buttonStatus = ButtonStatus.BUTTON1;
                        upPressed = false;
                        break;
                    default:
                        break;
                }
            }
        }

        private void ChangeButtonStatus3()
        {
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Down))
                downPressed = true;

            if (keyState.IsKeyDown(Keys.Up))
                upPressed = true;

            if (keyState.IsKeyUp(Keys.Down) && downPressed == true)
            {
                switch (buttonStatus)
                {
                    case ButtonStatus.BUTTON1:
                        buttonStatus = ButtonStatus.BUTTON2;
                        downPressed = false;
                        break;
                    case ButtonStatus.BUTTON2:
                        buttonStatus = ButtonStatus.BUTTON3;
                        downPressed = false;
                        break;
                    case ButtonStatus.BUTTON3:
                        buttonStatus = ButtonStatus.BUTTON1;
                        downPressed = false;
                        break;
                    default:
                        break;
                }
            }

            if (keyState.IsKeyUp(Keys.Up) && upPressed == true)
            {
                switch (buttonStatus)
                {
                    case ButtonStatus.BUTTON1:
                        buttonStatus = ButtonStatus.BUTTON3;
                        upPressed = false;
                        break;
                    case ButtonStatus.BUTTON2:
                        buttonStatus = ButtonStatus.BUTTON1;
                        upPressed = false;
                        break;
                    case ButtonStatus.BUTTON3:
                        buttonStatus = ButtonStatus.BUTTON2;
                        upPressed = false;
                        break;
                    default:
                        break;
                }
            }
        }

        private void ChangeButtonStatus4()
        {
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Down))
                downPressed = true;

            if (keyState.IsKeyDown(Keys.Up))
                upPressed = true;

            if (keyState.IsKeyUp(Keys.Down) && downPressed == true)
            {
                switch (buttonStatus)
                {
                    case ButtonStatus.BUTTON1:
                        buttonStatus = ButtonStatus.BUTTON2;
                        downPressed = false;
                        break;
                    case ButtonStatus.BUTTON2:
                        buttonStatus = ButtonStatus.BUTTON3;
                        downPressed = false;
                        break;
                    case ButtonStatus.BUTTON3:
                        buttonStatus = ButtonStatus.BUTTON4;
                        downPressed = false;
                        break;
                    case ButtonStatus.BUTTON4:
                        buttonStatus = ButtonStatus.BUTTON1;
                        downPressed = false;
                        break;
                    default:
                        break;
                }
            }

            if (keyState.IsKeyUp(Keys.Up) && upPressed == true)
            {
                switch (buttonStatus)
                {
                    case ButtonStatus.BUTTON1:
                        buttonStatus = ButtonStatus.BUTTON4;
                        upPressed = false;
                        break;
                    case ButtonStatus.BUTTON2:
                        buttonStatus = ButtonStatus.BUTTON1;
                        upPressed = false;
                        break;
                    case ButtonStatus.BUTTON3:
                        buttonStatus = ButtonStatus.BUTTON2;
                        upPressed = false;
                        break;
                    case ButtonStatus.BUTTON4:
                        buttonStatus = ButtonStatus.BUTTON3;
                        upPressed = false;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
