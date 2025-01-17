using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.SFPlayer
{
    public partial class SorceryFightPlayer : ModPlayer
    {
        public bool rctAnimation = false;
        public int rctTimer = 0;
        public Vector2 rctFrozenPosition = Vector2.Zero;

        public void PostAnimUpdate()
        {
            if (!rctAnimation) return;

            rctTimer ++;
            Player.creativeGodMode = true;
            if (Player.statLife < Player.statLifeMax2)
            {
                Player.statLife ++;
            }

            if (rctFrozenPosition == Vector2.Zero)
            {
                rctFrozenPosition = Player.position;
            }
            Player.position = rctFrozenPosition;

            if (rctTimer % 90 == 0)
            {
                SoundEngine.PlaySound(SorceryFightSounds.CommonHeartBeat with { Volume = 2f }, Player.Center);
            }

            int numParticles = rctTimer / 90;
            for (int i = 0; i <= numParticles; i ++)
            {
                Vector2 particlePosition = Player.Center + new Vector2(Main.rand.NextFloat(-100f, 100f), Main.rand.NextFloat(-100f, 100f));
                Vector2 particleVelocity = particlePosition.DirectionTo(Player.Center) * 3;
                LineParticle particle = new LineParticle(particlePosition, particleVelocity, false, 30, 0.5f, Color.Wheat);
                GeneralParticleHandler.SpawnParticle(particle);
            }


            if (rctTimer >= 300)
            {
                rctAnimation = false;
                rctTimer = 0;
                rctFrozenPosition = Vector2.Zero;
                unlockedRCT = true;

                for (int i = 0; i < 100; i ++)
                {
                    Vector2 particleOffsetPosition = Player.Center + new Vector2(Main.rand.NextFloat(-200f, 200f), Main.rand.NextFloat(-200f, 200f));
                    Vector2 particleVelocity = Player.Center.DirectionTo(particleOffsetPosition) * 6;
                    LineParticle particle = new LineParticle(Player.Center, particleVelocity, false, 90, 2f, Color.Wheat);
                    GeneralParticleHandler.SpawnParticle(particle);
                }

                string keybindText = "[" + SFKeybinds.UseRCT.GetAssignedKeys()[Player.whoAmI] + "]" + SFUtils.GetLocalizationValue("Mods.sorceryFight.Misc.UnlockedRCT.KeyBindMessage");
                ChatHelper.SendChatMessageToClient(SFUtils.GetNetworkText("Mods.sorceryFight.Misc.UnlockedRCT.GeneralMessage"), Color.Green, Player.whoAmI);
                ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(keybindText), Color.Green, Player.whoAmI);
                SorceryFightUI.UpdateTechniqueUI.Invoke();
            }
        }
        public void PreventRCTAnimDeath()
        {
            Player.dead = false;
            Player.statLife = 1;
            Player.immuneTime = 120;
            Player.respawnTimer = 0;
        }
    }
}
