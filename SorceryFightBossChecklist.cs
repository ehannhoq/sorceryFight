using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.NPCs.Mahoraga;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight
{
    public class SorceryFightBossChecklist : ModSystem
    {
        private LocalizedText GetSpawnInfo(string name) => SFUtils.GetLocalization($"Mods.sorceryFight.BossChecklistIntegration.{name}.SpawnInfo");
        public override void PostSetupContent()
        {
            if (ModLoader.TryGetMod("BossChecklist", out Mod bossChecklist))
            {
                string entryName = "Mahoraga";
                float order = 16.5f;
                
                Action<SpriteBatch, Rectangle, Color> portrait = (SpriteBatch sb, Rectangle rect, Color color) =>
                {
                    Texture2D texture = ModContent.Request<Texture2D>("sorceryFight/Content/NPCs/Bosses/TenShadows/Mahoraga/MahoragaBoss_BossChecklist").Value;
                    Vector2 centered = new Vector2(rect.Center.X - (texture.Width / 2), rect.Center.Y - (texture.Height / 2));
                    sb.Draw(texture, centered, color);
                };

                bossChecklist.Call(
                    "LogBoss",
                    Mod,
                    entryName,
                    order,
                    () => SorceryFightDownedBossSystem.downedMahoraga,
                    ModContent.NPCType<MahoragaBoss>(),
                    new Dictionary<string, object>()
                    {
                        ["spawnInfo"] = GetSpawnInfo(entryName),
                        ["customPortrait"] = portrait
                    }
                );
            }
        }        
    }
}