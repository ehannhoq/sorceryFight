using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

public class SorceryFightGlobalProjectile : GlobalProjectile
{
    public override bool InstancePerEntity => true;

    //public override void SetDefaults(Projectile projectile)
    //{
    //    if ((projectile.minion || projectile.sentry) && projectile.ModProjectile?.Mod == Mod)
    //        projectile.netImportant = true;
    //}

    //public override void PostAI(Projectile projectile)
    //{
    //    if (Main.netMode == NetmodeID.SinglePlayer)
    //        return;

    //    // Check for either minion or sentry belonging to this mod
    //    bool isMinionOrSentry = (projectile.minion || projectile.sentry)
    //        && projectile.ModProjectile?.Mod == Mod;

    //    if (!isMinionOrSentry)
    //        return;

    //    if (projectile.owner == Main.myPlayer && projectile.timeLeft % 30 == 0)
    //        projectile.netUpdate = true;
    //}
}