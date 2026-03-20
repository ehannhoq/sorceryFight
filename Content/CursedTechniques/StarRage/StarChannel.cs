using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs.StarRage;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.StarRage
{
    public class StarChannel : CursedTechnique
    {

        public static readonly int FRAME_COUNT = 8;
        public static readonly int TICKS_PER_FRAME = 5;

        public static Texture2D texture;
        public static Texture2D convergenceTexture;
        public static Texture2D collisionTexture;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.StarChannel.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.StarChannel.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.StarChannel.LockedDescription");
        public override float Cost => 50f;
        public override Color textColor => new Color(132, 4, 4);
        public override bool DisplayNameInGame => true;
        public override int Damage => 100;
        public override int MasteryDamageMultiplier => 18;
        public override float Speed => 0f;

        private float blackholeThreshold = 360f;

        private float starRegen => 34f;

        //Lifetime is made useless but must be implmented 
        public override float LifeTime => 240f;

        private bool keyHeld = false;
        public float animScale;


        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<StarChannel>();
        }

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.EyeofCthulhu);
        }

        public override void SetStaticDefaults()
        {
            if (Main.dedServ) return;
            texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/StarRage/StarChannel", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            animScale = 1f;
            //Projectile.timeLeft = (int)LifeTime;
        }

        public override int UseTechnique(SorceryFightPlayer sf)
        {
            int index = base.UseTechnique(sf);
            Main.projectile[index].rotation = (Main.MouseWorld - sf.Player.Center).ToRotation();
            return index;
        }

        public override void AI()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                keyHeld = SFKeybinds.UseTechnique.Current;

                Projectile.Center = Main.MouseWorld;

                Player player = Main.player[Projectile.owner];
                SorceryFightPlayer sf = Main.player[Projectile.owner].SorceryFight();

                if (keyHeld)
                {

                    //add star power
                    sf.starEnergyRegenPerSecond += starRegen;
                    //check if star energy is going to be full, if it is, play sound effect
                    if (100f < sf.starEnergy + SFUtils.RateSecondsToTicks(sf.starEnergyRegenPerSecond - sf.starEnergyUsagePerSecond))
                    {
                        SoundEngine.PlaySound(SorceryFightSounds.PachinkoBallCollision, Projectile.Center);
                    }

                    blackholeThreshold--;
                }

                if (!keyHeld)
                {
                    Projectile.Kill();
                }

                if(blackholeThreshold <= 0)
                {
                    Main.NewText("BLACKHOLE TRIGGERED");
                    //Spawn black hole at Garuda position then kill him


                    foreach (Projectile projectile in Main.ActiveProjectiles)
                    {
                        if (projectile.type == ModContent.ProjectileType<GarudaHead>() && projectile.owner == Projectile.owner)
                        {
                            int blackHoleDamage = 10;
                            Projectile.NewProjectile(
                            projectile.GetSource_FromThis(),
                            projectile.Center,
                            Vector2.Zero,
                            ModContent.ProjectileType<BlackholeProjectile>(),
                            blackHoleDamage,
                            0f,
                            player.whoAmI
                            );


                            projectile.Kill();
                            //Need code to uncheck the garuda summon box in the UI and remove the buff
                            sf.summonGaruda = false;
                            player.ClearBuff(ModContent.BuffType<SummonGarudaBuff>());
                            Projectile.Kill();
                        }

                    }

                }

            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            if (texture == null && !Main.dedServ)
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/StarRage/StarChannel").Value;


            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, animScale, SpriteEffects.None, 0f);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                SorceryFightPlayer sf = Main.player[Projectile.owner].SorceryFight();
                sf.garudaCurrentTarget = target;
            }
        }
    }
}
