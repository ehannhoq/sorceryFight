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
    public class PebbleBarrage : CursedTechnique
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.PebbleBarrage.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.PebbleBarrage.Description");

        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.PebbleBarrage.LockedDescription");

        public override float Cost => 30f;

        public override Color textColor => Color.White;

        public override bool DisplayNameInGame => false;

        public override int Damage => 50;

        public override int MasteryDamageMultiplier => 50;

        public override float Speed => 15f;

        public override float LifeTime => 300;

        ref float tick => ref Projectile.ai[0];

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<PebbleBarrage>();
        }

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.EyeofCthulhu);
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Main.dedServ) return;
            if (Main.myPlayer != Projectile.owner) return;

            CameraController.CameraShake(30, 30f, 10f);
        }

        public override void AI()
        {
        }
    }
}