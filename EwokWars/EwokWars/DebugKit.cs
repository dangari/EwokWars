using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace EwokWars
{
   public class DebugKit
    {        
        private StreamWriter log;

        public void DrawBBox(Rectangle Bbox, GraphicsDevice graphicDevice, SpriteBatch spriteBatch)
        {
            Texture2D t = new Texture2D(graphicDevice, 1, 1);
            t.SetData(new[] { Color.White });
            int bw = 2;
            spriteBatch.Draw(t, new Rectangle(Bbox.Left, Bbox.Top, bw, Bbox.Height), Color.Red); // Left
            spriteBatch.Draw(t, new Rectangle(Bbox.Right, Bbox.Top, bw, Bbox.Height), Color.Red); // Right
            spriteBatch.Draw(t, new Rectangle(Bbox.Left, Bbox.Top, Bbox.Width, bw), Color.Red); // Top
            spriteBatch.Draw(t, new Rectangle(Bbox.Left, Bbox.Bottom, Bbox.Width, bw), Color.Red); // Bottom
        }

        public void logWriter(String strLogText)
        {

            if (!File.Exists("logfile.txt"))
            {
                log = new StreamWriter("logfile.txt");
            }
            else
            {
                log = File.AppendText("logfile.txt");
            }
            // Write to the file:
            log.WriteLine(DateTime.Now);
            log.WriteLine(strLogText);
            log.WriteLine();
            log.Close();
        }

    }
}
