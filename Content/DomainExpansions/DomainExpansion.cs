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
using Terraria.GameContent;
using CalamityMod.Particles;


namespace sorceryFight.Content.DomainExpansions
{
    public abstract class DomainExpansion
    {
        public static Texture2D BaseTexture = ModContent.Request<Texture2D>("sorceryFight/Content/DomainExpansions/DomainExpansionBase", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

        /// <summary>
        /// The internal name of the domain, used to retrieve DisplayName, Description, and LockedDescription, and DomainTexture.
        /// </summary>
        public abstract string InternalName { get; }
        public string DisplayName => SFUtils.GetLocalizationValue($"Mods.sorceryFight.DomainExpansions.{InternalName}.DisplayName");

        /// <summary>
        /// The sound played when the domain is first casted (after the "Domain Expansion: _________" text plays).
        /// </summary>
        public abstract SoundStyle CastSound { get; }

        /// <summary>
        /// The sure hit range of the domain. Also used to draw DrawInnerDomain(Spritebatch spriteBatch) for players inside the range.
        /// </summary>
        public abstract float SureHitRange { get; }

        /// <summary>
        /// Whether or not the domain is closed. Used to determine if players and NPCs can leave/enter the sure hit radius.
        /// </summary>
        public abstract bool ClosedDomain { get; }

        /// <summary>
        /// Main draw method for the domain.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public abstract void Draw(SpriteBatch spriteBatch);

        /// <summary>
        /// The index of this domain in ActiveDomains
        /// </summary>
        public int id;

        /// <summary>
        /// The whoAmI of the caster of the domain.
        /// </summary>
        public int owner;

        /// <summary>
        /// The center of the domain.
        /// </summary>
        public Vector2 center;

        /// <summary>
        /// The ID of the domain that this domain is clashing with.
        /// </summary>
        public int clashingWith = -1;

        /// <summary>
        /// Runs any logic that needs to be constantly updated. Call base.Update() to auto disallow entering/leaving the domain.
        /// </summary>
        public virtual void Update()
        {
            if (ClosedDomain)
                DomainBarrier();
        }

        /// <summary>
        /// Prevents any player or NPC from entering or leaving the domain.
        /// </summary>
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

            if (this is PlayerDomainExpansion)
            {
                bool outsideDomain = distanceSquared > SureHitRange.Squared() + 25000;

                if (outsideDomain && Main.myPlayer == owner)
                {
                    DomainExpansionController.CloseDomain(id);
                    return;
                }
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

        /// <summary>
        /// Draws on the players screen if they are in the sure hit radius.
        /// </summary>
        /// <param name="action"></param>
        public virtual void DrawInnerDomain(Action action)
        {
            if (Vector2.DistanceSquared(Main.LocalPlayer.Center, this.center) <= SureHitRange.Squared())
            {
                action.Invoke();
            }
        }

        public virtual void DrawClashing(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(BaseTexture, center - Main.screenPosition, new Rectangle(0, 0, BaseTexture.Width, BaseTexture.Height), Color.White, 0f, new Rectangle(0, 0, BaseTexture.Width, BaseTexture.Height).Size() * 0.5f, 2f, SpriteEffects.None, 0f);

            DrawInnerDomain(() =>
            {
                Texture2D whiteTexture = TextureAssets.MagicPixel.Value;
                Rectangle screenRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);

                spriteBatch.Draw(whiteTexture, screenRectangle, Color.Black);
            });
        }

        /// <summary>
        /// Override if domain has instance variables or other data that needs to be reset.
        /// </summary>
        public virtual void CloseDomain() { }
    }
}