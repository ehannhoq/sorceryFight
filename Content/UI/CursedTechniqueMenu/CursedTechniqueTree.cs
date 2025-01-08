using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.CursedTechniques.Limitless;
using sorceryFight.Content.InnateTechniques;
using sorceryFight.Content.UI.InnateTechniqueSelector;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace sorceryFight.Content.UI.CursedTechniqueMenu
{
    public class CursedTechniqueTree : UIElement
    {
        public Vector2 center;
        List<TechniqueIcon> techniqueIcons;
        public CursedTechniqueTree(Texture2D closeButtonTexture, Texture2D backgroundTexture)
        {
            SorceryFightPlayer player = Main.LocalPlayer.GetModPlayer<SorceryFightPlayer>();
            techniqueIcons = new List<TechniqueIcon>();

            UIImage background = new UIImage(backgroundTexture);
            background.Left.Set(18f, 0f);
            background.Top.Set(closeButtonTexture.Height + 24f, 0f);
            Append(background);

            center = new Vector2(18f + backgroundTexture.Width / 2, closeButtonTexture.Height + 24f + backgroundTexture.Height / 2);

            Texture2D centerIconBackgroundTexture = ModContent.Request<Texture2D>($"sorceryFight/Content/UI/InnateTechniqueSelector/{player.innateTechnique.Name}_BG", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Texture2D centerIconTexture = ModContent.Request<Texture2D>($"sorceryFight/Content/UI/InnateTechniqueSelector/{player.innateTechnique.Name}_Icon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            SpecialUIElement centerIconBG = new SpecialUIElement(centerIconBackgroundTexture, default, -1f, 0.05f, 0.75f);
            centerIconBG.Left.Set(center.X - centerIconBackgroundTexture.Width / 2, 0f);
            centerIconBG.Top.Set(center.Y - centerIconBackgroundTexture.Height / 2, 0f);

            SpecialUIElement centerIcon = new SpecialUIElement(centerIconTexture, player.innateTechnique.DisplayName, default, default, 0.75f);
            centerIcon.Left.Set(center.X - centerIconTexture.Width / 2, 0f);
            centerIcon.Top.Set(center.Y - centerIconTexture.Height / 2, 0f);

            Append(centerIconBG);
            Append(centerIcon);

            /**
            Why did I choose to draw each individual innate technique tree line by line?
            Trust me, I tried so hard to make it dynamic, but I realized that it would:
            1. Take a lot of work
            2. Require me to rewrite a lot of code.
            So, if you are seeing this, and want to take a crack at making it dynamic, go ahead!
            - ehann.
            **/

            switch (player.innateTechnique.Name)
            {
                case "Limitless":
                    DrawLimitless(center, player);
                    break;
                case "Shrine":
                    DrawShrine(center, player);
                    break;
            }

        }

        void DrawLimitless(Vector2 center, SorceryFightPlayer player)
        {
            List<CursedTechnique> cursedTechniques = player.innateTechnique.CursedTechniques;
            List<PassiveTechnique> passiveTechniques = player.innateTechnique.PassiveTechniques;

            float iconSize = 30;
            int originIconCount = 4;
            float distance = 80f;
            Vector2[] originPositions = OriginPositionHelper(iconSize, originIconCount, distance, 1f / 3f);

            List<TechniqueIcon> ctIcons = new List<TechniqueIcon>();
            List<TechniqueIcon> ptIcons = new List<TechniqueIcon>();

            for (int i = 0; i < cursedTechniques.Count; i++)
            {
                CursedTechnique ct = cursedTechniques[i];
                string texturePath = $"sorceryFight/Content/UI/CursedTechniqueMenu/Limitless/c{i}";
                Texture2D texture = ModContent.Request<Texture2D>(texturePath, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

                bool unlocked = ct.Unlocked(player);
                string hoverText = unlocked ? $"{ct.DisplayName}\n{ct.GetStats(player)}\n{ct.Description}" : $"{ct.LockedDescription}";

                TechniqueIcon icon = new TechniqueIcon(texture, unlocked, hoverText);
                ctIcons.Add(icon);
            }

            for (int i = 0; i < passiveTechniques.Count; i++)
            {
                PassiveTechnique pt = passiveTechniques[i];
                string texturePath = $"sorceryFight/Content/UI/CursedTechniqueMenu/Limitless/p{i}";
                Texture2D texture = ModContent.Request<Texture2D>(texturePath, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

                bool unlocked = pt.Unlocked(player);
                string hoverText = unlocked ? $"{pt.DisplayName}\n{pt.Stats}\n{pt.Description}" : $"{pt.LockedDescription}";

                TechniqueIcon icon = new TechniqueIcon(texture, unlocked, hoverText);
                ptIcons.Add(icon);
            }


            Span<TechniqueIcon> ctIconsSpan = CollectionsMarshal.AsSpan(ctIcons);
            Span<TechniqueIcon> ptIconsSpan = CollectionsMarshal.AsSpan(ptIcons);

            ref TechniqueIcon amplifiedBlueIcon = ref ctIconsSpan[0];
            ref TechniqueIcon maximumBlueIcon = ref ctIconsSpan[1];
            ref TechniqueIcon reversalRedIcon = ref ctIconsSpan[2];
            ref TechniqueIcon hollowPurpleIcon = ref ctIconsSpan[3];
            ref TechniqueIcon hollowPurple200Icon = ref ctIconsSpan[4];

            ref TechniqueIcon infinityIcon = ref ptIconsSpan[0];
            ref TechniqueIcon amplifiedAuraIcon = ref ptIconsSpan[1];
            ref TechniqueIcon maximumAuraIcon = ref ptIconsSpan[2];


            ref Vector2 amplifiedBluePos = ref originPositions[0];
            ref Vector2 reversalRedPos = ref originPositions[1];
            ref Vector2 infinityPos = ref originPositions[2];
            ref Vector2 amplifiedAuraPos = ref originPositions[3];


            infinityIcon.Left.Set(infinityPos.X, 0f);
            infinityIcon.Top.Set(infinityPos.Y, 0f);

            amplifiedAuraIcon.Left.Set(amplifiedAuraPos.X, 0f);
            amplifiedAuraIcon.Top.Set(amplifiedAuraPos.Y, 0f);

            amplifiedBlueIcon.Left.Set(amplifiedBluePos.X, 0f);
            amplifiedBlueIcon.Top.Set(amplifiedBluePos.Y, 0f);

            reversalRedIcon.Left.Set(reversalRedPos.X, 0f);
            reversalRedIcon.Top.Set(reversalRedPos.Y, 0f);


            float vectorRotation = 1f / 3f;

            // Maximum Blue
            Vector2 maximumBluePos = new Vector2(amplifiedBluePos.X + iconSize / 2, amplifiedBluePos.Y + iconSize / 2);
            maximumBluePos = maximumBluePos.DirectionFrom(center);
            maximumBluePos = maximumBluePos.RotatedBy(vectorRotation);
            maximumBluePos.Normalize();
            maximumBluePos *= distance * 2;
            maximumBlueIcon.Left.Set(maximumBluePos.X + center.X - iconSize / 2, 0f);
            maximumBlueIcon.Top.Set(maximumBluePos.Y + center.Y - iconSize / 2, 0f);

            // Maximum Aura
            Vector2 maximumAuraPos = new Vector2(amplifiedAuraPos.X + iconSize / 2, amplifiedAuraPos.Y + iconSize / 2);
            maximumAuraPos = maximumAuraPos.DirectionFrom(center);
            maximumAuraPos = maximumAuraPos.RotatedBy(vectorRotation);
            maximumAuraPos.Normalize();
            maximumAuraPos *= distance * 2;
            maximumAuraIcon.Left.Set(maximumAuraPos.X + center.X - iconSize / 2, 0f);
            maximumAuraIcon.Top.Set(maximumAuraPos.Y + center.Y - iconSize / 2, 0f);

            // Hollow Purple
            Vector2 hollowPurplePos = new Vector2(reversalRedPos.X + iconSize / 2, reversalRedPos.Y + iconSize / 2);
            hollowPurplePos = hollowPurplePos.DirectionFrom(center);
            hollowPurplePos = hollowPurplePos.RotatedBy(vectorRotation);
            hollowPurplePos.Normalize();
            hollowPurplePos *= distance * 2;
            hollowPurpleIcon.Left.Set(hollowPurplePos.X + center.X - iconSize / 2, 0f);
            hollowPurpleIcon.Top.Set(hollowPurplePos.Y + center.Y - iconSize / 2, 0f);

            // 200 % Hollow Purple
            Vector2 hollowPurple200Pos = new Vector2(reversalRedPos.X + iconSize / 2, reversalRedPos.Y + iconSize / 2);
            hollowPurple200Pos = hollowPurple200Pos.DirectionFrom(center);
            hollowPurple200Pos = hollowPurple200Pos.RotatedBy(vectorRotation * 2);
            hollowPurple200Pos.Normalize();
            hollowPurple200Pos *= distance * 3f;
            hollowPurple200Icon.Left.Set(hollowPurple200Pos.X + center.X - iconSize / 2, 0f);
            hollowPurple200Icon.Top.Set(hollowPurple200Pos.Y + center.Y - iconSize / 2, 0f);

            maximumAuraIcon.parents.Add(amplifiedAuraIcon);
            maximumBlueIcon.parents.Add(amplifiedBlueIcon);
            hollowPurpleIcon.parents.AddRange([reversalRedIcon, maximumBlueIcon]);
            hollowPurple200Icon.parents.Add(hollowPurpleIcon);

            foreach (TechniqueIcon icon in ctIcons)
            {
                Append(icon);
                techniqueIcons.Add(icon);
            }

            foreach (TechniqueIcon icon in ptIcons)
            {
                Append(icon);
                techniqueIcons.Add(icon);
            }
        }

        void DrawShrine(Vector2 center, SorceryFightPlayer player)
        {

        }


        Vector2[] OriginPositionHelper(float iconSize, int n, float distanceFromCenter, float rotationOffset = 0f)
        {
            Vector2[] originPositions = new Vector2[n];
            float rotation = 2 * (float)Math.PI / n;
            for (int i = 0; i < n; i++)
            {
                float x = MathF.Cos(i * rotation + rotationOffset) * distanceFromCenter;
                float y = MathF.Sin(i * rotation + rotationOffset) * distanceFromCenter;
                originPositions[i] = new Vector2(x - iconSize / 2, y - iconSize / 2) + center;
            }
            return originPositions;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (techniqueIcons == null || techniqueIcons.Count == 0)
                return;

            foreach (TechniqueIcon icon in techniqueIcons)
            {
                icon.DrawLines();
            }

            foreach (TechniqueIcon icon in techniqueIcons)
            {
                icon.DrawIcon();
            }
        }
    }
}