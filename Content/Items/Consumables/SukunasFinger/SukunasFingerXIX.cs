using Terraria.Localization;

namespace sorceryFight.Content.Items.Consumables.SukunasFinger
{
    public class SukunasFingerXIX : SukunasFingerBase
    {
        public override int Id => 19;
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs(Id);
    }
}