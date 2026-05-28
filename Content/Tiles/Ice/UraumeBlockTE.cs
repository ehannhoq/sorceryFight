using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace sorceryFight.Content.Tiles.Ice
{
    public class UraumeBlockTE : ModTileEntity
    {
        private const int DECAY_TIME = 900; // 15 seconds
        private int timer;

        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == ModContent.TileType<UraumeBlock>();
        }

        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction, int alternate)
        {
            if (Main.netMode == Terraria.ID.NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 1);
                NetMessage.SendData(Terraria.ID.MessageID.TileEntityPlacement, number: i, number2: j, number3: Type);
                return -1;
            }

            return Place(i, j);
        }

        public override void OnNetPlace()
        {
            timer = DECAY_TIME;
        }

        public override void Update()
        {
            int x = Position.X;
            int y = Position.Y;

            if (!IsTileValidForEntity(x, y))
            {
                Kill(x, y);
                return;
            }

            timer--;

            if (timer <= 0)
            {
                if (Main.netMode != Terraria.ID.NetmodeID.MultiplayerClient)
                {
                    WorldGen.KillTile(x, y, noItem: true);

                    if (Main.netMode == Terraria.ID.NetmodeID.Server)
                    {
                        NetMessage.SendTileSquare(-1, x, y, 1);
                    }
                }

                for (int i = 0; i < 5; i++)
                {
                    Dust dust = Dust.NewDustDirect(
                        new Vector2(x * 16, y * 16),
                        16, 16,
                        Terraria.ID.DustID.IceTorch,
                        Main.rand.NextFloat(-1f, 1f),
                        Main.rand.NextFloat(-2f, -0.5f),
                        120,
                        default,
                        0.9f
                    );
                    dust.noGravity = true;
                }

                Kill(x, y);
            }
        }

        public new int Place(int i, int j)
        {
            int id = base.Place(i, j);
            if (id != -1 && ByID.TryGetValue(id, out TileEntity te) && te is UraumeBlockTE ute)
            {
                ute.timer = DECAY_TIME;
            }
            return id;
        }
    }
}