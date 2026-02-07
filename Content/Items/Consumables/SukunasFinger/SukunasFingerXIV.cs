using Terraria.Localization;

namespace sorceryFight.Content.Items.Consumables.SukunasFinger
{
    public class SukunasFingerXIV : SukunasFingerBase
    {
        public override int Id => 14;
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs(Id);
    }
}