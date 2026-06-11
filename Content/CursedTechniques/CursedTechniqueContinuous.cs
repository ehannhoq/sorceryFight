using System.IO;
using Microsoft.Xna.Framework;
using ReLogic.Peripherals.RGB;
using sorceryFight.SFPlayer;
using Terraria;

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

            if (!keyHeld)
            {
                Destroy();
            }
        }


        /// <summary>
        /// Kills the projectile. Called when keyHeld is false. Override this to implement custom behavior when key is no longer held down.
        /// </summary>
        public virtual void Destroy()
        {
            Projectile.Kill();
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