using Terraria.Localization;

namespace sorceryFight.Content.Items.Consumables.SukunasFinger
{
    public class SukunasFingerVIII : SukunasFingerBase
    {
        public override int Id => 8;
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs(Id);
    }
}