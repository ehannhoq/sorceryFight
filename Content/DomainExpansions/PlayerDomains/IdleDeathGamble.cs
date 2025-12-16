using System;
using CalamityMod.NPCs.DevourerofGods;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs.PrivatePureLoveTrain;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.DomainExpansions.PlayerDomains
{
    public class IdleDeathGamble : PlayerDomainExpansion
    {
        public override string InternalName => "IdleDeathGamble";

        public override string Description(SorceryFightPlayer player)
        {
            string desc = SFUtils.GetLocalizationValue($"Mods.sorceryFight.DomainExpansions.{InternalName}.Description");
            desc += "\n";
            if (player.HasDefeatedBoss(ModContent.NPCType<DevourerofGodsHead>()))
            {
                desc += SFUtils.GetLocalizationValue($"Mods.sorceryFight.DomainExpansions.{InternalName}.TierIII");
            }
            else if (player.unlockedRCT)
            {
                desc += SFUtils.GetLocalizationValue($"Mods.sorceryFight.DomainExpansions.{InternalName}.TierII");
            }
            else
                desc += SFUtils.GetLocalizationValue($"Mods.sorceryFight.DomainExpansions.{InternalName}.TierI");

            return desc;

        }
        public override SoundStyle CastSound => SorceryFightSounds.IdleDeathGambleOpening;

        public override int Tier => 2;

        public override float SureHitRange => 1150f;

        public override float Cost => 15f;

        public override bool ClosedDomain => true;

        float tick = 0f;
        float whiteFade = 0f;

        int[] rolls = new int[3];
        Texture2D rollOneTexture;
        Texture2D rollTwoTexture;
        Texture2D rollThreeTexture;
        Vector2 rollOnePos;
        Vector2 rollTwoPos;
        Vector2 rollThreePos;
        bool rolled;
        float rollSpeed = 0f;

        int pachinkoMachineLoops = 0;
        public static Texture2D pachinkoMachineTexture => ModContent.Request<Texture2D>("sorceryFight/Content/DomainExpansions/IdleDeathGambleAssets/PachinkoMachine", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        float pachinkoRollSpeed = 0f;
        public float originPachinkoMachinePosX;


        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawInnerDomain(() =>
            {
                Texture2D whitePixel = TextureAssets.MagicPixel.Value;
                Rectangle screenRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
                Color fadeColor = new Color(255f, 255f, 255f, whiteFade);

                spriteBatch.Draw(whitePixel, screenRectangle, fadeColor);

                if (tick > 120)
                {
                    Rectangle dtSrc = new Rectangle(0, 0, DomainTexture.Width, DomainTexture.Height);
                    spriteBatch.Draw(DomainTexture, dtSrc, Color.White);

                    Rectangle src = new Rectangle(0, 0, pachinkoMachineTexture.Width, pachinkoMachineTexture.Height);
                    for (int i = 0; i < 10; i++)
                    {
                        spriteBatch.Draw(pachinkoMachineTexture, new Vector2(originPachinkoMachinePosX, pachinkoMachineTexture.Height + 50f) + new Vector2((pachinkoMachineTexture.Width + 100) * i, 0f), src, Color.White, MathF.PI, src.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
                        spriteBatch.Draw(pachinkoMachineTexture, new Vector2(originPachinkoMachinePosX, Main.screenHeight - pachinkoMachineTexture.Height - 50f) + new Vector2((pachinkoMachineTexture.Width + 100) * i, 0f), src, Color.White, 0f, src.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
                    }

                    if (pachinkoMachineLoops > 0)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            spriteBatch.Draw(pachinkoMachineTexture, new Vector2(originPachinkoMachinePosX, pachinkoMachineTexture.Height + 50f) - new Vector2((pachinkoMachineTexture.Width + 100) * j, 0f), src, Color.White, MathF.PI, src.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
                            spriteBatch.Draw(pachinkoMachineTexture, new Vector2(originPachinkoMachinePosX, Main.screenHeight - pachinkoMachineTexture.Height - 50f) - new Vector2((pachinkoMachineTexture.Width + 100) * j, 0f), src, Color.White, 0f, src.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
                        }
                    }

                    if (pachinkoMachineLoops > 0 && Main.myPlayer == owner)
                    {
                        Rectangle src2 = new Rectangle(0, 0, rollOneTexture.Width, rollOneTexture.Height);

                        spriteBatch.Draw(rollOneTexture, rollOnePos, src2, Color.White, 0f, src2.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
                        spriteBatch.Draw(rollTwoTexture, rollTwoPos, src2, Color.White, 0f, src2.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
                        spriteBatch.Draw(rollThreeTexture, rollThreePos, src2, Color.White, 0f, src2.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
                    }
                }
            },
            () => spriteBatch.Draw(BaseTexture, center - Main.screenPosition, new Rectangle(0, 0, BaseTexture.Width, BaseTexture.Height), Color.White, 0f, new Rectangle(0, 0, BaseTexture.Width, BaseTexture.Height).Size() * 0.5f, 2f, SpriteEffects.None, 0f)
            );
        }

        public override void SureHitEffect(NPC npc) { }

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.WallofFlesh);
        }

        public override void Update()
        {
            base.Update();

            if ((whiteFade += 0.015f) > 1f)
            {
                whiteFade = 1f;
            }

            tick++;

            if (tick == 1)
            {
                originPachinkoMachinePosX = Main.screenWidth + pachinkoMachineTexture.Width;
            }


            if (tick > 120)
            {

                if (tick == 121)
                    SoundEngine.PlaySound(SorceryFightSounds.IDGWoosh, center);

                if (pachinkoRollSpeed++ > 50f)
                    pachinkoRollSpeed = 50f;

                if (Main.myPlayer == owner)
                {
                    if (!rolled)
                    {
                        rolled = true;
                        Roll(Main.player[owner]);
                        rollOneTexture = ModContent.Request<Texture2D>($"sorceryFight/Content/DomainExpansions/IdleDeathGambleAssets/{rolls[0]}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                        rollTwoTexture = ModContent.Request<Texture2D>($"sorceryFight/Content/DomainExpansions/IdleDeathGambleAssets/{rolls[1]}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                        rollThreeTexture = ModContent.Request<Texture2D>($"sorceryFight/Content/DomainExpansions/IdleDeathGambleAssets/{rolls[2]}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

                        rollTwoPos = new Vector2(Main.screenWidth / 2f, -rollTwoTexture.Height);
                        rollOnePos = new Vector2(rollTwoPos.X - rollOneTexture.Width - 100f, rollTwoPos.Y);
                        rollThreePos = new Vector2(rollTwoPos.X + rollThreeTexture.Width + 100f, rollTwoPos.Y);

                        SoundEngine.PlaySound(SorceryFightSounds.IDGSlots with { Volume = 2f }, center);
                    }
                    else
                        if (rollSpeed++ > 150f)
                        rollSpeed = 150f;
                }

                originPachinkoMachinePosX -= pachinkoRollSpeed;
                if (originPachinkoMachinePosX < -pachinkoMachineTexture.Width)
                {
                    originPachinkoMachinePosX = Main.screenWidth + pachinkoMachineTexture.Width;
                    pachinkoMachineLoops++;
                    SoundEngine.PlaySound(SorceryFightSounds.IDGWooshLoop, center);
                }

                if (pachinkoMachineLoops > 0 && Main.myPlayer == owner)
                {
                    if (tick < 290)
                    {
                        rollOnePos.Y += rollSpeed;
                        if (rollOnePos.Y > Main.screenHeight + rollOneTexture.Height)
                            rollOnePos.Y = -rollOneTexture.Height;
                    }
                    else
                    {
                        float distLeft = rollOnePos.Y - Main.screenHeight / 2;
                        rollOnePos.Y -= distLeft * 0.5f;
                    }

                    if (tick < 340)
                    {
                        rollTwoPos.Y += rollSpeed;
                        if (rollTwoPos.Y > Main.screenHeight + rollTwoTexture.Height)
                            rollTwoPos.Y = -rollTwoTexture.Height;
                    }
                    else
                    {
                        float distLeft = rollTwoPos.Y - Main.screenHeight / 2;
                        rollTwoPos.Y -= distLeft * 0.5f;
                    }

                    if (tick < 390)
                    {
                        rollThreePos.Y += rollSpeed;
                        if (rollThreePos.Y > Main.screenHeight + rollThreeTexture.Height)
                            rollThreePos.Y = -rollThreeTexture.Height;
                    }
                    else
                    {
                        float distLeft = rollThreePos.Y - Main.screenHeight / 2;
                        rollThreePos.Y -= distLeft * 0.5f;
                    }
                }
            }

            if (tick > 490 && Main.myPlayer == owner)
            {
                SorceryFightPlayer sfPlayer = Main.LocalPlayer.GetModPlayer<SorceryFightPlayer>();
                if (rolls[0] == rolls[1] && rolls[0] == rolls[2])
                {
                    if (sfPlayer.HasDefeatedBoss(ModContent.NPCType<DevourerofGodsHead>()))
                    {
                        StageIIIReward();
                        return;
                    }

                    if (sfPlayer.unlockedRCT)
                    {
                        StageIIReward();
                        return;
                    }

                    StageIReward();
                    return;
                }

                if (rolls[0] == rolls[1] || rolls[0] == rolls[2] || rolls[1] == rolls[2])
                {
                    tick = 120;
                    rolled = false;
                    return;
                }

                int highest = Math.Max(rolls[0], rolls[1]);
                highest = Math.Max(highest, rolls[2]);
                sfPlayer.idleDeathGambleBuffStrength = highest;
                Main.LocalPlayer.AddBuff(ModContent.BuffType<IdleDeathGambleBuff>(), SFUtils.BuffSecondsToTicks(6.25f * highest + 3.75f));
                DomainExpansionController.CloseDomain(owner);
            }
        }

        public override void OnClose()
        {
            tick = 0f;
            whiteFade = 0f;
            rolls = new int[3];
            rollOnePos = Vector2.Zero;
            rollTwoPos = Vector2.Zero;
            rollThreePos = Vector2.Zero;
            rolled = false;
            rollSpeed = 0f;
            pachinkoMachineLoops = 0;
            pachinkoRollSpeed = 0f;
        }

        void StageIReward()
        {
            Main.LocalPlayer.AddBuff(ModContent.BuffType<IdleDeathGambleJackpotBuffI>(), SFUtils.BuffSecondsToTicks(7.25f * rolls[0] + 3.75f));
            DomainExpansionController.CloseDomain(owner);
        }

        void StageIIReward()
        {
            Main.LocalPlayer.AddBuff(ModContent.BuffType<IdleDeathGambleJackpotBuffII>(), SFUtils.BuffSecondsToTicks(7.25f * rolls[0] + 3.75f));
            DomainExpansionController.CloseDomain(owner);
        }

        void StageIIIReward()
        {
            Main.LocalPlayer.AddBuff(ModContent.BuffType<IdleDeathGambleJackpotBuffIII>(), SFUtils.BuffSecondsToTicks(7.25f * rolls[0] + 3.75f));
            DomainExpansionController.CloseDomain(owner);
        }

        void Roll(Player player)
        {
            int bossesKilled = player.GetModPlayer<SorceryFightPlayer>().numberBossesDefeated;

            float mean = 5 + (0.07f * bossesKilled);
            float std = 3;

            rolls[0] = (int)(GaussianCurve(mean, std));

            for (int i = 1; i < 3; i++)
            {
                if (SFUtils.Roll((int)(bossesKilled * 1.25)))
                {
                    rolls[i] = rolls[i - 1];
                    continue;
                }

                rolls[i] = (int)(GaussianCurve(mean, std));
            }

            rolls.Clamp(1, 9);
        }

        float GaussianCurve(float mean, float std)
        {
            float u, v;
            u = 1.0f - Main.rand.NextFloat();
            v = 1.0f - Main.rand.NextFloat();

            float randStdNormal = MathF.Sqrt(-2.0f * MathF.Log(u)) * MathF.Sin(2.0f * MathF.PI * v);
            return mean + std * randStdNormal;
        }
    }
}