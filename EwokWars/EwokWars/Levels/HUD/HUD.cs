using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace EwokWars.Levels.HUD
{
    abstract class HUD
    {
        virtual protected SpriteFont spriteFont { get; set; }
        virtual protected Viewport viewport { get; set; }
        protected Objects.Hero heroRef;
        protected int lastHeroHP;
        protected int lastHeroEnergy;
        protected enum HUDTextDirectory { HP, ENERGY, ROOM_DESCRIPTOR, POSITION, KEY_INFO_1, DBG_INFO_1 }
        protected Dictionary<HUDTextDirectory, HUD_Text> hudText;
        protected Room activeRoomRef;

        protected struct HUD_Text
        {
            public String text;
            public int x, y;
            public Color fontColor;
            public Color borderColor;

            public HUD_Text(String text, int x, int y, Color fontColor, Color borderColor)
            {          
                this.x = x;
                this.y = y;
                this.fontColor = fontColor;
                this.borderColor = borderColor;
                this.text = text;
            }
        }

        public HUD()
        {
            hudText = new Dictionary<HUDTextDirectory, HUD_Text>();
        }

        public abstract void Initialize(Viewport viewport, Objects.Hero hero, Room activeRoom);
        public abstract void LoadContent(ContentManager content);
        public abstract void Draw(SpriteBatch spriteBatch);

        public virtual void Update(GameTime gameTime)
        {
            // It's dispensable to create a new HUD_Text object if 
            // Hero's health has not changed
            if (lastHeroHP != heroRef.Health)
            {
                hudText[HUDTextDirectory.HP] = new HUD_Text("HP: " + heroRef.Health.ToString(), 5, 5, Color.White, Color.Black);
                lastHeroHP = heroRef.Health;
            }

            if (lastHeroEnergy != heroRef.Energy)
            {
                hudText[HUDTextDirectory.ENERGY] = new HUD_Text("Energy: " + heroRef.Energy.ToString(), 5, 25, Color.White, Color.Black);
                lastHeroEnergy = heroRef.Energy;
            }
        }

        protected void DrawBorderedText(SpriteBatch spriteBatch, HUD_Text hudText)
        {
            DrawBorderedText(spriteBatch, hudText.text, hudText.x, hudText.y, hudText.fontColor, hudText.borderColor);
        }

        protected void DrawBorderedText(SpriteBatch spriteBatch, String text, int x, int y, Color fontColor)
        {
            DrawBorderedText(spriteBatch, text, x, y, fontColor, Color.Black);
        }

        protected void DrawBorderedText(SpriteBatch spriteBatch, String text, int x, int y, Color fontColor, Color borderColor)
        {
            spriteBatch.DrawString(spriteFont, text, new Vector2(x + 1, y + 1), borderColor);
            spriteBatch.DrawString(spriteFont, text, new Vector2(x + 1, y - 1), borderColor);
            spriteBatch.DrawString(spriteFont, text, new Vector2(x - 1, y + 1), borderColor);
            spriteBatch.DrawString(spriteFont, text, new Vector2(x - 1, y - 1), borderColor);
            spriteBatch.DrawString(spriteFont, text, new Vector2(x, y), fontColor);
        }
    }
}
