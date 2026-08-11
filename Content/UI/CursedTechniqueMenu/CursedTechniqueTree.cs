using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.SFPlayer;
using sorceryFight.Content.InnateTechniques;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace sorceryFight.Content.UI.CursedTechniqueMenu
{
    public class CursedTechniqueTree : UIElement
    {
        private const float ICON_SIZE = 60f;

        public Vector2 center;
        List<TechniqueIcon> techniqueIcons;
        public CursedTechniqueTree(Texture2D closeButtonTexture, Texture2D backgroundTexture)
        {
            if (Main.dedServ) return;

            SorceryFightPlayer sfPlayer = Main.LocalPlayer.SorceryFight();
            techniqueIcons = new List<TechniqueIcon>();

            UIImage background = new UIImage(backgroundTexture);
            background.Left.Set(18f, 0f);
            background.Top.Set(closeButtonTexture.Height + 24f, 0f);
            Append(background);

            center = new Vector2(18f + backgroundTexture.Width / 2, closeButtonTexture.Height + 24f + backgroundTexture.Height / 2);

            Texture2D centerIconBackgroundTexture = ModContent.Request<Texture2D>($"sorceryFight/Content/UI/InnateTechniqueSelector/{sfPlayer.innateTechnique.InternalName}_BG", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Texture2D centerIconTexture = ModContent.Request<Texture2D>($"sorceryFight/Content/UI/InnateTechniqueSelector/{sfPlayer.innateTechnique.InternalName}_Icon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            SpecialUIElement centerIconBG = new SpecialUIElement(centerIconBackgroundTexture, default, -1f, 0.05f, 1.5f);
            centerIconBG.Left.Set(center.X - centerIconBackgroundTexture.Width / 2, 0f);
            centerIconBG.Top.Set(center.Y - centerIconBackgroundTexture.Height / 2, 0f);

            SpecialUIElement centerIcon = new SpecialUIElement(centerIconTexture, sfPlayer.innateTechnique.DisplayName, default, default, 1.0f);
            centerIcon.Left.Set(center.X - centerIconTexture.Width / 2, 0f);
            centerIcon.Top.Set(center.Y - centerIconTexture.Height / 2, 0f);

            Append(centerIconBG);
            Append(centerIcon);

            BuildTechniqueIcons(sfPlayer);

            /**
            Why did I choose to draw each individual innate technique tree line by line?
            Trust me, I tried so hard to make it dynamic, but I realized that it would:
            1. Take a lot of work
            2. Require me to rewrite a lot of code.
            So, if you are seeing this, and want to take a crack at making it dynamic, go ahead!
            - ehann.
            **/

            switch (sfPlayer.innateTechnique.InternalName)
            {
                case "Limitless":
                    DrawLimitless(center, sfPlayer);
                    break;
                case "Shrine":
                    DrawShrine(center, sfPlayer);
                    break;
                case "Vessel":
                    DrawVessel(center, sfPlayer);
                    break;
                case "BloodManipulation":
                    DrawBloodManipulation(center, sfPlayer);
                    break;
                case "StarRage":
                    DrawStarRage(center, sfPlayer);
                    break;
                case "IceFormation":
                    DrawIceFormation(center, sfPlayer);
                    break;
                case "TenShadows":
                    DrawTenShadows(center, sfPlayer);
                    break;
                case "PrivatePureLoveTrain":
                    DrawPPLT(center, sfPlayer);
                    break;
                case "HeavenlyRestriction":
                    DrawHeavenlyRestriction(center, sfPlayer);
                    break;
            }
        }

        private void BuildTechniqueIcons(SorceryFightPlayer sfPlayer)
        {
            foreach (CursedTechnique ct in sfPlayer.innateTechnique.CursedTechniques)
            {
                TechniqueIcon icon = new TechniqueIcon(
                    texture: ModContent.Request<Texture2D>($"sorceryFight/Content/UI/TechniqueIcons/{sfPlayer.innateTechnique.InternalName}/{ct.InternalName}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                    unlocked: ct.IsUnlocked(sfPlayer),
                    hoverText: ct.IsUnlocked(sfPlayer) ? $"{ct.DisplayName}\n{ct.GetStats(sfPlayer)}\n{ct.Description}" : $"{ct.GetUnlockRequirement()}"
                );
                techniqueIcons.Add(icon);
                Append(icon);
            }

            foreach (PassiveTechnique pt in sfPlayer.innateTechnique.PassiveTechniques)
            {
                TechniqueIcon icon = new TechniqueIcon(
                    texture: ModContent.Request<Texture2D>($"sorceryFight/Content/UI/TechniqueIcons/{sfPlayer.innateTechnique.InternalName}/{pt.InternalName}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                    unlocked: pt.IsUnlocked(sfPlayer),
                    hoverText: pt.IsUnlocked(sfPlayer) ? $"{pt.DisplayName}\n{pt.GetStats(sfPlayer)}\n{pt.Description}" : $"{pt.GetUnlockRequirement()}"
                );
                techniqueIcons.Add(icon);
                Append(icon);
            }
        }

        private List<TechniqueIcon> GetCursedTechniqueIcons(SorceryFightPlayer sfPlayer)
        {
            return techniqueIcons.GetRange(0, sfPlayer.innateTechnique.CursedTechniques.Count);
        }

        private List<TechniqueIcon> GetPassiveTechniqueIcons(SorceryFightPlayer sfPlayer)
        {
            return techniqueIcons.GetRange(sfPlayer.innateTechnique.CursedTechniques.Count, sfPlayer.innateTechnique.PassiveTechniques.Count);
        }

        private void DrawLimitless(Vector2 center, SorceryFightPlayer sfPlayer)
        {
            float distance = 100f;
            var originPositions = OriginPositionHelper(iconSize: 60, n: 5, distanceFromCenter: distance, rotationOffset: 1f / 3f);

            Span<TechniqueIcon> ctIconsSpan = CollectionsMarshal.AsSpan(GetCursedTechniqueIcons(sfPlayer));
            Span<TechniqueIcon> ptIconsSpan = CollectionsMarshal.AsSpan(GetPassiveTechniqueIcons(sfPlayer));

            ref TechniqueIcon amplifiedBlueIcon = ref ctIconsSpan[0];
            ref TechniqueIcon maximumBlueIcon = ref ctIconsSpan[1];
            ref TechniqueIcon reversalRedIcon = ref ctIconsSpan[2];
            ref TechniqueIcon maximumRedIcon = ref ctIconsSpan[3];
            ref TechniqueIcon hollowPurpleIcon = ref ctIconsSpan[4];
            ref TechniqueIcon hollowPurple200Icon = ref ctIconsSpan[5];

            ref TechniqueIcon infinityIcon = ref ptIconsSpan[0];
            ref TechniqueIcon amplifiedAuraIcon = ref ptIconsSpan[1];
            ref TechniqueIcon maximumAuraIcon = ref ptIconsSpan[2];
            ref TechniqueIcon fallingBlossomEmotionIcon = ref ptIconsSpan[3];


            ref Vector2 amplifiedBluePos = ref originPositions[0];
            ref Vector2 hollowPurplePos = ref originPositions[1];
            ref Vector2 reversalRedPos = ref originPositions[2];
            ref Vector2 infinityPos = ref originPositions[3];
            ref Vector2 amplifiedAuraPos = ref originPositions[4];


            infinityIcon.Left.Set(infinityPos.X, 0f);
            infinityIcon.Top.Set(infinityPos.Y, 0f);

            amplifiedAuraIcon.Left.Set(amplifiedAuraPos.X, 0f);
            amplifiedAuraIcon.Top.Set(amplifiedAuraPos.Y, 0f);

            amplifiedBlueIcon.Left.Set(amplifiedBluePos.X, 0f);
            amplifiedBlueIcon.Top.Set(amplifiedBluePos.Y, 0f);

            reversalRedIcon.Left.Set(reversalRedPos.X, 0f);
            reversalRedIcon.Top.Set(reversalRedPos.Y, 0f);

            hollowPurpleIcon.Left.Set(hollowPurplePos.X, 0f);
            hollowPurpleIcon.Top.Set(hollowPurplePos.Y, 0f);

            float vectorRotation = 1f / 3f;

            // Maximum Blue
            Vector2 maximumBluePos = new Vector2(amplifiedBluePos.X + ICON_SIZE / 2, amplifiedBluePos.Y + ICON_SIZE / 2);
            maximumBluePos = maximumBluePos.DirectionFrom(center);
            maximumBluePos = maximumBluePos.RotatedBy(vectorRotation);
            maximumBluePos.Normalize();
            maximumBluePos *= distance * 2;
            maximumBlueIcon.Left.Set(maximumBluePos.X + center.X - ICON_SIZE / 2, 0f);
            maximumBlueIcon.Top.Set(maximumBluePos.Y + center.Y - ICON_SIZE / 2, 0f);

            // Maximum Red
            Vector2 maximumRedPos = new Vector2(reversalRedPos.X + ICON_SIZE / 2, reversalRedPos.Y + ICON_SIZE / 2);
            maximumRedPos = maximumRedPos.DirectionFrom(center);
            maximumRedPos = maximumRedPos.RotatedBy(vectorRotation);
            maximumRedPos.Normalize();
            maximumRedPos *= distance * 2;
            maximumRedIcon.Left.Set(maximumRedPos.X + center.X - ICON_SIZE / 2, 0f);
            maximumRedIcon.Top.Set(maximumRedPos.Y + center.Y - ICON_SIZE / 2, 0f);

            // Maximum Aura
            Vector2 maximumAuraPos = new Vector2(amplifiedAuraPos.X + ICON_SIZE / 2, amplifiedAuraPos.Y + ICON_SIZE / 2);
            maximumAuraPos = maximumAuraPos.DirectionFrom(center);
            maximumAuraPos = maximumAuraPos.RotatedBy(vectorRotation);
            maximumAuraPos.Normalize();
            maximumAuraPos *= distance * 2;
            maximumAuraIcon.Left.Set(maximumAuraPos.X + center.X - ICON_SIZE / 2, 0f);
            maximumAuraIcon.Top.Set(maximumAuraPos.Y + center.Y - ICON_SIZE / 2, 0f);

            // Falling Blossom Emotion
            Vector2 fallingBlossomEmotionPos = new Vector2(infinityPos.X + ICON_SIZE / 2, infinityPos.Y + ICON_SIZE / 2);
            fallingBlossomEmotionPos = fallingBlossomEmotionPos.DirectionFrom(center);
            fallingBlossomEmotionPos = fallingBlossomEmotionPos.RotatedBy(vectorRotation);
            fallingBlossomEmotionPos.Normalize();
            fallingBlossomEmotionPos *= distance * 2;
            fallingBlossomEmotionIcon.Left.Set(fallingBlossomEmotionPos.X + center.X - ICON_SIZE / 2, 0f);
            fallingBlossomEmotionIcon.Top.Set(fallingBlossomEmotionPos.Y + center.Y - ICON_SIZE / 2, 0f);


            // 200 % Hollow Purple
            Vector2 hollowPurple200Pos = new Vector2(hollowPurplePos.X + ICON_SIZE / 2, hollowPurplePos.Y + ICON_SIZE / 2);
            hollowPurple200Pos = hollowPurple200Pos.DirectionFrom(center);
            hollowPurple200Pos = hollowPurple200Pos.RotatedBy(vectorRotation);
            hollowPurple200Pos.Normalize();
            hollowPurple200Pos *= distance * 2f;
            hollowPurple200Icon.Left.Set(hollowPurple200Pos.X + center.X - ICON_SIZE / 2, 0f);
            hollowPurple200Icon.Top.Set(hollowPurple200Pos.Y + center.Y - ICON_SIZE / 2, 0f);


            maximumAuraIcon.parents.Add(amplifiedAuraIcon);
            maximumBlueIcon.parents.Add(amplifiedBlueIcon);
            maximumRedIcon.parents.Add(reversalRedIcon);
            fallingBlossomEmotionIcon.parents.Add(infinityIcon);
            hollowPurple200Icon.parents.Add(hollowPurpleIcon);
        }

        void DrawShrine(Vector2 center, SorceryFightPlayer sfPlayer)
        {
            float distance = 100f;
            Vector2[] originPositions = OriginPositionHelper(ICON_SIZE, n: 3, distanceFromCenter: distance);

            Span<TechniqueIcon> ctIconsSpan = CollectionsMarshal.AsSpan(GetCursedTechniqueIcons(sfPlayer));
            Span<TechniqueIcon> ptIconsSpan = CollectionsMarshal.AsSpan(GetPassiveTechniqueIcons(sfPlayer));

            ref TechniqueIcon dismantleIcon = ref ctIconsSpan[0];
            ref TechniqueIcon cleaveIcon = ref ctIconsSpan[1];
            ref TechniqueIcon instantDismantleIcon = ref ctIconsSpan[2];
            ref TechniqueIcon divineFlameIcon = ref ctIconsSpan[3];
            ref TechniqueIcon worldCuttingSlashIcon = ref ctIconsSpan[4];

            ref TechniqueIcon domainAmpIcon = ref ptIconsSpan[0];
            ref TechniqueIcon hollowWickerBasketIcon = ref ptIconsSpan[1];

            ref Vector2 domainAmpPos = ref originPositions[0];
            ref Vector2 dismantlePos = ref originPositions[1];
            ref Vector2 cleavePos = ref originPositions[2];

            dismantleIcon.Left.Set(dismantlePos.X, 0f);
            dismantleIcon.Top.Set(dismantlePos.Y, 0f);

            cleaveIcon.Left.Set(cleavePos.X, 0f);
            cleaveIcon.Top.Set(cleavePos.Y, 0f);

            domainAmpIcon.Left.Set(domainAmpPos.X, 0f);
            domainAmpIcon.Top.Set(domainAmpPos.Y, 0f);

            Vector2 hollowWickerBasketPos = domainAmpPos + new Vector2(distance, 0f);
            hollowWickerBasketIcon.Left.Set(hollowWickerBasketPos.X, 0f);
            hollowWickerBasketIcon.Top.Set(hollowWickerBasketPos.Y, 0f);

            Vector2 instantDismantlePos = new Vector2(dismantlePos.X + ICON_SIZE / 2, dismantlePos.Y + ICON_SIZE / 2);
            instantDismantlePos = instantDismantlePos.DirectionFrom(center);
            instantDismantlePos = instantDismantlePos.RotatedBy(0.2f);
            instantDismantlePos.Normalize();
            instantDismantlePos *= distance * 2;
            instantDismantleIcon.Left.Set(instantDismantlePos.X + center.X - ICON_SIZE / 2, 0f);
            instantDismantleIcon.Top.Set(instantDismantlePos.Y + center.Y - ICON_SIZE / 2, 0f);

            Vector2 divineFlamePos = new Vector2(cleavePos.X + ICON_SIZE / 2, cleavePos.Y + ICON_SIZE / 2);
            divineFlamePos = divineFlamePos.DirectionFrom(center);
            divineFlamePos = divineFlamePos.RotatedBy(-0.2f);
            divineFlamePos.Normalize();
            divineFlamePos *= distance * 2;
            divineFlameIcon.Left.Set(divineFlamePos.X + center.X - ICON_SIZE / 2, 0f);
            divineFlameIcon.Top.Set(divineFlamePos.Y + center.Y - ICON_SIZE / 2, 0f);

            Vector2 worldCuttingSlashPos = instantDismantlePos + new Vector2(-distance, 0f);
            worldCuttingSlashIcon.Left.Set(worldCuttingSlashPos.X + center.X - ICON_SIZE / 2, 0f);
            worldCuttingSlashIcon.Top.Set(worldCuttingSlashPos.Y + center.Y - ICON_SIZE / 2, 0f);

            hollowWickerBasketIcon.parents.Add(domainAmpIcon);
            instantDismantleIcon.parents.Add(dismantleIcon);
            divineFlameIcon.parents.AddRange([dismantleIcon, cleaveIcon]);
            worldCuttingSlashIcon.parents.Add(instantDismantleIcon);
        }

        void DrawVessel(Vector2 _, SorceryFightPlayer sfPlayer)
        {
            Vector2[] originPositions = OriginPositionHelper(iconSize: ICON_SIZE, n: 8, distanceFromCenter: 120f);

            for (int i = 0; i < sfPlayer.innateTechnique.CursedTechniques.Count + sfPlayer.innateTechnique.PassiveTechniques.Count; i++)
            {
                techniqueIcons[i].Left.Set(originPositions[i].X, 0f);
                techniqueIcons[i].Top.Set(originPositions[i].Y, 0f);
            }
        }


        void DrawBloodManipulation(Vector2 _, SorceryFightPlayer sfPlayer)
        {
            Vector2[] originPositions = OriginPositionHelper(iconSize: ICON_SIZE, n: 9, distanceFromCenter: 120f);

            for (int i = 0; i < sfPlayer.innateTechnique.CursedTechniques.Count + sfPlayer.innateTechnique.PassiveTechniques.Count; i++)
            {
                techniqueIcons[i].Left.Set(originPositions[i].X, 0f);
                techniqueIcons[i].Top.Set(originPositions[i].Y, 0f);
            }
        }

        void DrawStarRage(Vector2 _, SorceryFightPlayer sfPlayer)
        {
            Vector2[] originPositions = OriginPositionHelper(iconSize: ICON_SIZE, n: 8, distanceFromCenter: 150f);

            for (int i = 0; i < sfPlayer.innateTechnique.CursedTechniques.Count + sfPlayer.innateTechnique.PassiveTechniques.Count; i++)
            {
                techniqueIcons[i].Left.Set(originPositions[i].X, 0f);
                techniqueIcons[i].Top.Set(originPositions[i].Y, 0f);
            }
        }

        void DrawIceFormation(Vector2 _, SorceryFightPlayer sfPlayer)
        {
            Vector2[] originPositions = OriginPositionHelper(iconSize: ICON_SIZE, n: 3, distanceFromCenter: 80f);

            for (int i = 0; i < sfPlayer.innateTechnique.CursedTechniques.Count + sfPlayer.innateTechnique.PassiveTechniques.Count; i++)
            {
                techniqueIcons[i].Left.Set(originPositions[i].X, 0f);
                techniqueIcons[i].Top.Set(originPositions[i].Y, 0f);
            }
        }

        void DrawTenShadows(Vector2 _, SorceryFightPlayer sfPlayer)
        {
            Vector2[] originPositions = OriginPositionHelper(iconSize: ICON_SIZE, n: 6, distanceFromCenter: 80f);

            for (int i = 0; i < sfPlayer.innateTechnique.CursedTechniques.Count + sfPlayer.innateTechnique.PassiveTechniques.Count; i++)
            {
                techniqueIcons[i].Left.Set(originPositions[i].X, 0f);
                techniqueIcons[i].Top.Set(originPositions[i].Y, 0f);
            }
        }

        void DrawPPLT(Vector2 _, SorceryFightPlayer sfPlayer)
        {
            Vector2[] originPositions = OriginPositionHelper(iconSize: ICON_SIZE, n: 5, distanceFromCenter: 120f);

            for (int i = 0; i < sfPlayer.innateTechnique.CursedTechniques.Count + sfPlayer.innateTechnique.PassiveTechniques.Count; i++)
            {
                techniqueIcons[i].Left.Set(originPositions[i].X, 0f);
                techniqueIcons[i].Top.Set(originPositions[i].Y, 0f);
            }
        }


        void DrawHeavenlyRestriction(Vector2 _, SorceryFightPlayer sfPlayer)
        {
            Vector2[] originPositions = OriginPositionHelper(iconSize: ICON_SIZE, n: 5, distanceFromCenter: 100f);

            for (int i = 0; i < sfPlayer.innateTechnique.CursedTechniques.Count + sfPlayer.innateTechnique.PassiveTechniques.Count; i++)
            {
                techniqueIcons[i].Left.Set(originPositions[i].X, 0f);
                techniqueIcons[i].Top.Set(originPositions[i].Y, 0f);
            }

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
