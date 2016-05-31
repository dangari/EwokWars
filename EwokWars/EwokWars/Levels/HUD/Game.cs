using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace EwokWars.Levels.HUD
{
    class Game : HUD
    {
        public override void LoadContent(ContentManager content)
        {
            this.spriteFont = content.Load<SpriteFont>("Fonts/HUD");
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            foreach (HUD_Text currentText in hudText.Values)
            {
                DrawBorderedText(spriteBatch, currentText);
            }

            spriteBatch.End();
        }

        public override void Initialize(Viewport viewport, Objects.Hero hero, Room activeRoom)
        {
            this.viewport = viewport;
            this.heroRef = hero;
            this.lastHeroHP = heroRef.Health;
            this.lastHeroEnergy = heroRef.Energy;

            hudText.Clear();

            hudText.Add(HUDTextDirectory.HP, new HUD_Text("HP: " + heroRef.Health.ToString(), 5, 5, Color.White, Color.Black));
            hudText.Add(HUDTextDirectory.ENERGY, new HUD_Text("Energy: " + heroRef.Energy.ToString(), 5, 25, Color.White, Color.Black));
            hudText.Add(HUDTextDirectory.ROOM_DESCRIPTOR, new HUD_Text("Location: " + activeRoom.Name, 5, 45, Color.White, Color.Black));
            hudText.Add(HUDTextDirectory.KEY_INFO_1, new HUD_Text("Press [ESC] to pause", 5, 65, Color.White, Color.Black));
            hudText.Add(HUDTextDirectory.DBG_INFO_1, new HUD_Text("This is our game HUD", 5, viewport.Height - 25, Color.White, Color.Black));
        }
    }
}
