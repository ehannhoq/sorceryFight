using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
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

        public override string InternalName => "HakarisDoor";

        public Color rarity;

        public HakarisDoor()
        {
            Technique.baseDamage = 2;
            Technique.damagePerBoss = 2;
            Technique.cost = 30;
            Technique.lifetime = FRAME_COUNT * TICKS_PER_FRAME + 15;
        }


        public override int UseTechnique(SorceryFightPlayer sf)
        {
            Player player = sf.Player;

            if (Main.myPlayer == player.whoAmI)
            {
                Vector2 mousePos = Main.MouseWorld;
                var entitySource = player.GetSource_FromThis();
                return Projectile.NewProjectile(entitySource, mousePos, Vector2.Zero, GetProjectileType(), CalculateTrueDamage(sf), 0f, player.whoAmI);
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
                    Projectile.damage = (int)(CalculateTrueDamage(Main.player[Projectile.owner].SorceryFight()) * 1.5);
                }
                else
                {
                    rarity = Color.Red;
                    Projectile.damage = (int)(CalculateTrueDamage(Main.player[Projectile.owner].SorceryFight()) * 2);
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