using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.PrivatePureLoveTrain
{
    public class PassingThrough : CursedTechnique
    {
        public static Texture2D texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/PrivatePureLoveTrain/PassingThrough", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.PassingThrough.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.PassingThrough.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.PassingThrough.LockedDescription");
        public override float Cost => 300f;
        public override Color textColor => new Color(59, 64, 112);
        public override bool DisplayNameInGame => true;
        public override int Damage => 200;
        public override int MasteryDamageMultiplier => 30;
        public override float Speed => 30f;
        public override float LifeTime => 180f;


        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<PassingThrough>();
        }

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.SkeletronPrime);
        }

        public override int UseTechnique(SorceryFightPlayer sf)
        {
            Player player = sf.Player;

            if (player.whoAmI == Main.myPlayer)
            {

                sf.cursedEnergy -= CalculateTrueCost(sf);

                if (DisplayNameInGame)
                {
                    int index1 = CombatText.NewText(player.getRect(), textColor, DisplayName.Value);
                    Main.combatText[index1].lifeTime = 180;
                }

                var entitySource = player.GetSource_FromThis();

                Vector2 mousePos = Main.MouseWorld;
                Vector2 posOffset = Vector2.UnitX * 750f;
                posOffset = posOffset.RotatedByRandom(2 * MathF.PI);
                Vector2 pos = mousePos + posOffset;

                Vector2 dir = pos.DirectionTo(mousePos) * Speed;

                SoundEngine.PlaySound(SorceryFightSounds.CommonWoosh, pos);

                return Projectile.NewProjectile(entitySource, pos, dir, GetProjectileType(), (int)CalculateTrueDamage(sf), 4f, player.whoAmI);
            }
            return -1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 300;
            Projectile.height = 175;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = (int)LifeTime;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, sourceRectangle.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
            return false;
        }
    }

}