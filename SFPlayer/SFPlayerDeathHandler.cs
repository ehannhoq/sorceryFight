using Microsoft.Xna.Framework;
using sorceryFight.Content.Buffs.Vessel;
using sorceryFight.Content.Particles;
using sorceryFight.Content.Particles.UIParticles;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.SFPlayer
{
    public partial class SorceryFightPlayer : ModPlayer
    {
        public bool preventDeath = false;
        public bool onDeath = false;
        public Vector2 deathPosition = Vector2.Zero;
        public bool rctAnimation = false;
        public int rctTimer = 0;

        public void PreventDeath()
        {
            preventDeath = true;
            Player.dead = false;
            Player.statLife = 1;
            Player.immuneTime = 120;
            Player.respawnTimer = 0;
        }

        public void RCTAnimation()
        {
            if (!rctAnimation) return;

            rctTimer++;
            Player.creativeGodMode = true;
            if (Player.statLife < Player.statLifeMax2)
            {
                Player.statLife++;
            }

            if (deathPosition == Vector2.Zero)
            {
                deathPosition = Player.position;
            }
            Player.position = deathPosition;

            if (rctTimer % 90 == 0)
            {
                SoundEngine.PlaySound(SorceryFightSounds.CommonHeartBeat with { Volume = 2f }, Player.Center);
            }

            int numParticles = rctTimer / 90;
            for (int i = 0; i <= numParticles; i++)
            {
                Vector2 particlePosition = Player.Center + new Vector2(Main.rand.NextFloat(-100f, 100f), Main.rand.NextFloat(-100f, 100f));
                Vector2 particleVelocity = particlePosition.DirectionTo(Player.Center) * 3;
                LinearParticle particle = new LinearParticle(particlePosition, particleVelocity, Color.Wheat, false, 0.9f, 0.5f, 30);
                ParticleController.SpawnParticle(particle);
            }


            if (rctTimer >= 300)
            {
                rctAnimation = false;
                rctTimer = 0;
                deathPosition = Vector2.Zero;
                unlockedRCT = true;

                if (heavenlyRestriction)
                {
                    ChatHelper.SendChatMessageToClient(SFUtils.GetNetworkText("Mods.sorceryFight.Misc.LeftItAllBehind.GeneralMessage"), Color.Green, Player.whoAmI);
                }
                else
                {
                    string keybindText = "[" + SFKeybinds.UseRCT.GetAssignedKeys()[Player.whoAmI] + "]" + SFUtils.GetLocalizationValue("Mods.sorceryFight.Misc.UnlockedRCT.KeyBindMessage");
                    ChatHelper.SendChatMessageToClient(SFUtils.GetNetworkText("Mods.sorceryFight.Misc.UnlockedRCT.GeneralMessage"), Color.Green, Player.whoAmI);
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(keybindText), Color.Green, Player.whoAmI);
                }

                SorceryFightUI.UpdateTechniqueUI.Invoke();

                for (int i = 0; i < 100; i++)
                {
                    Vector2 particleOffsetPosition = Player.Center + new Vector2(Main.rand.NextFloat(-200f, 200f), Main.rand.NextFloat(-200f, 200f));
                    Vector2 particleVelocity = Player.Center.DirectionTo(particleOffsetPosition) * 6;
                    LinearParticle particle = new LinearParticle(Player.Center, particleVelocity, Color.Wheat, false, 0.9f, 2f, 90);
                    ParticleController.SpawnParticle(particle);
                }
            }
        }


        public override void UpdateDead()
        {
            ResetBuffs();
            deathPosition = Player.position;

            if (rctAnimation)
            {
                PreventDeath();
            }

            if (!onDeath)
            {
                OnDeath();
                onDeath = true;
            }


            disableRegenFromDE = false;
            disableRegenFromProjectiles = false;
        }

        private void OnDeath()
        {
            if (!rctAnimation && sukunasFingerConsumed >= 1)
            {
                //King of Curses is set to 2 ticks when it's re-applied, this reapplies it if the player dies again
                if (Player.HasBuff(ModContent.BuffType<KingOfCursesBuff>()) && innateTechnique.Name == "Shrine")
                    Player.AddBuff(ModContent.BuffType<KingOfCursesBuff>(), SFUtils.BuffSecondsToTicks(2));

                else if (innateTechnique.Name == "Vessel")
                {
                    int chance = SorceryFightMod.IsDevMode() ? 100 : 15 + (int)(sukunasFingerConsumed * 3);
                    if (SFUtils.Roll(chance))
                    {
                        PreventDeath();
                        int messageIndex = Main.rand.Next(6);
                        ChatHelper.SendChatMessageToClient(SFUtils.GetNetworkText("Mods.sorceryFight.Misc.SukunaRevive." + messageIndex), new Color(220,40,40), Player.whoAmI);

                        Player.AddBuff(ModContent.BuffType<KingOfCursesBuff>(), SFUtils.BuffSecondsToTicks(15 + (sukunasFingerConsumed * 2.25f)));
                    }
                }
            }
        }
    }
}
