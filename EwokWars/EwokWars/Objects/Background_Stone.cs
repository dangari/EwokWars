using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EwokWars.Objects
{
    class Background_Stone : StaticObjects
    {
        public Background_Stone() 
        {
            Pos = new Vector2(0, 0);
            Rotation = 0f;
            spriteOrigin = GameObject.SpriteOrigin.TOP_LEFT;
        }

        public override void LoadContent(ContentManager content)
        {
            Texture = content.Load<Texture2D>("Images/Terrain/bg_stone_filled");

            // We don't want to collide with the background, so we simply 
            // use an unreachable Boundingbox
            UpdateBoundingBox(new Rectangle(-1, -1, 0, 0));
        }

        public override void Initialize()
        {
        }

        public override void Update(GameTime gameTime)
        {
        }
    }
}
