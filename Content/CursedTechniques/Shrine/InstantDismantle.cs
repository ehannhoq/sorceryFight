using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs.Vessel;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.Shrine
{
    public class InstantDismantle : CursedTechnique
    {
        public static Texture2D texture;

        public override string InternalName => "InstantDismantle";

        public ref float spawnedByDE => ref Projectile.ai[0];
        public ref float randomSprite => ref Projectile.ai[1];
        public ref float randomRotation => ref Projectile.ai[2];


        public InstantDismantle()
        {
            Technique.baseDamage = 40;
            Technique.damagePerBoss = 15;
            Technique.cost = 33;
            Technique.lifetime = 2;
        }


        public override int UseTechnique(SorceryFightPlayer sf)
        {
            Player player = sf.Player;


            if (Main.myPlayer == player.whoAmI)
            {
                Vector2 mousePos = Main.MouseWorld;
                var entitySource = player.GetSource_FromThis();
                int index = Projectile.NewProjectile(entitySource, mousePos, Vector2.Zero, GetProjectileType(), (int)CalculateTrueDamage(sf), 0f, player.whoAmI);
                Main.projectile[index].ai[0] = 0;
                Main.projectile[index].ai[1] = Main.rand.Next(0, 3);
                Main.projectile[index].ai[2] = Main.rand.NextFloat(0, 6);
                sf.cursedEnergy -= CalculateTrueCost(sf);
                return index;
            }
            return -1;
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 63;
            Projectile.height = 176;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }


        public override void AI()
        {
            if (spawnedByDE == 1)
            {
                Projectile.damage = 100;
            }
        }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (spawnedByDE == 1)
            {
                float targetHealth = target.life;
                float additionalDamage = targetHealth * 0.001f;
                modifiers.FinalDamage.Flat += additionalDamage;
            }

            base.ModifyHitNPC(target, ref modifiers);
        }


        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            if (texture == null && !Main.dedServ)
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Shrine/InstantDismantle").Value;

            int frameHeight = texture.Height / 3;
            int frameY = (int)randomSprite * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);
            Rectangle srcRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, srcRectangle, Color.White, (int)randomRotation, origin, 1f, SpriteEffects.None, 0f);

            return false;
        }
    }
}