using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using MonoMod.Cil;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.DomainExpansions
{
    public enum DomainNetMessage : byte
    {
        ExpandDomain,
        CloseDomain
    }

    public class DomainExpansionController : ModSystem
    {
        public static Dictionary<int, DomainExpansion> ActiveDomains = new Dictionary<int, DomainExpansion>();

        public override void PostUpdateNPCs()
        {
            foreach (var item in ActiveDomains)
            {
                var value = item.Value;

                value.Update();
            }
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

                foreach (var domain in ActiveDomains.Values)
                    domain.Draw(Main.spriteBatch);

                Main.spriteBatch.End();
            });
        }

        public static void ActivateDomain(SorceryFightPlayer sfPlayer)
        {
            if (ActiveDomains.ContainsKey(sfPlayer.Player.whoAmI)) return;

            DomainExpansion de = sfPlayer.innateTechnique.DomainExpansion;
            de.center = sfPlayer.Player.Center;
            de.owner = sfPlayer.Player.whoAmI;

            SoundEngine.PlaySound(de.CastSound, sfPlayer.Player.Center);
            ActiveDomains[de.owner] = de;
        }
    }
}
