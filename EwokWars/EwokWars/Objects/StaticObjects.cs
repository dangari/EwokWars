using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EwokWars.Objects
{
    abstract class StaticObjects : GameObject
    {
        override public float Rotation
        {
            get { return base.Rotation; }
            set
            {
                if (value != 0f && value != 90f && value != 180f && value != 270f)
                    throw new InvalidOperationException();

                base.Rotation = value;
            }
        }

        public override void OnCollision(GameObject other)
        {
            // For most of the StaticObjects it's enough to prevent DynamicObjects from
            // gliding into this object
            Physics.Collision.ResolveIntersection(other, this);
        }
    }
}
