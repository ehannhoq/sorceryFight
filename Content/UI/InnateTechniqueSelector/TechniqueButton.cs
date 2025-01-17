using sorceryFight.SFPlayer;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.InnateTechniques;
using Terraria;

namespace sorceryFight.Content.UI.InnateTechniqueSelector
{
    public class TechnqiueButton : SFButton
    {   
        InnateTechnique technique;
        public TechnqiueButton(Texture2D texture, string hoverText, InnateTechnique technique) : base(texture, hoverText) 
        {
            this.technique = technique;
        }

        public override void OnClick()
        {
            var player = Main.LocalPlayer.GetModPlayer<SorceryFightPlayer>();
            player.innateTechnique = null;
            InnateTechniqueSelector parent = (InnateTechniqueSelector)Parent;
            parent.OnClick(technique);
        }
    }
}
