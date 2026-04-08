using System;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Localization;

namespace sorceryFight.Content.Items.Consumables.SukunasFinger
{
    public class SukunasFingerVI : SukunasFingerBase
    {
        public override int Id => 6;
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs(Id);
    }
}