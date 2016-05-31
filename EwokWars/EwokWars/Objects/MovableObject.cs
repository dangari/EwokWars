using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace EwokWars.Objects
{
    class MovableObject : DynamicObjects
    {
        private String model;
        private Hero heroRef;

        private Vector2 initialPosition;

        public MovableObject(Vector2 position, String model, Hero heroRef)
        {
            initialPosition = position;          
            this.model = model;
            this.heroRef = heroRef;
        }

        public override void Initialize()
        {
            this.Pos = initialPosition;
        }

        public override void Update(GameTime gameTime)
        {
            UpdateBoundingBox();
        }

        public override void LoadContent(ContentManager content)
        {
            Texture = content.Load<Texture2D>("Images/hayBale");
        }

        public override void OnCollision(GameObject other)
        {
            if (other is Hero)
            {
                Physics.Collision.ResolveIntersection(this, other);
            }         
            else if(!(other is Sword))
            {
                Vector2 sep = Physics.Collision.GetSeparatingVector(this, other);
                this.Pos += sep;
                heroRef.Pos += sep;
            }
        }
    }
}
