using Terraria.Localization;

namespace sorceryFight.Content.Items.Consumables.SukunasFinger
{
    public class SukunasFingerXV : SukunasFingerBase
    {
        public override int Id => 15;
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs(Id);
    }
}