using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Tiles.Ice;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using System;
using System.Collections.Generic;
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
        public override float Cost => 2f;
        public override float CursedCostPerSecond => 5f;

        public override Color textColor => new Color(100, 180, 255);
        public override bool DisplayNameInGame => true;

        public override int Damage => 0;
        public override int MasteryDamageMultiplier => 0;

        public override float Speed => 0f;
        public override float LifeTime => 9000f;
        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.KingSlime);
        }

        public bool keyHeld;
        private int placeCooldown = 0;
        private static readonly int PLACE_DELAY = 3;

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
            Projectile.friendly = false;
            Projectile.timeLeft = (int)LifeTime;
        }

        public override int UseTechnique(SorceryFightPlayer sf)
        {
            Player player = sf.Player;

            if (player.whoAmI == Main.myPlayer)
            {
                var entitySource = player.GetSource_FromThis();

                sf.cursedEnergy -= CalculateTrueCost(sf);

                if (DisplayNameInGame)
                {
                    int index1 = CombatText.NewText(player.getRect(), textColor, DisplayName.Value);
                    Main.combatText[index1].lifeTime = 120;
                }

                return Projectile.NewProjectile(entitySource, player.Center, Vector2.Zero, GetProjectileType(), 0, 0, player.whoAmI);
            }
            return -1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            SorceryFightPlayer sf = player.SorceryFight();

            if (Main.myPlayer == Projectile.owner)
            {
                keyHeld = SFKeybinds.UseTechnique.Current;
            }

            if (!keyHeld || sf.cursedEnergy <= 0)
            {
                Projectile.Kill();
                return;
            }

            ActiveDrain(sf);

            if (placeCooldown > 0)
            {
                placeCooldown--;
                return;
            }

            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 mousePos = Main.MouseWorld;
                int tileX = (int)(mousePos.X / 16f);
                int tileY = (int)(mousePos.Y / 16f);

                if (!Main.tile[tileX, tileY].HasTile)
                {
                    WorldGen.PlaceTile(tileX, tileY, ModContent.TileType<UraumeBlock>(), forced: false, style: 0);

                    if (Main.tile[tileX, tileY].HasTile)
                    {
                        ModContent.GetInstance<UraumeBlockTE>().Place(tileX, tileY);

                        if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.SendTileSquare(-1, tileX, tileY, 1);
                        }

                        for (int i = 0; i < 4; i++)
                        {
                            Dust dust = Dust.NewDustDirect(
                                new Vector2(tileX * 16, tileY * 16),
                                16, 16,
                                DustID.IceTorch,
                                Main.rand.NextFloat(-1.5f, 1.5f),
                                Main.rand.NextFloat(-1.5f, 1.5f),
                                150,
                                default,
                                1f
                            );
                            dust.noGravity = true;
                        }
                    }
                }

                placeCooldown = PLACE_DELAY;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override string GetStats(SorceryFightPlayer sf)
        {
            string stats = $"Cost: {Math.Round(CalculateTrueCost(sf), 2)} CE\n"
                + $"Drain: {CursedCostPerSecond} CE/s\n"
                + "Hold to place temporary ice blocks at cursor";
            return stats;
        }
    }
}