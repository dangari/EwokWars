using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EwokWars.Objects
{
    class Walls : StaticObjects
    {
        String name;

        public Walls(float Rotation, float x, float y, String name)
        {
            this.Pos = new Vector2(x, y);
            this.name = name;
            this.Rotation = Rotation * PI / 180;
        }
        public override void Initialize()
        {

        }

        public override void LoadContent(ContentManager content)
        {
            Texture = content.Load<Texture2D>(name);
            // objects only with a ratation 0 or 90
            if (Rotation == 0f)
                BoundingBox = new Rectangle((int)Pos.X - Texture.Width / 2, (int)Pos.Y - Texture.Height / 2, Texture.Width, Texture.Height);
            else
                BoundingBox = new Rectangle((int)Pos.X - Texture.Height / 2, (int)Pos.Y - Texture.Width / 2, Texture.Height, Texture.Width);
            this.MiddlePoint = new Vector2(Texture.Width / 2, Texture.Height / 2);
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void OnCollision(GameObjects other)
        {
            throw new NotImplementedException();
        }
    }
}
