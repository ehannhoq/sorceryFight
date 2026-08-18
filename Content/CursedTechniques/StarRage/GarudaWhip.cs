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
        public static Texture2D texture;

        public override string InternalName => "GarudaWhip";

        // public override float Cost => 40f;
        // public override float StarCost => 20f;
        // public override int Damage => 30;
        // public override int MasteryDamageMultiplier => 50;
        // public override float Speed => 30f;
        // public override float LifeTime => 300f;

        public GarudaWhip()
        {
            Technique.baseDamage = 10;
            Technique.damagePerBoss = 6;
            Technique.cost = 10;
            Technique.speed = 30f;
        }

        public override int UseTechnique(SorceryFightPlayer sf)
        {
            Player player = sf.Player;
            if (player.whoAmI != Main.myPlayer) return -1;

            // sf.starEnergy -= StarCost;
            Vector2 velocity = (Main.MouseWorld - player.MountedCenter).SafeNormalize(Vector2.Zero) * speed;

            return Projectile.NewProjectile(
                player.GetSource_FromThis(),
                player.MountedCenter,
                velocity,
                ModContent.ProjectileType<GarudaWhipProjectile>(),
                CalculateTrueDamage(sf),
                0,
                player.whoAmI
            );
        }
    }
}