using Microsoft.Xna.Framework;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.InnateTechniques;
using sorceryFight.Content.PassiveTechniques;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace sorceryFight
{
    public partial class SorceryFightPlayer : ModPlayer
    {
        #region Global Variables
        public bool hasUIOpen = false;
        #endregion

        #region Global Cursed Technique Stuff
        public InnateTechnique innateTechnique = new InnateTechnique();
        public CursedTechnique selectedTechnique = new CursedTechnique();
        public float mastery = 0f;
        public float cursedEnergy = 100f;
        public float maxCursedEnergy = 100f;
        public float cursedEnergyRegenPerSecond;

        #endregion

        #region Cursed Energy Modifiers
        #endregion

        public override void SaveData(TagCompound tag)
        {
            tag["innateTechnique"] = innateTechnique.Name;
            tag["mastery"] = mastery;
            tag["cursedEnergy"] = cursedEnergy;
        }

        public override void LoadData(TagCompound tag)
        {
            string innateTechniqueName = tag.ContainsKey("innateTechnique") ? tag.GetString("innateTechnique") : "";
            innateTechnique = InnateTechnique.GetInnateTechnique(innateTechniqueName);
            
            mastery = tag.ContainsKey("mastery") ? tag.GetFloat("mastery") : 0f;
            cursedEnergy = tag.ContainsKey("cursedEnergy") ? tag.GetFloat("cursedEnergy") : 0f;

            cursedEnergyRegenPerSecond = calculateCERegenRate();
            maxCursedEnergy = calculateMaxCE(); // Will calculate at the very end, to ensure all buffs are accounted for.
        }

        public override void PostUpdate()
        {
            if (SFKeybinds.UseTechnique.JustPressed)
            {
                ShootTechnique();
            }
            if (cursedEnergy < maxCursedEnergy)
            {
                cursedEnergy += SorceryFight.TicksToSeconds(cursedEnergyRegenPerSecond);
                if (cursedEnergy > maxCursedEnergy)
                {
                    cursedEnergy = maxCursedEnergy;
                }
            }
        }

        public override void PostUpdateBuffs()
        {
            if (cursedEnergy < 1)
            {
                cursedEnergy = 1;
                Player.AddBuff(ModContent.BuffType<BurntTechnique>(), (int)SorceryFight.SecondsToTicks(10));
            }

            foreach (PassiveTechnique passiveTechnique in innateTechnique.PassiveTechniques)
            {
                if (cursedEnergy <= 1)
                {
                    passiveTechnique.isActive = false;
                }

                if (passiveTechnique.isActive)
                {
                    passiveTechnique.Apply(Player);
                }
                else
                {
                    passiveTechnique.Remove(Player);
                }
            }
        }

        public override void UpdateDead()
        {
            foreach (PassiveTechnique passiveTechnique in innateTechnique.PassiveTechniques)
            {
                passiveTechnique.isActive = false;
            }
        }

        private float calculateMaxCE()
        {
            float sum = 0f;
            float baseCE = 100f;

            // This is where all the conditions will go for modifiers.

            return baseCE + sum;
        }

        private float calculateCERegenRate()
        {
            float sum = 0f;
            float baseRegen = 10f;
            
            // This is where all the conditions will go for modifiers.

            return baseRegen + sum;
        }

        private void ShootTechnique()
        {
            if (selectedTechnique.Name == "None Selected.")
            {
                return;
            }

            Vector2 playerPos = Player.MountedCenter;
            Vector2 mousePos = Main.MouseWorld;
            Vector2 dir = (mousePos - playerPos).SafeNormalize(Vector2.Zero) * selectedTechnique.Speed;
            var entitySource = Player.GetSource_FromThis();

            selectedTechnique.Shoot(entitySource, playerPos, dir, Player);
        }
    }
}