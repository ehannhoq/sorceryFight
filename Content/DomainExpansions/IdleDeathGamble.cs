using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using sorceryFight.Content.CursedTechniques.Vessel;
using sorceryFight.SFPlayer;
using CalamityMod.Events;
using Terraria.ID;
using sorceryFight.Content.Buffs.PrivatePureLoveTrain;
using System;

namespace sorceryFight.Content.DomainExpansions
{
    public class IdleDeathGamble : DomainExpansion
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.DomainExpansions.IdleDeathGamble.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.DomainExpansions.IdleDeathGamble.Description");
        public override int CostPerSecond { get; set; } = 10;

        int rollTimer;
        int rollOneValue, rollTwoValue, rollThreeValue;
        bool rolled;
        Vector2 rollOnePosition;
        Vector2 rollTwoPosition;
        Vector2 rollThreePosition;
        Texture2D rollOneTexture;
        Texture2D rollTwoTexture;
        Texture2D rollThreeTexture;
        Vector2 pachinkoMachinePosition;
        Texture2D pachinkoMachineTexture;

        ref float pachinkoMachineLoops => ref NPC.ai[2];
        ref float rollSpeed => ref NPC.ai[3];

        public override void SetDefaults()
        {
            Scale = 0.1f;
            GoalScale = 2f;
            rollTimer = 0;
            rolled = false;
            rollOneValue = 0;
            rollTwoValue = 0;
            rollThreeValue = 0;

            base.SetDefaults();

            if (Main.dedServ) return;
            BackgroundTexture = ModContent.Request<Texture2D>("sorceryFight/Content/DomainExpansions/IdleDeathGamble", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            pachinkoMachineTexture = ModContent.Request<Texture2D>("sorceryFight/Content/DomainExpansions/IdleDeathGambleAssets/PachinkoMachine", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void AI()
        {
            base.AI();

            Player player = Owners[NPC.whoAmI];
            player.Center = NPC.Center;

            if (Main.myPlayer != player.whoAmI) return;

            if (NPC.ai[0] > 30)
            {
                rollTimer++;

                if (pachinkoMachinePosition == Vector2.Zero)
                {
                    pachinkoMachinePosition = Owners[NPC.whoAmI].Center - Main.screenPosition + new Vector2((Main.screenWidth / 2) + (pachinkoMachineTexture.Width / 2), (Main.screenHeight / 2) - pachinkoMachineTexture.Height - player.height);
                }


                pachinkoMachinePosition.X -= 20f * (1 + ((NPC.ai[0] - 30) / 30));

                if (NPC.ai[0] > 300)
                    NPC.ai[0] = 300;

                if (pachinkoMachinePosition.X < -pachinkoMachineTexture.Width)
                {
                    pachinkoMachinePosition.X = Main.screenWidth;
                    pachinkoMachineLoops++;
                }

                if (pachinkoMachineLoops == 0) return;

                rollSpeed ++;
                if (rollSpeed > 10) rollSpeed = 10;

                if (!rolled)
                {
                    rollSpeed = 0;
                    rollOneValue = 3;
                    rollTwoValue = 2;
                    rollThreeValue = 7;
                    // rollOneValue = Main.rand.Next(8) + 1;
                    // rollTwoValue = Main.rand.Next(8) + 1;
                    // rollThreeValue = Main.rand.Next(8) + 1;

                    rollOneTexture = ModContent.Request<Texture2D>($"sorceryFight/Content/DomainExpansions/IdleDeathGambleAssets/{rollOneValue}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                    rollTwoTexture = ModContent.Request<Texture2D>($"sorceryFight/Content/DomainExpansions/IdleDeathGambleAssets/{rollTwoValue}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                    rollThreeTexture = ModContent.Request<Texture2D>($"sorceryFight/Content/DomainExpansions/IdleDeathGambleAssets/{rollThreeValue}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

                    if (pachinkoMachineLoops <= 1)
                    {
                        rollOnePosition = Owners[NPC.whoAmI].Center - Main.screenPosition + new Vector2(-rollOneTexture.Width - 150f, -(Main.screenHeight / 2) - (rollOneTexture.Height / 2));
                        rollTwoPosition = Owners[NPC.whoAmI].Center - Main.screenPosition + new Vector2(0, -(Main.screenHeight / 2) - (rollTwoTexture.Height / 2));
                        rollThreePosition = Owners[NPC.whoAmI].Center - Main.screenPosition + new Vector2(rollThreeTexture.Width + 150f, -(Main.screenHeight / 2) - (rollThreeTexture.Height / 2));
                    }
                    else
                    {
                        rollOnePosition = Owners[NPC.whoAmI].Center - Main.screenPosition + new Vector2(-rollOneTexture.Width - 150f, 0);
                        rollTwoPosition = Owners[NPC.whoAmI].Center - Main.screenPosition;
                        rollThreePosition = Owners[NPC.whoAmI].Center - Main.screenPosition + new Vector2(rollThreeTexture.Width + 150f, 0);
                    }

                    rolled = true;
                }

                if (rollTimer < 240)
                {
                    if (rollTimer < 180)
                        rollOnePosition.Y += 200f * (0.1f * rollSpeed);
                    if (rollTimer < 210)
                        rollTwoPosition.Y += 200f * (0.1f * rollSpeed);
                    if (rollTimer < 240)
                        rollThreePosition.Y += 200f * (0.1f * rollSpeed);;

                    if (rollOnePosition.Y > Main.screenHeight + rollOneTexture.Height / 2)
                        rollOnePosition.Y = 0 - rollOneTexture.Height / 2;
                    if (rollTwoPosition.Y > Main.screenHeight + rollTwoTexture.Height / 2)
                        rollTwoPosition.Y = 0 - rollTwoTexture.Height / 2;
                    if (rollThreePosition.Y > Main.screenHeight + rollThreeTexture.Height / 2)
                        rollThreePosition.Y = 0 - rollThreeTexture.Height / 2;
                }


                if (rollTimer >= 180)
                {
                    Vector2 target = Owners[NPC.whoAmI].Center - Main.screenPosition - new Vector2(rollOneTexture.Width + 150, 0);

                    if (rollTimer == 180)
                        rollOnePosition.Y = target.Y + 200;
                    else
                    {
                        float difference = rollOnePosition.Y - target.Y;

                        rollOnePosition.Y -= difference / 2f;

                        if (rollTimer >= 240)
                            rollOnePosition = target;
                    }
                }

                if (rollTimer >= 210)
                {
                    Vector2 target = Owners[NPC.whoAmI].Center - Main.screenPosition;
                    if (rollTimer == 210)
                        rollTwoPosition.Y = target.Y + 200;
                    else
                    {
                        float difference = rollTwoPosition.Y - target.Y;

                        rollTwoPosition.Y -= difference / 2f;

                        if (rollTimer >= 270)
                            rollTwoPosition = target;
                    }
                }

                if (rollTimer >= 240)
                {
                    Vector2 target = Owners[NPC.whoAmI].Center - Main.screenPosition + new Vector2(rollThreeTexture.Width + 150, 0);
                    if (rollTimer == 240)
                        rollThreePosition.Y = target.Y + 200;
                    else
                    {
                        float difference = rollThreePosition.Y - target.Y;

                        rollThreePosition.Y -= difference / 2f;

                        if (rollTimer >= 300)
                            rollThreePosition = target;
                    }
                }

                if (rollTimer >= 390)
                {
                    if (rollOneValue == rollTwoValue && rollOneValue == rollThreeValue)
                    {
                        player.AddBuff(ModContent.BuffType<IdleDeathGambleJackpotBuff>(), SorceryFight.BuffSecondsToTicks(6.25f * rollOneValue + 3.75f));
                        Remove(Owners[NPC.whoAmI].GetModPlayer<SorceryFightPlayer>());
                        return;
                    }

                    if (rollOneValue == rollTwoValue || rollOneValue == rollThreeValue || rollTwoValue == rollThreeValue)
                    {
                        rollTimer = 0;
                        rolled = false;
                        return;
                    }

                    int highest = Math.Max(rollOneValue, rollTwoValue);
                    highest = Math.Max(highest, rollThreeValue);
                    Owners[NPC.whoAmI].GetModPlayer<SorceryFightPlayer>().idleDeathGambleBuffStrength = highest;
                    player.AddBuff(ModContent.BuffType<IdleDeathGambleBuff>(), SorceryFight.BuffSecondsToTicks(6.25f * highest + 3.75f));
                    Remove(Owners[NPC.whoAmI].GetModPlayer<SorceryFightPlayer>());
                }
            }
        }

        public override void NPCDomainEffect(NPC npc) { }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 bgOrigin = new Vector2(BackgroundTexture.Width / 2, BackgroundTexture.Height / 2);
            spriteBatch.Draw(BackgroundTexture, NPC.Center - Main.screenPosition, default, Color.White, NPC.rotation, bgOrigin, BackgroundScale, SpriteEffects.None, 0f);

            if (Main.myPlayer == Owners[NPC.whoAmI].whoAmI)
            {
                if (rolled)
                {
                    DrawRolls(spriteBatch);
                }
                DrawPachinkoMachines(spriteBatch);
            }
            return false;
        }

        void DrawPachinkoMachines(SpriteBatch spriteBatch)
        {
            Vector2 pachinkoOrigin = new Vector2(pachinkoMachineTexture.Width / 2, pachinkoMachineTexture.Height / 2);

            for (int i = 0; i < 10; i++)
            {
                spriteBatch.Draw(pachinkoMachineTexture, pachinkoMachinePosition + new Vector2(pachinkoMachineTexture.Width * 2 * i, 0), default, Color.White, NPC.rotation, pachinkoOrigin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(pachinkoMachineTexture, pachinkoMachinePosition - new Vector2(0, Main.screenHeight - (pachinkoMachineTexture.Height * 2) - (Main.LocalPlayer.height * 2)) + new Vector2(pachinkoMachineTexture.Width * 2 * i, 0), default, Color.White, NPC.rotation + MathHelper.Pi, pachinkoOrigin, 1f, SpriteEffects.None, 0f);
            }

            if (pachinkoMachineLoops == 0) return;

            for (int i = 0; i < 10; i++)
            {
                spriteBatch.Draw(pachinkoMachineTexture, pachinkoMachinePosition - new Vector2(pachinkoMachineTexture.Width * 2 * i, 0), default, Color.White, NPC.rotation, pachinkoOrigin, 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(pachinkoMachineTexture, pachinkoMachinePosition - new Vector2(0, Main.screenHeight - (pachinkoMachineTexture.Height * 2) - (Main.LocalPlayer.height * 2)) - new Vector2(pachinkoMachineTexture.Width * 2 * i, 0), default, Color.White, NPC.rotation + MathHelper.Pi, pachinkoOrigin, 1f, SpriteEffects.None, 0f);
            }
        }

        void DrawRolls(SpriteBatch spriteBatch)
        {
            Vector2 rollOneOrigin = new Vector2(rollOneTexture.Width / 2, rollOneTexture.Height / 2);
            Vector2 rollTwoOrigin = new Vector2(rollTwoTexture.Width / 2, rollTwoTexture.Height / 2);
            Vector2 rollThreeOrigin = new Vector2(rollThreeTexture.Width / 2, rollThreeTexture.Height / 2);

            spriteBatch.Draw(rollOneTexture, rollOnePosition, default, Color.White, NPC.rotation, rollOneOrigin, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(rollTwoTexture, rollTwoPosition, default, Color.White, NPC.rotation, rollTwoOrigin, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(rollThreeTexture, rollThreePosition, default, Color.White, NPC.rotation, rollThreeOrigin, 1f, SpriteEffects.None, 0f);
        }
    }
}