using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Items.Weapons.Melee;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace sorceryFight.Content.Tiles
{
    public class ObliviousSwordTile : ModTile
    {
        public override string Texture => "sorceryFight/Content/Tiles/ObliviousSwordTile/0";

        private Texture2D texture;
        private const int FRAMES = 30;
        private const int TICKS_PER_FRAME = 2;

        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileNoFail[Type] = true;
            Main.tileHammer[Type] = true;
            Main.tileSpelunker[Type] = true;
            MinPick = 80;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new[] { 200, 200 };
            TileObjectData.newTile.Origin = new Point16(0, 0);
            TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
            TileObjectData.newTile.AnchorTop = AnchorData.Empty;
            TileObjectData.newTile.AnchorLeft = AnchorData.Empty;
            TileObjectData.newTile.AnchorRight = AnchorData.Empty;
            TileObjectData.addTile(Type);


            AddMapEntry(Color.White, SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Melee.ObliviousSword.DisplayName"));

            RegisterItemDrop(ModContent.ItemType<ObliviousSword>());
        }

        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (frameCounter++ >= TICKS_PER_FRAME)
            {
                frameCounter = 0;
                frame = (frame + 1) % FRAMES;
            }

        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            if (tile.TileFrameX != 0 || tile.TileFrameY != 0)
                return false;

            int currentFrame = Main.tileFrame[Type];
            texture = ModContent.Request<Texture2D>($"sorceryFight/Content/Tiles/ObliviousSwordTile/{currentFrame}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            Vector2 zero = Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange);
            zero += new Vector2(16, 16);
            Vector2 drawOffset = new Vector2(i * 16 - Main.screenPosition.X, j * 16 - Main.screenPosition.Y) + zero;


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.NonPremultiplied
            );


            spriteBatch.Draw
                (
                    texture,
                    drawOffset,
                    texture.Frame(),
                    Color.White,
                    0f,
                    texture.Frame().Size() * 0.5f,
                    0.5f,
                    SpriteEffects.None,
                    0f
                );

            Main.spriteBatch.End();
            Main.spriteBatch.Begin();

            return false;
        }
    }
}