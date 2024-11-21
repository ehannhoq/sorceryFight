using System.Collections.Generic;
using Microsoft.Xna.Framework;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.CursedTechniques.Limitless;
using sorceryFight.Content.DomainExpansions;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.Buffs.Limitless;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;

namespace sorceryFight.Content.InnateTechniques
{
    public class LimitlessTechnique : InnateTechnique
    {
        public bool hasInfinity = false;
        public int domainExpansionTimer = -1;
        public override string Name => "Limitless";
        public override List<PassiveTechnique> PassiveTechniques { get; } = new List<PassiveTechnique>
        {
            new InfinityBuff(),
            new AmplifiedAuraBuff(),
            new MaximumAmplifiedAuraBuff()
        };
        public override List<CursedTechnique> CursedTechniques { get; } = new List<CursedTechnique>
        {
            new AmplificationBlue(),
            new MaximumOutputBlue(),

            new ReversalRed(),
            new HollowPurple(),

            new HollowPurple200Percent()
        };

        public override DomainExpansion DomainExpansion { get; } = new UnlimitedVoid();

        public override bool IsValid { get; } = true;

        public override void PostUpdate(SorceryFightPlayer sf)
        {
            if (domainExpansionTimer == -1)
            {
                return;
            }

            domainExpansionTimer ++;

            if (domainExpansionTimer == 1)
            {
                int index = CombatText.NewText(sf.Player.getRect(), Color.White, "Domain Expansion:");
                Main.combatText[index].lifeTime = 90;
            }

            if (domainExpansionTimer == 101)
            {
                int index = CombatText.NewText(sf.Player.getRect(), Color.White, "Unlimited Void");
                Main.combatText[index].lifeTime = 90;
            }

            if (domainExpansionTimer == 181)
            {
                SoundEngine.PlaySound(SorceryFightSounds.UnlimitedVoid, sf.Player.Center);
            }
            
            if (domainExpansionTimer == 211)
            {
                Terraria.DataStructures.IEntitySource entitySource = sf.Player.GetSource_FromThis();
                Vector2 position = sf.Player.Center;

                if (Main.myPlayer == sf.Player.whoAmI)
                    sf.domainIndex = NPC.NewNPC(entitySource, (int)position.X, (int)position.Y, ModContent.NPCType<UnlimitedVoid>(), 0, default, sf.Player.whoAmI);
                domainExpansionTimer = -1;
            }

        }

        public override void ExpandDomain(SorceryFightPlayer sf)
        {
            domainExpansionTimer = 0;
        }

        public override void CloseDomain(SorceryFightPlayer sf)
        {
            UnlimitedVoid.frozenValues.Clear();
            base.CloseDomain(sf);
        }
    }
}