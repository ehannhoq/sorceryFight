using Terraria.Localization;

namespace sorceryFight.Content.Items.Consumables.SukunasFinger
{
    public class SukunasFingerX : SukunasFingerBase
    {
        public override int Id => 10;
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs(Id);
    }
}