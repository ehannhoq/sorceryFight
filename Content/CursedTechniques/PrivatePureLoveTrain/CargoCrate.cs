using System;
using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.PrivatePureLoveTrain
{
    public class CargoCrate : CursedTechnique
    {
        public static Texture2D texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/PrivatePureLoveTrain/CargoCrate", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        public static Texture2D hitTexture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/PrivatePureLoveTrain/CargoCrateHit", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        public static Texture2D punchTexture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/PrivatePureLoveTrain/CargoCratePunch", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.CargoCrate.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.CargoCrate.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.CargoCrate.LockedDescription");
        public override float Cost => 300f;
        public override Color textColor => new Color(59, 64, 112);
        public override bool DisplayNameInGame => true;
        public override int Damage => 5500;
        public override int MasteryDamageMultiplier => 350;
        public override float Speed => 0f;
        public override float LifeTime => 180f;
        public Vector2 direction;
        public bool impactFrame = false;
        float distance = 0f;


        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<CargoCrate>();
        }

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.Golem);
        }


        public override void SetDefaults()
        {
            Projectile.width = 88;
            Projectile.height = 88;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = (int)LifeTime;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.aiStyle = 14;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            direction = Main.MouseWorld - player.Center;
            direction.Normalize();
            switch ((int)Projectile.ai[1])
            {
                case 0:
                    Projectile.Center = player.Center + direction * 80f;
                    for (int i = 0; i < Main.projectile.Length; i++)
                    {
                        if (i == Projectile.whoAmI)
                            continue;

                        Projectile proj = Main.projectile[i];

                        if (proj.active && proj.type == ModContent.ProjectileType<CargoCrate>() && proj.owner == Projectile.owner && proj.ai[1] == 0 && proj.timeLeft >= 170)
                        {
                            Projectile.active = false;
                            return;
                        }

                    }
                    Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, Projectile.type, 0, 0, player.whoAmI, ai1: 1);
                    break;
                case 1:
                    Projectile.aiStyle = 0;
                    Projectile.timeLeft = 10;
                    Projectile.Center = player.Center;
                    Projectile.rotation = direction.ToRotation();
                    break;
                case 2:
                    Projectile.timeLeft = 27;
                    Projectile.aiStyle = 0;
                    Projectile.frame = 0;
                    Projectile.rotation = Main.rand.NextFloat(0f, 360f);
                    break;
            }

            

            
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            switch ((int)Projectile.ai[1])
            {
                case 0:
                    if (Projectile.timeLeft == 170)
                    {
                        SoundEngine.PlaySound(SorceryFightSounds.CargoCrateImpact, Projectile.Center);
                        CameraController.CameraShake(5, 5, 5);
                        Projectile.velocity = direction * 17f;
                        Projectile.frame++;
                    }

                    if (Projectile.timeLeft < 170)
                    {
                        player.GetModPlayer<SorceryFightPlayer>().disableRegenFromProjectiles = false;
                        Projectile.rotation = Projectile.velocity.ToRotation();
                        if (Projectile.timeLeft % 10 == 0)
                        {
                            Projectile.frame++;
                        }

                        if (Projectile.frame > 3)
                        {
                            Projectile.frame = 2;
                        }
                    }
                    else
                    {
                        Projectile.Center = player.Center + direction * 80f;
                        player.GetModPlayer<SorceryFightPlayer>().disableRegenFromProjectiles = true;
                        Projectile.rotation = direction.ToRotation();
                    }

                    break;
                case 1:
                    distance += 0.1f;
                    Projectile.Center = Vector2.Lerp(player.Center, player.Center + direction * 40f, distance);
                    if(Projectile.timeLeft % 3 == 0)
                        Projectile.frame++;
                    break;
                case 2:
                    if (Projectile.timeLeft % 3 == 0)
                    {
                        if (Projectile.timeLeft >= 16)
                        {

                            Projectile.frame++;
                        }
                        else
                        {
                            Projectile.frame--;
                        }
                    }
                    break;

            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.velocity = Vector2.Zero;
            Projectile.Kill();
        }

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.ai[1] == 0)
            {
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                player.GetModPlayer<SorceryFightPlayer>().disableRegenFromProjectiles = false;
                Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, this.Type, 0, 0, player.whoAmI, ai1: 2);
            }

                
        }
        public override bool PreDraw(ref Color lightColor)
        {
            switch ((int)Projectile.ai[1])
            {
                case 0:
                    int frameHeight = texture.Height / 4;
                    int frameY = Projectile.frame * frameHeight;
                    Vector2 scale = new Vector2(1.25f, 1.25f);
                    Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);
                    Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
                    Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, scale, SpriteEffects.None, 0f);
                    break;
                case 1:
                    frameHeight = punchTexture.Height / 4;
                    frameY = Projectile.frame * frameHeight;
                    origin = new Vector2(punchTexture.Width / 2, frameHeight / 2);
                    sourceRectangle = new Rectangle(0, frameY, punchTexture.Width, frameHeight);
                    Main.EntitySpriteDraw(punchTexture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, 1.25f, SpriteEffects.None, 0f);
                    break;
                case 2:
                    SpriteBatch spriteBatch = Main.spriteBatch;
                    frameHeight = hitTexture.Height / 5;
                    frameY = Projectile.frame * frameHeight;
                    origin = new Vector2(hitTexture.Width / 2, frameHeight / 2);
                    sourceRectangle = new Rectangle(0, frameY, hitTexture.Width, frameHeight);
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
                    spriteBatch.Draw(hitTexture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, 1f, SpriteEffects.None, 0f);
                    spriteBatch.End();
                    spriteBatch.Begin();
                    break;

            }

            return false;
        }
        
        
    }

}