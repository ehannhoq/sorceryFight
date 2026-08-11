using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Tiles.Ice
{
    public class UraumeBlockGlobalProjectile : GlobalProjectile
    {
        public override bool PreAI(Projectile projectile)
        {
            if (!projectile.active)
                return true;

            if (!projectile.hostile)
                return true;

            if (!IsOnUraumeBlock(projectile))
                return true;

            int tileX = (int)(projectile.Center.X / 16f);
            int tileY = (int)(projectile.Center.Y / 16f);

            for (int d = 0; d < 4; d++)
            {
                Dust dust = Dust.NewDustDirect(
                    new Vector2(tileX * 16, tileY * 16),
                    16, 16,
                    Terraria.ID.DustID.IceTorch,
                    projectile.velocity.X * 0.2f,
                    projectile.velocity.Y * 0.2f,
                    150,
                    default,
                    1f
                );
                dust.noGravity = true;
            }

            if (Main.netMode != Terraria.ID.NetmodeID.MultiplayerClient)
            {
                projectile.Kill();
            }

            return false;
        }

        public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
        {
            if (!projectile.friendly)
                return true;

            int tileX = (int)(projectile.Center.X / 16f);
            int tileY = (int)(projectile.Center.Y / 16f);

            for (int x = tileX - 1; x <= tileX + 1; x++)
            {
                for (int y = tileY - 1; y <= tileY + 1; y++)
                {
                    if (!WorldGen.InWorld(x, y, 1))
                        continue;

                    Tile tile = Main.tile[x, y];
                    if (tile.HasTile && tile.TileType == ModContent.TileType<UraumeBlock>())
                    {
                        projectile.velocity = oldVelocity;
                        return false;
                    }
                }
            }

            return true;
        }

        private bool IsOnUraumeBlock(Projectile projectile)
        {
            int uraumeType = ModContent.TileType<UraumeBlock>();

            Rectangle hitbox = projectile.Hitbox;

            int left = hitbox.Left / 16;
            int right = hitbox.Right / 16;
            int top = hitbox.Top / 16;
            int bottom = hitbox.Bottom / 16;

            for (int x = left; x <= right; x++)
            {
                for (int y = top; y <= bottom; y++)
                {
                    if (!WorldGen.InWorld(x, y, 1))
                        continue;

                    Tile tile = Main.tile[x, y];
                    if (tile.HasTile && tile.TileType == uraumeType)
                        return true;
                }
            }

            return false;
        }
    }
}