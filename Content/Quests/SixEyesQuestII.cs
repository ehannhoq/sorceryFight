using System;
using System.Linq;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using sorceryFight.Content.Items.Accessories;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Quests
{
    public class SixEyesQuestII : Quest
    {
        public override bool IsAvailable(SorceryFightPlayer sfPlayer)
        {
            return sfPlayer.SixEyes;
        }
        
        public override bool IsCompleted(SorceryFightPlayer sfPlayer)
        {
            return false;
        }

        public override void GiveRewards(SorceryFightPlayer sfPlayer)
        {
            sfPlayer.sixEyesLevel = 3;
        }
    }
}