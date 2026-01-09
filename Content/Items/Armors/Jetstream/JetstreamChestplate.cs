using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Bloodflare;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using sorceryFight.Content.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Armors.Jetstream
{
    [AutoloadEquip(EquipType.Body)]
    public class JetstreamChestplate : ModItem
    {
        public static int maxHp = 50;
        public static float meleeDmg = 0.20f;
        public static float meleeSpeed = 0.35f;

        public static float setBonusMeleeDmg = 0.10f;
        public static float setBonusTrueMeleeDmg = 0.10f;
        public static float setBonusMurasamaDmg = 0.20f;
        public static float setBonusMurasamaAttackSpeed = 0.25f;
        public static int setBonusTrueMeleeHealing = 5;
        public static int setBonusBloodshedStrikes = 25;
        public static int setBonusBloodshedCooldown = 25;
        public static int setBonusHeartDropChance = 10;

        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Armors.JetstreamChestplate.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Armors.JetstreamChestplate.Tooltip").WithFormatArgs(maxHp, (int)(meleeDmg * 100), (int)(meleeSpeed * 100));

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.rare = ItemRarityID.Pink;
            Item.defense = 62;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Melee) *= 1 + meleeDmg;
            player.GetAttackSpeed(DamageClass.Melee) += 1 + meleeSpeed;
        }
        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == ModContent.ItemType<JetstreamMask>() && body.type == Type && legs.type == ModContent.ItemType<JetstreamLeggings>();
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<BloodflareBodyArmor>(), 1);
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 10);
            recipe.AddIngredient(ModContent.ItemType<ExoPrism>(), 10);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.Register();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = SFUtils.GetLocalization("Mods.sorceryFight.Armors.JetstreamSetBonus").WithFormatArgs(
                (int)(setBonusMeleeDmg * 100),
                (int)(setBonusTrueMeleeDmg * 100),
                (int)(setBonusMurasamaDmg * 100),
                (int)(setBonusMurasamaAttackSpeed * 100),
                setBonusTrueMeleeHealing,
                setBonusBloodshedStrikes,
                setBonusBloodshedCooldown,
                setBonusHeartDropChance
            ).Value;

            player.GetDamage(DamageClass.Melee) *= 1 + setBonusMeleeDmg;
            if (player.HeldItem.type == ModContent.ItemType<Murasama>())
            {
                player.GetDamage(DamageClass.Melee) *= 1 + setBonusMurasamaDmg;
                player.GetAttackSpeed(DamageClass.Melee) *= 1 + setBonusMurasamaAttackSpeed;
            }
        }

        private class SetBonusPlayer : ModPlayer
        {
            public int trueMeleeStrikesCounter = 0;
            public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
            {
                if (target.friendly) return;
                
                if (Player.armor[0].type == ModContent.ItemType<JetstreamMask>() && Player.armor[1].type == ModContent.ItemType<JetstreamChestplate>() && Player.armor[2].type == ModContent.ItemType<JetstreamLeggings>())
                {
                    CalamityMod.TrueMeleeDamageClass tmInstance = SFUtils.GetInternalFieldFromCalamity<CalamityMod.TrueMeleeDamageClass>("CalamityMod.TrueMeleeDamageClass", "Instance");
                    CalamityMod.TrueMeleeNoSpeedDamageClass tmNSInstance = SFUtils.GetInternalFieldFromCalamity<CalamityMod.TrueMeleeNoSpeedDamageClass>("CalamityMod.TrueMeleeNoSpeedDamageClass", "Instance");

                    if (modifiers.DamageType == tmInstance || modifiers.DamageType == tmNSInstance)
                    {
                        Player.Heal(5);

                        if (!Player.HasBuff(ModContent.BuffType<Bloodshed>()) && !Player.HasBuff(ModContent.BuffType<BloodshedCooldown>()))
                        {
                            if (trueMeleeStrikesCounter++ >= setBonusBloodshedCooldown)
                            {
                                Player.AddBuff(ModContent.BuffType<Bloodshed>(), SFUtils.BuffSecondsToTicks(10));
                                trueMeleeStrikesCounter = 0;
                            }
                        }
                    }

                    if (SFUtils.Roll(setBonusHeartDropChance))
                    {
                        Item.NewItem(target.GetSource_FromThis(), target.getRect(), ItemID.Heart);
                    }
                }
            }
        }
    }
}