using System.Linq;
using CalamityMod.Items.Placeables.Furniture.Paintings;
using sorceryFight.Content.Items.Accessories;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Quests
{
    public class BlackFlashQuestI : Quest
    {
        private const int BLACK_FLASH_COUNT = 3;
        public override bool IsCompleted(SorceryFightPlayer sfPlayer)
        {
            int count = 0;

            if (sfPlayer.TryGetQuestData(this, "BlackFlashCounter", out object obj))
            {
                count = (int)obj;
            }
            else
            {
                sfPlayer.ModifyQuestData(this, "BlackFlashCounter", 0);
            }

            if (sfPlayer.blackFlashTimeLeft == -59)
            {
                count++;
                sfPlayer.ModifyQuestData(this, "BlackFlashCounter", count);
            }

            Main.NewText($"{count}/{BLACK_FLASH_COUNT}");

            return count >= BLACK_FLASH_COUNT && sfPlayer.Player.inventory.Any(item => item.type == ModContent.ItemType<ThankYouPainting>());
        }
        public override void GiveRewards(SorceryFightPlayer sfPlayer)
        {
            sfPlayer.Player.QuickSpawnItem(sfPlayer.Player.GetSource_Misc("PICTURE_LOCKET"), ModContent.ItemType<PictureLocket>());
        }
    }
}