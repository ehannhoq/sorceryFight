using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.DomainExpansions
{
    public abstract class PlayerDomainExpansion : DomainExpansion
    {
        public abstract override string InternalName { get; }
        public virtual string Description(SorceryFightPlayer player) => SFUtils.GetLocalizationValue($"Mods.sorceryFight.DomainExpansions.{InternalName}.Description");
        public virtual string LockedDescription => SFUtils.GetLocalizationValue($"Mods.sorceryFight.DomainExpansions.{InternalName}.LockedDescription");
        public abstract override SoundStyle CastSound { get; }
        public abstract override float SureHitRange { get; }
        public abstract override bool ClosedDomain { get; }
        public abstract override void Draw(SpriteBatch spriteBatch);

        /// <summary>
        /// The texture of the domain.
        /// </summary>
        public virtual Texture2D DomainTexture => ModContent.Request<Texture2D>($"sorceryFight/Content/DomainExpansions/PlayerDomains/{InternalName}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

        /// <summary>
        /// The CE cost/second of the domain. If manually reducing CE cost, subtract 'SFUtils.RateSecondsToTicks(Cost)' from the player's CE.
        /// </summary>
        public abstract float Cost { get; }

        /// <summary>
        /// Unlocking condition for the domain.
        /// </summary>
        /// <param name="sf">The SorceryFightPlayer ModPlayer of the player</param>
        /// <returns></returns>
        public abstract bool Unlocked(SorceryFightPlayer sf);

        /// <summary>
        /// The effect that happens when any NPC is in the sure hit radius.
        /// </summary>
        /// <param name="npc"></param>
        public abstract void SureHitEffect(NPC npc);

        public bool cursedEnergyTax = false;

        /// <summary>
        /// Runs any logic that needs to be constantly updated. Call base.Update() to auto disallow entering/leaving the domain, 
        /// auto-applying sure hit effects, applying buffs/reducing CE cost for the caster, and auto-closing the domain at
        /// depleted CE or death.
        /// </summary>
        public override void Update()
        {
            base.Update();

            if (clashingWith == -1)
            {
                foreach (NPC npc in Main.npc)
                {
                    if (npc.active && npc.type != NPCID.TargetDummy)
                    {
                        float distance = Vector2.DistanceSquared(npc.Center, this.center);
                        if (distance < SureHitRange.Squared())
                        {
                            SureHitEffect(npc);
                        }
                    }
                }
            }

            if (Main.myPlayer == owner)
            {
                Main.LocalPlayer.wingTime = Main.LocalPlayer.wingTimeMax;
                SorceryFightPlayer sfPlayer = Main.LocalPlayer.SorceryFight();
                sfPlayer.disableRegenFromDE = true;

                if (clashingWith == -1)
                    sfPlayer.cursedEnergy -= SFUtils.RateSecondsToTicks(Cost);

                else
                {
                    if (!cursedEnergyTax)
                    {
                        cursedEnergyTax = true;
                        TaskScheduler.Instance.AddContinuousTask(() =>
                        {
                            sfPlayer.cursedEnergy -= SFUtils.RateSecondsToTicks(75);
                        }, 180);
                    }
                }

                if (sfPlayer.Player.dead || sfPlayer.cursedEnergy < 10)
                {
                    DomainExpansionController.CloseDomain(id);
                }
            }
        }
    }
}
