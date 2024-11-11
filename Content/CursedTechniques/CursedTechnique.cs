using Microsoft.Xna.Framework;
using sorceryFight.Content.PassiveTechniques;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques
{
    public class CursedTechnique : ModProjectile
    {
        public virtual new string Name { get; set; } = "None Selected.";
        public virtual string Stats 
        {
            get
            {
                if (Cost == -1)
                {
                    return $"Damage: {Damage}\n" 
                        + $"CE Consumption: {CostPercentage}% of Max CE.\n";
                }
                else
                {
                    return $"Damage: {Damage}\n" 
                        + $"CE Consumption: {Cost} CE\n";
                }
            }
        }
        public virtual string Description { get; set; } = "None Selected.";
        public virtual string LockedDescription { get; set; } = "None Selected.";
        public virtual float Cost { get; set; } = 0;
        public virtual float CostPercentage { get; set; } = -1;
        public virtual float MasteryNeeded { get; set; } = 0f;
        public virtual Color textColor { get; set; } = new Color(255, 255, 255);
        public virtual bool DisplayNameInGame { get; set; } = true;

        public virtual int Damage { get; set; } = 0;
        public virtual float Speed { get; set; } = 0f;
        public virtual float LifeTime { get; set; } = 30f;
        public virtual bool Unlocked { get; set; } = false;
        public virtual int GetProjectileType()
        {
            return ModContent.ProjectileType<CursedTechnique>();
        }
        public virtual float GetProjectileSpeed()
        {
            return Speed;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Projectile.Kill();
        }

        public virtual bool Shoot(Terraria.DataStructures.IEntitySource spawnSource, Vector2 position, Vector2 velocity, Player player)
        {
            SorceryFightPlayer sf = player.GetModPlayer<SorceryFightPlayer>();

            if (sf.Player.HasBuff<BurntTechnique>())
            {
                int index = CombatText.NewText(player.getRect(), Color.DarkRed, "You cannot use this technique!");
				Main.combatText[index].lifeTime = 180;
				return false;
            }

            float ceCost = CalculateCECost(sf);

            if (sf.cursedEnergy < ceCost)
            {
                int index = CombatText.NewText(player.getRect(), Color.DarkRed, "Not enough Cursed Energy!");
				Main.combatText[index].lifeTime = 180;
                return false;
            }

            sf.cursedEnergy -= ceCost;

            if (sf.selectedTechnique.DisplayNameInGame)
            {
                int index1 = CombatText.NewText(player.getRect(), sf.selectedTechnique.textColor, sf.selectedTechnique.Name);
                Main.combatText[index1].lifeTime = 180;
            }
            return true;
        }

        private float CalculateCECost(SorceryFightPlayer sf)
        {
            return Cost != -1 ? Cost : sf.maxCursedEnergy * (CostPercentage / 100);
        }
    }
}