// using System;
// using CalamityMod.Particles;
// using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.Graphics;
// using sorceryFight.Content.Items.Weapons.Melee;
// using Terraria;
// using Terraria.ModLoader;

// namespace sorceryFight.Content.Projectiles.Melee
// {
//     public class InvertedSpearCharge : ModProjectile
//     {
//         private static Texture2D texture => ModContent.Request<Texture2D>("sorceryFight/Content/Projectiles/Melee/InvertedSpearCharge", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

//         public override void SetDefaults()
//         {
//             Projectile.width = texture.Width * 2;
//             Projectile.height = texture.Height * 2;
//             Projectile.friendly = true;
//             Projectile.penetrate = -1;
//             Projectile.tileCollide = false;
//             Projectile.DamageType = CursedTechniqueDamageClass.Instance;
//             Projectile.usesIDStaticNPCImmunity = true;
//             Projectile.idStaticNPCHitCooldown = 60;
//             Projectile.frameCounter = 0;
//         }

//         public override void AI()
//         {
//             if (Projectile.ai[0] < InvertedSpear.chargeUpMax)
//                 Projectile.ai[0]++;

//             Projectile.ai[2] += 0.05f;

//             Projectile.idStaticNPCHitCooldown = 60 - (int)(55 * (Projectile.ai[0] / 120));


//             Player player = Main.player[Projectile.owner];
//             Vector2 playerRotatedPoint = player.RotatedRelativePoint(player.MountedCenter, true);
//             float velocityAngle = Projectile.velocity.ToRotation();

//             Projectile.velocity = (Main.MouseWorld - playerRotatedPoint).SafeNormalize(Vector2.UnitX * player.direction);
//             Projectile.direction = (Math.Cos(velocityAngle) > 0).ToDirectionInt();
//             Projectile.rotation = velocityAngle + (Projectile.direction == -1).ToInt() * MathHelper.Pi;
//             Projectile.Center = playerRotatedPoint + velocityAngle.ToRotationVector2();
//             player.ChangeDir(Projectile.direction);

//             if (Main.myPlayer == Projectile.owner)
//             {
//                 if (player.CantUseSword(Projectile))
//                 {
//                     Projectile.Kill();
//                 }
//             }

//         }

//         public override bool PreDraw(ref Color lightColor)
//         {
//             Rectangle src = new Rectangle(0, 0, texture.Width, texture.Height);
//             Vector2 origin = new Vector2(0, texture.Height);

//             Projectile.ai[1] += Projectile.ai[0] / 120 * 0.5f * (MathF.Cos(Projectile.ai[2] * MathF.PI) / 4 + 0.75f);
//             float rotation = Projectile.ai[1] % (2 * MathF.PI);

//             Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, src, Color.White, rotation, origin, 1f, SpriteEffects.None, 0);

//             return false;
//         }
//         public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
//         {
//             modifiers.ArmorPenetration += target.defense;
//             modifiers.ScalingArmorPenetration += 1f;
//             modifiers.Defense *= 0;
//             base.ModifyHitNPC(target, ref modifiers);
//         }

//         public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
//         {
//             for (int i = 0; i < 3; i++)
//             {
//                 Vector2 veloVariation = new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-10f, 10f));
//                 int colVariation = Main.rand.Next(-38, 100);
//                 float scale = Main.rand.NextFloat(1f, 1.25f);
//                 float scalar = Main.rand.NextFloat(5f, 15f);
//                 SparkParticle particle = new SparkParticle(target.Center, (Projectile.velocity * scalar) + veloVariation, false, 30, scale, new Color(225 + colVariation, 242 + colVariation, 97 + colVariation));
//                 GeneralParticleHandler.SpawnParticle(particle);
//             }

//             for (int i = 0; i < 2; i++)
//             {
//                 Vector2 veloVariation = new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-10f, 10f));
//                 int colVariation = Main.rand.Next(-38, 100);
//                 float scale = Main.rand.NextFloat(1f, 1.25f);
//                 float scalar = Main.rand.NextFloat(5f, 15f);
//                 LineParticle particle = new LineParticle(target.Center, (Projectile.velocity * scalar) + veloVariation, false, 30, scale, new Color(225 + colVariation, 242 + colVariation, 97 + colVariation));
//                 GeneralParticleHandler.SpawnParticle(particle);
//             }

//             for (int i = 0; i < 2; i++)
//             {
//                 Vector2 posVariation = new Vector2(Main.rand.NextFloat(-10, 10), Main.rand.NextFloat(-10, 10));
//                 SparkleParticle particle = new SparkleParticle(target.Center + posVariation, Vector2.Zero, new Color(225, 242, 97), Color.White, 1f, 10, 0.75f, 0.2f);
//                 GeneralParticleHandler.SpawnParticle(particle);
//             }
//         }
//     }
// }
