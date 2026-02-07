using Terraria.Localization;

namespace sorceryFight.Content.Items.Consumables.SukunasFinger
{
    public class SukunasFingerXI : SukunasFingerBase
    {
        public override int Id => 11;
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs(Id);
    }
}