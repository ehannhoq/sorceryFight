using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.SFPlayer;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.IceFormation
{
    public class IcePlace : CursedTechnique
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.IcePlace.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.IcePlace.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.IcePlace.LockedDescription");
        public override float Cost => 8f;

        public override Color textColor => new Color(100, 180, 255);
        public override bool DisplayNameInGame => true;

        public override int Damage => 0;
        public override int MasteryDamageMultiplier => 0;

        public override float Speed => 0f;
        public override float LifeTime => 900f; // ice melts in 15 sec
        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.KingSlime);
        }

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<IcePlace>();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            //Projectile.friendly = false;
            Projectile.timeLeft = (int)LifeTime;
        }

        public override int UseTechnique(SorceryFightPlayer sf)
        {
            Player player = sf.Player;

            if (player.whoAmI == Main.myPlayer)
            {
                Vector2 mousePos = Main.MouseWorld;
                var entitySource = player.GetSource_FromThis();

                sf.cursedEnergy -= CalculateTrueCost(sf);

                if (DisplayNameInGame)
                {
                    int index1 = CombatText.NewText(player.getRect(), textColor, DisplayName.Value);
                    Main.combatText[index1].lifeTime = 120;
                }

                // Spawn projectile at cursor position with zero velocity
                return Projectile.NewProjectile(entitySource, mousePos, Vector2.Zero, GetProjectileType(), 0, 0, player.whoAmI);
            }
            return -1;
        }

        public override void AI()
        {
            // Convert cursor position to tile coordinates
            int tileX = (int)(Projectile.Center.X / 16f);
            int tileY = (int)(Projectile.Center.Y / 16f);

            // On first tick, place the ice block
            if (Projectile.ai[0] == 0f)
            {
                Projectile.ai[0] = 1f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // Only place if the tile is empty
                    if (!Main.tile[tileX, tileY].HasTile)
                    {
                        WorldGen.PlaceTile(tileX, tileY, TileID.IceBlock, forced: false, style: 0);

                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendTileSquare(-1, tileX, tileY, 1);
                        }
                    }
                }

                // Spawn some frost particles for visual feedback
                for (int i = 0; i < 8; i++)
                {
                    Dust dust = Dust.NewDustDirect(
                        Projectile.position,
                        Projectile.width,
                        Projectile.height,
                        DustID.IceTorch,
                        Main.rand.NextFloat(-2f, 2f),
                        Main.rand.NextFloat(-2f, 2f),
                        150,
                        default,
                        1.2f
                    );
                    dust.noGravity = true;
                }
            }

            // When the projectile dies (timeLeft runs out), the ice gets removed in OnKill
            // Keep projectile alive but invisible at the tile location
            Projectile.Center = new Vector2(tileX * 16 + 8, tileY * 16 + 8);
        }

        public override void OnKill(int timeLeft)
        {
            int tileX = (int)(Projectile.Center.X / 16f);
            int tileY = (int)(Projectile.Center.Y / 16f);

            // Remove the ice block when the projectile expires
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Tile tile = Main.tile[tileX, tileY];
                if (tile.HasTile && tile.TileType == TileID.IceBlock)
                {
                    WorldGen.KillTile(tileX, tileY, noItem: true);

                    if (Main.netMode == NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, tileX, tileY, 1);
                    }
                }
            }

            // Melt particles
            for (int i = 0; i < 6; i++)
            {
                Dust dust = Dust.NewDustDirect(
                    Projectile.position,
                    Projectile.width,
                    Projectile.height,
                    DustID.IceTorch,
                    Main.rand.NextFloat(-1f, 1f),
                    Main.rand.NextFloat(-3f, -1f),
                    100,
                    default,
                    0.8f
                );
                dust.noGravity = true;
            }

            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // Don't draw the projectile itself — the placed tile is the visual
            return false;
        }

        public override string GetStats(SorceryFightPlayer sf)
        {
            string stats = $"Cost: {Math.Round(CalculateTrueCost(sf), 2)} CE\n"
                + $"Duration: {LifeTime / 60f}s\n"
                + "Places a temporary ice block at cursor";
            return stats;
        }
    }
}