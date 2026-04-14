// using System;
// using System.IO;
// using System.Linq;
// using CalamityMod.Buffs.DamageOverTime;
// using Microsoft.Xna.Framework;
// using Microsoft.Xna.Framework.Graphics;
// using Terraria;
// using Terraria.Audio;
// using Terraria.ModLoader;

// namespace sorceryFight.Content.DomainExpansions.NPCDomains
// {
//     public class AshenedPillarsOfCalamity : NPCDomainExpansion
//     {

//         public override string InternalName => "AshenedPillarsOfCalamity";

//         public override SoundStyle CastSound => SorceryFightSounds.AshenedPillarsOfCalamity;

//         public override int Tier => 1;

//         public override float SureHitRange => 2000f;

//         public override bool ClosedDomain => false;
//         const int frames = 100;
//         const int frames_per_tick = 1;

//         int frame = 0;
//         int frameTime = 0;
//         public Texture2D currentTexture;

//         public override bool ExpandCondition(NPC npc)
//         {
//             if (npc.GetGlobalNPC<NPCDomainController>().domainCounter == 0 && npc.life > npc.lifeMax * 0.80 && npc.life <= npc.lifeMax * 0.90) return true;
//             if (npc.GetGlobalNPC<NPCDomainController>().domainCounter == 1 && npc.life > npc.lifeMax * 0.01 && npc.life <= npc.lifeMax * 0.05) return true;

//             return false;
//         }

//         public override void Update()
//         {
//             base.Update();

//             frameTime++;
//             if (frameTime >= frames_per_tick)
//             {
//                 frameTime = 0;
//                 frame++;
//                 if (frame >= frames)
//                 {
//                     frame = 0;
//                 }
//             }
//         }
        
//         public override void Draw(SpriteBatch spriteBatch)
//         {
//             currentTexture = ModContent.Request<Texture2D>($"sorceryFight/Content/DomainExpansions/NPCDomains/AshenedPillarsOfCalamity/{frame}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

//             Rectangle src = new Rectangle(0, 0, currentTexture.Width, currentTexture.Height);
//             spriteBatch.Draw(currentTexture, center - Main.screenPosition, src, Color.White, 0f, src.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
//         }

//         public override void SureHitEffect(Player player)
//         {
//             player.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 1);
//         }

//         public override void OnClose()
//         {
//             frame = 0;
//             frameTime = 0;
//             currentTexture = null;
//         }
//     }
// }
