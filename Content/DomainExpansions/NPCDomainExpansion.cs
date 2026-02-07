using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.DomainExpansions.NPCDomains;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace sorceryFight.Content.DomainExpansions
{
    public abstract class NPCDomainExpansion : DomainExpansion
    {
        public abstract override string InternalName { get; }
        public abstract override SoundStyle CastSound { get; }
        public abstract override float SureHitRange { get; }
        public abstract override bool ClosedDomain { get; }
        public abstract override void Draw(SpriteBatch spriteBatch);

        /// <summary>
        /// The texture of the domain.
        /// </summary>
        public virtual Texture2D DomainTexture => ModContent.Request<Texture2D>($"sorceryFight/Content/DomainExpansions/NPCDomains/{InternalName}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

        /// <summary>
        /// The condition for expanding the domain.
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        public abstract bool ExpandCondition(NPC npc);

        /// <summary>
        /// The effect that happens when any player is in the sure hit radius.
        /// </summary>
        /// <param name="player"></param>
        public abstract void SureHitEffect(Player player);

        /// <summary>
        /// Runs any logic that needs to be constantly updated. Call base.Update() to auto disallow entering/leaving the domain, 
        /// auto-applying sure hit effects, and auto-closing the domain at death.
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (clashingWith == -1)
            {
                foreach (Player player in Main.player)
                {
                    if (player.active && !player.dead && !player.SorceryFight().immuneToDomains)
                    {
                        float distance = Vector2.DistanceSquared(player.Center, this.center);
                        if (distance < SureHitRange.Squared())
                        {
                            SureHitEffect(player);
                        }
                    }
                }
            }

            if (!Main.npc[owner].active || Main.npc[owner].life <= Main.npc[owner].lifeMax * 0.01f)
            {
                DomainExpansionController.CloseDomain(id);
                Main.npc[owner].GetGlobalNPC<NPCDomainController>().domainCounter = 0;
            }
        }
    }
}
