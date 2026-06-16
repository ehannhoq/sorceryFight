using Microsoft.Xna.Framework;
using sorceryFight.Content.Buffs.Vessel;
using sorceryFight.Content.Particles;
using sorceryFight.Content.Particles.UIParticles;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.SFPlayer
{
    public partial class SorceryFightPlayer : ModPlayer
    {
        public bool preventDeath = false;
        public bool deathFlag = false;
        public Vector2 deathPosition = Vector2.Zero;
        public bool rctAnimation = false;


        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            if (preventDeath)
            {
                preventDeath = false;
                Player.statLife = 1;
                Player.immune = true;
                Player.immuneTime = 60;
                playSound = false;
                genDust = false;
                onRevive?.Invoke();
                return false;
            }

            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
        }




        public override void UpdateDead()
        {
            ResetBuffs();
            deathPosition = Player.position;

            if (!deathFlag)
            {
                OnDeath();
                deathFlag = true;
            }

            disableRegenFromDE = false;
            disableRegenFromProjectiles = false;
        }


        private void OnDeath()
        {
            onDeath?.Invoke();
            if (!rctAnimation && sukunasFingerConsumed >= 1)
            {
                //King of Curses is set to 2 ticks when it's re-applied, this reapplies it if the player dies again
                if (Player.HasBuff(ModContent.BuffType<KingOfCursesBuff>()) && innateTechnique.InternalName == "Shrine")
                    Player.AddBuff(ModContent.BuffType<KingOfCursesBuff>(), SFUtils.BuffSecondsToTicks(2));

                else if (innateTechnique.InternalName == "Vessel")
                {
                    int chance = SorceryFightMod.IsDevMode() ? 100 : 15 + (int)(sukunasFingerConsumed * 3);
                    if (SFUtils.Roll(chance))
                    {
                        preventDeath = true;
                        int messageIndex = Main.rand.Next(6);
                        ChatHelper.SendChatMessageToClient(SFUtils.GetNetworkText("Mods.sorceryFight.Misc.SukunaRevive." + messageIndex), new Color(220,40,40), Player.whoAmI);

                        Player.AddBuff(ModContent.BuffType<KingOfCursesBuff>(), SFUtils.BuffSecondsToTicks(15 + (sukunasFingerConsumed * 2.25f)));
                    }
                }
            }
        }
    }
}
