using Microsoft.Xna.Framework;
using sorceryFight.Content.Buffs;
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
                return $"Damage: {Damage}\n" 
                    + $"CE Consumption: {Cost} CE\n";
            }
        }
        public virtual string Description { get; set; } = "None Selected.";
        public virtual string LockedDescription { get; set; } = "None Selected.";
        public virtual float Cost { get; set; } = 0;
        public virtual float MasteryNeeded { get; set; } = 0f;
        public virtual Color textColor { get; set; } = new Color(255, 255, 255);
        public virtual bool DisplayNameInGame { get; set; } = true;

        public virtual int Damage { get; set; } = 0;
        public virtual float Speed { get; set; } = 0f;
        public virtual float LifeTime { get; set; } = 30f;
        public virtual bool Unlocked(SorceryFightPlayer sf) { return false; }
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
                int index = CombatText.NewText(player.getRect(), Color.DarkRed, "Your technique is exhausted!");
				Main.combatText[index].lifeTime = 180;
				return false;
            }

            if (sf.cursedEnergy < Cost)
            {
                int index = CombatText.NewText(player.getRect(), Color.DarkRed, "Not enough Cursed Energy!");
				Main.combatText[index].lifeTime = 180;
                return false;
            }

            sf.cursedEnergy -= Cost;

            if (sf.selectedTechnique.DisplayNameInGame)
            {
                int index1 = CombatText.NewText(player.getRect(), sf.selectedTechnique.textColor, sf.selectedTechnique.Name);
                Main.combatText[index1].lifeTime = 180;
            }
            return true;
        }
    }
}