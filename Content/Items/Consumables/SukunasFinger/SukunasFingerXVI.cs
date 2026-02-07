using Terraria.Localization;

namespace sorceryFight.Content.Items.Consumables.SukunasFinger
{
    public class SukunasFingerXVI : SukunasFingerBase
    {
        public override int Id => 16;
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs(Id);
    }
}