using Microsoft.Xna.Framework;
using sorceryFight.SFPlayer;
using Steamworks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.HeavenlyRestriction
{
    public class FlashStep : CursedTechnique
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.FlashStep.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.FlashStep.Description");

        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.FlashStep.LockedDescription");

        public override float Cost => 30f;

        public override Color textColor => Color.White;

        public override bool DisplayNameInGame => false;

        public override int Damage => 50;

        public override int MasteryDamageMultiplier => 50;

        public override float Speed => 15f;

        public override float LifeTime => 1;

        ref float tick => ref Projectile.ai[0];
        private const float tileSize = 16f;
        private float minDistance = 30f * tileSize;
        private float maxDistance = 50f * tileSize;


        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<FlashStep>();
        }

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.SkeletronPrime);
        }

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];

            if (Main.myPlayer == player.whoAmI)
            {
                SorceryFightPlayer sfPlayer = player.SorceryFight();
                float distanceDiff = maxDistance - minDistance;
                float trueMaxDistance = minDistance + ((sfPlayer.numberBossesDefeated / SorceryFight.totalBosses) * distanceDiff);

                Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);

                float currentDistance;
                for (currentDistance = 0; currentDistance < trueMaxDistance; currentDistance += 0.1f)
                {
                    Point tilePos = (player.Center + dir * currentDistance).ToTileCoordinates();
                    
                    if (!WorldGen.InWorld(tilePos.X, tilePos.Y))
                        break;
                    
                    Tile tile = Main.tile[tilePos];

                    bool walkableTile = !tile.HasTile || !Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType] || tile.IsActuated;

                    bool passable = walkableTile || (tile.LiquidAmount > 0 && tile.LiquidType == LiquidID.Water);
                    if (!passable)
                        break;
                }

                player.Center += dir * currentDistance;
            }

            Projectile.Kill();
        }
    }
}