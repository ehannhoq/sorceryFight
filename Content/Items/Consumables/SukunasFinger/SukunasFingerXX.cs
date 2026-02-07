using Terraria.Localization;

namespace sorceryFight.Content.Items.Consumables.SukunasFinger
{
    public class SukunasFingerXX : SukunasFingerBase
    {
        public override int Id => 20;
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs(Id);
    }
}