using sorceryFight.Content.Projectiles.Pets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.Pets
{
    public class MohuDogBuff : ModBuff
    {

        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.MohuDogBuff.DisplayName");
        public override LocalizedText Description => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.MohuDogBuff.Description");

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            bool unused = false;
            player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<MohuDogProjectile>());
        }
    }
}