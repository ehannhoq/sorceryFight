using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Items.Weapons.Melee;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace sorceryFight
{
    public class SFChestLoot : ModSystem
    {
        public override void PostWorldGen()
        {
			for (int chestIndex = 0; chestIndex < 1000; chestIndex++) {
				Chest chest = Main.chest[chestIndex];

				if (chest != null && Main.tile[chest.x, chest.y].TileType == TileID.Containers && Main.tile[chest.x, chest.y].TileFrameX == 2 * 36) {
                    if (SFUtils.Roll(13))
                        chest.item[0].SetDefaults(ModContent.ItemType<MeiMeiAxe>());

                    if (SFUtils.Roll(13))
                        chest.item[0].SetDefaults(ModContent.ItemType<PotentialSword>());
				}
			}
        }
    }
}