using System.Collections.Generic;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.CursedTechniques.Limitless;
using sorceryFight.Content.DomainExpansions;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.Buffs.Limitless;
using sorceryFight.Content.DomainExpansions.PlayerDomains;
using Microsoft.Xna.Framework;
using sorceryFight.SFPlayer;
using Terraria.ID;
using sorceryFight.Content.Buffs.Shrine;
using sorceryFight.Content.Quests;
using Terraria.Graphics.Effects;
using System.Linq;
using Terraria.Audio;
using System;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.ModLoader;
using sorceryFight.Content.Projectiles.VFX;
using sorceryFight.Content.Items.Accessories;

namespace sorceryFight.Content.InnateTechniques
{
    public class LimitlessTechnique : InnateTechnique
    {
        public override string InternalName => "Limitless";

        public override Color innateBGColor => new Color(150, 219, 235, 85);

        public override Color innateBorderColor => new Color(0, 0, 0, 128);


        private Projectile hollowPurple = null;
        private int postHollowPurpleSummonTick = 0;

        public override List<PassiveTechnique> PassiveTechniques { get; } = new List<PassiveTechnique>
        {
            new Infinity()
                .SetUnlock(NPCID.EyeofCthulhu),

            new AmplifiedAura()
                .SetUnlock(NPCID.SkeletronHead),

            new MaximumAmplifiedAura()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.defeatedMechBossThree)
                .SetUnlockRequirement("Mods.sorceryFight.UnlockRequirements.MechBossThree"),

            new FallingBlossomEmotion()
                .SetUnlock(NPCID.HallowBoss)
        };
        public override List<CursedTechnique> CursedTechniques { get; } = new List<CursedTechnique>
        {
            new AmplificationBlue()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.defeatedEvilBoss)
                .SetUnlockRequirement("Mods.sorceryFight.UnlockRequirements.EvilBoss"),

            new MaximumOutputBlue()
                .SetUnlock(NPCID.WallofFlesh),

            new ReversalRed()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.unlockedRCT)
                .SetUnlockRequirement("Mods.sorceryFight.UnlockRequirements.RCT"),

            new MaximumOutputRed()
                .SetUnlock(NPCID.Golem),

            new HollowPurple()
                .SetUnlock(NPCID.CultistBoss),

            new HollowPurple200Percent()
                .SetUnlock(NPCID.MoonLordCore)
        };

        public override PlayerDomainExpansion DomainExpansion => new UnlimitedVoid();


        public override void RCTAnimation(SorceryFightPlayer sf)
        {
            SetupRCTAnimation(sf);

            if (rctTimer < 120)
            {
                if (!Filters.Scene["SF:BlackScreen"].IsActive())
                {
                    Filters.Scene.Activate("SF:BlackScreen").GetShader().UseProgress(1f);
                    SoundController.MuteSounds();
                    Main.hideUI = true;
                }
                return;
            }

            int tick1 = rctTimer - 120;
            if (tick1 < 120)
            {
                if (Filters.Scene["SF:BlackScreen"].IsActive())
                {
                    Filters.Scene["SF:BlackScreen"].GetShader().UseProgress(0f);
                    Filters.Scene.Deactivate("SF:BlackScreen");
                    SoundController.UnmuteSounds();
                    Main.hideUI = false;
                }

                float easeInProgress = tick1 / 60f;
                easeInProgress = Math.Clamp(easeInProgress, 0f, 1f);

                if (!Filters.Scene["SF:LimitlessRCTFilter"].IsActive())
                {
                    Filters.Scene.Activate("SF:LimitlessRCTFilter");
                }
                Filters.Scene["SF:LimitlessRCTFilter"].GetShader().UseProgress(easeInProgress);

                float yFunc = (MathF.Sin((MathHelper.TwoPi * easeInProgress) - MathHelper.PiOver2) / 2f) + 0.5f;
                float y = 5 * yFunc;

                sf.deathPosition.Y -= y;
                return;
            }

            Vector2 planteraPos = Main.npc[RCTGranter.planteraIndex].Center;
            Vector2 hollowPurplePos = sf.Player.MountedCenter + sf.Player.MountedCenter.DirectionTo(planteraPos) * 50f;

            if (tick1 == 120)
            {
                string line = SFUtils.GetLocalizationValue("Mods.sorceryFight.Misc.UnlockedRCT.Limitless");
                string[] parts = line.Split('~');


                sf.sfUI.InitializeChant([.. parts], 120, 30, new UI.Chants.ChantTextStyle(
                    textColor: new Color(245, 225, 171, 255),
                    text2Color: new Color(227, 191, 141, 255),
                    borderWidth: 2.0f,
                    borderColor: new Color(191, 128, 38, 255),
                    border2Color: new Color(176, 96, 35, 255),
                    glowRadius: 3.0f,
                    glowColor: new Color(237, 225, 190, 255)
                ),
                () =>
                {
                    int index = Projectile.NewProjectile(sf.Player.GetSource_FromThis(), hollowPurplePos, Vector2.Zero, ModContent.ProjectileType<HollowPurple>(), 0, 0, sf.Player.whoAmI, ai1: 1f);
                    hollowPurple = Main.projectile[index];
                });
            }

            if (hollowPurple == null) return;

            postHollowPurpleSummonTick++;
            float hollowPurpleCastTime = sf.cursedOfuda ? MathF.Floor(CursedOfuda.cursedTechniqueCastTimeDecrease * 90f) : 90f;
            float hollowPurpleCollisionTime = HollowPurpleCollision.FRAMES * HollowPurpleCollision.TICKS_PER_FRAME;

            if (postHollowPurpleSummonTick == (int)(hollowPurpleCastTime + hollowPurpleCollisionTime + 1))
            {
                CursedTechnique hollowPurpleCT = hollowPurple.ModProjectile as CursedTechnique;

                hollowPurple.velocity = (planteraPos - hollowPurple.Center).SafeNormalize(Vector2.UnitX) * hollowPurpleCT.speed;
                Filters.Scene["SF:LimitlessRCTFilter"].GetShader().UseProgress(0f);
                Filters.Scene.Deactivate("SF:LimitlessRCTFilter");
                GrantRCT(sf);

                hollowPurple = null;
            }
        }
    }
}
