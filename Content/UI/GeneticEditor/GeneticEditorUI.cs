using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.CalClone;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Graphics;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace sorceryFight.Content.UI.GeneticEditor
{
    public class GeneticEditorUI : UIElement
    {
        UITextField bossesCountInput;
        bool rctState;

        List<int> bosses => new([
            NPCID.KingSlime,
            ModContent.NPCType<DesertScourgeHead>(),
            NPCID.EyeofCthulhu,
            ModContent.NPCType<Crabulon>(),
            NPCID.EaterofWorldsHead,
            NPCID.BrainofCthulhu,
            ModContent.NPCType<HiveMind>(),
            ModContent.NPCType<PerforatorHive>(),
            NPCID.QueenBee,
            NPCID.SkeletronHead,
            NPCID.Deerclops,
            ModContent.NPCType<EbonianPaladin>(),
            ModContent.NPCType<CrimulanPaladin>(),
            NPCID.WallofFlesh,
            ModContent.NPCType<Cryogen>(),
            NPCID.QueenSlimeBoss,
            ModContent.NPCType<AquaticScourgeHead>(),
            NPCID.Retinazer,
            NPCID.Spazmatism,
            NPCID.TheDestroyer,
            NPCID.SkeletronPrime,
            ModContent.NPCType<BrimstoneElemental>(),
            ModContent.NPCType<CalamitasClone>(),
            NPCID.Plantera,
            ModContent.NPCType<Leviathan>(),
            ModContent.NPCType<Anahita>(),
            NPCID.Golem,
            ModContent.NPCType<AstrumAureus>(),
            ModContent.NPCType<PlaguebringerGoliath>(),
            ModContent.NPCType<RavagerHead>(),
            NPCID.DukeFishron,
            NPCID.HallowBoss,
            NPCID.CultistBoss,
            ModContent.NPCType<AstrumDeusHead>(),
            NPCID.MoonLordCore,
            ModContent.NPCType<Dragonfolly>(),
            ModContent.NPCType<ProfanedGuardianCommander>(),
            ModContent.NPCType<ProfanedGuardianHealer>(),
            ModContent.NPCType<ProfanedGuardianDefender>(),
            ModContent.NPCType<Providence>(),
            ModContent.NPCType<StormWeaverHead>(),
            ModContent.NPCType<CeaselessVoid>(),
            ModContent.NPCType<Signus>(),
            ModContent.NPCType<Polterghast>(),
            ModContent.NPCType<OldDuke>(),
            ModContent.NPCType<DevourerofGodsHead>(),
            ModContent.NPCType<Yharon>(),
            ModContent.NPCType<ThanatosHead>(),
            ModContent.NPCType<Artemis>(),
            ModContent.NPCType<AresBody>(),
            ModContent.NPCType<Apollo>(),
            ModContent.NPCType<SupremeCalamitas>()
        ]);

        public GeneticEditorUI()
        {
            Width.Set(1200f, 0f);
            Height.Set(400f, 0f);
            Top.Set(Main.screenHeight / Main.UIScale / 2 - Height.Pixels / 2, 0f);
            Left.Set(Main.screenWidth / Main.UIScale / 2 - Width.Pixels / 2, 0f);

            BuildManualBossSide();
            BuildPresetSide();
        }


        private void BuildManualBossSide()
        {
            bossesCountInput = new UITextField($"Enter number of bosses killed [0 - {bosses.Count}]");
            bossesCountInput.Width.Set(325f, 0f);
            bossesCountInput.Height.Set(25f, 0f);

            UIText rctLabel = new UIText("Unlocked RCT?");
            rctLabel.Top.Set(4f, 0f);

            UIToggleImage rctButton = new UIToggleImage(Main.Assets.Request<Texture2D>("Images/UI/Settings_Toggle"), 13, 13, new Point(17, 1), new Point(1, 1));
            rctButton.SetState(Main.LocalPlayer.SorceryFight().unlockedRCT);
            rctState = rctButton.IsOn;
            rctButton.OnLeftClick += (evt, el) =>
            {
                rctState = ((UIToggleImage)el).IsOn;
            };

            float labelWidth = rctLabel.MinWidth.Pixels;
            float spacing = 8f;

            UIElement rctGroup = new UIElement();
            rctGroup.Width.Set(labelWidth + spacing + rctButton.Width.Pixels, 0f);
            rctGroup.Height.Set(25f, 0f);
            rctGroup.Left.Set(bossesCountInput.Width.Pixels + 10f, 0f);

            rctButton.Left.Set(labelWidth + spacing, 0f);
            rctButton.Top.Set(rctGroup.Height.Pixels / 2f - rctButton.Height.Pixels / 2f, 0f);

            rctGroup.Append(rctLabel);
            rctGroup.Append(rctButton);

            float rowWidth = bossesCountInput.Width.Pixels + 10f + rctGroup.Width.Pixels;

            UIButton<string> submit = new UIButton<string>("Submit.");
            submit.Width.Set(rowWidth, 0f);
            submit.Height.Set(50f, 0f);
            submit.Top.Set(bossesCountInput.Height.Pixels + 10f, 0f);
            submit.OnLeftClick += OnSubmit;

            UIElement manualBossSide = new UIElement();
            manualBossSide.Width.Set(rowWidth, 0f);
            manualBossSide.Height.Set(bossesCountInput.Height.Pixels + submit.Height.Pixels + 10f, 0f);
            manualBossSide.Top.Set(Height.Pixels / 2f - manualBossSide.Height.Pixels / 2f, 0f);
            manualBossSide.Left.Set(Width.Pixels / 4f - manualBossSide.Width.Pixels / 2f, 0f);

            manualBossSide.Append(bossesCountInput);
            manualBossSide.Append(rctGroup);
            manualBossSide.Append(submit);

            Append(manualBossSide);
        }

        private void BuildPresetSide()
        {
            List<UIButton<string>> buttons = new();
            var preBossButton = new UIButton<string>("Pre-Boss");
            preBossButton.OnLeftClick += (UIMouseEvent evt, UIElement listeningElement) => OnPresetSubmit(0);
            buttons.Add(preBossButton);
            var postSkeletronButton = new UIButton<string>("Post-Skeletron");
            postSkeletronButton.OnLeftClick += (UIMouseEvent evt, UIElement listeningElement) => OnPresetSubmit(10);
            buttons.Add(postSkeletronButton);
            var postWoFButton = new UIButton<string>("Post-Wall of Flesh");
            postWoFButton.OnLeftClick += (UIMouseEvent evt, UIElement listeningElement) => OnPresetSubmit(14);
            buttons.Add(postWoFButton);
            var postMechButton = new UIButton<string>("Post-Mechanical Bosses");
            postMechButton.OnLeftClick += (UIMouseEvent evt, UIElement listeningElement) => OnPresetSubmit(21);
            buttons.Add(postMechButton);
            var postPlanteraButton = new UIButton<string>("Post-Plantera");
            postPlanteraButton.OnLeftClick += (UIMouseEvent evt, UIElement listeningElement) => OnPresetSubmit(24);
            buttons.Add(postPlanteraButton);
            var postGolemButton = new UIButton<string>("Post-Golem");
            postGolemButton.OnLeftClick += (UIMouseEvent evt, UIElement listeningElement) => OnPresetSubmit(27);
            buttons.Add(postGolemButton);
            var postMLButton = new UIButton<string>("Post-Moon Lord");
            postMLButton.OnLeftClick += (UIMouseEvent evt, UIElement listeningElement) => OnPresetSubmit(36);
            buttons.Add(postMLButton);
            var postProvidenceButton = new UIButton<string>("Post-Providence");
            postProvidenceButton.OnLeftClick += (UIMouseEvent evt, UIElement listeningElement) => OnPresetSubmit(41);
            buttons.Add(postProvidenceButton);
            var postDoGButton = new UIButton<string>("Post-Devourer Of Gods");
            postDoGButton.OnLeftClick += (UIMouseEvent evt, UIElement listeningElement) => OnPresetSubmit(47);
            buttons.Add(postDoGButton);
            var postSCalButton = new UIButton<string>("Post-Supreme Calamitas");
            postSCalButton.OnLeftClick += (UIMouseEvent evt, UIElement listeningElement) => OnPresetSubmit(bosses.Count);
            buttons.Add(postSCalButton);


            UIElement presetSide = new();
            presetSide.Width.Set(350f, 0f);
            presetSide.Height.Set(Height.Pixels - 20, 0f);
            presetSide.Top.Set(Height.Pixels / 2f - presetSide.Height.Pixels / 2f, 0f);
            presetSide.Left.Set(3 * Width.Pixels / 4f - presetSide.Width.Pixels / 2f, 0f);

            for (int i = 0; i < buttons.Count; i++)
            {
                UIButton<string> button = buttons[i];
                button.Width.Set(330f, 0);
                button.Height.Set((Height.Pixels - 20 - (10 * (buttons.Count - 1))) / buttons.Count, 0f);
                button.Top.Set((button.Height.Pixels + 10) * i, 0f);
                button.Left.Set(presetSide.Width.Pixels / 2f - button.Width.Pixels / 2f, 0f);
                presetSide.Append(button);
            }

            Append(presetSide);
        }

        private void OnSubmit(UIMouseEvent evt, UIElement listeningElement)
        {
            SorceryFightPlayer sfPlayer = Main.LocalPlayer.SorceryFight();

            if (int.TryParse(bossesCountInput.Text, out int bossesCount))
            {
                if (bossesCount < 0 || bossesCount > bosses.Count) return;

                List<int> trimmedList = bosses[..bossesCount];
                sfPlayer.bossesDefeated = trimmedList.ToHashSet();
            }

            sfPlayer.unlockedRCT = rctState;
            SorceryFightUI.UpdateTechniqueUI?.Invoke();

            TextInputEXT.TextInput -= bossesCountInput.OnTextInput;
            SorceryFightUI sfUI = (SorceryFightUI)Parent;
            sfUI.RemoveElement(this);
        }

        private void OnPresetSubmit(int i)
        {
            List<int> trimmedList = bosses[..i];
            SorceryFightPlayer sfPlayer = Main.LocalPlayer.SorceryFight();

            sfPlayer.bossesDefeated = trimmedList.ToHashSet();
            sfPlayer.unlockedRCT = i >= 36;
            SorceryFightUI.UpdateTechniqueUI?.Invoke();

            SorceryFightUI sfUI = (SorceryFightUI)Parent;
            TextInputEXT.TextInput -= bossesCountInput.OnTextInput;
            sfUI.RemoveElement(this);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                TextureAssets.MagicPixel.Value,
                new Rectangle((int)Left.Pixels, (int)Top.Pixels, (int)Width.Pixels, (int)Height.Pixels),
                new Color(22, 44, 77, 170)
            );
        }
    }

    public class UITextField : UIElement
    {
        public string Text = "";
        public string Hint;
        public bool Focused = false;

        private int _cursorTimer = 0;
        private bool _wasMouseDownLastFrame = false;

        public UITextField(string hint = "")
        {
            Hint = hint;
            TextInputEXT.TextInput += OnTextInput;
        }

        public override void LeftClick(UIMouseEvent evt)
        {
            Focused = true;
            PlayerInput.WritingText = true;
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!Focused) return;

            PlayerInput.WritingText = true;
            Main.instance.HandleIME();

            bool mouseDown = Main.mouseLeft;
            if (!ContainsPoint(new Vector2(Main.mouseX, Main.mouseY)) && mouseDown && !_wasMouseDownLastFrame)
            {
                Focused = false;
                PlayerInput.WritingText = false;
            }
            _wasMouseDownLastFrame = mouseDown;

            _cursorTimer++;
            if (_cursorTimer > 60) _cursorTimer = 0;
        }

        public void OnTextInput(char c)
        {
            if (!Focused) return;

            if (c == '\b')
            {
                if (Text.Length > 0)
                    Text = Text[..^1];
            }
            else if (c == '\r')
            {
                Focused = false;
                PlayerInput.WritingText = false;
            }
            else
            {
                Text += c;
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dims = GetDimensions();

            spriteBatch.Draw(TextureAssets.MagicPixel.Value, dims.ToRectangle(), Focused ? new Color(80, 80, 80) : new Color(50, 50, 50));

            DynamicSpriteFont font = FontAssets.MouseText.Value;
            string display = Text.Length == 0 && !Focused ? Hint : Text;
            Color textColor = Text.Length == 0 && !Focused ? Color.Gray : Color.White;

            if (Focused && _cursorTimer < 30)
                display += "|";

            spriteBatch.DrawString(font, display, new Vector2(dims.X + 4, dims.Y + 4), textColor);
        }
    }
}