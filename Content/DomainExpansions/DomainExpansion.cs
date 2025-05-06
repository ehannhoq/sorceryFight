using Microsoft.Xna.Framework.Graphics;
using Terraria;
using sorceryFight.SFPlayer;
using Microsoft.Xna.Framework;
using sorceryFight.Content.Buffs;
using Terraria.ID;
using CalamityMod.NPCs.NormalNPCs;
using Terraria.ModLoader;


namespace sorceryFight.Content.DomainExpansions
{
    public enum DomainPhase
    {
        WaitingToExpand,
        Expanding,
        Expanded
    }

    public abstract class DomainExpansion
    {
        public abstract string InternalName { get; }
        public string DisplayName => SFUtils.GetLocalizationValue($"Mods.sorceryFight.DomainExpansions.{InternalName}.DisplayName");
        public string Discription => SFUtils.GetLocalizationValue($"Mods.sorceryFight.DomainExpansions.{InternalName}.Discription");
        public string LockedDiscription => SFUtils.GetLocalizationValue($"Mods.sorceryFight.DomainExpansions.{InternalName}.LockedDiscription");

        public abstract Texture2D DomainTexture { get; }
        public abstract float SureHitRange { get; }
        public abstract float Cost { get; }
        public abstract bool Unlocked(SorceryFightPlayer sf);
        public abstract void SureHitEffect(NPC npc);

        public Vector2 center;
        public DomainPhase phase;
        public float[] ai = new float[5];
        public ref float increment => ref ai[0];

        public virtual void Update()
        {
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.type != NPCID.TargetDummy && npc.type != ModContent.NPCType<SuperDummyNPC>())
                {
                    float distance = Vector2.DistanceSquared(npc.Center, this.center);
                    if (distance < SureHitRange.Squared())
                    {
                        SureHitEffect(npc);
                    }
                }
            }
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {

        }

        public virtual void ExpandDomain(SorceryFightPlayer sf)
        {
            DomainExpansionController.ActiveDomains[sf.Player.whoAmI] = this;
            center = sf.Player.Center;
            sf.disableRegenFromDE = true;
            phase = DomainPhase.WaitingToExpand;
        }

        public virtual void CloseDomain(SorceryFightPlayer sf)
        {
            Main.npc[sf.domainIndex].active = false;
            sf.AddDeductableDebuff(ModContent.BuffType<BrainDamage>(), SorceryFightPlayer.DefaultBrainDamageDuration);
            sf.expandedDomain = false;
            sf.disableRegenFromDE = false;
            sf.domainIndex = -1;
        }
    }
}
