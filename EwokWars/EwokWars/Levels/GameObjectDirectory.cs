using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace EwokWars.Levels
{
    public abstract class GameObjectDirectory
    {
        public static Object Create(String identifier, Object[] args = null)
        {
            switch (identifier)
            {
                case "Wall_Stone":
                    return new Objects.Wall_Stone(
                        Convert.ToInt32(args[0]),
                        Convert.ToInt32(args[1]),
                        (Objects.GameObject.SpriteOrigin) args[2],
                        Convert.ToSingle(args[3]));
                case "Background_Stone":
                    return new Objects.Background_Stone();
                case "Hero":
                    return new Objects.Hero(
                        Convert.ToInt32(args[0]),
                        Convert.ToInt32(args[1]));
                case "HUD.Game":
                    return new Levels.HUD.Game();
                case "HUD.None":
                    return new Levels.HUD.Empty();
                case "HUD.Debug":
                    return new Levels.HUD.Debug();
                default:
                    throw new ArgumentException("Gameobject '" + identifier + "' not found.");
            }
        }
    }
}
