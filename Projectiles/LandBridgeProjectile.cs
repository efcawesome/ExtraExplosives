﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static ExtraExplosives.GlobalMethods;

namespace ExtraExplosives.Projectiles
{
	public class LandBridgeProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("LandBridge");
		}

		public override void SetDefaults()
		{
			projectile.tileCollide = true;
			projectile.width = 5;
			projectile.height = 5;
			projectile.aiStyle = 16;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.timeLeft = 100;
		}

		public override bool OnTileCollide(Vector2 old)
		{

			projectile.position.Y -= 2;


			projectile.velocity = Vector2.Zero;

			projectile.aiStyle = 0;
			return true;
		}

		public override void Kill(int timeLeft)
		{
			//Create Bomb Sound
			Main.PlaySound(SoundID.Item14, (int)projectile.Center.X, (int)projectile.Center.Y);

			//Create Bomb Damage
			//ExplosionDamage(5f, projectile.Center, 70, 20, projectile.owner); //No damage needed

			//Create Bomb Explosion
			CreateExplosion(projectile.Center);

			//Create Bomb Dust
			CreateDust(projectile.Center, 500);
		}

		private void CreateExplosion(Vector2 position)
		{

			int height = 10; //Height of arena

			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				return;
			}

			for (int x = 3; x <= Main.maxTilesX - 3; x++)
			{
				for (int y = height; y >= 0; y--)
				{
					int xPosition = x;
					int yPosition = (int)(-y + position.Y / 16.0f);



					//The following happens whether the block is breakable or not as the following methods cannot break or replace blocks that already exist.
					if (!OutOfBounds(xPosition, yPosition))
					{
						//Breaks Liquid
						Main.tile[xPosition, yPosition].liquid = Tile.Liquid_Water;
						WorldGen.SquareTileFrame(xPosition, yPosition, true);

						//Place Outline

						if ((y == 0) || y == height)
						{
							WorldGen.PlaceTile(xPosition, yPosition, TileID.Platforms);
						}
						if (y >= 1 && y < height) { WorldGen.PlaceWall(xPosition, yPosition, WallID.GrayBrick); }
						if (y == height / 2 && x % 6 == 0) { WorldGen.PlaceTile(xPosition, yPosition, TileID.Torches); }
						NetMessage.SendTileSquare(-1, xPosition, yPosition, 1);
					}
				}

			}
		}

		private void CreateDust(Vector2 position, int amount)
		{
			Dust dust;
			Vector2 updatedPosition;

			for (int i = 0; i <= amount; i++)
			{
				if (Main.rand.NextFloat() < DustAmount)
				{
					//---Dust 1---
					if (Main.rand.NextFloat() < 1f)
					{
						updatedPosition = new Vector2(position.X - 2000 / 2, position.Y - 2000 / 2);

						dust = Main.dust[Terraria.Dust.NewDust(updatedPosition, 2000, 2000, 186, 0f, 0f, 0, new Color(159, 0, 255), 5f)];
						dust.noGravity = true;
						dust.shader = GameShaders.Armor.GetSecondaryShader(88, Main.LocalPlayer);
						dust.fadeIn = 3f;
					}

					//---Dust 2---
					if (Main.rand.NextFloat() < 1f)
					{
						updatedPosition = new Vector2(position.X - 2000 / 2, position.Y - 2000 / 2);

						dust = Main.dust[Terraria.Dust.NewDust(updatedPosition, 2000, 2000, 186, 0f, 0f, 0, new Color(0, 17, 255), 5f)];
						dust.noGravity = true;
						dust.shader = GameShaders.Armor.GetSecondaryShader(88, Main.LocalPlayer);
						dust.fadeIn = 3f;
					}

					//---Dust 3---
					if (Main.rand.NextFloat() < 1f)
					{
						updatedPosition = new Vector2(position.X - 2000 / 2, position.Y - 2000 / 2);

						dust = Main.dust[Terraria.Dust.NewDust(updatedPosition, 2000, 2000, 186, 0f, 0f, 0, new Color(255, 0, 150), 5f)];
						dust.noGravity = true;
						dust.shader = GameShaders.Armor.GetSecondaryShader(88, Main.LocalPlayer);
						dust.fadeIn = 3f;
					}
				}
			}
		}

		//This returns true if the arena is going out of bounds
		private bool OutOfBounds(int posX, int posY)
		{
			//Tests If Tile Is OutOfBounds
			if (posX <= 2.5f || posY <= 2.5f || posX > Main.maxTilesX || posY > Main.maxTilesY)
			{
				return true;
			}
			return false;
		}
	}
}