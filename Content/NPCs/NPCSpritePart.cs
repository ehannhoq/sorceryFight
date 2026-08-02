using System.Drawing;
using Microsoft.Xna.Framework.Graphics;

namespace sorceryFight.Content.NPCs
{
    public struct NPCSpritePart
    {
        public Texture2D sprite;
        public int frames;
        public int ticksPerFrame;

        public int frame;
        public int frameTime;

        public NPCSpritePart(Texture2D sprite, int frames, int ticksPerFrame)
        {
            this.sprite = sprite;
            this.frames = frames;
            this.ticksPerFrame = ticksPerFrame;

            frame = 0;
            frameTime = 0;
        }
    }
}