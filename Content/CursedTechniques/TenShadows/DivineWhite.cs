using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.SFPlayer;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using ReLogic.Content;

namespace sorceryFight.Content.CursedTechniques.TenShadows
{

    //divine white is the parent
    public class DivineWhite : CursedTechniqueSummon
    {
        public override SummonStyle Style => SummonStyle.GroundedMinion;

        public override LocalizedText DisplayName =>
            SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.DivineWhite.DisplayName");
        public override string Description =>
            SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.DivineWhite.Description");
        public override string LockedDescription =>
            SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.DivineWhite.LockedDescription");

        public override float Cost => 50f;
        public override Color textColor => new Color(240, 240, 255);
        public override bool DisplayNameInGame => true;
        public override int Damage => 25;
        public override int MasteryDamageMultiplier => 40;
        public override float Speed => 8f;
        public override float LifeTime => 0f;
        public override float DetectionRange => 800f;
        public override float Gravity => 0.4f;
        public override float MaxFallSpeed => 12f;
        public override bool CanContactDamage => true;

        public override Color selectorBGColor { get; set; }
        public override Color selectorBorderColor { get; set; }

        private const int FRAME_COUNT = 7;
        private const int TICKS_PER_FRAME = 6;

        private bool IsMoving => MathF.Abs(Projectile.velocity.X) > 0.5f || MathF.Abs(Projectile.velocity.Y) > 0.5f;

        public static Texture2D texture;
        public static Texture2D runTexture;

        public override string ParentInnateName => "TenShadows";

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.EyeofCthulhu);
        }

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<DivineWhite>();
        }

        public override int UseTechnique(SorceryFightPlayer sf)
        {
            Player player = sf.Player;

            if (player.whoAmI != Main.myPlayer)
                return -1;

            int whiteType = ModContent.ProjectileType<DivineWhite>();

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.owner == player.whoAmI && proj.type == whiteType)
                {
                    proj.Kill();

                    if (DisplayNameInGame)
                    {
                        int idx = CombatText.NewText(player.getRect(), textColor, $"{DisplayName.Value} - Dismissed");
                        Main.combatText[idx].lifeTime = 120;
                    }
                    return -1;
                }
            }

            sf.cursedEnergy -= CalculateTrueCost(sf);

            if (DisplayNameInGame)
            {
                int idx = CombatText.NewText(player.getRect(), textColor, DisplayName.Value);
                Main.combatText[idx].lifeTime = 180;
            }

            var source = player.GetSource_FromThis();
            Vector2 spawnPos = Main.MouseWorld;
            int damage = (int)CalculateTrueDamage(sf);

            int whiteIndex = Projectile.NewProjectile(
                source,
                spawnPos + new Vector2(-30f, 0f),
                Vector2.Zero,
                whiteType,
                damage,
                0f,
                player.whoAmI
            );

            if (Main.projectile.IndexInRange(whiteIndex))
                Main.projectile[whiteIndex].originalDamage = Damage;

            int blackIndex = Projectile.NewProjectile(
                source,
                spawnPos + new Vector2(30f, 0f),
                Vector2.Zero,
                ModContent.ProjectileType<DivineBlack>(),
                damage,
                0f,
                player.whoAmI
            );

            if (Main.projectile.IndexInRange(blackIndex))
                Main.projectile[blackIndex].originalDamage = Damage;

            Main.NewText($"Black spawned: index={blackIndex}, type={ModContent.ProjectileType<DivineBlack>()}");


            return whiteIndex;
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = FRAME_COUNT;
        }

        public override void SummonSetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
        }

        //public override void AI()
        //{
        //    Main.NewText("DIVINE WHITE AI running");
        //}

        public override void SummonAI()
        {

            Main.NewText($"Style={Style}, Owner null={Owner == null}, Target null={Target == null}");


            AnimateFrames(FRAME_COUNT, TICKS_PER_FRAME);

            if (Target != null)
            {
                HopToward(Target.Center);
                Projectile.spriteDirection = MathF.Sign(Target.Center.X - Projectile.Center.X);
            }
            else
            {
                if (!Projectile.WithinRange(Owner.Center, 150f))
                    HopToward(Owner.Center);

                if (MathF.Abs(Projectile.velocity.X) > 0.5f)
                    Projectile.spriteDirection = MathF.Sign(Projectile.velocity.X);
            }

            if (!Projectile.WithinRange(Owner.Center, 2000f))
            {
                Projectile.Center = Owner.Center;
                Projectile.netUpdate = true;
            }
        }

        private bool OnGround => MathF.Abs(Projectile.velocity.Y) < 0.1f;

        private void HopToward(Vector2 target)
        {
            if (OnGround && SummonTimer % 30f >= 29f)
            {
                float dist = Vector2.Distance(Projectile.Center, target);
                float hopStrength = MathHelper.Clamp(dist / 200f, 0.3f, 1f);

                Vector2 dir = (target - Projectile.Center).SafeNormalize(Vector2.UnitX);
                Projectile.velocity = dir * Speed * hopStrength + new Vector2(MathF.Sign(dir.X) * 2f * hopStrength, -8f * hopStrength);
                Projectile.tileCollide = false;
                Projectile.netUpdate = true;
            }
            else
            {
                Projectile.tileCollide = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            SpriteEffects flip = Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;


            if (IsMoving)
            {
                if (runTexture == null && !Main.dedServ)
                    runTexture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/TenShadows/DivineWhiteRun", AssetRequestMode.ImmediateLoad).Value;

                int frameHeight = runTexture.Height / FRAME_COUNT;
                int frameY = Projectile.frame * frameHeight;
                Vector2 origin = new Vector2(runTexture.Width / 2, frameHeight / 2);
                Rectangle sourceRectangle = new Rectangle(0, frameY, runTexture.Width, frameHeight);

                spriteBatch.Draw(runTexture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, Projectile.scale, flip, 0f);
            }
            else
            {
                if (texture == null && !Main.dedServ)
                    texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/TenShadows/DivineWhite", AssetRequestMode.ImmediateLoad).Value;

                Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);

                spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, Projectile.scale, flip, 0f);
            }

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                int blackType = ModContent.ProjectileType<DivineBlack>();
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].owner == Projectile.owner && Main.projectile[i].type == blackType)
                    {
                        Main.projectile[i].Kill();
                        break;
                    }
                }
            }

            base.OnKill(timeLeft);
        }
    }
}