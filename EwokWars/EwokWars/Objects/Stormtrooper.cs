using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EwokWars.Objects
{
    class Stormtrooper : LivingObject
    {
        private List<GameObject> currentCollisions = new List<GameObject>();
        private enum Directional { LEFT, RIGHT }
        private Directional directional = Directional.LEFT;
        
        private CollisionType currentCollisionType;
        private float speed = 100;
        private Vector2 directionVector;
        private Vector2 initialDirectionVector;
        private Vector2 initialPosition;

        //Sichtfeld
        private Rectangle xView;
        private Rectangle yView;

        //Texturen
        private Texture2D dead;
        private Texture2D StormtrooperText;

        // Debug
        public Rectangle XView
        {
            get { return xView; }
            set { xView = value; }
        }
       
        //Debug
        public Rectangle YView
        {
            get { return yView; }
            set { yView = value; }
        }

        public Stormtrooper(float x, float y, bool movesHorizontal)
        {
            invincibleTimeAfterHit = 300f;
            initialPosition = new Vector2(x, y);
            this.pushBack = new DynamicObjects.PushBack(this);

            if (movesHorizontal)
            {
                initialDirectionVector = new Vector2(1, 0);
                //Rotation = 180f;
            }
            else
            {
                initialDirectionVector = new Vector2(0, 1);
                //Rotation = 90f;
            }
        }

        public override void Initialize()
        {
            Health = 100;
            directional = Directional.LEFT;
            directionVector = initialDirectionVector;
            IsAlive = true;
            currentStatus = Status.PATROL;
            Pos = initialPosition;
            currentCollisions.Clear();
        }
        
        public override bool Intersects(GameObject other)
        {
            if (other.BoundingBox.Intersects(this.BoundingBox))
            {
                currentCollisionType = CollisionType.OBJECTCOLLISION;
                return true;
            }
            
            if ((other.BoundingBox.Intersects(xView) || other.BoundingBox.Intersects(yView) && other is Hero))
            {
                currentCollisionType = CollisionType.VIEWCOLLISION;
                return true;
            }
            
            return false;
        }

        // Handle all occured Collisions
        public override void OnCollision(GameObject other)
        {
            if (!IsAlive)
                return;

            if (currentCollisionType == CollisionType.NONE)
                currentCollisionType = CollisionType.OBJECTCOLLISION;

            // Handle sword collisions
            if (other is Sword && currentCollisionType == CollisionType.OBJECTCOLLISION
                && currentStatus != Status.BLOCK)
            {
                Sword s = (Sword)other;

                // Set the pushback direction
                switch (s.Parent.CurrentDirection)
                {
                    case DynamicObjects.ViewDirection.UP:
                        pushBack.Direction = new Vector2(0, -1);
                        break;
                    case DynamicObjects.ViewDirection.DOWN:
                        pushBack.Direction = new Vector2(0, 1);
                        break;
                    case DynamicObjects.ViewDirection.RIGHT:
                        pushBack.Direction = new Vector2(1, 0);
                        break;
                    case DynamicObjects.ViewDirection.LEFT:
                        pushBack.Direction = new Vector2(-1, 0);
                        break;
                    default:
                        throw new InvalidOperationException("Invalid HeroView!");
                }

                Health -= 25;
                currentStatus = Status.BLOCK;
                pushBack.Time = 10;
                return;
            }

            // Handle hero collisions
            if (other is Hero)
            {
                if (currentCollisionType == CollisionType.VIEWCOLLISION)
                {
                    currentCollisions.Add(other);
                }
                else
                {
                    Hero h = (Hero)other;
                    h.Health -= 25;

                    // Set Hero's pushback direction
                    ViewDirection direction = Physics.Collision.GetCollisionSide(this, other);
                    switch (direction)
                    {
                        case ViewDirection.LEFT:
                            h.PerformPushback(new Vector2(-1, 0));
                            break;
                        case ViewDirection.RIGHT:
                            h.PerformPushback(new Vector2(1, 0));
                            break;
                        case ViewDirection.UP:
                            h.PerformPushback(new Vector2(-1, 0));
                            break;
                        case ViewDirection.DOWN:
                            h.PerformPushback(new Vector2(1, 0));
                            break;
                        default:
                            throw new InvalidOperationException("Unknown direction!");
                    }
                }

                return;
            }

            // TODO: Handle all other collisions
            if (currentCollisionType == CollisionType.OBJECTCOLLISION)
            {
                currentCollisions.Add(other);
                Physics.Collision.ResolveIntersection(this, other);

                if (directional == Directional.LEFT)
                    directional = Directional.RIGHT;
                else
                    directional = Directional.LEFT;

                currentStatus = Status.PATROL;
            }

            currentCollisionType = CollisionType.NONE;
        }

        public override void LoadContent(ContentManager content)
        {
            StormtrooperText = content.Load<Texture2D>("Images/Stormtrooper_2");
            dead = content.Load<Texture2D>("Images/Hero/hero_dead");
            Texture = StormtrooperText;

            // die beiden sichtachsen
            xView = new Rectangle((int)Pos.X - 300 - Texture.Width / 2, (int)Pos.Y - Texture.Height / 2, 600 + Texture.Width, Texture.Height);
            yView = new Rectangle((int)Pos.X - Texture.Width / 2, (int)Pos.Y - 300 - Texture.Height / 2, Texture.Width, Texture.Height + 600);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            collisionHandling();
            if (Health <= 0)
            {
                Texture = dead;
                IsAlive = false;
            }
               
            else
            {
                Texture = StormtrooperText;
                switch (currentStatus)
                {
                    case Status.PATROL:
                        Patrol(gameTime);
                        break;
                    case Status.ATTACK:
                        break;
                    case Status.BLOCK:
                        break;
                    case Status.FOLLOW:
                        break;
                    default:
                        break;
                }

                UpdateBoundingBox();
                UpdateView();
            }
        }

        // brechnet die neue Position
        private void Patrol(GameTime gameTime)
        {
            Vector2 newPos = this.Pos;
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // je nachdem ob sich der Stormtrooper Horizontal bewegt oder nicht wird direction geändert
            if (directional == Directional.RIGHT)
            {
                newPos.X -= directionVector.X * speed * seconds;
                newPos.Y -= directionVector.Y * speed * seconds;
            }
            else
            {
                newPos.X += directionVector.X * speed * seconds;
                newPos.Y += directionVector.Y * speed * seconds;
            }

            this.Pos = newPos;
        }

        private void UpdateView()
        {
            xView.X = (int)this.Pos.X - 300 - Texture.Width / 2;
            xView.Y = (int)this.Pos.Y - Texture.Height / 2;
            yView.X = (int)this.Pos.X - Texture.Width / 2;
            yView.Y = (int)this.Pos.Y - 300 - Texture.Height / 2;
        }

        /// <summary>
        /// Sorgt dafür wenn der Hero im Sichtfeld ist das er sich zu diesem bewegt und ihm folgt
        /// </summary>
        /// <param name="hero">Hero</param>
        private void moveToHero(Hero hero)
        {
            Vector2 newPos = this.Pos;
            float addPos = 500 * 0.01f;
            int moveDirection = 1;
            if (hero.BoundingBox.Intersects(this.yView))
            {
                if (hero.Pos.X != this.Pos.X)
                {
                    float difference = hero.Pos.X - this.Pos.X;
                    if (difference < 0)
                        moveDirection = -1;
                    if (Math.Abs(difference) > addPos && currentStatus != Status.ATTACK)
                        newPos.X += addPos * moveDirection;
                    else
                    {
                        newPos.X = hero.Pos.X;
                        currentStatus = Status.ATTACK;
                    }
                }
            }
            if (hero.BoundingBox.Intersects(this.xView))
            {
                if (hero.Pos.Y != this.Pos.Y)
                {
                    float difference = hero.Pos.Y - this.Pos.Y;
                    if (difference > 0)
                        moveDirection = -1;
                    if (Math.Abs(difference) > addPos && currentStatus != Status.ATTACK)
                        newPos.Y += addPos * -(moveDirection);
                    else
                    {
                        newPos.Y = hero.Pos.Y;
                        currentStatus = Status.ATTACK;
                    }
                }

            }
            this.Pos = newPos;
        }

        private void collisionHandling()
        {
            bool viewCollision = false;
            bool objectCollision = false;
            Hero tempHero = null;
            GameObject other = null;
            // schaut welche Kollision statt gefunden haben
            foreach (GameObject go in currentCollisions)
            {
                if (go is Hero)
                {

                    viewCollision = true;
                    tempHero = (Hero)go;
                }
                else
                {
                    objectCollision = true;
                    other = go;
                }
            }

            if (viewCollision && objectCollision)
            {
                Vector2 newPos = this.Pos;
                if (other != null)
                {
                    if (tempHero.BoundingBox.Intersects(yView))
                    {
                        Physics.Collision.ResolveIntersection(this, other);
                        newPos.X += this.Pos.X - newPos.X;
                    }
                    if (tempHero.BoundingBox.Intersects(xView))
                    {
                        Physics.Collision.ResolveIntersection(this, other);
                        newPos.Y += this.Pos.Y - newPos.Y;
                    }
                    currentStatus = Status.PATROL;
                }
                this.Pos = newPos;
            }
            else if (viewCollision == false && objectCollision)
            {
                Vector2 newPos = this.Pos;
                Physics.Collision.ResolveIntersection(this, other);
                this.Pos += this.Pos - newPos;
                
                Rotation = (Rotation * 180 / PI) - 180f;
                currentStatus = Status.PATROL;
            }
            else if (viewCollision && objectCollision == false)
            {
                if (tempHero != null)
                {
                    if (!(this.BoundingBox.Intersects(tempHero.BoundingBox)))
                        moveToHero(tempHero);
                }

            }
            else if (viewCollision == false && objectCollision == false)
            {
                currentStatus = Status.PATROL;
            }
            currentCollisions.Clear();

        }
    }
}
