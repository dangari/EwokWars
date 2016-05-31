using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace EwokWars.Objects
{
    public abstract class LivingObject: DynamicObjects
    {
        public bool IsAlive = true;
        protected enum Status { PATROL, ATTACK, BLOCK, DEAD, FOLLOW }
        protected enum CollisionType { VIEWCOLLISION, OBJECTCOLLISION, NONE }
        protected Status currentStatus;
        protected float invincibleTimeAfterHit = 1500f;
        protected float invincibleTimeLeft = 0f;
        protected bool canReclaimHealth = false;
        protected bool canReclaimEnergy = false;
        private float timeSinceLastHit = 0f;

        private int health = 100;
        public virtual int Health
        {
            get { return health; }
            set
            {           
                // only make an object invincible when it health is reduced
                if (value < health)
                {
                    if (invincibleTimeLeft <= 0)
                    {
                        timeSinceLastHit = 0f;
                        health = value;
                        invincibleTimeLeft = invincibleTimeAfterHit;
                    }
                }
                else
                {
                    health = value;
                }

                if (health < 0) health = 0;
            }
        }

        private int energy = 100;
        public int Energy 
        {
            get { return energy; }
            set { energy = value; }
        }

        private int energyTimeSinceRefresh = 0;
        private int energyReclaimFactor = 4; // Gained energy per second
        public int EnergyReclaimFactor
        {
            get { return energyReclaimFactor; }
            set { energyReclaimFactor = value; }
        }

        private int healthTimeSinceRefresh = 0;
        private int healthReclaimFactor = 1; // Gained health per second
        public int HealthReclaimFactor
        {
            get { return healthReclaimFactor; }
            set { healthReclaimFactor = value; }
        }

        private int timeUntilHealthRefresh = 5000; // Milliseconds
        public int TimeUntilHealthRefresh
        {
            get { return timeUntilHealthRefresh; }
            set { timeUntilHealthRefresh = value; }
        }

        public bool IsInvincible()
        {
            return invincibleTimeLeft > 0;
        }

        public void PerformPushback(Vector2 direction)
        {
                pushBack.Time = 10;
                pushBack.Direction = direction;            
        }

        public override void Update(GameTime gameTime)
        {
            // Handle pushback animations and invincible times
            if (IsAlive == true)
            {
                if (invincibleTimeLeft > 0)
                    invincibleTimeLeft -= gameTime.ElapsedGameTime.Milliseconds;

                if (pushBack.Time > 0)
                    pushBack.Update(gameTime);

                if (canReclaimEnergy)
                {
                    energyTimeSinceRefresh += gameTime.ElapsedGameTime.Milliseconds;

                    if (Energy < 100 && energyTimeSinceRefresh >= 1000)
                    {
                        Energy += EnergyReclaimFactor;
                         energyTimeSinceRefresh = 0;
                    }

                    if (Energy > 100) Energy = 100;
                }

                if (canReclaimHealth)
                {
                    healthTimeSinceRefresh += gameTime.ElapsedGameTime.Milliseconds;

                    if (timeSinceLastHit < TimeUntilHealthRefresh)
                    {
                        timeSinceLastHit += gameTime.ElapsedGameTime.Milliseconds;
                    }
                    

                    if (Health < 100 && healthTimeSinceRefresh >= 1000
                        && timeSinceLastHit >= TimeUntilHealthRefresh)
                    {
                        Health += HealthReclaimFactor;
                        healthTimeSinceRefresh = 0;

                        if (Health > 100) Health = 100;
                    }        
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch, GraphicsDevice gd, DebugKit debugKit = null)
        {
            // Draw this object with a red glow while it's invincible
            if (invincibleTimeLeft > 0 && IsAlive)
                spriteBatch.Draw(Texture, Pos, null, Color.Red, Rotation, MiddlePoint, 1.0f, SpriteEffects.None, 0f);
            else
                spriteBatch.Draw(Texture, Pos, null, Color.White, Rotation, MiddlePoint, 1.0f, SpriteEffects.None, 0f);

            if (debugKit != null)
                debugKit.DrawBBox(BoundingBox, gd, spriteBatch);
        }
    }
}
