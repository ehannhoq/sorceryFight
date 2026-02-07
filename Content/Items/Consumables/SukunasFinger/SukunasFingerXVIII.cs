using Terraria.Localization;

namespace sorceryFight.Content.Items.Consumables.SukunasFinger
{
    public class SukunasFingerXVIII : SukunasFingerBase
    {
        public override int Id => 18;
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs(Id);
    }
}