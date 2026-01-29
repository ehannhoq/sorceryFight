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
    public class LightspeedBarrage : CursedTechnique
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.LightspeedBarrage.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.LightspeedBarrage.Description");

        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.LightspeedBarrage.LockedDescription");

        public override float Cost => 30f;

        public override Color textColor => Color.White;

        public override bool DisplayNameInGame => false;

        public override int Damage => 50;

        public override int MasteryDamageMultiplier => 50;

        public override float Speed => 15f;

        public override float LifeTime => 1;

        ref float tick => ref Projectile.ai[0];

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<LightspeedBarrage>();
        }

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.EyeofCthulhu);
        }

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center - new Vector2(0f, player.height / 2f) + new Vector2(10 * player.direction);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (tick > LifeTime)
                Projectile.Kill();

            tick++;
        }


    }
}