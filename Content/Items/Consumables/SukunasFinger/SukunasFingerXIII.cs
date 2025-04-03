using System;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Localization;

namespace sorceryFight.Content.Items.Consumables.SukunasFinger
{
    public class SukunasFingerXIII : SukunasFingerBase
    {
        public override int Id => 13;
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs(Id);
    }
}