using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Cutscenes
{
    public class CutsceneManager : ModSystem
    {
        public static Queue<Cutscene> cutscenes = new();

        public Cutscene ActiveCutscene { get; private set; }

        public override void Load()
        {
            On_Main.DrawPlayers_BehindNPCs += DrawBehindNPCS;
        }

        public override void Unload()
        {
            On_Main.DrawPlayers_BehindNPCs -= DrawBehindNPCS;
        }

        private void DrawBehindNPCS(On_Main.orig_DrawPlayers_BehindNPCs orig, Main self)
        {
            orig(self);
            Main.spriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.NonPremultiplied,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                null,
                Main.GameViewMatrix.ZoomMatrix
            );
            ActiveCutscene?.DrawBehindNPCs(Main.spriteBatch);
            Main.spriteBatch.End();
        }

        public override void PostUpdateEverything()
        {
            if (Main.dedServ) return;

            if (ActiveCutscene != null)
            {
                ActiveCutscene.Update();
                ActiveCutscene.Timer++;

                if (ActiveCutscene.Timer >= ActiveCutscene.CutsceneLength)
                {
                    ActiveCutscene.OnEnd();
                    ActiveCutscene.Timer = 0;
                    ActiveCutscene = null;
                }
            }
            else
            {
                if (cutscenes.TryDequeue(out Cutscene result))
                {
                    ActiveCutscene = result;
                    ActiveCutscene.OnStart();
                }
            }
        }

        public static void QueueCutscene(Cutscene cutscene)
        {
            cutscenes.Enqueue(cutscene);
        }

        public static void DisablePlayerInput(bool status)
        {
            Main.LocalPlayer.GetModPlayer<DisableInputPlayer>().disableInput = status;
        }

        public static void FreezeNPC(NPC npc, bool status) {
            npc.GetGlobalNPC<CutsceneFreezeNPC>().freeze = status;
        }

        public class DisableInputPlayer : ModPlayer
        {
            public bool disableInput = false;

            public override void SetControls()
            {
                if (!disableInput)
                    return;

                Player.controlUp = false;
                Player.controlDown = false;
                Player.controlLeft = false;
                Player.controlRight = false;
                Player.controlJump = false;
                Player.controlUseItem = false;
                Player.controlUseTile = false;
                Player.controlThrow = false;
                Player.controlInv = false;
                Player.controlSmart = false;
                Player.controlHook = false;
                Player.controlMount = false;
                Player.controlTorch = false;
                Player.controlQuickHeal = false;
                Player.controlQuickMana = false;
            }
        }

        public class CutsceneFreezeNPC : GlobalNPC
        {
            public bool freeze = false;
            public override bool InstancePerEntity => true;
            public override bool PreAI(NPC npc)
            {
                if (freeze)
                {
                    npc.velocity = Vector2.Zero;
                    npc.dontTakeDamage = true;
                    return false;
                }

                return true;
            }

            public override void FindFrame(NPC npc, int frameHeight)
            {
                if (freeze) {}
            }
        }
    }
}
