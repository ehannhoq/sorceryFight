using Microsoft.Xna.Framework;
using sorceryFight.Content.Buffs.Vessel;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.Vessel
{
    public class FullIncarnationKOC : CursedTechnique
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.FullIncarnationKOC.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.FullIncarnationKOC.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.FullIncarnationKOC.LockedDescription");
        public override float Cost => 1000f;
        public override float BloodCost => 100f;
        public override Color textColor => new Color(255, 0, 0);
        public override bool DisplayNameInGame => true;
        public override int Damage => 0;
        public override int MasteryDamageMultiplier => 0;
        public override float Speed => 0f;
        public override float LifeTime => 300f;

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.sukunasSkull;
        }

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<FullIncarnationKOC>();
        }


        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        public override void AI()
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Main.player[Projectile.owner].AddBuff(ModContent.BuffType<KingOfCursesBuff>(), 9999999);
            }

            Projectile.Kill();
        }
    }
}