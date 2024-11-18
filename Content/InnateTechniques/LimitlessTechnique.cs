using System.Collections.Generic;
using Microsoft.Xna.Framework;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.CursedTechniques.Limitless;
using sorceryFight.Content.DomainExpansions;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.Buffs.Limitless;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.InnateTechniques
{
    public class LimitlessTechnique : InnateTechnique
    {
        public bool hasInfinity = false;
        public int domainExpansionTimer = -1;
        public override string Name { get; set; } = "Limitless";
        public override List<PassiveTechnique> PassiveTechniques { get; set; } = new List<PassiveTechnique>
        {
            new InfinityBuff(), // 5% Mastery
            new AmplifiedAuraBuff(),
            new MaximumAmplifiedAuraBuff()
        };
        public override List<CursedTechnique> CursedTechniques { get; set; } = new List<CursedTechnique>
        {
            new AmplificationBlue(), // 0% Mastery
            new MaximumOutputBlue(), // 10% Mastery

            new ReversalRed(), // 40% Mastery
            new HollowPurple(), // 60% Mastery

            new HollowPurple200Percent() // 80% Mastery

            // new UnlimitedVoid() // 100% Mastery
        };

        public override bool IsValid { get; set; } = true;

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

            if (domainExpansionTimer == 211)
            {
                Terraria.DataStructures.IEntitySource entitySource = sf.Player.GetSource_FromThis();
                Vector2 position = sf.Player.Center;

                if (Main.myPlayer == sf.Player.whoAmI)
                    sf.domainIndex = NPC.NewNPC(entitySource, (int)position.X, (int)position.Y, ModContent.NPCType<UnlimitedVoid>(), 0, default, sf.Player.whoAmI);
            }
        }

        public override void DomainExpansion(SorceryFightPlayer sf)
        {
            domainExpansionTimer = 0;
        }
    }
}