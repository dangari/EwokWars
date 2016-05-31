using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using EwokWars.Objects;

namespace EwokWars.Physics
{
    public abstract class Collision
    {
        public struct AABB // = Axis aligned BoundingBox
        {
            private Vector2 aMin;
            private Vector2 aMax;
            private Vector2 bMin;
            private Vector2 bMax;
            public float left;
            public float right;
            public float top;
            public float bottom;

            public AABB(GameObject main, GameObject other)
            {
                aMin = new Vector2(main.BoundingBox.X, main.BoundingBox.Top);
                aMax = new Vector2(main.BoundingBox.Right, main.BoundingBox.Bottom);
                bMin = new Vector2(other.BoundingBox.X, other.BoundingBox.Top);
                bMax = new Vector2(other.BoundingBox.Right, other.BoundingBox.Bottom);
                left = (bMin.X - aMax.X);
                right = (bMax.X - aMin.X);
                top  = (bMin.Y - aMax.Y);
                bottom  = (bMax.Y - aMin.Y);
            }
        }

        public struct Axis
        {
            public Vector2 middle;
            public Vector2 down;
            public Vector2 right;

            public Axis(Rectangle BBox)
            {
                middle = new Vector2(BBox.X, BBox.Y);
                down = new Vector2(BBox.X, BBox.Y + BBox.Height);
                right = new Vector2(BBox.X + BBox.Width, BBox.Y);
            }
        }
       
        public static void PortalCollision(Portal source, Portal target, Hero heroRef, DynamicObjects.ViewDirection entrySide)
        {
			if (Levels.Level.lastUsedPortal != source)
            {
                if (source.RoomRef != target.RoomRef)
                    Levels.Level.NextRoom = target.RoomRef;

                const int portalOffset = 20;

                switch (entrySide)
                {
                    case DynamicObjects.ViewDirection.LEFT:
                        heroRef.Pos = new Vector2(target.Pos.X + target.Texture.Height + portalOffset, target.Pos.Y);
                        break;
                    case DynamicObjects.ViewDirection.UP:
                        heroRef.Pos = new Vector2(target.Pos.X, target.Pos.Y + target.Texture.Height + portalOffset);
                        break;
                    case DynamicObjects.ViewDirection.RIGHT:
                        heroRef.Pos = new Vector2(target.Pos.X - target.Texture.Height - portalOffset, target.Pos.Y);
                        break;
                    case DynamicObjects.ViewDirection.DOWN:
                        heroRef.Pos = new Vector2(target.Pos.X, target.Pos.Y - target.Texture.Height - portalOffset);
                        break;
                    default:
                        throw new InvalidOperationException("I can't handle ViewDirections " +
                            "of type " + entrySide);
                }
            }           
        }

        public static Vector2 GetSeparatingVector(GameObject dynamicObject, GameObject levelObject)
        {
            float mtdX = 0f;
            float mtdY = 0f;
            Physics.Collision.AABB aaBB = new Physics.Collision.AABB(dynamicObject, levelObject);

            if (Math.Abs(aaBB.left) < aaBB.right)
                mtdX = aaBB.left;
            else
                mtdX = aaBB.right;

            if (Math.Abs(aaBB.top) < aaBB.bottom)
                mtdY = aaBB.top;
            else
                mtdY = aaBB.bottom;

            // 0 the axis with the largest mtd value.
            if (Math.Abs(mtdX) < Math.Abs(mtdY))
                mtdY = 0;
            else
                mtdX = 0;

            return new Vector2(mtdX, mtdY);
        }

        public static DynamicObjects.ViewDirection GetCollisionSide(GameObject dynamicObject, GameObject levelObject)
        {
            Vector2 v = GetSeparatingVector(dynamicObject, levelObject);

            if (v.X == 0 && v.Y > 0)
                return DynamicObjects.ViewDirection.UP;
            if (v.X == 0 && v.Y < 0)
                return DynamicObjects.ViewDirection.DOWN;
            if (v.Y == 0 && v.X > 0)
                return DynamicObjects.ViewDirection.LEFT;
            if (v.Y == 0 && v.X < 0)
                return DynamicObjects.ViewDirection.RIGHT;

            throw new InvalidOperationException("Well, this shouldn't happen. " +
                "I can't process this Vector: " + v.ToString());
        }

        public static void ResolveIntersection(GameObject dynamicObject, GameObject levelObject)
        {
            dynamicObject.Pos += GetSeparatingVector(dynamicObject, levelObject);
        }

    }
}
