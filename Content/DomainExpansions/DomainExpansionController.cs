using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.DomainExpansions.NPCDomains;
using sorceryFight.Content.DomainExpansions.PlayerDomains;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.DomainExpansions
{
    public class DomainExpansionController : ModSystem
    {
        public enum DomainNetMessage : byte
        {
            ExpandDomain,
            CloseDomain,
            ClashingDomains
        }
        public static class DomainExpansionFactory
        {
            public enum DomainExpansionType : byte
            {
                None,
                UnlimitedVoid,
                MalevolentShrine,
                IdleDeathGamble,
                Home
            }
            public static DomainExpansion Create(DomainExpansionType type)
            {
                return type switch
                {
                    DomainExpansionType.UnlimitedVoid => new UnlimitedVoid(),
                    DomainExpansionType.MalevolentShrine => new MalevolentShrine(),
                    DomainExpansionType.IdleDeathGamble => new IdleDeathGamble(),
                    DomainExpansionType.Home => new Home(),
                    _ => null
                };
            }

            public static DomainExpansionType GetDomainExpansionType(DomainExpansion de)
            {
                if (de is UnlimitedVoid) return DomainExpansionType.UnlimitedVoid;
                if (de is MalevolentShrine) return DomainExpansionType.MalevolentShrine;
                if (de is IdleDeathGamble) return DomainExpansionType.IdleDeathGamble;
                if (de is Home) return DomainExpansionType.Home;

                return DomainExpansionType.None;
            }
        }

        private class PlayerDomainHPTracker : ModPlayer
        {
            public override void OnHurt(Player.HurtInfo info)
            {
                int damage = info.Damage;
                if (DomainExpansions.TryGet(domain => domain is PlayerDomainExpansion && domain.owner == Player.whoAmI, out DomainExpansion de) && damage >= Player.statLifeMax2 * 0.25f)
                {
                    CloseDomain(de.id);
                }
            }
        }

        private class NPCDomainHPTracker : GlobalNPC
        {
            public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.GetDomain() != null && lateInstantiation;

            public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
            {
                OnHit(npc, hit);
            }

            public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
            {
                OnHit(npc, hit);
            }

            private void OnHit(NPC npc, NPC.HitInfo hit)
            {
                int damage = hit.Damage;
                if (DomainExpansions.TryGet(domain => domain is NPCDomainExpansion && domain.owner == npc.whoAmI, out DomainExpansion de) && damage >= npc.lifeMax * 0.05f)
                {
                    CloseDomain(de.id);
                }
            }
        }

        public static DomainExpansion[] DomainExpansions;
        public static List<DomainExpansion> ActiveDomains => [.. DomainExpansions.Where(de => de != null)];
        public static Dictionary<int, int> ClashingDomains;

        public override void Load()
        {
            DomainExpansions = new DomainExpansion[16];
            ClashingDomains = new Dictionary<int, int>();

            IL_Main.DoDraw_DrawNPCsOverTiles += DrawDomainLayer;
        }

        public override void Unload()
        {
            DomainExpansions = null;
            IL_Main.DoDraw_DrawNPCsOverTiles -= DrawDomainLayer;
        }

        public override void OnWorldUnload()
        {
            for (int i = 0; i < DomainExpansions.Length; i++)
            {
                DomainExpansions[i] = null;
            }
        }

        private void DrawDomainLayer(ILContext il)
        {
            if (Main.dedServ) return;
            var cursor = new ILCursor(il);

            cursor.Goto(0);

            cursor.EmitDelegate(() =>
            {

                Main.spriteBatch.Begin(
                    SpriteSortMode.Immediate,
                    BlendState.NonPremultiplied,
                    SamplerState.LinearClamp,
                    DepthStencilState.None,
                    RasterizerState.CullNone,
                    null,
                    Main.GameViewMatrix.ZoomMatrix
                );

                foreach (DomainExpansion de in ActiveDomains)
                {
                    if (de == null) continue;

                    if (de.clashingWith == -1 || !de.ClosedDomain)
                    {
                        de.Draw(Main.spriteBatch);
                    }
                    else if (DomainExpansions[de.clashingWith] != null && !DomainExpansions[de.clashingWith].ClosedDomain)
                    {
                        de.Draw(Main.spriteBatch);
                    }
                    else
                    {
                        de.DrawClashing(Main.spriteBatch);
                    }
                }

                Main.spriteBatch.End();
            });
        }



        public override void PostUpdateNPCs()
        {
            foreach (DomainExpansion de in ActiveDomains)
            {
                de.Update();
            }

            if (ClashingDomains.Count > 0) HandleClashes();
        }

        /// <summary>
        /// Expands the given domain expanasion.
        /// If expanding a player domain in multiplayer, it is called by the caster's client and then synced to the server and other clients.
        /// NPC domains are already synced to all clients in multiplayer.
        /// </summary>
        /// <param name="whoAmI">The caster's whoAmI</param>
        /// <param name="de">The caster's Domain Expansion.</param>
        public static void ExpandDomain(int whoAmI, DomainExpansion de)
        {
            if (de is PlayerDomainExpansion)
            {
                Player caster = Main.player[whoAmI];
                de.center = caster.Center;

                if (Main.netMode == NetmodeID.MultiplayerClient && Main.myPlayer == whoAmI)
                {
                    ModPacket packet = ModContent.GetInstance<SorceryFight>().GetPacket();
                    packet.Write((byte)MessageType.SyncDomain);
                    packet.Write(whoAmI);
                    packet.Write((byte)DomainNetMessage.ExpandDomain);
                    packet.Write((byte)DomainExpansionFactory.GetDomainExpansionType(de));
                    packet.Write(-1);
                    packet.Write(-1);
                    packet.Send();
                }
            }
            else if (de is NPCDomainExpansion)
            {
                NPC caster = Main.npc[whoAmI];
                de.center = caster.Center;
            }

            de.owner = whoAmI;
            SoundEngine.PlaySound(de.CastSound, de.center);

            int id;
            if (de.ClosedDomain && de is not ISimpleDomain)
                id = DomainExpansions.Append(de);
            else
                id = DomainExpansions.Prepend(de);

            DomainExpansions[id].id = id;
            de.OnExpand();

            SetClashingDomains(de);
        }


        /// <summary>
        /// Closes the given domain expanasion.
        /// If closing a player domain in multiplayer, it is called by the caster's client and then synced to the server and other clients.
        /// NPC domains are already synced to all clients in multiplayer.
        /// </summary>
        /// <param name="id">The id of the domain.</param>
        public static void CloseDomain(int id)
        {
            DomainExpansion de = DomainExpansions[id];

            de.OnClose();

            ResetClashingDomains(de);

            if (de is PlayerDomainExpansion)
            {
                if (Main.myPlayer == de.owner)
                {
                    SorceryFightPlayer sf = Main.LocalPlayer.GetModPlayer<SorceryFightPlayer>();
                    sf.disableRegenFromDE = false;

                    if (de is not ISimpleDomain)
                    {
                        sf.AddDeductableDebuff(ModContent.BuffType<BrainDamage>(), SorceryFightPlayer.DefaultBrainDamageDuration);
                    }


                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        ModPacket packet = ModContent.GetInstance<SorceryFight>().GetPacket();
                        packet.Write((byte)MessageType.SyncDomain);
                        packet.Write(de.owner);
                        packet.Write((byte)DomainNetMessage.CloseDomain);
                        packet.Write((byte)1);
                        packet.Write(id);
                        packet.Write(-1);
                        packet.Send();
                    }
                }
            }
            DomainExpansions[id] = null;
        }


        public static void SetClashingDomains(DomainExpansion origin)
        {
            if (origin is ISimpleDomain)
            {
                foreach (DomainExpansion de in ActiveDomains)
                {
                    if (de.id == origin.id) continue;
                    if (de is ISimpleDomain) continue;

                    float distance = Vector2.Distance(origin.center, de.center);
                    if (distance < origin.SureHitRange || distance < de.SureHitRange)
                    {
                        TaskScheduler.Instance.AddDelayedTask(() =>
                        {
                            if (origin != null && ActiveDomains.Contains(origin))
                                CloseDomain(origin.id);

                        }, 60 * (-Math.Abs(origin.Tier - de.Tier) + 7));
                    }
                }
                return;
            }

            List<int> clashingDomains = new List<int>();

            foreach (DomainExpansion de in ActiveDomains)
            {
                if (de.id == origin.id) continue;
                if (de is ISimpleDomain)
                {
                    TaskScheduler.Instance.AddDelayedTask(() =>
                    {
                        if (de != null && ActiveDomains.Contains(de))
                            CloseDomain(de.id);

                    }, 60 * (-Math.Abs(origin.Tier - de.Tier) + 7));
                    continue;
                }

                float distance = Vector2.Distance(origin.center, de.center);
                if (distance < origin.SureHitRange || distance < de.SureHitRange)
                {
                    de.clashingWith = origin.id;
                    if (clashingDomains.Count < 1)
                    {
                        origin.clashingWith = de.id;
                    }
                    clashingDomains.Add(de.id);
                }
            }

            if (clashingDomains.Count > 1)
            {
                TaskScheduler.Instance.AddDelayedTask(() =>
                {
                    foreach (int id in clashingDomains) CloseDomain(id);
                }, 300);
            }

            else if (clashingDomains.Count == 1)
            {
                ClashingDomains[origin.id] = clashingDomains[0];
            }
        }

        public static void ResetClashingDomains(DomainExpansion origin)
        {
            foreach (DomainExpansion de in ActiveDomains)
            {
                float distance = Vector2.Distance(origin.center, de.center);

                if (distance < origin.SureHitRange || distance < de.SureHitRange)
                {
                    de.clashingWith = -1;
                }
            }
        }

        public static void HandleClashes()
        {
            foreach (var item in ClashingDomains)
            {
                if (DomainExpansions[item.Key] == null || DomainExpansions[item.Value] == null)
                {
                    ClashingDomains.Remove(item.Key);
                    continue;
                }

                DomainExpansion originDE = DomainExpansions[item.Key];
                DomainExpansion opposingDomain = DomainExpansions[item.Value];


                int tierDiff = Math.Abs(originDE.Tier - opposingDomain.Tier);

                if (originDE.Tier < opposingDomain.Tier)
                {
                    opposingDomain.health -= (int)SFUtils.RateSecondsToTicks(100 * tierDiff);
                }
                else if (originDE.Tier > opposingDomain.Tier)
                {
                    originDE.health -= (int)SFUtils.RateSecondsToTicks(100 * tierDiff);
                }
                else if (originDE.Tier == opposingDomain.Tier)
                {
                    if (originDE.ClosedDomain && !opposingDomain.ClosedDomain)
                    {
                        originDE.health -= (int)SFUtils.RateSecondsToTicks(75);
                    }
                    else if (!originDE.ClosedDomain && opposingDomain.ClosedDomain)
                    {
                        opposingDomain.health -= (int)SFUtils.RateSecondsToTicks(75);
                    }

                    else if (!originDE.ClosedDomain && !opposingDomain.ClosedDomain)
                    {
                        originDE.health -= (int)SFUtils.RateSecondsToTicks(50);
                        opposingDomain.health -= (int)SFUtils.RateSecondsToTicks(50);
                    }
                }
            }
        }
    }
}
