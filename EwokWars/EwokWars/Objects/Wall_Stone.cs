using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EwokWars.Objects
{
    class Wall_Stone : StaticObjects
    {       
        public Wall_Stone(int posX, int posY, GameObject.SpriteOrigin spriteOrigin, float rotation) 
        {
            Pos = new Vector2(posX, posY);
            this.spriteOrigin = spriteOrigin;
            Rotation = rotation;
        }

        public override void LoadContent(ContentManager content)
        {
            Texture = content.Load<Texture2D>("Images/Terrain/walls_stone");

            // Since our walls don't move, we need to call UpdateBoundingBox only once
            UpdateBoundingBox();
        }

        public override void Initialize()
        {
        }

        public override void Update(GameTime gameTime)
        {

        }
    }
}
