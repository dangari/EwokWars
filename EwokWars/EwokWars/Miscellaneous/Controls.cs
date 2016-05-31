using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using EwokWars.Objects;

namespace EwokWars.Miscellaneous
{
    class Controls
    {
        private const float SQRT_2 = 1.414f;

        private KeyToViewMapper keyToViewMapper;
        private Hero hero;

        public Controls(Hero hero)
        {
            this.hero = hero;
        }

        private struct KeyToViewMapper
        {
            private Hero.ViewDirection currentView;

            public Hero.ViewDirection getView() { return currentView; }

            public KeyToViewMapper(KeyboardState keyState)
            {
                currentView = Hero.ViewDirection.NONE;

                if (keyState.IsKeyDown(Keys.Right))
                    currentView = Hero.ViewDirection.RIGHT;

                if (keyState.IsKeyDown(Keys.Left))
                    currentView = Hero.ViewDirection.LEFT;

                if (keyState.IsKeyDown(Keys.Down))
                    currentView = Hero.ViewDirection.DOWN;

                if (keyState.IsKeyDown(Keys.Up))
                    currentView = Hero.ViewDirection.UP;

                if (keyState.IsKeyDown(Keys.Right) && keyState.IsKeyDown(Keys.Down))
                    currentView = Hero.ViewDirection.DOWNRIGHT;

                if (keyState.IsKeyDown(Keys.Left) && keyState.IsKeyDown(Keys.Down))
                    currentView = Hero.ViewDirection.DOWNLEFT;

                if (keyState.IsKeyDown(Keys.Right) && keyState.IsKeyDown(Keys.Up))
                    currentView = Hero.ViewDirection.UPRIGHT;

                if (keyState.IsKeyDown(Keys.Left) && keyState.IsKeyDown(Keys.Up))
                    currentView = Hero.ViewDirection.UPLEFT;

            }
        }


        public void UpdatePosition(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            keyToViewMapper = new KeyToViewMapper(keyState);
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 newPosition = hero.Pos;


            switch (keyToViewMapper.getView())
            {
                case Hero.ViewDirection.UP:
                    newPosition.Y -= hero.Speed * seconds;
                    break;
                case Hero.ViewDirection.DOWN:
                    newPosition.Y += hero.Speed * seconds;
                    break;
                case Hero.ViewDirection.RIGHT:
                    newPosition.X += hero.Speed * seconds;
                    break;
                case Hero.ViewDirection.LEFT:
                    newPosition.X -= hero.Speed * seconds;
                    break;
                case Hero.ViewDirection.DOWNRIGHT:
                    newPosition.X += hero.Speed / SQRT_2 * seconds;
                    newPosition.Y += hero.Speed / SQRT_2 * seconds;
                    break;
                case Hero.ViewDirection.DOWNLEFT:
                    newPosition.X -= hero.Speed / SQRT_2 * seconds;
                    newPosition.Y += hero.Speed / SQRT_2 * seconds;
                    break;
                case Hero.ViewDirection.UPRIGHT:
                    newPosition.X += hero.Speed / SQRT_2 * seconds;
                    newPosition.Y -= hero.Speed / SQRT_2 * seconds;
                    break;
                case Hero.ViewDirection.UPLEFT:
                    newPosition.X -= hero.Speed / SQRT_2 * seconds;
                    newPosition.Y -= hero.Speed / SQRT_2 * seconds;
                    break;
                default:
                    break;

            }

            hero.Direction = newPosition - hero.Pos;
            hero.Pos = newPosition;
            
            if (keyToViewMapper.getView() == Hero.ViewDirection.DOWN ||
                keyToViewMapper.getView() == Hero.ViewDirection.UP ||
                keyToViewMapper.getView() == Hero.ViewDirection.LEFT ||
                keyToViewMapper.getView() == Hero.ViewDirection.RIGHT)
                hero.CurrentDirection = keyToViewMapper.getView();    
        }

        public bool attack()
        {
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Space))
                return true;
            return false;
        }
    }
}