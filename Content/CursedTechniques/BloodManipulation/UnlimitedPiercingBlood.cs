using CalamityMod.Particles;
using CalamityMod.Sounds;
using Microsoft.Build.Graph;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Particles;
using sorceryFight.Content.Projectiles.Melee;
using sorceryFight.SFPlayer;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.BloodManipulation
{
    public class UnlimitedPiercingBlood : CursedTechnique
    {

        public static readonly int FRAME_COUNT = 8;
        public static readonly int TICKS_PER_FRAME = 5;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.UnlimitedPiercingBlood.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.UnlimitedPiercingBlood.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.UnlimitedPiercingBlood.LockedDescription");
        public override float Cost => 40f;

        public override float BloodCost => 20f;

        public override Color textColor => new Color(255, 0, 0);
        public override bool DisplayNameInGame => true;

        public override int Damage => 30;
        public override int MasteryDamageMultiplier => 50;

        private bool keyHeld = false;

        public float BloodCostPerSecond => 4f;

        public override float Speed => 0f;
        public override float LifeTime => 300f;

        private float spawnTimer = 0f;

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.SkeletronHead);
        }


        public static Texture2D texture;

        public bool animating;
        public float animScale;


        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = FRAME_COUNT;
        }

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<SlicingExorcism>();
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 65;
            Projectile.height = 65;
        }
        public override void AI()
        {
            keyHeld = SFKeybinds.UseTechnique.Current;

            if (keyHeld)
            {
                spawnTimer++;

                if (spawnTimer >= 60f)
                {
                    spawnTimer = 0f;

                    SorceryFightPlayer sf = Main.player[Projectile.owner].SorceryFight();
                    sf.bloodEnergyUsagePerSecond += BloodCostPerSecond;
                    Player player = Main.player[Projectile.owner];
                    // auraIndices[player.whoAmI] = Projectile.NewProjectile(entitySource, playerPos, Vector2.Zero, ModContent.ProjectileType<AmplifiedAuraProjectile>(), 0, 0, player.whoAmI);

                    Projectile.NewProjectile(
                    player.GetSource_FromThis(),
                    player.Center,
                    player.DirectionTo(Main.MouseWorld) * Speed, // initial velocity toward mouse if no target
                    ModContent.ProjectileType<UnlimitedPiercingBloodProjectile>(),
                    Damage,
                    0f,
                    player.whoAmI
                    //targetIndex
                    );
                }

            } else {
               Projectile.Kill();
            }


        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            if (texture == null && !Main.dedServ)
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/BloodManipulation/SlicingExorcism").Value;


            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, animScale, SpriteEffects.None, 0f);

            return false;
        }




    }
}