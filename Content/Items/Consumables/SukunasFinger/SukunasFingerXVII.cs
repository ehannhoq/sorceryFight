using Terraria.Localization;

namespace sorceryFight.Content.Items.Consumables.SukunasFinger
{
    public class SukunasFingerXVII : SukunasFingerBase
    {
        public override int Id => 17;
        public override LocalizedText DisplayName => base.DisplayName.WithFormatArgs(Id);
    }
}