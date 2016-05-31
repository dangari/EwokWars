using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using Microsoft.Xna.Framework;

// TODO: Replace those console outputs with a level object initialization
namespace EwokWars.Levels
{
    class LevelParser
    {
        private XmlDocument config;
        private const String configDirectory = "../../../../EwokWarsContent/Levels/";
        private Objects.Hero heroRef;
       
        // The hero attribute is used to carry information from one level to another. If this
        // attribute is null, LevelParser will create a new hero
        public LevelParser() 
        {
            config = new XmlDocument();            
        }

        // If the attribute hero equals null, we will create a new Hero. Otherwise we will use 
        // the given hero for the new Level
        public Level Parse(String configName, Objects.Hero hero = null, Boolean validate = true)
        {
            this.heroRef = hero;
            Level result = new Level();

            List<Objects.Portal> portals = new List<Objects.Portal>();

            if (validate)
                config.Load(GetValidatedReader(configDirectory + configName));
            else
                config.Load(configDirectory + configName);

            result.HUD = createHUD();
            portals = parsePortals();

            // Parse the rooms 
            {
                HashSet<int> parsedRooms = new HashSet<int>();
                XmlNodeList rooms = config.SelectNodes("/level/room");

                foreach (XmlNode entry in rooms)
                {
                    Room newRoom = createSingleRoom(entry, portals);

                    if (parsedRooms.Contains(newRoom.Id))
                        throw new LevelParserException("You can't define a Room twice! (Room #" + newRoom.Id + ")");
                    else
                        parsedRooms.Add(newRoom.Id);

                    result.addRoom(newRoom);
                }     
            }

            // Finally, attach our hero to the level
            result.Hero = heroRef;

            return result;
        }

        private HUD.HUD createHUD()
        {
            HUD.HUD result;

            XmlNode hud = config.SelectSingleNode("/level/hud");
            if (hud == null)
                result = (Levels.HUD.HUD)GameObjectDirectory.Create("HUD.Game");
            else
                result = (Levels.HUD.HUD)GameObjectDirectory.Create("HUD." + hud.Attributes["type"].Value);

            return result;
        }

        private Room createSingleRoom(XmlNode roomNode, List<Objects.Portal> availablePortals)
        {
            Room result = null;

            // Instantiate the new room
            {   
                float width = float.Parse(roomNode.Attributes["width"].Value);
                float height = float.Parse(roomNode.Attributes["height"].Value);
                int id = int.Parse(roomNode.Attributes["rId"].Value);
                String roomName = GetCurrentElementText(roomNode.SelectSingleNode("name"));

                result = new Room(id, width, height, roomName);
            }
            
            // Set the background and the walls
            {
                String backgroundType = GetCurrentElementText(roomNode.SelectSingleNode("design/background"));
                String wallType = GetCurrentElementText(roomNode.SelectSingleNode("design/walls"));

                result.GameObjects.Add(
                    (Objects.GameObject)GameObjectDirectory.Create(backgroundType));

                result.GameObjects.Add((Objects.GameObject)
                    GameObjectDirectory.Create(wallType, new Object[] { 0, 0, 
                    Objects.GameObject.SpriteOrigin.TOP_LEFT, 0 }));

                result.GameObjects.Add((Objects.GameObject)
                    GameObjectDirectory.Create(wallType, new Object[] { 0, 0, 
                    Objects.GameObject.SpriteOrigin.BOTTOM_LEFT, 90f }));

                result.GameObjects.Add((Objects.GameObject)
                    GameObjectDirectory.Create(wallType, new Object[] { result.Width, 0, 
                    Objects.GameObject.SpriteOrigin.TOP_RIGHT, 0f }));

                result.GameObjects.Add((Objects.GameObject)
                    GameObjectDirectory.Create(wallType, new Object[] { result.Width, result.Height, 
                    Objects.GameObject.SpriteOrigin.TOP_RIGHT, 90f }));
            }

            // Parse all movable objects
            {
                XmlNodeList movableObjects = roomNode.SelectNodes("design/movableobject");

                foreach (XmlNode entry in movableObjects)
                {
                    float xPos = float.Parse(GetCurrentElementText(entry.SelectSingleNode("position/x")));
                    float yPos = float.Parse(GetCurrentElementText(entry.SelectSingleNode("position/y")));
                    String model = GetCurrentElementText(entry.SelectSingleNode("model"));

                    if (!model.Equals("hayBale"))
                    {
                        throw new LevelParserException("Invalid model type: '" + model + "'");
                    }

                    result.GameObjects.Add(new Objects.MovableObject(new Vector2(xPos, yPos), model, heroRef));
                }
            }

            // Parse the enemies
            {
                XmlNodeList enemylist = roomNode.SelectNodes("enemies/enemy");

                foreach (XmlNode entry in enemylist)
                {
                    String enemyType = entry.Attributes["type"].Value;

                    switch (enemyType)
                    {
                        case "battledroid":
                            if (entry.SelectSingleNode("patrolling") == null)
                            {
                                // read the position
                                float xPos = float.Parse(GetCurrentElementText(entry.SelectSingleNode("position/x")));
                                float yPos = float.Parse(GetCurrentElementText(entry.SelectSingleNode("position/y")));

                                // read the viewdirection
                                Objects.DynamicObjects.ViewDirection viewDirection 
                                    = Objects.DynamicObjects.ViewDirection.DOWN;
                                String sViewDirection = entry.Attributes["viewdirection"].Value;

                                if (sViewDirection == null)
                                {
                                    viewDirection = Objects.DynamicObjects.ViewDirection.DOWN;
                                }
                                else
                                {
                                    switch (sViewDirection)
                                    {
                                        case "north":
                                            {
                                                viewDirection = Objects.DynamicObjects.ViewDirection.UP;
                                                break;
                                            }
                                        case "south":
                                            {
                                                viewDirection = Objects.DynamicObjects.ViewDirection.DOWN;
                                                break;
                                            }
                                        case "west":
                                            {
                                                viewDirection = Objects.DynamicObjects.ViewDirection.LEFT;
                                                break;
                                            }
                                        case "east":
                                            {
                                                viewDirection = Objects.DynamicObjects.ViewDirection.RIGHT;
                                                break;
                                            }
                                    }
                                }

                                result.GameObjects.Add(new Objects.BattleDroid(new Vector2(xPos, yPos), viewDirection));
                            }
                            else
                            {
                                // read the waypoints
                                List<Vector2> waypoints = new List<Vector2>();

                                XmlNodeList positions = entry.SelectNodes("patrolling/position");

                                foreach (XmlNode currentPosition in positions)
                                {
                                    float xPos = float.Parse(GetCurrentElementText(currentPosition.SelectSingleNode("x")));
                                    float yPos = float.Parse(GetCurrentElementText(currentPosition.SelectSingleNode("y")));

                                    waypoints.Add(new Vector2(xPos, yPos));
                                }

                                result.GameObjects.Add(new Objects.BattleDroid(waypoints));
                            }
                            break;
                        case "stormtrooper":
                            XmlNode position = entry.SelectSingleNode("position");

                            if (position == null)
                            {
                                throw new LevelParserException("stormtrooper's child element must be of element type 'position'");
                            }
                            else
                            {
                                // read the position
                                float xPos = float.Parse(GetCurrentElementText(entry.SelectSingleNode("position/x")));
                                float yPos = float.Parse(GetCurrentElementText(entry.SelectSingleNode("position/y")));

                                // read the viewdirection (south / north == vertical; east / west == horizontal)
                                bool movesHorizontal = true;
                                String sViewDirection = entry.Attributes["viewdirection"].Value;

                                if (sViewDirection != null)
                                {
                                    switch (sViewDirection)
                                    {
                                        case "north":
                                            {
                                                movesHorizontal = false;
                                                break;
                                            }
                                        case "south":
                                            {
                                                movesHorizontal = false;
                                                break;
                                            }
                                    }
                                }

                                result.GameObjects.Add(new Objects.Stormtrooper(xPos, yPos, movesHorizontal));
                            }   
                            break;
                    }
                }
            }
            
            // Attach portals to the room
            {
                XmlNodeList portalList = roomNode.SelectNodes("design/portals/portal");
                bool definedHeroSpawnpoint = false;

                foreach (XmlNode entry in portalList)
                {
                    int requiredPortal = int.Parse(entry.Attributes["pIdRef"].Value);

                    // Find the correct portal
                    foreach (Objects.Portal p in availablePortals)
                    {
                        if (p.Id == requiredPortal)
                        {
                            float xPos = float.Parse(GetCurrentElementText(entry.SelectSingleNode("position/x")));
                            float yPos = float.Parse(GetCurrentElementText(entry.SelectSingleNode("position/y")));

                            if (p.Id == 0) // The portal with id=0 defines the hero entry point
                            {
                                if (result.Id == 0)
                                {
                                    if (this.heroRef == null)
                                        this.heroRef = (Objects.Hero)GameObjectDirectory.Create(
                                            "Hero", new Object[] { xPos, yPos });
                                    else
                                        this.heroRef.Pos = new Vector2(xPos, yPos);
                                }                              
                                else
                                    throw new LevelParserException("You can attach Objects.Portal #0 only to Room #0!");

                                definedHeroSpawnpoint = true;
                            }
                            else
                            {
                                p.Pos = new Vector2(xPos, yPos);
                                p.RoomRef = result;
                                result.GameObjects.Add(p);
                                break;
                            }
                        }
                    }
                }

                if (result.Id == 0 && !definedHeroSpawnpoint)
                    throw new LevelParserException("You have to attache Objects.Portal #0 to Room #0!");

            }

            return result;
        }

        private List<Objects.Portal> parsePortals()
        {
            XmlNodeList pDefintions = config.SelectNodes("/level/eventobjects/portaldefinition");
            
            // Yes, this looks a little bit scary. We need those Tuple<> thing to map our parsed 
            // Portals pPartner attributes together. The Tuple<int, int> construct contains 
            // the current Objects.Portal's Id as first parameter and the current Objects.Portal's partnerId as
            // second parameter
            Dictionary<Tuple<int, int>, Objects.Portal> localPortalDir = new Dictionary<Tuple<int, int>, Objects.Portal>();

            // First, parse all portals, including the Id, partnerId and model
            {
                bool parsedEntryPortal = false;

                foreach (XmlNode node in pDefintions)
                {
                    int parsedPortalId = int.Parse(node.Attributes["pId"].Value);
                    int parsedPortalPartner;

                    // Since the attribute 'pPartnerId' is optional, we have to check if it's 
                    // specified
                    try
                    {
                        parsedPortalPartner = int.Parse(node.Attributes["pPartnerId"].Value);
                    }
                    catch(System.NullReferenceException)
                    {
                        parsedPortalPartner = parsedPortalId;
                    }

                    if (parsedPortalId == 0)
                        parsedEntryPortal = true;

                    // Retrieve the model type
                    String model = GetCurrentElementText(node.ChildNodes[0]);
                    if (!(model.Equals("ladderUp") || model.Equals("ladderDown")
                        || model.Equals("portal") || model.Equals("none")))
                        throw new LevelParserException("Invalid model for portal #" + parsedPortalId + ": " + model);

                    // Check for duplicate Objects.Portal Id's
                    foreach (KeyValuePair<Tuple<int, int>, Objects.Portal> entry in localPortalDir)
                    {
                        if (entry.Key.Item1 == parsedPortalId)
                            throw new LevelParserException("You can't define portal #" +
                                parsedPortalId + " twice!");
                    }

                    localPortalDir.Add(new Tuple<int, int>(parsedPortalId, parsedPortalPartner),
                                       new Objects.Portal(parsedPortalId, model));
                }

                if (!parsedEntryPortal)
                    throw new LevelParserException("You must define a portal with pId='0' " + 
                        "and attach it to room #0 to define Hero's spawnpoint!");
            }
            
            // Now set the Portal's pPartner attributes
            foreach(KeyValuePair<Tuple<int, int>, Objects.Portal> entry in localPortalDir)
            {
                bool partnerFound = false;

                foreach(KeyValuePair<Tuple<int, int>, Objects.Portal> partner in localPortalDir)
                {
                    if (entry.Key.Item2 == partner.Value.Id)
                    {
                        entry.Value.pPartner = partner.Value;
                        partnerFound = true;
                        break;
                    }
                }

                if (!partnerFound)
                    throw new LevelParserException("Couldn't find Objects.Portal #" + entry.Key.Item1
                        + "'s partner Objects.Portal with Id " + entry.Key.Item2);
            }

            // In a final step, move our Portals from the Dictionary into the result list
            List<Objects.Portal> result = new List<Objects.Portal>();

            foreach (KeyValuePair<Tuple<int, int>, Objects.Portal> entry in localPortalDir)
                result.Add(entry.Value);

            return result;
        }

        private String GetCurrentElementText(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Text)
                    return child.Value;
            }

            Console.Write("Warning: Found no element text for XmlNode \"" + node.Name + "\".");

            return "";
        }

        // The DTD error handler
        private void ValidationCallBack(object sender, ValidationEventArgs args)
        {
            Console.Write("\r\n\tValidation error: " + args.Message);
            throw new XmlException(args.Message);
        }

        private XmlReader GetValidatedReader(String inputURI)
        {
            // Used for validation purposes
            XmlReaderSettings settings;

            settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;
            settings.ValidationType = ValidationType.DTD;

            //Set the validation event handler.
            settings.ValidationEventHandler += new ValidationEventHandler(ValidationCallBack);

            return XmlReader.Create(inputURI, settings);
        }
    }
}
