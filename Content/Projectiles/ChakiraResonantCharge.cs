using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace sorceryFight.Content.Projectiles
{
    public class ChakiraResonantCharge : ModProjectile
    {
        public override string Texture => "sorceryFight/Content/CursedTechniques/CursedTechnique";
        public static readonly int FRAMES = 60;
        public static readonly int TICKS_PER_FRAME = 1;

        private Texture2D texture;

        private ref float prog => ref Projectile.ai[0];
        private ref float logProg => ref Projectile.ai[1];
        private ref float beamIndex => ref Projectile.ai[2];

        public override void OnSpawn(IEntitySource source)
        {
            beamIndex = -1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 300;
            Projectile.height = 300;
            Projectile.ArmorPenetration = 7;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = CursedTechniqueDamageClass.Instance;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 30;
        }

        public override void AI()
        {
            if (Projectile.frameCounter++ >= TICKS_PER_FRAME)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= FRAMES)
                {
                    Projectile.frame = 0;
                }
            }

            if (Projectile.frame % 30 == 1)
                SoundEngine.PlaySound(SorceryFightSounds.ChakiraResonantProjectileAmbiance, Projectile.Center);


            Player player = Main.player[Projectile.owner];

            if (beamIndex == -1)
                beamIndex = Projectile.NewProjectile(player.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ChakiraResonantBeam>(), Projectile.damage, 0, Projectile.owner, Projectile.whoAmI);

            Projectile beamProj = Main.projectile[(int)beamIndex];
            beamProj.Center = Projectile.Center;

            if (Main.myPlayer == Projectile.owner)
            {
                beamProj.rotation = (Main.MouseWorld - player.Center).ToRotation();
                beamProj.netUpdate = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.NonPremultiplied,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                null,
                Main.GameViewMatrix.ZoomMatrix
            );

            if (beamIndex != -1)
            {
                Projectile beamProj = Main.projectile[(int)beamIndex];
                ChakiraResonantBeam beam = beamProj.ModProjectile as ChakiraResonantBeam;
                if (beam != null)
                {
                    Texture2D beamTexture = ModContent.Request<Texture2D>($"sorceryFight/Content/Projectiles/ChakiraResonantBeam/{beamProj.frame}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

                    Vector2 dir = beamProj.rotation.ToRotationVector2();
                    Vector2 start = beamProj.Center - Main.screenPosition;
                    Vector2 origin = new Vector2(0, beamTexture.Height / 2f);

                    for (float i = 0f; i < beamProj.ai[0]; i += ChakiraResonantBeam.STEP_SIZE)
                    {
                        Vector2 scale = new Vector2(1f, beamProj.ai[1]);
                        Main.EntitySpriteDraw(beamTexture, start + dir * i, null, Color.White, beamProj.rotation, origin, scale * 2f, SpriteEffects.None);
                    }
                }
            }

            texture = ModContent.Request<Texture2D>($"sorceryFight/Content/Projectiles/ChakiraResonantCharge/{Projectile.frame}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 projOrigin = sourceRectangle.Size() * 0.5f;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, projOrigin, logProg * 2f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin();
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (beamIndex != -1)
            {
                Projectile beamProj = Main.projectile[(int)beamIndex];
                beamProj.Kill();
            }
        }
    }
}
