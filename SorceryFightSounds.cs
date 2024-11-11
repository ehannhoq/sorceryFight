using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ModLoader;

namespace sorceryFight
{
    public class SorceryFightSounds : ModSystem
    {
        public static SoundStyle CommonFire;
        public static SoundStyle CommonWoosh;

        public static SoundStyle AmplificationBlueChargeUp;
        public static SoundStyle ReversalRedChargeUp;
        public static SoundStyle HollowPurpleSnap;

        public override void Load()
        {
            CommonFire = new("sorceryFight/Content/Sounds/CommonFire") { Volume = 1f };
            CommonWoosh = new("sorceryFight/Content/Sounds/CommonWoosh") { Volume = 1f };


            AmplificationBlueChargeUp = new("sorceryFight/Content/Sounds/Projectiles/AmplificationBlueChargeUp") { Volume = 1f };
            ReversalRedChargeUp = new("sorceryFight/Content/Sounds/Projectiles/ReversalRedChargeUp") { Volume = 1f };
            HollowPurpleSnap = new("sorceryFight/Content/Sounds/Projectiles/HollowPurpleSnap") { Volume = 1f };

        }
    }
}
