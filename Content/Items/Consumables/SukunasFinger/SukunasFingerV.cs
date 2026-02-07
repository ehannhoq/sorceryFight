using Terraria.Localization;

namespace sorceryFight.Content.Items.Consumables.SukunasFinger
{
    public class SukunasFingerV : SukunasFingerBase
    {
        public override int Id => 5;
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs(Id);
    }
}