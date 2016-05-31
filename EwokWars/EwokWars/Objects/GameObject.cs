using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace EwokWars.Objects
{
    public abstract class GameObject
    {
        protected const float PI = 3.14159f;
        
        public enum SpriteOrigin { CENTER, TOP_LEFT, TOP_RIGHT, BOTTOM_LEFT, BOTTOM_RIGHT };
        virtual public Texture2D Texture { get; set; }

        private Vector2 pos;
        virtual public Vector2 Pos
        {
            get { return pos; }
            set { pos = value; }
        }

        protected SpriteOrigin spriteOrigin;
        private bool isMiddlePointSet = false;
        virtual public Rectangle BoundingBox { get; protected set; }

        private Vector2 middlePoint;
        virtual public Vector2 MiddlePoint
        {
            get { return middlePoint; }
            protected set
            {
                middlePoint = value;
                isMiddlePointSet = true;
            }
        }
        
        private float rotation;
        virtual public float Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value * PI / 180f;
            }
        }

        public abstract void Initialize();       
        public abstract void Update(GameTime gameTime);
        public abstract void LoadContent(ContentManager content);
        public abstract void OnCollision(GameObject other);

        public virtual void Draw(SpriteBatch spriteBatch, GraphicsDevice gd, DebugKit debugKit = null)
        {
            spriteBatch.Draw(Texture, Pos, null, Color.White, Rotation, MiddlePoint, 1.0f, SpriteEffects.None, 0f);
            
            if(debugKit != null)
                debugKit.DrawBBox(BoundingBox, gd, spriteBatch);
        }

        public virtual bool Intersects(GameObject other)
        {
            if (other.BoundingBox.Intersects(this.BoundingBox))
                return true;

            return false;
        }

        protected virtual void UpdateBoundingBox(Rectangle boundingBox = default(Rectangle))
        {
            if (boundingBox != default(Rectangle)) // Apply user-defined boundingBox
            {
                BoundingBox = boundingBox;

                if (!isMiddlePointSet)
                {
                    switch (spriteOrigin)
                    {
                        case SpriteOrigin.CENTER:
                            MiddlePoint = new Vector2(Texture.Width / 2, Texture.Height / 2);
                            break;
                        case SpriteOrigin.TOP_LEFT:
                            MiddlePoint = new Vector2(0, 0);
                            break;
                        case SpriteOrigin.TOP_RIGHT:
                            MiddlePoint = new Vector2(Texture.Width, 0);
                            break;
                        case SpriteOrigin.BOTTOM_LEFT:
                            MiddlePoint = new Vector2(0, Texture.Height);
                            break;
                        case SpriteOrigin.BOTTOM_RIGHT:
                            MiddlePoint = new Vector2(Texture.Width, Texture.Height);
                            break;
                    }

                    isMiddlePointSet = true;
                }
                
                return;
            }
            
            switch (spriteOrigin) // Apply standard boundingBox
            {
                case SpriteOrigin.CENTER:
                    if (!isMiddlePointSet)
                    {
                        MiddlePoint = new Vector2(Texture.Width / 2, Texture.Height / 2);
                        isMiddlePointSet = true;
                    }
                    
                    BoundingBox = new Rectangle((int)Pos.X - Texture.Width / 2, (int)Pos.Y - Texture.Height / 2, Texture.Width, Texture.Height);
                    break;
                case SpriteOrigin.TOP_LEFT:
                    if (!isMiddlePointSet)
                    {
                        MiddlePoint = new Vector2(0, 0);
                        isMiddlePointSet = true;
                    }

                    if (Rotation == 0f)
                        BoundingBox = new Rectangle((int)Pos.X, (int)Pos.Y, Texture.Width, Texture.Height);
                    else if (Rotation == 90f * PI / 180f)
                        BoundingBox = new Rectangle((int)Pos.X - Texture.Height, (int)Pos.Y, Texture.Height, Texture.Width);
                    else if (Rotation == 180f * PI / 180f)
                        BoundingBox = new Rectangle((int)Pos.X - Texture.Width, (int)Pos.Y - Texture.Height, Texture.Width, Texture.Height);
                    else if (Rotation == 270f * PI / 180f)
                        BoundingBox = new Rectangle((int)Pos.X, (int)Pos.Y - Texture.Width, Texture.Height, Texture.Width);
                    else
                        throw new InvalidOperationException();

                    break;
                case SpriteOrigin.TOP_RIGHT:
                    if (!isMiddlePointSet)
                    {
                        MiddlePoint = new Vector2(Texture.Width, 0);
                        isMiddlePointSet = true;
                    }

                    if (Rotation == 0f)
                        BoundingBox = new Rectangle((int)Pos.X - Texture.Width, (int)Pos.Y, Texture.Width, Texture.Height);
                    else if (Rotation == 90f * PI / 180f)
                        BoundingBox = new Rectangle((int)Pos.X - Texture.Height, (int)Pos.Y - Texture.Width, Texture.Height, Texture.Width);
                    else if (Rotation == 180f * PI / 180f)
                        BoundingBox = new Rectangle((int)Pos.X, (int)Pos.Y - Texture.Height, Texture.Width, Texture.Height);
                    else if (Rotation == 270f * PI / 180f)
                        BoundingBox = new Rectangle((int)Pos.X, (int)Pos.Y, Texture.Height, Texture.Width);
                    else
                        throw new InvalidOperationException();

                    break;
                case SpriteOrigin.BOTTOM_LEFT:
                    if (!isMiddlePointSet)
                    {
                        MiddlePoint = new Vector2(0, Texture.Height);
                        isMiddlePointSet = true;
                    }

                    if (Rotation == 0f)
                        BoundingBox = new Rectangle((int)Pos.X, (int)Pos.Y - Texture.Height, Texture.Width, Texture.Height);
                    else if (Rotation == 90f * PI / 180f)
                        BoundingBox = new Rectangle((int)Pos.X, (int)Pos.Y, Texture.Height, Texture.Width);
                    else if (Rotation == 180f * PI / 180f)
                        BoundingBox = new Rectangle((int)Pos.X - Texture.Width, (int)Pos.Y, Texture.Width, Texture.Height);
                    else if (Rotation == 270f * PI / 180f)
                        BoundingBox = new Rectangle((int)Pos.X - Texture.Height, (int)Pos.Y - Texture.Width, Texture.Height, Texture.Width);
                    else
                        throw new InvalidOperationException();

                    break;
                case SpriteOrigin.BOTTOM_RIGHT:
                    if (!isMiddlePointSet)
                    {
                        MiddlePoint = new Vector2(Texture.Width, Texture.Height);
                        isMiddlePointSet = true;
                    }

                    if (Rotation == 0f)
                        BoundingBox = new Rectangle((int)Pos.X - Texture.Width, (int)Pos.Y - Texture.Height, Texture.Width, Texture.Height);
                    else if (Rotation == 90f * PI / 180f)
                        BoundingBox = new Rectangle((int)Pos.X, (int)Pos.Y - Texture.Width, Texture.Height, Texture.Width);
                    else if (Rotation == 180f * PI / 180f)
                        BoundingBox = new Rectangle((int)Pos.X, (int)Pos.Y, Texture.Width, Texture.Height);
                    else if (Rotation == 270f * PI / 180f)
                        BoundingBox = new Rectangle((int)Pos.X - Texture.Height, (int)Pos.Y, Texture.Height, Texture.Width);
                    else
                        throw new InvalidOperationException();

                    break;
            }
        }
    }
}
