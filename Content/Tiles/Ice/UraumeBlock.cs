using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Tiles.Ice
{
    public class UraumeBlock : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;

            MinPick = int.MaxValue;

            AddMapEntry(new Color(100, 180, 255));
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.15f;
            g = 0.35f;
            b = 0.55f;
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void PlaceInWorld(int i, int j, Item item)
        {
            ModContent.GetInstance<UraumeBlockTE>().Place(i, j);
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            ModContent.GetInstance<UraumeBlockTE>().Kill(i, j);
        }
    }
}