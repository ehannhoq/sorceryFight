using Microsoft.Build.Graph;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using sorceryFight.SFPlayer;
using System.Collections.Generic;
using System;

namespace sorceryFight.Content.CursedTechniques.PrivatePureLoveTrain
{
    public class PachinkoBalls : CursedTechnique
    {
        public static Texture2D texture;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.PachinkoBalls.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.PachinkoBalls.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.PachinkoBalls.LockedDescription");
        public override float Cost => 30f;
        public override Color textColor => new Color(108, 158, 240);
        public override bool DisplayNameInGame => true;
        public override int Damage => 30;
        public override int MasteryDamageMultiplier => 35;
        public override float Speed => 30f;
        public override float LifeTime => 300f;

        Dictionary<int, List<int>> enemyRicochets = new Dictionary<int, List<int>>();
        Dictionary<int, Color> rarity = new Dictionary<int, Color>();
        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return true;
            // return sf.HasDefeatedBoss(ModContent.NPCType<DesertScourgeHead>());
        }
        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<PachinkoBalls>();
        }

        public override void SetStaticDefaults()
        {            
            if (Main.dedServ) return;
            texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/PrivatePureLoveTrain/PachinkoBalls", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = (int)LifeTime;
        }

        public override void AI()
        {
            if (!enemyRicochets.ContainsKey(Projectile.whoAmI))
            {
                // the first index is the number of ricochets
                // everything after is the npc.whoAmI of all the npc's has already hit; preventing them from being hit again.
                enemyRicochets[Projectile.whoAmI] = new List<int> { 0 };
                
                int roll = Main.rand.Next(0, 100);
                if (roll < 89)
                    rarity[Projectile.whoAmI] = Color.Green;
                else if (roll < 98)
                {
                    rarity[Projectile.whoAmI] = Color.Yellow;
                    Projectile.damage = (int)(CalculateTrueDamage(Main.player[Projectile.owner].SorceryFight()) * 1.5);
                }
                else
                {
                    rarity[Projectile.whoAmI] = Color.Red;
                    Projectile.damage = (int)(CalculateTrueDamage(Main.player[Projectile.owner].SorceryFight()) * 2);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            if (enemyRicochets[Projectile.whoAmI][0] < 5)
            {
                float closestDistance = float.MaxValue;
                int closestNPCIndex = -1;
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    NPC npc = Main.npc[i];

                    if (!npc.active || npc.friendly || npc.whoAmI == target.whoAmI || enemyRicochets[Projectile.whoAmI][1..^0].Contains(npc.whoAmI)) continue;

                    float distance = Vector2.Distance(npc.Center, Projectile.Center);
                    if (distance > 250f) continue;

                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestNPCIndex = i;
                    }
                }

                if (closestNPCIndex != -1)
                {
                    NPC targetNPC = Main.npc[closestNPCIndex];
                    Projectile.velocity = Projectile.Center.DirectionTo(targetNPC.Center) * Speed;
                    SoundEngine.PlaySound(SorceryFightSounds.PachinkoBallCollision, Projectile.Center);

                    enemyRicochets[Projectile.whoAmI][0]++;
                    enemyRicochets[Projectile.whoAmI].Add(target.whoAmI);
                }
                else
                    Projectile.Kill();
            }
            else
                Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, rarity[Projectile.whoAmI], Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 2f, SpriteEffects.None, 0f);
            return false;
        }
    }
}