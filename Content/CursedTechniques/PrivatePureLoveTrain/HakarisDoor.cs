using System;
using System.Collections.Generic;
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
    public class HakarisDoor : CursedTechnique
    {
        public static readonly int FRAME_COUNT = 8;
        public static readonly int TICKS_PER_FRAME = 2;
        public static Texture2D texture;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.HakarisDoor.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.HakarisDoor.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.HakarisDoor.LockedDescription");
        public override float Cost => 100f;
        public override Color textColor => new Color(108, 158, 240);
        public override bool DisplayNameInGame => true;
        public override int Damage => 25;
        public override int MasteryDamageMultiplier => 11;
        public override float Speed => 0f;
        public override float LifeTime => 40f;
        public Color rarity;

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<HakarisDoor>();
        }

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.SkeletronHead);
        }

        public override int UseTechnique(SorceryFightPlayer sf)
        {
            Player player = sf.Player;
            sf.cursedEnergy -= Cost;

            if (Main.myPlayer == player.whoAmI)
            {
                if (DisplayNameInGame)
                {
                    int index1 = CombatText.NewText(player.getRect(), textColor, DisplayName.Value);
                    Main.combatText[index1].lifeTime = 180;
                }

                Vector2 mousePos = Main.MouseWorld;
                var entitySource = player.GetSource_FromThis();
                return Projectile.NewProjectile(entitySource, mousePos, Vector2.Zero, GetProjectileType(), (int)CalculateTrueDamage(sf), 0f, player.whoAmI);
            }
            return -1;
        }


        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = FRAME_COUNT;
            if (Main.dedServ) return;

            texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/PrivatePureLoveTrain/HakarisDoor", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 170;
            Projectile.height = 200;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = (int)LifeTime;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = Projectile.width;
                Projectile.ai[2] = Projectile.height;

                int roll = Main.rand.Next(0, 100);
                if (roll < 89)
                    rarity = Color.Green;
                else if (roll < 98)
                {
                    rarity = Color.Yellow;
                    Projectile.damage = (int)(CalculateTrueDamage(Main.player[Projectile.owner].GetModPlayer<SorceryFightPlayer>()) * 1.5);
                }
                else
                {
                    rarity = Color.Red;
                    Projectile.damage = (int)(CalculateTrueDamage(Main.player[Projectile.owner].GetModPlayer<SorceryFightPlayer>()) * 2);
                }

                SoundEngine.PlaySound(SorceryFightSounds.TrainDoorsClosing, Projectile.Center);
            }

            if (Projectile.frame > FRAME_COUNT - 4)
            {
                Projectile.width = (int)Projectile.ai[1];
                Projectile.height = (int)Projectile.ai[2];
            }
            else
            {
                Projectile.width = 0;
                Projectile.height = 0;
            }

            if (Projectile.frameCounter++ >= TICKS_PER_FRAME)
            {
                Projectile.frameCounter = 0;

                if (Projectile.frame++ >= FRAME_COUNT - 1)
                {
                    Projectile.frame = FRAME_COUNT - 1;
                }
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 projOrigin = sourceRectangle.Size() * 0.5f;


            Main.EntitySpriteDraw(texture, Projectile.position + new Vector2(Projectile.ai[1] / 2, Projectile.ai[2] / 2) - Main.screenPosition, sourceRectangle, rarity, Projectile.rotation, projOrigin, 5f, SpriteEffects.None, 0f);
            return false;
        }
    }
}