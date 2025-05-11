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
        public string Description => SFUtils.GetLocalizationValue($"Mods.sorceryFight.DomainExpansions.{InternalName}.Description");
        public string LockedDescription => SFUtils.GetLocalizationValue($"Mods.sorceryFight.DomainExpansions.{InternalName}.LockedDiscription");
        public abstract SoundStyle CastSound { get; }
        public abstract Texture2D DomainTexture { get; }
        public abstract float SureHitRange { get; }
        public abstract float Cost { get; }
        public abstract bool ClosedDomain { get; }
        public abstract bool Unlocked(SorceryFightPlayer sf);
        public abstract void SureHitEffect(NPC npc);
        public abstract void Draw(SpriteBatch spriteBatch);
        public virtual bool targetsNPCs { get; } = false;

        public int owner;
        public Vector2 center;
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
                Main.LocalPlayer.wingTime = Main.LocalPlayer.wingTimeMax;
                SorceryFightPlayer sfPlayer = Main.LocalPlayer.GetModPlayer<SorceryFightPlayer>();
                sfPlayer.disableRegenFromDE = true;
                sfPlayer.cursedEnergy -= SFUtils.RateSecondsToTicks(Cost);

                if (sfPlayer.Player.dead || sfPlayer.cursedEnergy < 10)
                {
                    CloseDomain(sfPlayer);
                }
            }

            DomainBarrier();
        }

        public virtual void SetDefaults() {}

        public virtual void DomainBarrier()
        {
            foreach (NPC npc in Main.ActiveNPCs)
            {
                float npcDistance = Vector2.DistanceSquared(npc.Center, center);
                if (npcDistance < SureHitRange.Squared() && npcDistance > SureHitRange.Squared() - 10000)
                {
                    Vector2 toNPC = npc.Center - center;
                    Vector2 velocity = npc.velocity;

                    if (velocity.Length() > 0f)
                    {
                        Vector2 dirToPlayer = Vector2.Normalize(toNPC);
                        Vector2 velDir = Vector2.Normalize(velocity);

                        if (Vector2.Dot(velDir, dirToPlayer) > 0f)
                        {
                            npc.velocity = Vector2.Zero;
                        }
                    }
                }
            }

            if (Main.dedServ) return;

            Player player = Main.player[Main.myPlayer];

            Vector2 toPlayer = player.Center - center;
            float distanceSquared = toPlayer.LengthSquared();

            bool outsideDomain = distanceSquared > SureHitRange.Squared() + 25000;

            if (outsideDomain && Main.myPlayer == owner)
            {
                SorceryFightPlayer sfPlayer = Main.LocalPlayer.GetModPlayer<SorceryFightPlayer>();
                CloseDomain(sfPlayer);
                return;
            }

            if (distanceSquared < SureHitRange.Squared() + 50000 &&
                distanceSquared > SureHitRange.Squared() - 50000)
            {
                Vector2 radialDir = Vector2.Normalize(toPlayer);
                Vector2 vel = player.velocity;

                float radialSpeed = Vector2.Dot(vel, radialDir);
                float speed = vel.Length();

                bool movingOut = distanceSquared < SureHitRange.Squared() && radialSpeed > -40f;
                bool movingIn = distanceSquared > SureHitRange.Squared() && radialSpeed < 40f;

                if (movingIn || movingOut)
                {
                    radialDir = -radialDir;

                }

                player.velocity = radialDir * speed;


            }
        }

        public virtual void DrawInnerDomain(Action action)
        {
            if (Vector2.DistanceSquared(Main.LocalPlayer.Center, this.center) <= SureHitRange.Squared())
            {
                action.Invoke();
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
        }
    }
}
