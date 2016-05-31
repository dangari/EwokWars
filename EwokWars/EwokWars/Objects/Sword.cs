using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace EwokWars.Objects
{
    public class Sword : DynamicObjects
    {
        private SoundEffect swordSound;
        private Boolean animation = false;
        private float rotationSum = 0;
        private DynamicObjects.ViewDirection oldView;
        public delegate Vector2 PositionCalculator();
        private PositionCalculator positionCalculator;

        private bool aniSound = false;

        public LivingObject Parent { get; private set; }

        public Sword(LivingObject parent, PositionCalculator positionCalculator)
        {
            Parent = parent;
            this.positionCalculator = positionCalculator;
        }

        public override Vector2 Pos
        {
            get
            {
                return base.Pos;
            }
            set
            {
                // Set the sword position, based on the user-defined function
                base.Pos = positionCalculator();

                if (Parent.CurrentDirection != oldView)
                {
                    switch (Parent.CurrentDirection)
                    {
                        case DynamicObjects.ViewDirection.UP:
                            Rotation = 0f;                          
                            break;
                        case DynamicObjects.ViewDirection.DOWN:
                            Rotation = 180f;
                            break;
                        case DynamicObjects.ViewDirection.RIGHT:
                            Rotation = 90f;
                            break;
                        case DynamicObjects.ViewDirection.LEFT:
                            Rotation = 90f;
                            break;
                    }

                    // What is this value used to?
                    rotationSum = 0f;
                }

                oldView = Parent.CurrentDirection;
            }
        }

        public override void Initialize()
        {
            animation = false;
            rotationSum = 0;
        }

        public override void OnCollision(GameObject other)
        {
            if(Parent.IsAlive)
                other.OnCollision(this);
        }

        public override void LoadContent(ContentManager content)
        {
            swordSound = content.Load<SoundEffect>("Sounds/SlowSabr");
            Texture = content.Load<Texture2D>("Images/sword");
            MiddlePoint = new Vector2(Texture.Width, Texture.Height / 2);
        }

        public override void Update(GameTime gameTime)
        {
            if (animation)
            {
                if (aniSound == false)
                {
                    swordSound.Play(0.5f,0f,0f);
                    aniSound = true;
                }

                swordAnimation(gameTime);
            }

            UpdateBoundingBox();
        }

        private void swordAnimation(GameTime gameTime)
        {
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float anTime = 300;

            switch (Parent.CurrentDirection)
            {
                case DynamicObjects.ViewDirection.UP:
                    if (rotationSum < 90)
                    {
                        Rotation = (Rotation * 180 / PI) + anTime * seconds;
                        rotationSum += anTime * seconds;
                    }
                    else
                    {
                        Rotation = (Rotation * 180 / PI) - rotationSum;
                        rotationSum = 0;
                        animation = false;
                        aniSound = false;
                    }
                    break;
                case DynamicObjects.ViewDirection.DOWN:
                    if (rotationSum < 90)
                    {
                        Rotation = (Rotation * 180 / PI) + anTime * seconds;
                        rotationSum += anTime * seconds;
                    }
                    else
                    {
                        Rotation = (Rotation * 180 / PI) - rotationSum;
                        rotationSum = 0;
                        animation = false;
                        aniSound = false;
                    }
                    break;
                case DynamicObjects.ViewDirection.RIGHT:
                    if (rotationSum < 90)
                    {
                        Rotation = (Rotation * 180 / PI) + anTime * seconds;
                        rotationSum += anTime * seconds;
                    }
                    else
                    {
                        Rotation = (Rotation * 180 / PI) - rotationSum;
                        rotationSum = 0;
                        animation = false;
                        aniSound = false;
                    }
                    break;
                case DynamicObjects.ViewDirection.LEFT:
                    if (rotationSum < 90)
                    {
                        Rotation = (Rotation * 180 / PI) - anTime * seconds;
                        rotationSum += anTime * seconds;
                    }
                    else
                    {
                        Rotation = (Rotation * 180 / PI) + rotationSum;
                        rotationSum = 0;
                        animation = false;
                        aniSound = false;
                    }
                    break;
                default:
                    break;
            }
        }

        public bool isAnimated()
        {
            return animation;
        }

        public void startAnimation()
        {
            animation = true;
        }

        protected override void UpdateBoundingBox(Rectangle boundingBox = default(Rectangle))
        {
            Vector2 BBoxEdge;

            // Calculate the Boundingbox edge's
            {
                if (rotationSum == 0)
                    BBoxEdge = new Vector2(Texture.Width, Texture.Height);
                else
                {
                    float hyp = Texture.Width; // Hypotenuse
                    float ank; //  Ankathete
                    float gek; //  Gegenkathete

                    gek = (float)Math.Sin(rotationSum / 180 * PI) * hyp;
                    ank = (float)Math.Cos(rotationSum / 180 * PI) * hyp;
                    BBoxEdge = new Vector2(ank, gek);
                }
            }

            switch (Parent.CurrentDirection)
            {
                case DynamicObjects.ViewDirection.UP:
                    base.UpdateBoundingBox(new Rectangle((int)(Pos.X - BBoxEdge.X), (int)(Pos.Y - BBoxEdge.Y), (int)BBoxEdge.X, (int)BBoxEdge.Y));
                    break;
                case DynamicObjects.ViewDirection.DOWN:
                    base.UpdateBoundingBox(new Rectangle((int)(Pos.X), (int)(Pos.Y), (int)BBoxEdge.X, (int)BBoxEdge.Y));
                    break;
                case DynamicObjects.ViewDirection.RIGHT:
                    base.UpdateBoundingBox(new Rectangle((int)(Pos.X), (int)(Pos.Y - BBoxEdge.X), (int)BBoxEdge.Y, (int)BBoxEdge.X));
                    break;
                case DynamicObjects.ViewDirection.LEFT:
                    base.UpdateBoundingBox(new Rectangle((int)(Pos.X - BBoxEdge.Y), (int)(Pos.Y - BBoxEdge.X), (int)BBoxEdge.Y, (int)BBoxEdge.X));
                    break;
            }
        }
    }
}
