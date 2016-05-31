using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EwokWars.Objects
{
    class TestBox : StaticObjects
    {
        public TestBox(int posX, int posY, GameObject.SpriteOrigin spriteOrigin, float rotation) 
        {
            Pos = new Vector2(posX, posY);
            this.spriteOrigin = spriteOrigin;
            Rotation = rotation;
        }


        public override void LoadContent(ContentManager content)
        {
            Texture = content.Load<Texture2D>("Images/testbox_wide");
            UpdateBoundingBox();
        }

        public override void Initialize()
        {
            
        }

        public override void Update(GameTime gameTime)
        {
            
        }

        public override void OnCollision(GameObject other)
        {
            Console.WriteLine(Physics.Collision.GetCollisionSide(this, other));
            base.OnCollision(other);
        }

    }
}
