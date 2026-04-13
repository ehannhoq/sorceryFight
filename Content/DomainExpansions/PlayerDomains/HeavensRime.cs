using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.CursedTechniques.IceFormation;
using sorceryFight.Content.Particles;
using sorceryFight.Content.Particles.UIParticles;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static sorceryFight.Utilities.ProjectilePool;

namespace sorceryFight.Content.DomainExpansions.PlayerDomains
{
    public class HeavensRime : PlayerDomainExpansion
    {

        public static readonly ProjectilePool Pool = new ProjectilePool();

        //static constructor for setting up the projectile pool
        static HeavensRime()
        {
            Pool.Add(new ProjectileEntry(
                () => ModContent.ProjectileType<IcicleHailProjectile>(),
                sf => true,
                weight: 10,
                damageMultiplier: 1f
            ));

            Pool.Add(new ProjectileEntry(
                () => ProjectileID.Blizzard,
                sf => sf.HasDefeatedBoss(NPCID.SkeletronHead),
                weight: 5,
                damageMultiplier: 1f
            ));

            Pool.Add(new ProjectileEntry(
                () => ProjectileID.FrostWave,
                sf => sf.HasDefeatedBoss(NPCID.MoonLordCore),
                weight: 2,
                damageMultiplier: 1f
            ));

            if (ModLoader.TryGetMod("CalamityMod", out _))
            {
                Pool.Add(new ProjectileEntry(
                    () => ModContent.ProjectileType<CalamityMod.Projectiles.Summon.EndoBeam>(),
                    sf => sf.HasDefeatedBoss(ModContent.NPCType<Yharon>()),
                    weight: 3,
                    damageMultiplier: 1.5f
                ));
            }

        }

        public override string InternalName => "HeavensRime";

        public override SoundStyle CastSound => SorceryFightSounds.UnlimitedVoid;

        public override int Tier => 1;

        public override float SureHitRange => 1150f;

        public override float Cost => 100f;

        public override bool ClosedDomain => true;

        float tick = 0f;
        float whiteFade = 0f;

        public override void SureHitEffect(NPC npc)
        {
            if (Main.myPlayer == owner)
            {
                if (tick % 2 == 0)
                {
                    SorceryFightPlayer sf = Main.player[owner].SorceryFight();
                    ProjectileEntry entry = Pool.Pick(sf);

                    float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                    Vector2 spawnPos = this.center + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * SureHitRange;

                    Vector2 velocity = (npc.Center - spawnPos).SafeNormalize(Vector2.Zero) * 40f;

                    int index = Projectile.NewProjectile(
                        Main.LocalPlayer.GetSource_FromThis(),
                        spawnPos,
                        velocity,
                        entry.GetProjectileType(),
                        100, //damage
                        0f,
                        owner
                    );

                    if (index >= 0 && index < Main.maxProjectiles)
                    {
                        Main.projectile[index].timeLeft = 300;
                        Main.projectile[index].damage = (int)(Main.projectile[index].damage * entry.DamageMultiplier);
                    }
                }
            }
        }

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.MoonLordCore);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            DrawInnerDomain(() =>
            {
                //Main.NewText("greater than 140 ticks");
                Texture2D whiteTexture = TextureAssets.MagicPixel.Value;
                Rectangle screenRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);

                spriteBatch.Draw(whiteTexture, screenRectangle, Color.Black);
            },
            () => spriteBatch.Draw(BaseTexture, center - Main.screenPosition, new Rectangle(0, 0, BaseTexture.Width, BaseTexture.Height), Color.White, 0f, new Rectangle(0, 0, BaseTexture.Width, BaseTexture.Height).Size() * 0.5f, 2f, SpriteEffects.None, 0f)
            );

            if (tick > 250)
            {
                Rectangle src = new Rectangle(0, 0, DomainTexture.Width, DomainTexture.Height);
                spriteBatch.Draw(DomainTexture, center - Main.screenPosition, src, Color.White, 0f, src.Size() * 0.5f, 2f, SpriteEffects.None, 0f);
            }
        }

        public override void Update()
        {
            base.Update();

            tick++;

            //what does this do?
            if ((whiteFade += 0.03f) > 1)
                whiteFade = 1;

            if (tick == 340)
                whiteFade = 0;

            if (tick < 250)
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector2 offsetPos = center + new Vector2(Main.rand.NextFloat(-2000f, 2000f), Main.rand.NextFloat(-2000f, 2000f));
                    Vector2 velocity = center.DirectionTo(offsetPos) * 80f;

                    List<Color> colors = [
                        new Color(91, 91, 245), // blue
                        new Color(201, 110, 235), // magenta
                        new Color(79, 121, 219), // cyan
                        new Color(124, 42, 232), // purple
                    ];

                    int roll = Main.rand.Next(colors.Count);
                    Color color = colors[roll];

                    LinearParticle particle = new LinearParticle(center, velocity, color, false, 1, 3, 180);
                    ParticleController.SpawnParticle(particle);
                }
            }

            if (Main.myPlayer == owner && tick > 1 && tick < 390)
            {
                Player player = Main.player[owner];
                player.Center = center;
            }

        }

        public override void OnClose()
        {
            tick = 0;
            whiteFade = 0;
        }

    }
}
