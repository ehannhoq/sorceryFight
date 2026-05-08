using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.StructureHelper
{
    public class CommandListener : ModPlayer
    {
        private static Dictionary<string, Action<string[]>> MasterCommands = new()
        {
            { "tp", TeleportPlayer },
            { "structure", StructureHandler.ProcessCommand }
        };

        private bool isChatting = false;
        private string lastChat = "";
        public override void PreUpdate()
        {
            string currentChat = Main.chatText;

            if (isChatting && currentChat == "")
            {
                isChatting = false;


                if (lastChat.StartsWith("/"))
                {
                    lastChat = lastChat[1..];

                    string[] tokens = lastChat.Split(' ');

                    string command = tokens[0];
                    string[] args = tokens.Skip(1).ToArray();

                    if (MasterCommands.TryGetValue(command, out Action<string[]> action))
                    {
                        action(args);
                    }
                }
                lastChat = "";
            }

            if (currentChat != lastChat)
            {
                isChatting = true;
                lastChat = currentChat;
            }
        }

        private static void TeleportPlayer(string[] args)
        {
            if (args.Length != 2)
            {
                Main.NewText("Usage: /tp [x] [y]");
                return;
            }

            int x = int.Parse(args[0]);
            int y = int.Parse(args[1]);

            Main.LocalPlayer.position.X = x;
            Main.LocalPlayer.position.Y = y;
        }

    }
}
