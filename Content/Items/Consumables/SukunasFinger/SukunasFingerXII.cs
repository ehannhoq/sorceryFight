using Terraria.Localization;

namespace sorceryFight.Content.Items.Consumables.SukunasFinger
{
    public class SukunasFingerXII : SukunasFingerBase
    {
        public override int Id => 12;
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs(Id);
    }
}