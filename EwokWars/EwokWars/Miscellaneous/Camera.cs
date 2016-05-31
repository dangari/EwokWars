using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EwokWars.Miscellaneous
{
    class Camera
    {
        private Matrix transformation;
        private Vector2 pos;
        private int viewportWidth;
        private int viewportHeight;
        private int worldWidth;
        private int worldHeight;
        
        public Camera(Viewport viewport, int worldWidth, int worldHeight)
        {
            pos = Vector2.Zero;
            viewportWidth = viewport.Width;
            viewportHeight = viewport.Height;
            this.worldWidth = worldWidth;
            this.worldHeight = worldHeight;
        }

        public void Move(Vector2 amount)
        {
            pos += amount;
        }

        public Vector2 WorldDimensions
        {
            get 
            { 
                return new Vector2(worldWidth, worldHeight); 
            }
            set 
            { 
                worldWidth = (int)value.X; 
                worldHeight = (int)value.Y; 
            }
        }

        public Vector2 Pos
        {
           get { return pos; }
           set
           {
               // Prevent the camera from leaving the playable area

               float leftBarrier = (float) viewportWidth * 0.5f;
               float rightBarrier = worldWidth - (float) viewportWidth * 0.5f;
               float topBarrier = worldHeight - (float) viewportHeight * 0.5f;
               float bottomBarrier = (float) viewportHeight * 0.5f;

               pos = value;

               if (pos.X < leftBarrier)
                   pos.X = leftBarrier;
               if (pos.X > rightBarrier)
                   pos.X = rightBarrier;
               if (pos.Y > topBarrier)
                   pos.Y = topBarrier;
               if (pos.Y < bottomBarrier)
                   pos.Y = bottomBarrier;
            }
        }

        // Returns the view matrix. SpriteBatch.Begin() uses this matrix to move the world accordingly.
        // Yes that's right, we move the world, not the camera.
        public Matrix GetTransformation()
        {
            transformation =
                Matrix.CreateTranslation(new Vector3(-pos.X, -pos.Y, 0)) *  // Move the world, so that Hero is placed 
                                                                            // in the viewports' top-left corner

                Matrix.CreateTranslation(new Vector3(viewportWidth * 0.5f,  // Move the world again to ensure that 
                viewportHeight * 0.5f, 0));                                 // Hero is placed in the viewports' center
                
            return transformation;
        }
    }
}
