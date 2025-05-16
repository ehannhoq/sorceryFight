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
            CloseDomain
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



        public static Vector2 previousScreenPosition;
        public static Vector2 targetLerpPosition;
        public static List<DomainExpansion> ActiveDomains = new List<DomainExpansion>();

        public override void PostUpdateNPCs()
        {
            foreach (DomainExpansion de in ActiveDomains)
                if (ActiveDomains.Any(domain => domain.owner == de.owner)) // Safegaurd, as closing domains can throw an exception.
                    de.Update();
        }

        public override void Load()
        {
            IL_Main.DoDraw_DrawNPCsOverTiles += DrawDomainLayer;
        }

        public override void Unload()
        {
            IL_Main.DoDraw_DrawNPCsOverTiles -= DrawDomainLayer;
        }

        public override void OnWorldUnload()
        {
            ActiveDomains.Clear();
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
                    if (ActiveDomains.Any(domain => domain.owner == de.owner)) // Safegaurd, as closing domains can throw an exception.
                        de.Draw(Main.spriteBatch);

                Main.spriteBatch.End();
            });
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
                if (ActiveDomains.Any(domain => domain.owner == whoAmI)) return;

                Player caster = Main.player[whoAmI];
                de.center = caster.Center;
                de.owner = whoAmI;

                SoundEngine.PlaySound(de.CastSound, caster.Center);
                ActiveDomains.Add(de);

                if (Main.netMode == NetmodeID.MultiplayerClient && Main.myPlayer == whoAmI) // Only send packet if the client who casted the domain called this method.
                {
                    ModPacket packet = ModContent.GetInstance<SorceryFight>().GetPacket();
                    packet.Write((byte)MessageType.SyncDomain);
                    packet.Write(whoAmI);
                    packet.Write((byte)DomainExpansionFactory.GetDomainExpansionType(de));
                    packet.Write((byte)DomainNetMessage.ExpandDomain);
                    packet.Send();
                }
            }
            else if (de is NPCDomainExpansion)
            {
                if (ActiveDomains.Any(domain => domain.owner == whoAmI)) return;

                NPC caster = Main.npc[whoAmI];
                de.center = caster.Center;
                de.owner = whoAmI;

                SoundEngine.PlaySound(de.CastSound, caster.Center);
                ActiveDomains.Add(de);
            }
        }


        /// <summary>
        /// Closes the given domain expanasion.
        /// If closing a player domain in multiplayer, it is called by the caster's client and then synced to the server and other clients.
        /// NPC domains are already synced to all clients in multiplayer.
        /// </summary>
        /// <param name="whoAmI">The caster's whoAmI</param>
        /// <param name="de">The caster's Domain Expansion.</param>
        public static void CloseDomain(int whoAmI)
        {
            DomainExpansion de = ActiveDomains.First(domain => domain.owner == whoAmI);
            de.CloseDomain();
            ActiveDomains.Remove(de);

            if (de is PlayerDomainExpansion)
            {
                if (Main.myPlayer == whoAmI)
                {
                    SorceryFightPlayer sf = Main.LocalPlayer.GetModPlayer<SorceryFightPlayer>();
                    sf.AddDeductableDebuff(ModContent.BuffType<BrainDamage>(), SorceryFightPlayer.DefaultBrainDamageDuration);
                    sf.disableRegenFromDE = false;

                    if (Main.netMode == NetmodeID.MultiplayerClient)
                    {
                        ModPacket packet = ModContent.GetInstance<SorceryFight>().GetPacket();
                        packet.Write((byte)MessageType.SyncDomain);
                        packet.Write(whoAmI);
                        packet.Write((byte)DomainExpansionFactory.GetDomainExpansionType(de));
                        packet.Write((byte)DomainNetMessage.CloseDomain);
                        packet.Send();
                    }
                }
            }
        }

        /// <summary>
        /// Used to change the client's camera position when an NPC is casting a domain, or during a domain clash.
        /// </summary>
        public override void ModifyScreenPosition()
        {
            if (NPCDomainController.npcIsCastingDomain)
            {
                if (previousScreenPosition == Vector2.Zero)
                    previousScreenPosition = Main.screenPosition;

                if (targetLerpPosition == Vector2.Zero)
                    targetLerpPosition = Main.screenPosition;


                targetLerpPosition = Vector2.Lerp(
                    targetLerpPosition,
                    NPCDomainController.npcCastingPosition - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2),
                    0.1f
                );

                Main.screenPosition = targetLerpPosition;
            }
            else if (previousScreenPosition != Vector2.Zero)
            {
                Main.screenPosition = previousScreenPosition;
                previousScreenPosition = Vector2.Zero;
                targetLerpPosition = Vector2.Zero;
            }
        }
    }
}
