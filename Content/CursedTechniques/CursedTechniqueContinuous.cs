using System;
using System.IO;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.DataStructures;

namespace sorceryFight.Content.CursedTechniques
{
    public abstract class CursedTechniqueContinuous : CursedTechnique
    {
        /// <summary>
        /// Current status of if the use cursed technique button is held down. Automatically set in AI(), so call base.AI() in implementation.
        /// When keyHeld changes, it automatically syncs with the server and other clients.
        /// </summary>
        public bool keyHeld;

        /// <summary>
        /// Converts the cost of the cursed technique to cost/second.
        /// </summary>
        public override float CalculateTrueCost(SorceryFightPlayer sf)
        {
            return SFUtils.RateSecondsToTicks(base.CalculateTrueCost(sf));
        }


        public override string GetStats(SorceryFightPlayer sf)
        {
            string localizationCategoryKey = "Mods.sorceryFight.Misc.CursedTechniques";

            string damage = SFUtils.GetLocalization(localizationCategoryKey + ".Damage")
                .WithFormatArgs(CalculateTrueDamage(sf)).Value;

            string ceCost = SFUtils.GetLocalization(localizationCategoryKey + ".ContinuousCost")
                .WithFormatArgs((int)MathF.Round(base.CalculateTrueCost(sf))).Value;

            string stats = damage + "\n" + ceCost;

            return stats;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }


        /// <summary>
        /// Base AI() method for ModProjectiles. This override automatically handles keybind status and destroys the projectile.
        /// </summary>
        public override void AI()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                if (SFKeybinds.UseTechnique.Current != keyHeld)
                {
                    keyHeld = SFKeybinds.UseTechnique.Current;
                    Projectile.netUpdate = true;
                }
            }

            Projectile.timeLeft = 2;

            SorceryFightPlayer sfPlayer = Main.player[Projectile.owner].SorceryFight();
            sfPlayer.disableRegenFromProjectiles = true;

            DrainCost(sfPlayer);

            if (!keyHeld)
            {
                Destroy(sfPlayer);
            }
        }

        public virtual void DrainCost(SorceryFightPlayer sfPlayer)
        {
            sfPlayer.cursedEnergy -= CalculateTrueCost(sfPlayer);
            if (sfPlayer.cursedEnergy <= 0)
            {
                Destroy(sfPlayer);
            }
        }


        /// <summary>
        /// Kills the projectile. Called when keyHeld is false. Override this to implement custom behavior when key is no longer held down.
        /// </summary>
        public virtual void Destroy(SorceryFightPlayer sfPlayer)
        {
            Projectile.Kill();
            sfPlayer.disableRegenFromProjectiles = false;
        }


        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(keyHeld);
        }


        public override void ReceiveExtraAI(BinaryReader reader)
        {
            keyHeld = reader.ReadBoolean();
        }
    }
}