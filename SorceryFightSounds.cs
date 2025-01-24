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
        public static SoundStyle CommonHeartBeat;

        public static SoundStyle AmplificationBlueChargeUp;
        public static SoundStyle ReversalRedChargeUp;
        public static SoundStyle HollowPurpleSnap;

        public static SoundStyle DismantleSlice;
        public static SoundStyle CleaveSwing;
        public static SoundStyle DivineFlameChargeUp;
        public static SoundStyle DivineFlameShoot;
        
        public static SoundStyle WorldCuttingSlash;
        


        public static SoundStyle UnlimitedVoid;
        public static SoundStyle MalevolentShrine;

        public override void Load()
        {
            CommonFire = new("sorceryFight/Content/Sounds/CommonFire") { Volume = 1f };
            CommonWoosh = new("sorceryFight/Content/Sounds/CommonWoosh") { Volume = 1f };
            CommonHeartBeat = new("sorceryFight/Content/Sounds/CommonHeartBeat") { Volume = 1f };

            AmplificationBlueChargeUp = new("sorceryFight/Content/Sounds/Projectiles/AmplificationBlueChargeUp") { Volume = 1f };
            ReversalRedChargeUp = new("sorceryFight/Content/Sounds/Projectiles/ReversalRedChargeUp") { Volume = 1f };
            HollowPurpleSnap = new("sorceryFight/Content/Sounds/Projectiles/HollowPurpleSnap") { Volume = 1f };

            DismantleSlice = new("sorceryFight/Content/Sounds/Projectiles/DismantleSlice") { Volume = 1f };
            CleaveSwing = new("sorceryFight/Content/Sounds/Projectiles/CleaveSwing") { Volume = 1f };
            DivineFlameChargeUp = new("sorceryFight/Content/Sounds/Projectiles/DivineFlameChargeUp") { Volume = 1f };
            DivineFlameShoot = new("sorceryFight/Content/Sounds/Projectiles/DivineFlameShoot") { Volume = 1f };
            WorldCuttingSlash = new("sorceryFight/Content/Sounds/Projectiles/WorldCuttingSlash") { Volume = 1f };

            MalevolentShrine = new("sorceryFight/Content/Sounds/DomainExpansions/MalevolentShrine") { Volume = 1f };
            UnlimitedVoid = new("sorceryFight/Content/Sounds/DomainExpansions/UnlimitedVoid") { Volume = 1f };
        }
    }
}
