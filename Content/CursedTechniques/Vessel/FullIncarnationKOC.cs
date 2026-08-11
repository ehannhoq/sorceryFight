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
        public override string InternalName => "FullIncarnationKOC";

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override void AI()
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Main.player[Projectile.owner].AddBuff(ModContent.BuffType<KingOfCurses>(), SFUtils.BuffSecondsToTicks(60));
            }

            Projectile.Kill();
        }
    }
}
