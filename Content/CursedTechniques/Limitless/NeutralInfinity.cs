using System;
using Ionic.Zip;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.Limitless
{
    /// <summary>
    ///  This is a temporary class, used for testing Infinity.
    /// </summary>
    public class NeutralInfinity : CursedTechnique
    {
        public override string Name { get; set; } = "Infinity";
        public override float Cost { get; set; } = 0f;
        public override float CostPercentage { get; set; } = -1f;
        public override float MasteryNeeded { get; set; } = 0f;
        public override Color textColor { get; set; } = new Color(108, 158, 240);

        public override int Damage { get; set ; } = 30;
        public override float Speed { get; set; } = 10f;
        public override float LifeTime { get; set; } = 100f;

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<NeutralInfinity>();
        }

        public override float GetProjectileSpeed()
        {
            return Speed;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.tileCollide = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            SorceryFightPlayer sf = player.GetModPlayer<SorceryFightPlayer>();
            sf.hasInfinity = !sf.hasInfinity;

            Projectile.Kill();
        }
    }
}