namespace sorceryFight.Content.NPCs.FingerBearer
{
    public class FingerBearer : BossNPC
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 45;
            NPC.height = 21;
            currentState = new FingerBearerDefaultState();
        }
    }
}