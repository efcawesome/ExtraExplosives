using ExtraExplosives.Items;
using ExtraExplosives.Items.Accessories;
using ExtraExplosives.Items.Accessories.AnarchistCookbook;
using ExtraExplosives.NPCs.CaptainExplosiveBoss.BossProjectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace ExtraExplosives.NPCs.CaptainExplosiveBoss
{
	[AutoloadBossHead]
	public class CaptainExplosiveBossAt0 : ModNPC
	{
		//Variables:
		//private static int hellLayer => Main.maxTilesY - 200;

		private const int sphereRadius = 300;

		private float moveCool
		{
			get => npc.ai[1];
			set => npc.ai[1] = value;
		}


		//public virtual string[] AltTexturesCap => new string[0];

		private int moveTime = 200;
		private int moveTimer = 60;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Captain Explosive");
			Main.npcFrameCount[npc.type] = 4;

		}

		//public override void AutoStaticDefaults()
		//{
		//	//AltTextures[0] = "NPCs/CaptainExplosiveBoss/CaptainExplosiveBoss";
		//	//AltTextures[1] = "NPCs/CaptainExplosiveBoss/CaptainExplosiveBossDamaged";
		//}

		public override void SetDefaults()
		{
			npc.aiStyle = -1;
			npc.lifeMax = 9800;
			npc.damage = 1000;
			npc.defense = 999999;
			npc.knockBackResist = 0f;
			npc.width = 200;
			npc.height = 200;
			npc.value = Item.buyPrice(0, 20, 0, 0);
			npc.npcSlots = 15f;
			npc.boss = true;
			npc.lavaImmune = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.buffImmune[24] = true;
			music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/CaptainExplosiveMusic");

			bossBag = ItemType<CaptainExplosiveTreasureBag>();
			npc.immortal = true;

			drawOffsetY = 50f;
		}

		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.625f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.6f);
		}

		public override bool CheckDead()
		{
			for (int k = 0; k < 8; k++)
			{
				Vector2 pos = npc.position + new Vector2(Main.rand.Next(npc.width - 8), Main.rand.Next(npc.height / 2));
				Gore.NewGore(pos, npc.velocity, mod.GetGoreSlot("Gores/CaptainExplosiveBoss/gore2"), 1f);
			}
			for (int k = 0; k < 8; k++)
			{
				Vector2 pos = npc.position + new Vector2(Main.rand.Next(npc.width - 8), Main.rand.Next(npc.height / 2));
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/CaptainExplosiveBoss/gore1"), 1f);
			}

			for (int g = 0; g < 20; g++)
			{
				int goreIndex = Gore.NewGore(new Vector2(npc.position.X + (float)(npc.width / 2) - 24f, npc.position.Y + (float)(npc.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
				Main.gore[goreIndex].scale = 1.5f;
				Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
				Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;
				goreIndex = Gore.NewGore(new Vector2(npc.position.X + (float)(npc.width / 2) - 24f, npc.position.Y + (float)(npc.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
				Main.gore[goreIndex].scale = 1.5f;
				Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X - 1.5f;
				Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;
				goreIndex = Gore.NewGore(new Vector2(npc.position.X + (float)(npc.width / 2) - 24f, npc.position.Y + (float)(npc.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
				Main.gore[goreIndex].scale = 1.5f;
				Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
				Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y - 1.5f;
				goreIndex = Gore.NewGore(new Vector2(npc.position.X + (float)(npc.width / 2) - 24f, npc.position.Y + (float)(npc.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
				Main.gore[goreIndex].scale = 1.5f;
				Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X - 1.5f;
				Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y - 1.5f;
			}
			return true;
		}

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			npc.immortal = false;
			npc.StrikeNPCNoInteraction(100, 0, -npc.direction);
		}

		public override void AI()
		{
			npc.life = 1;

			//check for the players death
			Player player = Main.player[npc.target];
			if (!player.active || player.dead)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead)
				{
					npc.velocity = new Vector2(0f, -15f);
					if (npc.timeLeft > 120)
					{
						npc.timeLeft = 120;
					}
					return;
				}
			}

			npc.TargetClosest(true);
			Vector2 vector89 = new Vector2(npc.Center.X, npc.Center.Y);
			float num716 = Main.player[npc.target].Center.X - vector89.X;
			float num717 = Main.player[npc.target].Center.Y - vector89.Y;
			float num718 = (float)Math.Sqrt((double)(num716 * num716 + num717 * num717));
			float num719 = 12f;
			num718 = num719 / num718;
			num716 *= num718;
			num717 *= num718;
			npc.velocity.X = ((npc.velocity.X * 100f + num716) / 101f) * 1 + .2f;
			npc.velocity.Y = ((npc.velocity.Y * 100f + num717) / 101f) * 1 + .2f;

			if (npc.ai[3] == 500)
			{
				npc.immortal = false;
				npc.StrikeNPCNoInteraction(100, 0, -npc.direction);
			}

			npc.ai[3]++;
			Main.NewText(npc.velocity);
		}

		public override void NPCLoot()  // What will drop when the npc is killed?
		{
			if (Main.expertMode)    // Expert mode only loot
			{
				npc.DropBossBags(); // Boss bag
			}
			int drop = Main.rand.NextBool() ? ItemType<BombardEmblem>() : ItemType<RandomFuel>();   // which item will 100% drop
			int dropChance = drop == ItemType<BombardEmblem>() ? ItemType<RandomFuel>() : ItemType<BombardEmblem>();    // find the other item
			npc.DropItemInstanced(npc.position, new Vector2(npc.width, npc.height), drop);  // drop the confirmed item
			if (Main.rand.Next(7) == 0) npc.DropItemInstanced(npc.position, new Vector2(npc.width, npc.height), dropChance);    // if the roll is sucessful drop the other
		}


		public override void FindFrame(int frameHeight)
		{

			npc.frameCounter += 2.0; //change the frame speed
			npc.frameCounter %= 100.0; //How many frames are in the animation
			npc.frame.Y = frameHeight * ((int)npc.frameCounter % 16 / 4); //set the npc's frames here

		}

		//public override void HitEffect(int hitDirection, double damage)
		//{
		//	if (npc.life <= 0)
		//	{
				
		//	}
		//}
	}
}