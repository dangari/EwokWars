using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EwokWars.Objects
{
    public abstract class DynamicObjects : GameObject
    {
        public enum ViewDirection { UP, DOWN, RIGHT, LEFT, DOWNRIGHT, DOWNLEFT, UPRIGHT, UPLEFT, NONE };
        public ViewDirection CurrentDirection { get; set; }

        protected class PushBack
        {
            public float Intensity { get; set; }
            public int Time { get; set; }
            public Vector2 Direction { private get; set; }
            private DynamicObjects outter; // Pointer to the instantiated object

            public PushBack(DynamicObjects thisptr)
            {
                this.outter = thisptr;
                Intensity = 700f;
            }

            public void Update(GameTime gameTime)
            {
                Vector2 newPos = outter.Pos;
                float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
                newPos.X += seconds * Intensity * Direction.X;
                newPos.Y += seconds * Intensity * Direction.Y;
                outter.Pos = newPos;
                Time--;
            }
        }

        // You can use this object to perform "pushback" animations on your
        // derived object
        protected PushBack pushBack;    
    }
}
