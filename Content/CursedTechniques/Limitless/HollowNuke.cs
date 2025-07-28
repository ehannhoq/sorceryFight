using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.Limitless
{
    public class HollowNuke : ModSystem
    {
        private float opacity = 0f;
        private Projectile maxBlue;
        private Projectile maxRed;
        private bool validHollowNuke = false;

        public override void PostUpdateProjectiles()
        {
            if (maxBlue == null || maxRed == null)
            {
                foreach (Projectile proj in Main.ActiveProjectiles)
                {
                    if (proj.type != ModContent.ProjectileType<MaximumOutputBlue>()) continue;
                    maxBlue = proj;

                    foreach (Projectile proj2 in Main.ActiveProjectiles)
                    {
                        if (proj2.type != ModContent.ProjectileType<MaximumOutputRed>() || proj.whoAmI == proj2.whoAmI) continue;

                        if (!IsValidCollision(proj, proj2, 0.9f)) continue;

                        maxRed = proj2;
                        validHollowNuke = true;
                    }
                }
            }

            if (maxRed != null && maxBlue != null)
            {
                if (!IsValidCollision(maxBlue, maxRed, 0.9f))
                {
                    maxBlue = null;
                    maxRed = null;
                    return;
                }

                float distance = Vector2.Distance(maxBlue.Center, maxRed.Center);
                if (distance <= 50f)
                {
                    Vector2 center = maxBlue.Center + (maxBlue.Center.DirectionTo(maxRed.Center) * (distance / 2f));

                    Collision(maxBlue.owner, center);

                    maxBlue.Kill();
                    maxBlue = null;

                    maxRed.Kill();
                    maxRed = null;
                }

                if (!Main.dedServ)
                {
                    if (distance > 50f)
                    {
                        opacity = 1f - (distance / 300f);
                        opacity = Math.Clamp(opacity, 0f, 1f);

                        if (!Filters.Scene["SF:HollowNuke"].Active)
                        {
                            Filters.Scene.Activate("SF:HollowNuke").GetShader().UseProgress(opacity);
                        }
                        else
                        {
                            Filters.Scene["SF:HollowNuke"].GetShader().UseProgress(opacity);
                        }
                    }
                    else
                    {
                        opacity = 1f;
                        Filters.Scene["SF:HollowNuke"].GetShader().UseProgress(opacity);
                    }
                }
            }
            else if (validHollowNuke)
            {
                validHollowNuke = false;

                if (!Main.dedServ)
                {
                    TaskScheduler.Instance.AddDelayedTask(() =>
                    {
                        TaskScheduler.Instance.AddContinuousTask(() =>
                            {
                                opacity -= 1 / 30f;
                                opacity = Math.Clamp(opacity, 0f, 1f);

                                Filters.Scene["SF:HollowNuke"].GetShader().UseProgress(opacity);
                            }, 30);
                    }, 90);
                }
            }
        }

        private bool IsValidCollision(Projectile stationaryProj, Projectile movingProj, float threshold)
        {
            Vector2 proj2Dir = movingProj.velocity.SafeNormalize(Vector2.Zero);
            Vector2 collisionDir = movingProj.Center.DirectionTo(stationaryProj.Center);
            float dotprd = Vector2.Dot(proj2Dir, collisionDir);

            if (dotprd >= threshold) return true;

            return false;
        }

        private void Collision(int owner, Vector2 center)
        {
            SoundEngine.PlaySound(SorceryFightSounds.CommonBoom, center);

            float minDist = 2000f;

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (Vector2.Distance(npc.Center, center) > minDist) continue;

                Main.player[owner].ApplyDamageToNPC(npc, 100000, 0f, 1, false, CursedTechniqueDamageClass.Instance, false);
            }

            foreach (Player player in Main.ActivePlayers)
            {
                if (Vector2.Distance(player.Center, center) > minDist) continue;

                player.statLife = (int)((float)player.statLifeMax2 * 0.90f);
            }
        }

    }
}
