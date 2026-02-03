using System.Linq;
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
            return sfPlayer.blackFlashCounter >= BLACK_FLASH_COUNT && sfPlayer.Player.inventory.Any(item => item.type == ModContent.ItemType<CalamityMod.Items.Placeables.Furniture.DevPaintings.ThankYouPainting>());
        }
        public override void GiveRewards(SorceryFightPlayer sfPlayer)
        {
            sfPlayer.Player.QuickSpawnItem(sfPlayer.Player.GetSource_Misc("PICTURE_LOCKET"), ModContent.ItemType<PictureLocket>());
        }
    }
}