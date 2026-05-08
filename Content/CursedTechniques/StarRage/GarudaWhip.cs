using Microsoft.Build.Graph;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Particles;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.StarRage
{
    public class GarudaWhip : CursedTechnique
    {

        //public static readonly int FRAME_COUNT = 8;
        //public static readonly int TICKS_PER_FRAME = 5;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.GarudaWhip.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.GarudaWhip.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.GarudaWhip.LockedDescription");
        public override float Cost => 40f;

        public override float StarCost => 20f;

        public override Color textColor => new Color(255, 0, 0);
        public override bool DisplayNameInGame => true;

        public override int Damage => 30;
        public override int MasteryDamageMultiplier => 50;

        public override float Speed => 30f;
        public override float LifeTime => 300f;
        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.SkeletronHead);
        }

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<GarudaWhipProjectile>();
        }


        public static Texture2D texture;

        public bool animating;
        public float animScale;



        public override int UseTechnique(SorceryFightPlayer sf)
        {
            Player player = sf.Player;
            if (player.whoAmI != Main.myPlayer) return -1;

            sf.cursedEnergy -= CalculateTrueCost(sf);
            sf.starEnergy -= StarCost;

            if (DisplayNameInGame)
            {
                int index = CombatText.NewText(player.getRect(), textColor, DisplayName.Value);
                Main.combatText[index].lifeTime = 180;
            }

            Vector2 velocity = (Main.MouseWorld - player.MountedCenter).SafeNormalize(Vector2.Zero) * Speed;

            return Projectile.NewProjectile(
                player.GetSource_FromThis(),
                player.MountedCenter,
                velocity,
                GetProjectileType(),
                (int)CalculateTrueDamage(sf),
                0,
                player.whoAmI
            );
        }
    }
}