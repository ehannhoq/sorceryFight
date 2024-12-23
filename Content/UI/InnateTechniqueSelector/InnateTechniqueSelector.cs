using System;
using System.Collections.Generic;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.InnateTechniques;
using Terraria;
using Terraria.Chat;
using Terraria.GameContent.UI.Elements;
using sorceryFight.Content.SFPlayer;
using Terraria.ModLoader;
using Terraria.UI;

namespace sorceryFight.Content.UI.InnateTechniqueSelector
{
    public class InnateTechniqueSelector : UIElement
    {
        private List<Vector2> iconPositions;
        private int timeCounter;
        private bool animate;
        private InnateTechnique selectedTechnique;

        public InnateTechniqueSelector()
        {
            iconPositions = new List<Vector2>();
            timeCounter = 0;
            animate = false;

            Vector2 screenCenter = new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);

            float magnatude = 150f;
            float rotation = 2 * (float)Math.PI / InnateTechnique.InnateTechniques.Count;
            for (int i = 0; i < InnateTechnique.InnateTechniques.Count; i++)
            {
                iconPositions.Add(new Vector2(
                    screenCenter.X + magnatude * (float)Math.Cos(i * rotation),
                    screenCenter.Y + magnatude * (float)Math.Sin(i * rotation)
                ));
            }

            
            UIText title = new UIText("Choose your Innate Technique.", 1.5f, false);
            title.Left.Set(screenCenter.X - 180f, 0f);
            title.Top.Set(screenCenter.Y - 100, 0f);
            Append(title);

            DrawLimitless();

            Recalculate();
        }

        private void DrawLimitless()
        {
            InnateTechnique t = new LimitlessTechnique();
            Texture2D iconTexture = ModContent.Request<Texture2D>($"sorceryFight/Content/UI/InnateTechniqueSelector/{t.Name}_Icon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Texture2D backgroundTexture = ModContent.Request<Texture2D>($"sorceryFight/Content/UI/InnateTechniqueSelector/{t.Name}_BG", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                
            SpecialUIElement background = new SpecialUIElement(backgroundTexture, -1f, 0.05f);
            background.Left.Set(iconPositions[0].X - (backgroundTexture.Width / 2), 0f);
            background.Top.Set(iconPositions[0].Y - (backgroundTexture.Height / 2), 0f);
                
            TechnqiueButton button = new TechnqiueButton(iconTexture, t.Name, t);
            button.Left.Set(iconPositions[0].X - (iconTexture.Width / 2), 0f);
            button.Top.Set(iconPositions[0].Y - (iconTexture.Height / 2), 0f);

            background.Recalculate();
            button.Recalculate();

            Append(background);
            Append(button);
        }

        public override void Update(GameTime gameTime)
        {
            if (animate && Elements.Count != 0)
                Elements.Clear();

            base.Update(gameTime);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            if (animate)
            {
                timeCounter++;

                Vector2 pos = Main.LocalPlayer.Center;

                if (timeCounter > 305)
                {
                    var player = Main.LocalPlayer;
                    player.GetModPlayer<SorceryFightPlayer>().innateTechnique = selectedTechnique;
                    ChatHelper.SendChatMessageToClient(SFUtils.GetNetworkText($"Mods.sorceryFight.Misc.InnateTechniqueUnlocker.{selectedTechnique.Name}"), Color.Aqua, player.whoAmI);
                    RemoveSelf();
                    animate = false;
                    return;
                }

                int particleCount = timeCounter / 120 + 1;

                if (timeCounter <= 300)
                {
                    for (int i = 0; i < particleCount; i++)
                    {
                        Vector2 offsetPos = pos + new Vector2(Main.rand.NextFloat(-100, 100), Main.rand.NextFloat(-100, 100));
                        Vector2 vel = offsetPos.DirectionTo(pos) * 2;

                        SparkleParticle particle = new SparkleParticle(offsetPos, vel, Color.Wheat, Color.White, 0.5f, 35);
                        GeneralParticleHandler.SpawnParticle(particle);
                    }
                }
                else
                {
                    for (int i = 0; i < 20; i++)
                    {
                        Vector2 targetPos = pos + new Vector2(Main.rand.NextFloat(-100, 100), Main.rand.NextFloat(-100, 100));
                        Vector2 vel = pos.DirectionTo(targetPos) * 5;

                        SparkleParticle particle = new SparkleParticle(pos, vel, Color.Wheat, Color.White, 0.5f, 60);
                        GeneralParticleHandler.SpawnParticle(particle);
                    }
                }
            }
        }
        public void OnClick(InnateTechnique selectedTechnique)
        {
            this.selectedTechnique = selectedTechnique;
            animate = true;
        }

        private void RemoveSelf()
        {
            SorceryFightUI sfUI = (SorceryFightUI)Parent;
            sfUI.RemoveElement(this);
        }
    }
}
