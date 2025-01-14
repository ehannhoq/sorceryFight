using System;
using CalamityMod.NPCs.Providence;
using Microsoft.Xna.Framework;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.Shrine
{
    public class DivineFlame : CursedTechnique
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.DivineFlame.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.DivineFlame.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.DivineFlame.LockedDescription");
        public override float Cost => 150f;
        public override Color textColor => new Color(120, 21, 8);
        public override bool DisplayNameInGame => true;
        public override int Damage => 1;
        public override int MasteryDamageMultiplier => 1;
        public override float Speed => 0f;
        public override float LifeTime => 2f;
        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<DivineFlame>();
        }
        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(ModContent.NPCType<Providence>());
        }
    }
}
