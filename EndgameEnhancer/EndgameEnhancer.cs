using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using EndgameEnhancer;
using Terraria_Server.Plugin;
using Terraria_Server;
using Terraria_Server.Events;
using Terraria_Server.Misc;
using System.IO;

namespace EndgameEnhancer
{
    public class EndgameEnhancer : Plugin
    {
        /*
         * @Developers
         * 
         * Plugins need to be in .NET 3.5
         * Otherwise TDSM will be unable to load it. 
         * 
         * As of June 16, 1:15 AM, TDSM should now load Plugins Dynamically.
         */

        // tConsole is used for when logging Output to the console & a log file.

        public File properties;
        public bool mobSpawn = true;
        public bool tileBreak = true;
        public bool explosives = true;
        public bool isEnabled = false;
        File combatXP;
        File defenseXP;
        File travelerXP;
        Vector2[] lastPos = new Vector2[255];

        public override void Load()
        {
            Name = "Endgame Enhancer";
            Description = "Enhances the Endgame of Terraria.";
            Author = "Nivek";
            Version = "1.01";
            TDSMBuild = 21; //Current Release - Working

            string pluginFolder = Statics.PluginPath + Path.DirectorySeparatorChar + "TDSM";
            //Create folder if it doesn't exist
            CreateDirectory(pluginFolder);

            //setup a new properties file
            properties = new File(pluginFolder + Path.DirectorySeparatorChar + "tdsmplugin.properties");
            properties.Load();
            //properties.Save();

            combatXP = new File(pluginFolder + Path.DirectorySeparatorChar + "combatXP.file");
            combatXP.Load();
            //combatXP.Save();

            defenseXP = new File(pluginFolder + Path.DirectorySeparatorChar + "defenseXP.file");
            defenseXP.Load();
            //defenseXP.Save();

            travelerXP = new File(pluginFolder + Path.DirectorySeparatorChar + "travelerXP.file");
            travelerXP.Load();
            //travelerXP.Save();




            //read properties data
            mobSpawn = properties.getValue("mobSpawn", true);
            tileBreak = properties.getValue("tileBreak", true);
            explosives = properties.getValue("explosives", true);

            isEnabled = true;
        }

        public override void Enable()
        {
            Program.tConsole.WriteLine(base.Name + " enabled.");
            //Register Hooks
            this.registerHook(Hooks.TILE_CHANGE);
            this.registerHook(Hooks.PLAYER_COMMAND);
            this.registerHook(Hooks.PLAYER_PROJECTILE);
            this.registerHook(Hooks.NPC_DEATH);
            this.registerHook(Hooks.PLAYER_HURT);
            this.registerHook(Hooks.PLAYER_MOVE);
            this.registerHook(Hooks.PLAYER_KEYPRESS);
            this.registerHook(Hooks.PLAYER_LOGIN);

            /*             
             if (!mobSpawn)
             {
                 Main.stopSpawns = isEnabled;
                 Program.tConsole.WriteLine("Disabled NPC Spawning");
             }*/
        }

        public override void Disable()
        {
            Program.tConsole.WriteLine(base.Name + " disabled.");
            isEnabled = false;
        }

        public override void onPlayerCommand(PlayerCommandEvent Event)
        {
            Player player = Event.Player;
            if (isEnabled == false) { return; }
            string[] commands = Event.Message.ToLower().Split(' '); //Split into sections (to lower case to work with it better)
            if (commands.Length > 0)
            {
                if (commands[0] != null && commands[0].Trim().Length > 0) //If it is nothing, and the string is actually something
                {

                    if (commands[0].Equals("/xp"))
                    {

                        if (commands[1].ToLower().StartsWith("c"))
                        {
                            int cXP = combatXP.getPlayerValue(player.Name);

                            Program.tConsole.WriteLine("[Endgame Enhancer] " + player.Name + " used /xp combat");
                            player.sendMessage("You have " + cXP + " Combat XP.");

                        }
                        if (commands[1].ToLower().StartsWith("d"))
                        {
                            int dXP = defenseXP.getPlayerValue(player.Name);

                            Program.tConsole.WriteLine("[Endgame Enhancer] " + player.Name + " used /xp defense");
                            player.sendMessage("You have " + dXP + " Defense XP.");


                        }
                        if (commands[1].ToLower().StartsWith("t"))
                        {
                            int tXP = travelerXP.getPlayerValue(player.Name);

                            Program.tConsole.WriteLine("[Endgame Enhancer] " + player.Name + " used /xp traveler");
                            player.sendMessage("You have " + tXP + " Traveler XP.");


                        }


                        if (commands[1].ToLower().StartsWith("a"))
                        {
                            int cXP = combatXP.getPlayerValue(player.Name);
                            int dXP = defenseXP.getPlayerValue(player.Name);
                            int tXP = travelerXP.getPlayerValue(player.Name);

                            Program.tConsole.WriteLine("[Endgame Enhancer] " + player.Name + " used /xp");
                            player.sendMessage(cXP + " Combat XP, " + dXP + " Defense XP, " + tXP + " Traveler XP");


                        }

                    }



                }
            }
        }

        public override void onTileChange(PlayerTileChangeEvent Event)
        {
            if (isEnabled == false || tileBreak == false)
            {
                Event.Cancelled = true;
            }

            //Program.tConsole.WriteLine("[Plugin] Cancelled Tile change of Player: " + ((Player)Event.Sender).Name);
        }

        public override void onPlayerProjectileUse(PlayerProjectileEvent Event)
        {
            if (isEnabled == false) { return; }
            if (!explosives)
            {

                int type = Event.Projectile.type;
                if (type == 28 || type == 29 || type == 37)
                {
                    Event.Cancelled = true;
                    Program.tConsole.WriteLine("[Plugin] Cancelled Explosive usage of Player: " + ((Player)Event.Sender).Name);
                }
            }


            if (isEnabled == false || explosives == false)
            {
                Event.Cancelled = true;
            }

        }


        public override void onPlayerHurt(PlayerHurtEvent Event)
        {
            //base.onPlayerHurt(Event);

            Player player = Event.Player;

            int xp = defenseXP.getPlayerValue(player.Name);
            xp++;

            defenseXP.setPlayerValue(player.Name, xp);
            defenseXP.Save();

            player.sendMessage("+1 Defense XP");

            Event.Cancelled = true;

        }


        public override void onNPCDeath(NPCDeathEvent Event)
        {
            //base.onNPCDeath(Event);

            Player player = Main.players[Event.Npc.target];

            int xp = combatXP.getPlayerValue(player.Name);
            xp++;

            combatXP.setPlayerValue(player.Name, xp);
            combatXP.Save();

            player.sendMessage("+1 Combat XP");

            //Program.tConsole.WriteLine("NPCDeath: " + Event.Npc.Name);
            Event.Cancelled = true;
        }



        public override void onPlayerJoin(PlayerLoginEvent Event)
        {
            //base.onPlayerKeyPress(Event);

            Player player = Event.Player;

            lastPos[player.whoAmi] = player.getLocation();

        }

        public override void onPlayerKeyPress(PlayerKeyPressEvent Event)
        {
            //base.onPlayerKeyPress(Event);
            Player player = Event.Player;

            //if (Event.KeysPressed.Left) player.sendMessage("--Left");
            //if (Event.KeysPressed.Right) player.sendMessage("--Right");
            //if (Event.KeysPressed.Up) player.sendMessage("--Up");
            //if (Event.KeysPressed.Down) player.sendMessage("--Down");
            //if (Event.KeysPressed.Jump) player.sendMessage("--Jump");

            if (Event.KeysPressed.Left || Event.KeysPressed.Right)
            {
                int xp = Math.Abs((int)(lastPos[player.whoAmi].X - player.getLocation().X));
                xp += travelerXP.getPlayerValue(player.Name);

                travelerXP.setPlayerValue(player.Name, xp);
                travelerXP.Save();

                lastPos[player.whoAmi] = player.getLocation();

            }

            if (Event.KeysPressed.Up)
            {
                int cXP = combatXP.getPlayerValue(player.Name);
                int dXP = defenseXP.getPlayerValue(player.Name);
                int tXP = travelerXP.getPlayerValue(player.Name);
                player.sendMessage(cXP + " Combat XP, " + dXP + " Defense XP, " + tXP + " Traveler XP");

            }


        }


        public override void onPlayerMove(PlayerMoveEvent Event)
        {
            //base.onPlayerMove(Event);
            //Player player = Event.Player;

            //player.sendMessage("You have moved.");
        }


        private static void CreateDirectory(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }

    }
}
