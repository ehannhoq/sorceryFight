using System;
using Microsoft.Xna.Framework;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.Shrine
{
    public class Cleave : CursedTechnique
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.Cleave.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.Cleave.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.Cleave.LockedDescription");
        public override float Cost => 150f;
        public override Color textColor => new Color(120, 21, 8);
        public override bool DisplayNameInGame => true;
        public override int BaseDamage => 1;
        public override int MaxDamage => 1;
        public override float Speed => 0f;
        public override float LifeTime => 2f;
        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<Cleave>();
        }
        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.SkeletronHead);
        }
        public override void UseTechnique(SorceryFightPlayer sf)
        {
            Player player = sf.Player;
            
            if (player.whoAmI == Main.myPlayer)
            {
                Vector2 playerPos = player.MountedCenter;
                Vector2 mousePos = Main.MouseWorld;
                Vector2 dir = (mousePos - playerPos).SafeNormalize(Vector2.Zero) * Speed;
                var entitySource = player.GetSource_FromThis();

                sf.cursedEnergy -= Cost;

                if (DisplayNameInGame)
                {
                    int index1 = CombatText.NewText(player.getRect(), textColor, DisplayName.Value);
                    Main.combatText[index1].lifeTime = 180;
                }
            }
        }
    }
}
