using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace EwokWars.Levels
{
    class Level
    {
        private Miscellaneous.Camera camera;
        private List<Room> roomList;
        private Viewport viewport;
        private ContentManager content;
        private Room activeRoom;
        private Room finalRoom;

        public static Room NextRoom { get; set; } // TODO: try to make this non-static
        public static Objects.Portal lastUsedPortal = null;

        public bool Cleared { get; private set; }

        private HUD.HUD hud;
        public HUD.HUD HUD
        {
            get { return hud; }
            set { hud = value; }
        }

        private Objects.Hero hero;
        public Objects.Hero Hero 
        { 
            get { return hero; }
            set { hero = value; }
        }

        public Level()
        {
            roomList = new List<Room>();
        }

        public void addRoom(Room room)
        {
            roomList.Add(room);
        }

        public void LoadContent(ContentManager content)
        {
            this.content = content;

            hud.LoadContent(content);

            foreach (Room entry in roomList)
                entry.LoadContent(content);

        }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice gd /*remove this parameter later*/)
        {
            // Update the camera's position
            camera.Pos = hero.Pos;
            
            activeRoom.Draw(spriteBatch, gd, camera.GetTransformation());
            hud.Draw(spriteBatch);
        }

        public void Initialize(Viewport viewport)
        {
            Cleared = false;
			this.viewport = viewport;

            if (NextRoom == null) // Set the starting room
            {
                activeRoom = GetStartingRoom();
                NextRoom = activeRoom;
            }

            SwitchRoom();

            foreach (Room entry in roomList)
                entry.Initialize(hero);

            finalRoom = GetFinalRoom();
        }

        private void SwitchRoom()
        {
            activeRoom = NextRoom;
            activeRoom.Initialize(hero);
            hud.Initialize(viewport, hero, activeRoom);
            camera = new Miscellaneous.Camera(viewport, (int)activeRoom.Width, (int)activeRoom.Height);
        }

        public void Update(GameTime gameTime)
        {
            if (NextRoom != activeRoom)
                SwitchRoom();

            activeRoom.Update(gameTime);
            hud.Update(gameTime);

            // Check if the current Level is solved
            if (activeRoom == finalRoom)
            {
                Cleared = true;

                foreach (Objects.GameObject entry in activeRoom.GameObjects)
                {
                    if (entry is Objects.LivingObject && entry != hero)
                    {
                        if (((Objects.LivingObject)entry).IsAlive == true)
                        {
                            Cleared = false;
                            break;
                        }
                    }
                }
            }
        }

        public void Reset()
        {
            Cleared = false;
            activeRoom = GetStartingRoom();
            NextRoom = activeRoom;
            hero.Reset();
            activeRoom.Initialize(hero);
            hud.Initialize(viewport, hero, activeRoom);
            camera = new Miscellaneous.Camera(viewport, (int)activeRoom.Width, (int)activeRoom.Height);
        }

        private Room GetStartingRoom()
        {
            foreach (Room entry in roomList)
            {
                if (entry.Id == 0)
                    return entry;
            }

            return null;
        }

        private Room GetFinalRoom()
        {
            int maxRoomId = 0;

            foreach (Room entry in roomList)
            {
                if (entry.Id > maxRoomId)
                    maxRoomId = entry.Id;
            }

            foreach (Room entry in roomList)
            {
                if (entry.Id == maxRoomId)
                    return entry;
            }

            throw new InvalidOperationException("This shouldn't happen, check this procedure");
        }
    }
}
