using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace EwokWars.Objects
{
    public class Hero : LivingObject
    {
        private new enum CollisionType { HERO, SWORD }

        public Sword sword { get; private set; }
        public Vector2 Direction { get; set; }
        public float Speed { get; set; }

        private CollisionType collisionType;
        private Miscellaneous.Controls heroController;

        private Texture2D front;
        private Texture2D left;
        private Texture2D right;
        private Texture2D back;
        private Texture2D dead;

        private Vector2 initialPosition;
        private SoundEffect hitSound;
        private float soundTime = 800f;

        public Hero(int X, int Y)
        {
            Speed = 500f;
            initialPosition = new Vector2(X, Y);
            Pos = initialPosition;
            pushBack = new DynamicObjects.PushBack(this);
            sword = new Sword(this, UpdateSword);
            heroController = new Miscellaneous.Controls(this);
            canReclaimEnergy = true;
            canReclaimHealth = true;
        }

        public override bool Intersects(GameObject other)
        {
            // TODO: Maybe we can avoid those runtime vtable lookup checks
            if (sword.Intersects(other) && (other is DynamicObjects) && sword.isAnimated())
            {
                collisionType = CollisionType.SWORD;
                return true;
            }

            if (base.Intersects(other))
            {
                collisionType = CollisionType.HERO;
                return true;
            }

            return false;
        }

        public override void OnCollision(GameObject other)
        {
            if (IsAlive)
            {
                if (!(other is MovableObject))
                {
                    switch (collisionType)
                    {
                        case CollisionType.HERO:
                            other.OnCollision(this);
                            if ((other is Stormtrooper || other is BattleDroid) && soundTime < 0)
                            {
                                LivingObject temp = (LivingObject)other;
                                if (temp.Health > 0)
                                {
                                    hitSound.Play(0.5f, 0f, 0f);
                                    soundTime = 800;
                                }
                            }
                            break;
                        case CollisionType.SWORD:
                            sword.OnCollision(other);
                            break;
                        default:
                            throw new InvalidOperationException("Unknown Collisiontype");
                    }
                }
            }
        }

        public override void Initialize()
        {
            sword.Initialize();
        }

        public override void Draw(SpriteBatch spriteBatch, GraphicsDevice gd, DebugKit debugKit = null)
        {
            //base.Draw(spriteBatch, gd, debugKit);

            if (CurrentDirection == ViewDirection.LEFT
                || CurrentDirection == ViewDirection.UP)
            {
                // Draw the sword only if Hero is still alive
                if (IsAlive)
                {
                    spriteBatch.Draw(sword.Texture, sword.Pos, null, Color.White, sword.Rotation,
                                     sword.MiddlePoint, 1.0f, SpriteEffects.None, 0f);
                }
                else
                {
                    spriteBatch.Draw(sword.Texture, sword.Pos, null, Color.White * 0f, sword.Rotation,
                                     sword.MiddlePoint, 1.0f, SpriteEffects.None, 0f);
                }

                // Draw this object with a red glow while it's invincible
                if (invincibleTimeLeft > 0 && IsAlive)
                    spriteBatch.Draw(Texture, Pos, null, Color.Red, Rotation, MiddlePoint, 1.0f, SpriteEffects.None, 0f);
                else
                    spriteBatch.Draw(Texture, Pos, null, Color.White, Rotation, MiddlePoint, 1.0f, SpriteEffects.None, 0f);

            }
            else
            {
                // Draw this object with a red glow while it's invincible
                if (invincibleTimeLeft > 0 && IsAlive)
                    spriteBatch.Draw(Texture, Pos, null, Color.Red, Rotation, MiddlePoint, 1.0f, SpriteEffects.None, 0f);
                else
                    spriteBatch.Draw(Texture, Pos, null, Color.White, Rotation, MiddlePoint, 1.0f, SpriteEffects.None, 0f);

                // Draw the sword only if Hero is still alive
                if (IsAlive)
                {
                    spriteBatch.Draw(sword.Texture, sword.Pos, null, Color.White, sword.Rotation,
                                     sword.MiddlePoint, 1.0f, SpriteEffects.None, 0f);
                }
                else
                {
                    spriteBatch.Draw(sword.Texture, sword.Pos, null, Color.White * 0f, sword.Rotation,
                                     sword.MiddlePoint, 1.0f, SpriteEffects.None, 0f);
                }
            }

            if (debugKit != null)
            {
                debugKit.DrawBBox(sword.BoundingBox, gd, spriteBatch);
                debugKit.DrawBBox(BoundingBox, gd, spriteBatch);
            }
        }

        public override void LoadContent(ContentManager content)
        {
            left = content.Load<Texture2D>("Images/Hero/hero_left");
            right = content.Load<Texture2D>("Images/Hero/hero_right");
            back = content.Load<Texture2D>("Images/Hero/hero_back");
            front = content.Load<Texture2D>("Images/Hero/hero_front");
            dead = content.Load<Texture2D>("Images/Hero/hero_dead");
            Texture = front;

            hitSound = content.Load<SoundEffect>("Sounds/hit");

            sword.LoadContent(content);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            soundTime -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (Health <= 0)
            {
                Rotation = 0f;
                IsAlive = false;
                Texture = dead;
            }
            else
            {
                sword.Update(gameTime);

                if (pushBack.Time <= 0)
                    heroController.UpdatePosition(gameTime);

                //Schwert angriff
                if (heroController.attack() && !sword.isAnimated() && Energy >= 10)
                {
                    sword.startAnimation();
                    Energy -= 10;
                }

                sword.Pos = this.Pos;
            }

            UpdateBoundingBox();
            if (CurrentDirection == ViewDirection.UP)
                BoundingBox = new Rectangle((int)(this.Pos.X) - Texture.Width / 2, (int)(this.Pos.Y), Texture.Width, Texture.Height / 2);
        }

        public void Reset()
        {
            Pos = initialPosition;
            Health = 100;
            Energy = 100;
            IsAlive = true;
            pushBack.Time = 0;
            invincibleTimeLeft = 0f;
        }

        //setzt das Schwert an die Richtige Stelle
        private Vector2 UpdateSword()
        {
            Vector2 swordOffset = Vector2.Zero;

            switch (CurrentDirection)
            {
                case ViewDirection.UP:
                    swordOffset = new Vector2(Texture.Width / 2, Texture.Height / 4);
                    Texture = back;
                    break;
                case ViewDirection.DOWN:
                    swordOffset = new Vector2(-Texture.Width / 2, Texture.Height / 4);
                    Texture = front;
                    break;
                case ViewDirection.RIGHT:
                    swordOffset = new Vector2(0, Texture.Height / 4);
                    Texture = right;
                    break;
                case ViewDirection.LEFT:
                    swordOffset = new Vector2(0, Texture.Height / 4);
                    Texture = left;
                    break;
                default:
                    throw new InvalidOperationException("Unknown viewdirection: " + CurrentDirection);
            }

            return (Pos + swordOffset);
        }
    }
}
