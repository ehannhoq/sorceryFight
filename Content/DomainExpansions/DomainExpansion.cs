using Microsoft.Xna.Framework.Graphics;
using Terraria;
using sorceryFight.SFPlayer;
using Microsoft.Xna.Framework;
using sorceryFight.Content.Buffs;
using Terraria.ID;
using CalamityMod.NPCs.NormalNPCs;
using Terraria.ModLoader;
using System;
using Terraria.Audio;
using sorceryFight.Content.InnateTechniques;


namespace sorceryFight.Content.DomainExpansions
{
    public abstract class DomainExpansion
    {

        public abstract string InternalName { get; }
        public string DisplayName => SFUtils.GetLocalizationValue($"Mods.sorceryFight.DomainExpansions.{InternalName}.DisplayName");
        public string Description => SFUtils.GetLocalizationValue($"Mods.sorceryFight.DomainExpansions.{InternalName}.Discription");
        public string LockedDescription => SFUtils.GetLocalizationValue($"Mods.sorceryFight.DomainExpansions.{InternalName}.LockedDiscription");
        public abstract SoundStyle CastSound { get; }
        public abstract Texture2D DomainTexture { get; }
        public abstract float SureHitRange { get; }
        public abstract float Cost { get; }
        public abstract bool Unlocked(SorceryFightPlayer sf);
        public abstract void SureHitEffect(NPC npc);
        public abstract void Draw(SpriteBatch spriteBatch);

        public int owner;
        public Vector2 center;
        public float[] ai = new float[5];

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

            if (Main.myPlayer == owner)
            {
                SorceryFightPlayer sfPlayer = Main.player[owner].GetModPlayer<SorceryFightPlayer>();
                sfPlayer.disableRegenFromDE = true;
                sfPlayer.cursedEnergy -= SFUtils.RateSecondsToTicks(Cost);

                if (sfPlayer.Player.dead || sfPlayer.cursedEnergy < 10)
                {
                    CloseDomain(sfPlayer);
                }
            }
        }

        public virtual void DrawInnerDomain(Action action)
        {
            foreach (Player player in Main.player)
            {
                if (player.active && Vector2.DistanceSquared(player.Center, this.center) <= SureHitRange.Squared())
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        action.Invoke();
                    }
                }
            }
        }

        public virtual void CloseDomain(SorceryFightPlayer sf, bool supressSyncPacket = false)
        {
            if (Main.myPlayer == sf.Player.whoAmI)
            {
                sf.AddDeductableDebuff(ModContent.BuffType<BrainDamage>(), SorceryFightPlayer.DefaultBrainDamageDuration);
                sf.disableRegenFromDE = false;

                if (Main.netMode != NetmodeID.SinglePlayer && !supressSyncPacket)
                {
                    ModPacket packet = ModContent.GetInstance<SorceryFight>().GetPacket();
                    packet.Write((byte)MessageType.SyncDomain);

                    packet.Write(sf.Player.whoAmI);
                    packet.Write((byte)InnateTechniqueFactory.GetInnateTechniqueType(sf.innateTechnique));
                    packet.Write((byte)DomainNetMessage.ExpandDomain);
                    packet.Send();
                }
            }

            DomainExpansionController.ActiveDomains.Remove(sf.Player.whoAmI);
            ai = new float[5];
        }
    }
}
