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
        public static SoundStyle BlackFlashImpact;

        public static SoundStyle AmplificationBlueChargeUp;
        public static SoundStyle ReversalRedChargeUp;
        public static SoundStyle HollowPurpleSnap;

        public static SoundStyle DismantleSlice;
        public static SoundStyle CleaveSwing;
        public static SoundStyle DivineFlameChargeUp;
        public static SoundStyle DivineFlameShoot;
        public static SoundStyle DivineFlameExplosion;
        public static SoundStyle WorldCuttingSlash;
        
        public static SoundStyle SoulDismantle;
        public static SoundStyle PiercingBlood;

        public static SoundStyle PachinkoBallCollision;
        public static SoundStyle TrainDoorsClosing;

        public static SoundStyle UnlimitedVoid;
        public static SoundStyle MalevolentShrine;
        public static SoundStyle IdleDeathGambleOpening;
        public static SoundStyle IDGSlots;
        public static SoundStyle IDGWoosh;
        public static SoundStyle IDGWooshLoop;

        public override void Load()
        {
            CommonFire = new("sorceryFight/Content/Sounds/CommonFire") { Volume = 1f };
            CommonWoosh = new("sorceryFight/Content/Sounds/CommonWoosh") { Volume = 1f };
            CommonHeartBeat = new("sorceryFight/Content/Sounds/CommonHeartBeat") { Volume = 1f };
            BlackFlashImpact = new("sorceryFight/Content/Sounds/BlackFlashImpact") { Volume = 1f };

            AmplificationBlueChargeUp = new("sorceryFight/Content/Sounds/Projectiles/AmplificationBlueChargeUp") { Volume = 1f };
            ReversalRedChargeUp = new("sorceryFight/Content/Sounds/Projectiles/ReversalRedChargeUp") { Volume = 1f };
            HollowPurpleSnap = new("sorceryFight/Content/Sounds/Projectiles/HollowPurpleSnap") { Volume = 1f };

            DismantleSlice = new("sorceryFight/Content/Sounds/Projectiles/DismantleSlice") { Volume = 1f };
            CleaveSwing = new("sorceryFight/Content/Sounds/Projectiles/CleaveSwing") { Volume = 1f };
            DivineFlameChargeUp = new("sorceryFight/Content/Sounds/Projectiles/DivineFlameChargeUp") { Volume = 1f };
            DivineFlameShoot = new("sorceryFight/Content/Sounds/Projectiles/DivineFlameShoot") { Volume = 1f };
            DivineFlameExplosion = new("sorceryFight/Content/Sounds/Projectiles/DivineFlameExplosion") { Volume = 1f };
            WorldCuttingSlash = new("sorceryFight/Content/Sounds/Projectiles/WorldCuttingSlash") { Volume = 1f };

            SoulDismantle = new("sorceryFight/Content/Sounds/Projectiles/SoulDismantleSnip") { Volume = 1f };
            PiercingBlood = new("sorceryFight/Content/Sounds/Projectiles/PiercingBlood") { Volume = 1f };

            PachinkoBallCollision = new("sorceryFight/Content/Sounds/Projectiles/PachinkoBallCollision") { Volume = 1f };
            TrainDoorsClosing = new("sorceryFight/Content/Sounds/Projectiles/TrainDoorsClosing") { Volume = 1f };

            MalevolentShrine = new("sorceryFight/Content/Sounds/DomainExpansions/MalevolentShrine") { Volume = 1f };
            UnlimitedVoid = new("sorceryFight/Content/Sounds/DomainExpansions/UnlimitedVoid") { Volume = 1f };
            
            IdleDeathGambleOpening = new("sorceryFight/Content/Sounds/DomainExpansions/IdleDeathGambleOpening") { Volume = 1f };
            IDGWooshLoop = new("sorceryFight/Content/Sounds/DomainExpansions/IDGWooshLoop") { Volume = 1f };
            IDGWoosh = new("sorceryFight/Content/Sounds/DomainExpansions/IDGWoosh") { Volume = 1f };
            IDGSlots = new("sorceryFight/Content/Sounds/DomainExpansions/IDGSlots") { Volume = 1f };
        }
    }
}
