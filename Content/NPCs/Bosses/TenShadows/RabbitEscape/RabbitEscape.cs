using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using System;
using System.IO;

namespace sorceryFight.Content.NPCs.Bosses.TenShadows.RabbitEscape
{
    [AutoloadBossHead]
    public class RabbitEscape: ModNPC
    {
        public static int FRAME_COUNT = 7;
        public static int TICKS_PER_FRAME = 5;
        private enum ActionState
        {
            Chase,
            Shardscape,
            Moonlord,
            Circling,
            Boulder,
            Stunned
        }
        private enum AnimationState
        {
            Idle,
            Chase,
            Jump
        }
        public Rectangle arenaRect;
        public bool arenaSpawned = false;
        public Color arenaColor = Color.Goldenrod;
        public bool MoonlordSpawned = false;
        public ref float AI_State => ref NPC.ai[0];
		public ref float AI_Timer => ref NPC.ai[1];
		public ref float AI_FlutterTime => ref NPC.ai[2];
        public override bool CheckActive() => false;
        public override void SetStaticDefaults() 
        {
			Main.npcFrameCount[Type] = 7;
		}

        public override void SetDefaults() 
        {
			NPC.width = 30; 
			NPC.height = 30; 
			NPC.aiStyle = -1; 
			NPC.damage = 7; 
			NPC.defense = 2; 
			NPC.lifeMax = 7500; 
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.value = 25f;
            NPC.boss = true;
            NPC.knockBackResist = 1f;
		}

        public override void AI() 
        {
            NPC.spriteDirection = NPC.velocity.X > 0 ? -1 : 1;
			switch (AI_State) {
				case (float)ActionState.Chase:
					Chase();
					break;
                case (float)ActionState.Shardscape:
					Shardscape();
					break;
                case (float)ActionState.Moonlord:
					Moonlord();
					break;
                case (float)ActionState.Circling:
					//Circling();
					break;
                case (float)ActionState.Boulder:
					//Boulder();
					break;
                case (float)ActionState.Stunned:
					Stunned();
					break;
			}

            if(!arenaSpawned && Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.TargetClosest();
                arenaRect = new Rectangle((int)(Main.player[NPC.target].Center.X - 312f), (int)(Main.player[NPC.target].Center.Y - 368f), 624, 400);
                arenaSpawned = true;
                NPC.netUpdate = true;
            }
            foreach(Player player in Main.ActivePlayers)
            {
                if (!arenaRect.Intersects(player.Hitbox)) continue;
                player.wingTime = 0f;
                player.wings = 0;
                arenaCollision(ref player.position, ref player.velocity, player.width, player.height, arenaRect);
            }
            if (arenaRect.Intersects(NPC.Hitbox))
                arenaCollision(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, arenaRect);
		}


        public void Chase()
        {
            if (!NPC.HasValidTarget)
                NPC.TargetClosest();
            Vector2 direction = Main.player[NPC.target].Center - NPC.Center;
            direction.Normalize();
            NPC.velocity.X = direction.X * 3.7f;
            if (NPC.collideX && NPC.velocity.Y == 0f)
            {
                NPC.velocity.Y = -5f;
            }
            if (Main.player[NPC.target].position.Y + Main.player[NPC.target].height <= NPC.position.Y && NPC.velocity.Y == 0f)
            {
                NPC.velocity.Y = -5f * 1.2f;
            }
            if (AI_Timer++ >= 300)
            {
                AI_Timer = 0;
                NPC.velocity = Vector2.Zero;
                AI_State = (float)ActionState.Shardscape;
                return;
            }
        }
        public void Shardscape()
        {
            NPC.dontTakeDamage = true;
            NPC.Center = Main.player[NPC.target].Center + Vector2.UnitY * -750f;
            if(AI_Timer == 0 | AI_Timer % 240 == 0)
            {
                int avoid = Main.rand.Next(8);
                for(int i = 0; i < 8; i++)
                {
                    if(i == avoid) continue;
                    float t = i / 7f;
                    float shardPos = Vector2.Lerp(new Vector2(arenaRect.Left + 41f, arenaRect.Bottom), new Vector2(arenaRect.Right - 41f, arenaRect.Bottom), t).X;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), new Vector2(shardPos, arenaRect.Bottom - 81f), Vector2.Zero, ModContent.ProjectileType<RabbitShardscape_Indicator>(), 0, 0, 255);
                } 
            }

            
            if (AI_Timer++ >= 719) //stupid lazy chud
            {
                AI_Timer = 0;
                AI_State = (float)ActionState.Moonlord;
                return;
            }
        }

        public void Moonlord()
        {
            NPC.dontTakeDamage = true;
            NPC.Center = Main.player[NPC.target].Center + Vector2.UnitY * -750f;
            if(!MoonlordSpawned)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), arenaRect.Center() + Vector2.UnitY * 30f, Vector2.Zero, ModContent.ProjectileType<RabbitMoonlord>(), 0, 0, 255);
                MoonlordSpawned = true;
            }

            
            if (AI_Timer++ >= 900)
            {
                AI_Timer = 0;
                MoonlordSpawned = false;
                //AI_State = (float)ActionState.Circling;
                return;
            }
        }
        public void Stunned()
        {
            if (AI_Timer++ >= 180)
            {
                AI_Timer = 0;
                AI_State = (float)ActionState.Chase;
                return;
            }
        }

        public override void FindFrame(int frameHeight) 
        {
			NPC.spriteDirection = NPC.direction;
			switch (AI_State) {
				case (float)ActionState.Chase:
                    NPC.frameCounter++;
                    if(NPC.frameCounter % 5 == 0 && NPC.frameCounter > 0)
                    {
                        NPC.frame.Y = (int)NPC.frameCounter / 5 * frameHeight;
                    }
                    if(NPC.frameCounter >= 30)
                    {
                        NPC.frameCounter = 0;
                    }

                    if(NPC.velocity.Y > 0f)
                    {
                        NPC.frame.Y = 6 * frameHeight;
                    }
					break;
			}
		}
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Rectangle screenRect = new Rectangle(arenaRect.X - (int)Main.screenPosition.X, arenaRect.Y - (int)Main.screenPosition.Y, arenaRect.Width, arenaRect.Height);
            DrawBorder(spriteBatch, screenRect, arenaColor, 4f);
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }

        #region Arena
        public Rectangle ArenaRect
        {
            get => new Rectangle((int)NPC.ai[0], (int)NPC.ai[1], 
                                (int)NPC.ai[2], (int)NPC.ai[3]);
            set
            {
                NPC.localAI[0] = value.X;
                NPC.localAI[1] = value.Y;
                NPC.localAI[2] = value.Width;
                NPC.localAI[3] = value.Height;
            }
        }  

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(arenaRect.X);
            writer.Write(arenaRect.Y);
            writer.Write(arenaRect.Width);
            writer.Write(arenaRect.Height);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            arenaRect = new Rectangle(
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32(),
                reader.ReadInt32());
        }  
        public static void DrawBorder(SpriteBatch spriteBatch, Rectangle rect, Color color, float thickness)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, 
                rect.Width, (int)thickness), color);
            spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Bottom - (int)thickness, 
                rect.Width, (int)thickness), color);
            spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, 
                (int)thickness, rect.Height), color);
            spriteBatch.Draw(pixel, new Rectangle(rect.Right - (int)thickness, rect.Y, 
                (int)thickness, rect.Height), color);
        }
        public void arenaCollision(ref Vector2 pos, ref Vector2 velocity, int width, int height, Rectangle arena)
        {
            if (pos.X < arena.X)
            {
                pos.X = arena.X;
                velocity.X = Math.Max(velocity.X, 0f);
            }
            if (pos.X + width > arena.Right)
            {
                pos.X = arena.Right - width;
                velocity.X = Math.Min(velocity.X, 0f);
            }
            if (pos.Y < arena.Y)
            {
                pos.Y = arena.Y;
                velocity.Y = Math.Max(velocity.Y, 0f);
            }

            if (pos.Y + height > arena.Bottom)
            {
                pos.Y = arena.Bottom - height;
                velocity.Y = Math.Min(velocity.Y, -0.1f);
            }
        }

        #endregion


    }

    

}