using Microsoft.Xna.Framework.Graphics;

namespace sorceryFight.Content.Cutscenes
{
    public abstract class Cutscene
    {
        public abstract int CutsceneLength { get; }
        public virtual void OnStart() { }
        public virtual void OnEnd() { }
        public virtual void Update() { }
        public virtual void DrawBehindNPCs(SpriteBatch spriteBatch) { }
        public int Timer;
    }
}
