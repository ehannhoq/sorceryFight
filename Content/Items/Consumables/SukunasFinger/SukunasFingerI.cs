using Terraria.Localization;

namespace sorceryFight.Content.Items.Consumables.SukunasFinger
{
    public class SukunasFingerI : SukunasFingerBase
    {
        public override int Id => 1;
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs(Id);
    }
}