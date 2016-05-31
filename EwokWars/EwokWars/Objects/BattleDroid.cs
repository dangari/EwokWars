using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace EwokWars.Objects
{
    class BattleDroid : LivingObject
    {
        private List<Vector2> waypoints;
        private int currentWaypoint;

        private Texture2D dead;
        private Texture2D alive;

        private bool canSeeHero;
        private Rectangle fieldOfVision;
        private CollisionType collisionType;
        private Hero heroRef;
        private Vector2 initialPosition;
        private ViewDirection initialViewDirection;

        private const int HERO_DAMAGE_ON_COLLISION = 25;
        private const int DAMAGE_ON_HIT = 25;
        private const int SPEED_PATROL = 100;
        private const int SPEED_ATTACK = 250;
        private const int MAX_DISTANCE_ON_ATTACK = 500;

        private const int SCANNED_AREA_SIDE = 400;
        private const int SCANNED_AREA_FRONT = 250;
        /***************************************************
         *  Battledroid looks down:                         *
         *                                                  *
         *   <-- SCANNED_AREA_SIDE -->                      *
         *   --------------------------                     *
         *   |       | * * *  |       | /|\                 *
         *   |       |  ***   |       |   SCANNED_AREA_FRONT*
         *   |       |__ *____|       |  |                  *
         *   |                        |  |                  *
         *   |________________________| \|/                 *
         *                                                  *
         * *************** I like boxes *********************
         *            --------------------------            *
         *            |      /| * * *  |\       |           *
         *            |     / |  ***   | \      |           *
         *            |    /  |__ *____|  \     |           *
         *            |   / xxxxxxxxxxxxx  \    |           *
         *            |  / xxxxxxxxxxxxxxx  \   |           *
         *            | / xxxxxxxxxxxxxxxxx  \  |           *
         *            |/ xxxxxxxxxxxxxxxxxxx  \ |           *
         *            |_xxxxxxxxxxxxxxxxxxxxx__\|           *
         *  The 'x' mark the area in which you are attacked *
         * *************** I like boxes *******************/

        public BattleDroid(Vector2 position, ViewDirection viewDirection)
        {
            this.Pos = position;

            initialPosition = position;
            initialViewDirection = viewDirection;

            switch (viewDirection)
            {
                case ViewDirection.LEFT:
                    CurrentDirection = ViewDirection.LEFT;
                    Rotation = 270f;
                    break;
                case ViewDirection.RIGHT:
                    CurrentDirection = ViewDirection.RIGHT;
                    Rotation = 90f;
                    break;
                case ViewDirection.UP:
                    CurrentDirection = ViewDirection.UP;
                    Rotation = 0f;
                    break;
                case ViewDirection.DOWN:
                    CurrentDirection = ViewDirection.DOWN;
                    Rotation = 180f;
                    break;
                default:
                    throw new InvalidOperationException("Viewdirection '" + viewDirection.ToString() +
                        "' is not allowed for enemytype Battledroid!");
            }
        }

        public BattleDroid(List<Vector2> waypoints)
        {
            if (waypoints == null || waypoints.Count < 2)
                throw new InvalidOperationException("You have to specify at leat"
                    + " 2 waypoints when using this ctor!");

            this.waypoints = waypoints;
            Pos = waypoints[0];
            currentWaypoint = 0;
        }

        public override bool Intersects(GameObject other)
        {
            if (!IsAlive)
                return false;

            if (other is Hero)
            {
                {
                    Hero h = (Hero)other;

                    if (h.IsAlive == false)
                    {
                        canSeeHero = false;
                        collisionType = CollisionType.NONE;
                        return false;
                    }
                        
                }               

                // Check if we have a direct collision
                if (other.BoundingBox.Intersects(this.BoundingBox))
                {
                    collisionType = CollisionType.OBJECTCOLLISION;
                    return true;
                }
                else // Check if we have a field of vision collision
                {
                    switch (CurrentDirection)
                    {
                        case ViewDirection.LEFT:
                            fieldOfVision =
                                new Rectangle((int)Pos.X + (Texture.Width / 2) - SCANNED_AREA_FRONT,
                                              (int)Pos.Y - (SCANNED_AREA_SIDE / 2),
                                              SCANNED_AREA_FRONT, SCANNED_AREA_SIDE);

                            if (other.BoundingBox.Intersects(fieldOfVision))
                            {
                                if (DetailedIntersection(other, ref fieldOfVision, CurrentDirection))
                                {
                                    canSeeHero = true;
                                    currentStatus = Status.ATTACK;
                                }    
                                else
                                {
                                    canSeeHero = false;
                                }
                            }
                            else
                            {
                                canSeeHero = false;
                            }

                            break;
                        case ViewDirection.RIGHT:
                            fieldOfVision =
                                new Rectangle((int)Pos.X - (Texture.Width / 2),
                                              (int)Pos.Y - (SCANNED_AREA_SIDE / 2),
                                              SCANNED_AREA_FRONT, SCANNED_AREA_SIDE);

                            if (other.BoundingBox.Intersects(fieldOfVision))
                            {
                                if (DetailedIntersection(other, ref fieldOfVision, CurrentDirection))
                                {
                                    canSeeHero = true;
                                    currentStatus = Status.ATTACK;
                                }    
                                else
                                {
                                    canSeeHero = false;
                                }
                            }
                            else
                            {
                                canSeeHero = false;
                            }

                            break;
                        case ViewDirection.UP:
                            fieldOfVision =
                                new Rectangle((int)Pos.X - (SCANNED_AREA_SIDE / 2),
                                              (int)Pos.Y - SCANNED_AREA_FRONT + (Texture.Width / 2),
                                              SCANNED_AREA_SIDE, SCANNED_AREA_FRONT);

                            if (other.BoundingBox.Intersects(fieldOfVision))
                            {
                                if (DetailedIntersection(other, ref fieldOfVision, CurrentDirection))
                                {
                                    canSeeHero = true;
                                    currentStatus = Status.ATTACK;
                                }    
                                else
                                {
                                    canSeeHero = false;
                                }
                            }
                            else
                            {
                                canSeeHero = false;
                            }
                            break;
                        case ViewDirection.DOWN:
                            fieldOfVision =
                                new Rectangle((int)Pos.X - (SCANNED_AREA_SIDE / 2),
                                              (int)Pos.Y - (Texture.Width / 2),
                                              SCANNED_AREA_SIDE, SCANNED_AREA_FRONT);

                            if (other.BoundingBox.Intersects(fieldOfVision))
                            {
                                if (DetailedIntersection(other, ref fieldOfVision, CurrentDirection))
                                {
                                    canSeeHero = true;
                                    currentStatus = Status.ATTACK;
                                }
                                else
                                {
                                    canSeeHero = false;
                                }
                            }
                            else
                            {
                                canSeeHero = false;
                            }
                            break;
                    }

                    if (canSeeHero == true)
                    {
                        collisionType = CollisionType.VIEWCOLLISION;
                        return true;
                    }
                    else
                    {
                        return false;
                    }             
                }
            }
            else 
            {
                return other.BoundingBox.Intersects(this.BoundingBox);
            } 
        }

        public override void Initialize()
        {
            Health = 100;
            IsAlive = true;
            canSeeHero = false;
            pushBack = new PushBack(this);
            currentStatus = Status.PATROL;
            invincibleTimeAfterHit = 300f;
            invincibleTimeLeft = 0f;

            if (Texture == dead)
                Texture = alive;
            
            if (waypoints == null)
            {
                Pos = initialPosition;
                CurrentDirection = initialViewDirection;
            }
            else 
            {
                Pos = waypoints[0];
                currentWaypoint = 0;
            }
        }

        /* // Uncomment this method if you want to make Battledroid's field of vision visible
        public override void Draw(SpriteBatch spriteBatch, GraphicsDevice gd, DebugKit debugKit = null)
        {
            if(canSeeHero)
                spriteBatch.Draw(Texture, Pos, null, Color.Green, Rotation, MiddlePoint, 1.0f, SpriteEffects.None, 0f);
            else
                spriteBatch.Draw(Texture, Pos, null, Color.White, Rotation, MiddlePoint, 1.0f, SpriteEffects.None, 0f);

            if (debugKit != null)
            {

                debugKit.DrawBBox(BoundingBox, gd, spriteBatch);
                debugKit.DrawBBox(fieldOfVision, gd, spriteBatch);
            }
        }
        */

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Health <= 0)
            {
                Rotation = 0f;
                IsAlive = false;
                Texture = dead;
                return;
            }

            switch (currentStatus)
            {
                case Status.PATROL:
                    if (waypoints == null)
                    {
                        if (HasReachedPosition(initialPosition))
                        {
                            CurrentDirection = initialViewDirection;

                            switch (CurrentDirection)
                            {
                                case ViewDirection.UP:
                                    Rotation = 0f;
                                    break;
                                case ViewDirection.RIGHT:
                                    Rotation = 90f;
                                    break;
                                case ViewDirection.DOWN:
                                    Rotation = 180f;
                                    break;
                                case ViewDirection.LEFT:
                                    Rotation = 270f;
                                    break;
                            }
                        }                           
                        else
                        {
                            MoveToPosition(initialPosition, SPEED_PATROL, gameTime);
                        }                          
                    }
                    else
                    {
                        Vector2 nextWaypoint = waypoints[(currentWaypoint + 1) % waypoints.Count];

                        MoveToPosition(nextWaypoint, SPEED_PATROL, gameTime);

                        if (HasReachedPosition(nextWaypoint))
                            currentWaypoint++;
                    }

                    break;
                case Status.ATTACK:
                    if (heroRef.IsAlive == false)
                        currentStatus = Status.PATROL;
                    else
                        MoveToHero(gameTime);

                    break;
            }
            
            UpdateBoundingBox();
        }

        private bool HasReachedPosition(Vector2 position)
        {
            // We can't expect to accede the next waypoint EXACTLY (due to 
            // float -> int conversion inaccuracy), so we allow an almost
            // acceded state 
            const int noise = 2;

            float diffX = Math.Abs(Pos.X - position.X);
            float diffY = Math.Abs(Pos.Y - position.Y);

            return (diffX < noise) && (diffY < noise);
        }

        private bool DetailedIntersection(GameObject other, ref Rectangle scannedAreaRect, 
                                          ViewDirection viewDirection)
        {
            // Line equation; y = gradient*x + axisIntercept; 
            // gradient = dy / dx; -> we need gradient
            Vector2 point1, point2, point3, point4;
            float gradient1 = 0f, gradient2 = 0f, axisIntercept1 = 0f, axisIntercept2 = 0f;
            bool intersectionOccured = false;

            switch(viewDirection)
            {
                case ViewDirection.LEFT:
                {
                    point1 = new Vector2(scannedAreaRect.X + scannedAreaRect.Width, Pos.Y - (Texture.Height / 2));
                    point2 = new Vector2(scannedAreaRect.X, scannedAreaRect.Y);
                    point3 = new Vector2(scannedAreaRect.X, scannedAreaRect.Y + scannedAreaRect.Height);
                    point4 = new Vector2(scannedAreaRect.X + scannedAreaRect.Width, Pos.Y + (Texture.Height / 2));
                    gradient1 = (point2.Y - point1.Y) / (point2.X - point1.X);
                    gradient2 = gradient1 * (-1);
                    axisIntercept1 = point1.Y - (gradient1 * point1.X);
                    axisIntercept2 = point3.Y - (gradient2 * point3.X);

                    if (other.Pos.Y >= (gradient1 * other.Pos.X + axisIntercept1)
                        && other.Pos.Y <= (gradient2 * other.Pos.X + axisIntercept2))
                    {
                        intersectionOccured = true;
                    }

                    break;
                }
                case ViewDirection.RIGHT:
                {
                    point1 = new Vector2(scannedAreaRect.X, Pos.Y - (Texture.Height / 2));
                    point2 = new Vector2(scannedAreaRect.X + scannedAreaRect.Width, scannedAreaRect.Y);
                    point3 = new Vector2(scannedAreaRect.X + scannedAreaRect.Width, scannedAreaRect.Y + scannedAreaRect.Height);
                    point4 = new Vector2(scannedAreaRect.X, Pos.Y + (Texture.Height / 2));
                    gradient1 = (point2.Y - point1.Y) / (point2.X - point1.X);
                    gradient2 = gradient1 * (-1);
                    axisIntercept1 = point1.Y - (gradient1 * point1.X);
                    axisIntercept2 = point3.Y - (gradient2 * point3.X);
                   
                    if (other.Pos.Y >= (gradient1 * other.Pos.X + axisIntercept1)
                        && other.Pos.Y <= (gradient2 * other.Pos.X + axisIntercept2))
                    {
                        intersectionOccured = true;
                    }
                    
                    break;
                }
                case ViewDirection.UP:
                {
                    point1 = new Vector2(Pos.X + (Texture.Width / 2), Pos.Y + (Texture.Height / 2));
                    point2 = new Vector2(scannedAreaRect.X + scannedAreaRect.Width, scannedAreaRect.Y);
                    point3 = new Vector2(scannedAreaRect.X, scannedAreaRect.Y);
                    point4 = new Vector2(Pos.X - (Texture.Width / 2), Pos.Y + (Texture.Height / 2));
                    gradient1 = (point2.Y - point1.Y) / (point2.X - point1.X);
                    gradient2 = gradient1 * (-1);
                    axisIntercept1 = point1.Y - (gradient1 * point1.X);
                    axisIntercept2 = point3.Y - (gradient2 * point3.X);

                    if (other.Pos.X <= (other.Pos.Y - axisIntercept1) / gradient1
                        && other.Pos.X >= (other.Pos.Y - axisIntercept2) / gradient2)
                    {
                        intersectionOccured = true;
                    }

                    break;
                }
                case ViewDirection.DOWN:
                {
                    point1 = new Vector2(Pos.X + (Texture.Width / 2), Pos.Y - (Texture.Height / 2));
                    point2 = new Vector2(scannedAreaRect.X + scannedAreaRect.Width, scannedAreaRect.Y + scannedAreaRect.Height);
                    point3 = new Vector2(scannedAreaRect.X, scannedAreaRect.Y + scannedAreaRect.Height);
                    point4 = new Vector2(Pos.X - (Texture.Width / 2), Pos.Y - (Texture.Height / 2));
                    gradient1 = (point2.Y - point1.Y) / (point2.X - point1.X);
                    gradient2 = gradient1 * (-1);
                    axisIntercept1 = point1.Y - (gradient1 * point1.X);
                    axisIntercept2 = point3.Y - (gradient2 * point3.X);

                    if (other.Pos.X <= (other.Pos.Y - axisIntercept1) / gradient1
                        && other.Pos.X >= (other.Pos.Y - axisIntercept2) / gradient2)
                    {
                        intersectionOccured = true;
                    }

                    break;
                }
            }

            return intersectionOccured;
        }

        public override void LoadContent(ContentManager content)
        {
            alive = content.Load<Texture2D>("Images/Battledroid/battledroid");
            dead = content.Load<Texture2D>("Images/Hero/hero_dead");
            Texture = alive;
        }

        public override void OnCollision(GameObject other)
        {
            if (IsAlive == false)
                return;

            if (other is Sword)
            {
                if (heroRef == null)
                    heroRef = (Hero)((Sword)other).Parent;

                Sword s = (Sword)other;
                
                // Set Battledroid's pushback direction
                ViewDirection direction = Physics.Collision.GetCollisionSide(other, this);
                switch (direction)
                {
                    case ViewDirection.LEFT:
                        PerformPushback(new Vector2(-1, 0));
                        break;
                    case ViewDirection.RIGHT:
                        PerformPushback(new Vector2(1, 0));
                        break;
                    case ViewDirection.UP:
                        PerformPushback(new Vector2(0, -1));
                        break;
                    case ViewDirection.DOWN:
                        PerformPushback(new Vector2(0, 1));
                        break;
                    default:
                        throw new InvalidOperationException("Unknown direction!");
                }

                Health -= DAMAGE_ON_HIT;
                currentStatus = Status.ATTACK;
            }
            else if (other is Hero)
            {
                if(heroRef == null)
                    heroRef = (Hero)other;

                switch (collisionType)
                {
                    case CollisionType.OBJECTCOLLISION:
                        Hero h = (Hero)other;

                        if (!h.IsInvincible())
                        {
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
                                    h.PerformPushback(new Vector2(0, -1));
                                    break;
                                case ViewDirection.DOWN:
                                    h.PerformPushback(new Vector2(0, 1));
                                    break;
                                default:
                                    throw new InvalidOperationException("Unknown direction!");
                            }

                            h.Health -= HERO_DAMAGE_ON_COLLISION;        
                        }
                          
                        break;
                    case CollisionType.VIEWCOLLISION:
                        break;
                    default:
                        Physics.Collision.ResolveIntersection(this, other);
                        break;
                }

                currentStatus = Status.ATTACK;
                collisionType = CollisionType.NONE;
            }
            else
            {
                Physics.Collision.ResolveIntersection(this, other);
            }
        }

        private void MoveToPosition(Vector2 targetPosition, int speed, GameTime gameTime)
        {        
            float seconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 newPos = this.Pos, directionVector, directionVectorNormalized;

            directionVector = new Vector2(targetPosition.X - this.Pos.X,
                                                  targetPosition.Y - this.Pos.Y);

            {
                Vector2 temp = directionVector;
                directionVector.Normalize();
                directionVectorNormalized = directionVector;
                directionVector = temp;
            }
            
            // Those comparisons with 2 are necessary to avoid a shiver like 
            // behaviour, caused by rounding errors
            if (directionVector.X > 2)
            {
                newPos.X += directionVectorNormalized.X * speed * seconds;

                if (CurrentDirection != ViewDirection.RIGHT)
                {
                    CurrentDirection = ViewDirection.RIGHT;
                    Rotation = 90f;
                }
            }

            if (directionVector.X < -2)
            {
                newPos.X += directionVectorNormalized.X * speed * seconds;

                if (CurrentDirection != ViewDirection.LEFT)
                {
                    CurrentDirection = ViewDirection.LEFT;
                    Rotation = 270f;
                }
            }

            if (directionVector.Y > 2)
            {
                newPos.Y += directionVectorNormalized.Y * speed * seconds;

                if (CurrentDirection != ViewDirection.DOWN)
                {
                    CurrentDirection = ViewDirection.DOWN;
                    Rotation = 180f;
                }
            }

            if (directionVector.Y < -2)
            {
                newPos.Y += directionVectorNormalized.Y * speed * seconds;

                if (CurrentDirection != ViewDirection.UP)
                {
                    CurrentDirection = ViewDirection.UP;
                    Rotation = 0f;
                }
            }

            this.Pos = newPos;
        }

        private void MoveToHero(GameTime gameTime)
        {
            double distance;

            // Compute the distance between Hero and the Battledroid
            distance = Math.Sqrt(Math.Pow(heroRef.Pos.X - this.Pos.X, 2) + Math.Pow(heroRef.Pos.Y - this.Pos.Y, 2));

            if (distance > MAX_DISTANCE_ON_ATTACK)
            {
                currentStatus = Status.PATROL;
                return;
            }

            MoveToPosition(heroRef.Pos, SPEED_ATTACK, gameTime);
        }
    }
}
