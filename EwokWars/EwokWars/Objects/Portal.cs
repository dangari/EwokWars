using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EwokWars.Objects
{
    public class Portal : Objects.DynamicObjects
    {
        public int Id { get; private set; }
        public Levels.Room RoomRef { get; set; }
        public Portal pPartner { get; set; }
        private String model;

        public Portal(int id, String model)
        {
            Id = id;
            pPartner = null;
            spriteOrigin = SpriteOrigin.CENTER;
            Rotation = 0f;
            this.model = model;
        }

        public override void Initialize()
        {

        }

        public override void OnCollision(Objects.GameObject other)
        {
            if (pPartner == this) 
                return;

            if (other is Objects.Hero)
            {
                if (((Hero)other).IsInvincible())
                {
                    Physics.Collision.ResolveIntersection(other, this);
                }
                else
                {
                    Physics.Collision.PortalCollision(
                    this, pPartner, (Hero)other,
                    Physics.Collision.GetCollisionSide(this, other));
                }              
                
                return;
            }

            Physics.Collision.ResolveIntersection(other, this);
        }

        public override void LoadContent(ContentManager content)
        {
            switch (model)
            {
                case "portal":
                    Texture = content.Load<Texture2D>("Images/Portal/portal");
                    break;
                case "ladderUp":
                    Texture = content.Load<Texture2D>("Images/Portal/ladder_up");
                    break;
                case "ladderDown":
                    Texture = content.Load<Texture2D>("Images/Portal/ladder_down");
                    break;
                case "none":
                    Texture = content.Load<Texture2D>("Images/Portal/none");
                    break;
            }
            
            UpdateBoundingBox();
        }

        public override void Update(GameTime gameTime)
        {
            
        }
    }
}
