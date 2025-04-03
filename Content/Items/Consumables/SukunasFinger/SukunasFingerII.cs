using System;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Localization;

namespace sorceryFight.Content.Items.Consumables.SukunasFinger
{
    public class SukunasFingerII : SukunasFingerBase
    {
        public override int Id => 2;
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs(Id);
    }
}