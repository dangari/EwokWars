using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EwokWars.Levels
{
    public class Room
    {
        public List<Objects.GameObject> GameObjects { get; set; }
        private List<Objects.DynamicObjects> DynamicObjects { get; set; }
        public Objects.Hero HeroRef { get; private set; }
        public int Id { get; private set; }
        public String Name { get; set; }

        private DebugKit debugKit;
        private bool firstLoad = true;

        private Vector2 dimensions;
        public float Width
        {
            get { return dimensions.X; }
            private set { dimensions.X = value; }
        }

        public float Height
        {
            get { return dimensions.Y; }
            private set { dimensions.Y = value; }
        }

        public Room(int id, float width, float height, String name)
        {
            Id = id;
            dimensions = new Vector2(width, height);
            Name = name;
            GameObjects = new List<Objects.GameObject>();
            DynamicObjects = new List<Objects.DynamicObjects>();

            // Create a new debugKit instance if you want to make boundingboxes visible
            //debugKit = new DebugKit();
            debugKit = null;
        }

        public void Initialize(Objects.Hero heroRef)
        {
            this.HeroRef = heroRef;

            HeroRef.Initialize();

            //prevent doubleLoad at start
            if (firstLoad)
                firstLoad = false;
            else
            {
                DynamicObjects.Clear();
                foreach (Objects.GameObject go in GameObjects)
                {
                    go.Initialize();

                    if (go is Objects.DynamicObjects)
                        DynamicObjects.Add((Objects.DynamicObjects)go);
                }

                DynamicObjects.Add(HeroRef);
            }
        }

        public void Update(GameTime gameTime)
        {
            // Perform updates for all GameObjects...
            {
                HeroRef.Update(gameTime);

                foreach (Objects.GameObject go in GameObjects)
                    go.Update(gameTime);
            }

            // Perform collisionchecks for all DynamicObjects
            foreach (Objects.DynamicObjects dyn in DynamicObjects)
            {
                if (dyn != HeroRef && dyn.Intersects(HeroRef))
                    dyn.OnCollision(HeroRef);

                foreach (Objects.GameObject go in GameObjects)
                {
                    // Prevent collisionchecks with an object itself
                    if (dyn == go)
                        continue;

                    if (dyn.Intersects(go))
                        dyn.OnCollision(go);
                }
            }
        }

        public void LoadContent(ContentManager content)
        {
            HeroRef.LoadContent(content);

            foreach (Objects.GameObject go in GameObjects)
                go.LoadContent(content);
        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice gd /*remove this parameter later*/
            , Matrix transformationMatrix)
        {
            // Apply the camera's transformation matrix in order to move the world accordingly to Heros position
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, transformationMatrix);

            foreach (EwokWars.Objects.GameObject go in GameObjects)
                go.Draw(spriteBatch, gd, debugKit);

            HeroRef.Draw(spriteBatch, gd, debugKit);
            spriteBatch.End();
        }
    }
}
