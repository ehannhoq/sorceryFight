using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.NPCs.NormalNPCs;
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.Vessel
{
    public class ChainDismantle : CursedTechnique
    {
        public static readonly int FRAME_COUNT = 6;
        public static readonly int TICKS_PER_FRAME = 2;
        public static Texture2D texture;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.ChainDismantle.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.ChainDismantle.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.ChainDismantle.LockedDescription");
        public override float Cost => 350f;
        public override Color textColor => new Color(120, 21, 8);
        public override bool DisplayNameInGame => false;
        public override int Damage => 60;
        public override int MasteryDamageMultiplier => 50;
        public override float Speed => 0f;
        public override float LifeTime => 18f;

        List<int> hasHit;
        ref float isBarrage => ref Projectile.ai[2];
        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<ChainDismantle>();
        }
        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.sukunasFingerConsumed >= 17;
        }

        public override int UseTechnique(SorceryFightPlayer sf)
        {
            Player player = sf.Player;

            if (player.whoAmI == Main.myPlayer)
            {
                Vector2 playerPos = player.MountedCenter;
                Vector2 mousePos = Main.MouseWorld;
                Vector2 dir = (mousePos - playerPos).SafeNormalize(Vector2.Zero) * Speed;
                var entitySource = player.GetSource_FromThis();
                sf.cursedEnergy -= CalculateTrueCost(sf);

                int index = Projectile.NewProjectile(entitySource, player.Center, dir, GetProjectileType(), 1, 0, player.whoAmI);
                Main.projectile[index].ai[2] = 0f;
                return index;
            }
            return -1;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = FRAME_COUNT;

            if (Main.dedServ) return;
            texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Vessel/ChainDismantle", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 170;
            Projectile.height = 140;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            hasHit = new List<int>();
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (!hasHit.Contains(Projectile.whoAmI))
            {
                modifiers.FinalDamage.Flat = CalculateTrueDamage(Main.player[Projectile.owner].GetModPlayer<SorceryFightPlayer>());
                hasHit.Add(Projectile.whoAmI);

                if (isBarrage == 0)
                {
                    foreach (NPC npc in Main.ActiveNPCs)
                    {
                        if (npc.friendly || npc.type == NPCID.TargetDummy || npc.type == ModContent.NPCType<SuperDummyNPC>() || npc.whoAmI == target.whoAmI) continue;
                        float dist = Vector2.Distance(npc.Center, target.Center);
                        if (dist < 250f)
                        {
                            Player player = Main.player[Projectile.owner];
                            SorceryFightPlayer sf = player.GetModPlayer<SorceryFightPlayer>();

                            Vector2 playerPos = player.MountedCenter;
                            Vector2 mousePos = Main.MouseWorld;
                            Vector2 dir = (mousePos - playerPos).SafeNormalize(Vector2.Zero) * Speed;
                            var entitySource = player.GetSource_FromThis();

                            int index = Projectile.NewProjectile(entitySource, npc.Center, dir, GetProjectileType(), 1, 0, player.whoAmI);
                            Main.projectile[index].ai[2] = 1f;
                        }
                    }
                }
            }

            else
                Projectile.damage = 0;

            base.ModifyHitNPC(target, ref modifiers);
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            if (Projectile.ai[0] >= LifeTime)
            {
                Projectile.Kill();
            }

            if (Projectile.frameCounter++ >= TICKS_PER_FRAME)
            {
                Projectile.frameCounter = 0;

                if (Projectile.frame++ >= FRAME_COUNT - 1)
                {
                    Projectile.frame = 0;
                }
            }

            Player player = Main.player[Projectile.owner];

            if (isBarrage == 0)
            {
                Vector2 playerRotatedPoint = player.RotatedRelativePoint(player.MountedCenter, true);
                float velocityAngle = Projectile.velocity.ToRotation();
                float offset = 130f * Projectile.scale;

                Projectile.velocity = (Main.MouseWorld - playerRotatedPoint).SafeNormalize(Vector2.UnitX * player.direction);
                Projectile.Center = playerRotatedPoint + velocityAngle.ToRotationVector2() * offset;
                Projectile.rotation = velocityAngle + (Projectile.direction == -1).ToInt() * MathHelper.Pi;
            }

            if (Projectile.ai[0] == 1)
            {
                Projectile.ai[1] = Main.rand.NextFloat(0, MathHelper.TwoPi);
                SoundEngine.PlaySound(SorceryFightSounds.CleaveSwing with { Volume = 5f }, player.Center);
                SoundEngine.PlaySound(SorceryFightSounds.SoulDismantle, Projectile.Center);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 projOrigin = sourceRectangle.Size() * 0.5f;

            float velocityAngle = Projectile.velocity.ToRotation();
            Projectile.rotation = velocityAngle + (Projectile.direction == -1).ToInt() * MathHelper.Pi;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, -32).RotatedBy(Projectile.rotation), sourceRectangle, Color.White, Projectile.rotation + Projectile.ai[1], projOrigin, 1.5f, SpriteEffects.None, 0f);
            return false;
        }
    }
}
