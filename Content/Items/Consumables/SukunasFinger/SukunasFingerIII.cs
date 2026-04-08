using System;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Localization;

namespace sorceryFight.Content.Items.Consumables.SukunasFinger
{
    public class SukunasFingerIII : SukunasFingerBase
    {
        public override int Id => 3;
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs(Id);
    }
}