using System;
using ExtraExplosives.Dusts;
using ExtraExplosives.Items.Accessories.BombardierClassAccessories;
using ExtraExplosives.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExtraExplosives
{
    public class ExtraExplosivesGlobalProjectile : GlobalProjectile
    {
	    public override bool InstancePerEntity { get; } = true;
        public override bool CloneNewInstances { get; } = true;

        private bool _upVelocity = false;
        private bool _setFuze = false;
        private bool clone = false;
        private bool colliding = false;
        private Vector2 constVelocity;

        public override bool PreAI(Projectile projectile)
        {
	        if (projectile.type == ModContent.ProjectileType<NukeProjectileBomb>() ||
	            projectile.type == ModContent.ProjectileType<NukeProjectilePlane>()) return true;
			if (projectile.type == ModContent.ProjectileType<NPCProjectile>()) return true;
	        ExtraExplosivesPlayer mp = Main.player[projectile.owner].EE();
            if (!_upVelocity &&
                mp.LightweightBombshells &&
                mp.LightweightBombshellsActive &&
                projectile.aiStyle == 16)	// Lightweight Bombshells (working)
            {
	            //Main.NewText(2f * Main.player[projectile.owner].EE().LightweightBombshellVelocity);
	            projectile.velocity *= 2f;
	            if (mp.AnarchistCookbook)
		            projectile.velocity *= (180 - Main.player[projectile.owner].EE().LightweightBombshellVelocity) / 100;
	            Main.NewText("Velocity Up Run Once");
                _upVelocity = true;
            }

            if (!_setFuze &&
                mp.AnarchistCookbook &&
                mp.ShortFuseActive &&
                projectile.aiStyle == 16)
            {
	            //Main.NewText(mp.ShortFuseTime);
	            projectile.timeLeft = (int)(projectile.timeLeft * mp.ShortFuseTime);
	            _setFuze = true;
            }
            
            return base.PreAI(projectile);
        }


        private bool _aiFlag = false;
        private bool? _stickyGunpowderFlag = null;	// used to get the default collide value
        public override void AI(Projectile projectile)
        {
	        if (projectile.type == ModContent.ProjectileType<NukeProjectileBomb>() ||
	            projectile.type == ModContent.ProjectileType<NukeProjectilePlane>()) return;
			if (projectile.type == ModContent.ProjectileType<NPCProjectile>()) return;
			ExtraExplosivesPlayer mp = Main.player[projectile.owner].EE();
	        if (projectile.aiStyle == 16 &&
	            !projectile.arrow &&
	            !projectile.ranged &&
	            projectile.owner != 255)
	        {
		        if (!_aiFlag)
		        {
			        if (mp.ShortFuse && mp.ShortFuseActive)	projectile.timeLeft = (int)(projectile.timeLeft / 1.5f);// Short Fuze, (working)
			        if (mp.AlienExplosive && mp.AnarchistCookbook)
			        {
				        constVelocity = projectile.velocity;
			        }
			        _aiFlag = true;
		        }
		        
		        if (!mp.StickyGunpowder &&  _stickyGunpowderFlag != null)	// Sticky gunpowder reset
		        {
			        projectile.tileCollide = (bool)_stickyGunpowderFlag;
		        }
		        else if (mp.StickyGunpowder &&
		                 mp.StickyGunpowderActive) // Sticky Gunpowder (working)
		        {
			        if (_stickyGunpowderFlag == null) _stickyGunpowderFlag = projectile.tileCollide;	// Get the standard collide value
			        projectile.tileCollide = false;	// necessary for the code to work (for some reason)
			        try // This is vanilla code, i comprehend it, so it has been commented
			        {
				        int minX = (int) (projectile.position.X / 16f) - 1;
				        int maxX = (int) ((projectile.position.X + (float) projectile.width) / 16f) + 2;
				        int maxY = (int) (projectile.position.Y / 16f) - 1;
				        int minY = (int) ((projectile.position.Y + (float) projectile.height) / 16f) + 2;
				        if (minX < 0)	// If the projectile has gone outside the bounds of the world on the left
				        {
					        minX = 0;		// Set the x back to the lowest possible value
				        }

				        if (maxX > Main.maxTilesX)	// if the projectile has gone beyond the right of the world
				        {
					        maxX = Main.maxTilesX;	// set x to the highest possible value
				        }

				        if (maxY < 0)		// if the projectile has gone above the world
				        {
					        maxY = 0;	// set y to the lowest possible value
				        }

				        if (minY > Main.maxTilesY)	// if the projectile has gone below the world
				        {
					        minY = Main.maxTilesY;	// set y to the highest possible value
				        }

				        Vector2 collisionPoint = default(Vector2);	// collision position
				        for (int currentX = minX; currentX < maxX; currentX++)	// cycle through x cords
				        {
					        for (int currentY = maxY; currentY < minY; currentY++)	// cycle through y cords
					        {
						        if (Main.tile[currentX, currentY] != null && Main.tile[currentX, currentY].nactive() &&	// isnt null and is nactive and
						            (Main.tileSolid[Main.tile[currentX, currentY].type] ||		// (is solid
						             (Main.tileSolidTop[Main.tile[currentX, currentY].type] &&	//  or (has a solid top and
						              Main.tile[currentX, currentY].frameY == 0))) // has a frameY of 0))
						        {
							        collisionPoint.X = currentX * 16;	// get the point in world cords from tile cords
							        collisionPoint.Y = currentY * 16;	// get the point in world cords from tile cords, but the y
							        if (projectile.position.X + (float) projectile.width - 4f > collisionPoint.X &&	// projectile touches the point
							            projectile.position.X + 4f < collisionPoint.X + 16f &&	//touches the point but different on the x
							            projectile.position.Y + (float) projectile.height - 4f > collisionPoint.Y &&	// again touches the point
							            projectile.position.Y + 4f < collisionPoint.Y + 16f)	// and again touches the point but different but on the y
							        {
								        projectile.velocity.X = 0f;	// x velocity 0
								        projectile.velocity.Y = 0f;	// y velocity 0
							        }
						        }
					        }
				        }
			        }
			        catch	// if this breaks, you did something very wrong so print out a message informing the player how they should feel
			        {
				        Main.NewText("You killed the collision detector, it had a family, and you just killed it; You monster.");
			        }
		        }
	        }

	        if (mp.SupernaturalBomb &&
	            projectile.aiStyle == 16 && 
	            Array.IndexOf(StaticMethods.DoNotHome, projectile.type) == -1)
	        {
		        //dust
				float num248 = 0f;
				float num249 = 0f;

				Vector2 position71 = new Vector2(projectile.position.X + 3f + num248, projectile.position.Y + 3f + num249) - projectile.velocity * 0.5f;
				int width67 = projectile.width - 8;
				int height67 = projectile.height - 8;
		 		Color newColor = default(Color);
				int num250 = Dust.NewDust(position71, width67, height67, 59, 0f, 0f, 100, newColor, 1f);
				Dust dust3 = Main.dust[num250];
				dust3.scale *= 2f + (float)Main.rand.Next(10) * 0.1f;
				dust3 = Main.dust[num250];
				dust3.velocity *= 0.2f;
				Main.dust[num250].noGravity = true;
				Vector2 position72 = new Vector2(projectile.position.X + 3f + num248, projectile.position.Y + 3f + num249) - projectile.velocity * 0.5f;
				int width68 = projectile.width - 8;
				int height68 = projectile.height - 8;
				newColor = default(Color);
				num250 = Dust.NewDust(position72, width68, height68, 206, 0f, 0f, 100, newColor, 0.5f);
				Main.dust[num250].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
				dust3 = Main.dust[num250];
				dust3.velocity *= 0.05f;


				//Actual AI code
				float num132 = (float)Math.Sqrt((double)(projectile.velocity.X * projectile.velocity.X + projectile.velocity.Y * projectile.velocity.Y));
				float num133 = projectile.localAI[0];
				if (num133 == 0f)
				{
					projectile.localAI[0] = num132;
					num133 = num132;
				}
				float num134 = projectile.position.X;
				float num135 = projectile.position.Y;
				float num136 = 600f;
				bool flag3 = false;
				int num137 = 0;
				if (projectile.ai[1] == 0f)
				{
					for (int num138 = 0; num138 < 200; num138++)
					{
						if (Main.npc[num138].CanBeChasedBy(this, false) && (projectile.ai[1] == 0f || projectile.ai[1] == (float)(num138 + 1)))
						{
							float num139 = Main.npc[num138].position.X + (float)(Main.npc[num138].width / 2);
							float num140 = Main.npc[num138].position.Y + (float)(Main.npc[num138].height / 2);
							float num141 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num139) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num140);
							if (num141 < num136 && Collision.CanHit(new Vector2(projectile.position.X + (float)(projectile.width / 2), projectile.position.Y + (float)(projectile.height / 2)), 1, 1, Main.npc[num138].position, Main.npc[num138].width, Main.npc[num138].height))
							{
								num136 = num141;
								num134 = num139;
								num135 = num140;
								flag3 = true;
								num137 = num138;
							}
						}
					}
					if (flag3)
					{
						projectile.ai[1] = (float)(num137 + 1);
					}
					flag3 = false;
				}
				if (projectile.ai[1] > 0f)
				{
					int num142 = (int)(projectile.ai[1] - 1f);
					if (Main.npc[num142].active && Main.npc[num142].CanBeChasedBy(this, true) && !Main.npc[num142].dontTakeDamage)
					{
						float num143 = Main.npc[num142].position.X + (float)(Main.npc[num142].width / 2);
						float num144 = Main.npc[num142].position.Y + (float)(Main.npc[num142].height / 2);
						if (Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num143) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num144) < 1000f)
						{
							flag3 = true;
							num134 = Main.npc[num142].position.X + (float)(Main.npc[num142].width / 2);
							num135 = Main.npc[num142].position.Y + (float)(Main.npc[num142].height / 2);
						}
					}
					else
					{
						projectile.ai[1] = 0f;
					}
				}
				if (!projectile.friendly)
				{
					flag3 = false;
				}
				if (flag3)
				{
					float num145 = num133;
					Vector2 vector10 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
					float num146 = num134 - vector10.X;
					float num147 = num135 - vector10.Y;
					float num148 = (float)Math.Sqrt((double)(num146 * num146 + num147 * num147));
					num148 = num145 / num148;
					num146 *= num148;
					num147 *= num148;
					int num149 = 8;
					projectile.velocity.X = (projectile.velocity.X * (float)(num149 - 1) + num146) / (float)num149;
					projectile.velocity.Y = (projectile.velocity.Y * (float)(num149 - 1) + num147) / (float)num149;
				}

				projectile.rotation = projectile.velocity.ToRotation();
				//projectile.spriteDirection = projectile.direction;
	        }

	        else
	        {
		        if (mp.AlienExplosive &&
		            mp.AnarchistCookbook &&
		            projectile.aiStyle == 16 &&
		            !projectile.ranged &&
		            !projectile.thrown &&
		            !colliding)
		        {
			        projectile.velocity = constVelocity;
		        }
	        }
	        
	        base.AI(projectile);
        }

        public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
        {
	        if (Main.player[projectile.owner].EE().AlienExplosive)
	        {
		        colliding = true;
		        projectile.velocity = Vector2.Zero;
		        return true;
	        }
	        return base.OnTileCollide(projectile, oldVelocity);
        }

        private bool Active { get; set; } = false;

        public override bool PreKill(Projectile projectile, int timeLeft)
        {
	        int type = projectile.type;	// Dont only so i didnt have to rename the variables below (copied from vanilla dont @ me), inefficient but who cares
	        if (type == ProjectileID.Bomb || type == ProjectileID.Dynamite || type == ProjectileID.StickyBomb ||
	            type == ProjectileID.Explosives || type == ProjectileID.GrenadeII || type == ProjectileID.RocketII ||
	            type == ProjectileID.ProximityMineII || type == ProjectileID.GrenadeIV || type == ProjectileID.RocketIV ||
	            type == ProjectileID.ProximityMineIV || type == ProjectileID.RocketSnowmanII || type == ProjectileID.RocketSnowmanIV ||
	            type == ProjectileID.StickyDynamite || type == ProjectileID.BouncyBomb ||
	            type == ProjectileID.BombFish || type == ProjectileID.BouncyDynamite)
	        {
		        Main.NewText("Kill vanilla projectile");
		        if (!Main.player[projectile.owner].EE().BombardEmblem)
		        {
			        return base.PreKill(projectile, timeLeft);
		        }
		        // If its a vanilla explosive we are gonna emulate its behavior so we can decide if it destroys blocks
		        // I took like ~ hours to rewrite this entire block of code so its legible, it was painful
		        /*
		        int radius = 3;
				if (type == ProjectileID.Bomb || type == ProjectileID.StickyBomb || type == ProjectileID.BouncyBomb || type == ProjectileID.BombFish)
					radius = 4;

				if (type == ProjectileID.Dynamite || type == ProjectileID.StickyDynamite || type == ProjectileID.BouncyDynamite)
					radius = 7;

				if (type == ProjectileID.GrenadeIV || type == ProjectileID.RocketIV || type == ProjectileID.ProximityMineIV || type == ProjectileID.RocketSnowmanIV)
					radius = 5;

				if (type == ProjectileID.Explosives)
					radius = 10;

				int minX = (int)(projectile.position.X / 16f - (float)radius);
				int maxX = (int)(projectile.position.X / 16f + (float)radius);
				int minY = (int)(projectile.position.Y / 16f - (float)radius);
				int maxY = (int)(projectile.position.Y / 16f + (float)radius);
				if (minX < 0)
					minX = 0;

				if (maxX > Main.maxTilesX)
					maxX = Main.maxTilesX;

				if (minY < 0)
					minY = 0;

				if (maxY > Main.maxTilesY)
					maxY = Main.maxTilesY;

				bool tilesPresent = false;
				for (int i = minX; i <= maxX; i++) {
					for (int j = minY; j <= maxY; j++) {
						float x = Math.Abs(i - projectile.position.X / 16f);
						float y = Math.Abs(j - projectile.position.Y / 16f);
						double dist = Math.Sqrt(x * x + y * y);
						if (dist < radius && Main.tile[i, j] != null && Main.tile[i, j].wall == 0) {
							tilesPresent = true;
							break;
						}
					}
				}

				AchievementsHelper.CurrentlyMining = true;
				for (int i = minX; i <= maxX; i++) 
				{
					for (int j = minY; j <= maxY; j++) 
					{
						float x = Math.Abs((float)i - projectile.position.X / 16f);
						float y = Math.Abs((float)j - projectile.position.Y / 16f);
						double dist = Math.Sqrt(x * x + y * y);
						if (dist < radius)
							continue;

						bool tileDestroyable = true;
						if (Main.tile[i, j] != null && Main.tile[i, j].active()) 
						{
							tileDestroyable = true;
							if (Main.tileDungeon[Main.tile[i, j].type] || Main.tile[i, j].type == TileID.Dressers ||
							    TileID.Sets.BasicChest[Main.tile[i, j].type] || Main.tile[i, j].type == TileID.DemonAltar ||
							    Main.tile[i, j].type == TileID.Cobalt || Main.tile[i, j].type == TileID.Mythril ||
							    Main.tile[i, j].type == TileID.Adamantite || Main.tile[i, j].type == TileID.LihzahrdBrick ||
							    Main.tile[i, j].type == TileID.LihzahrdAltar || Main.tile[i, j].type == TileID.Palladium ||
							    Main.tile[i, j].type == TileID.Orichalcum || Main.tile[i, j].type == TileID.Titanium ||
							    Main.tile[i, j].type == TileID.Chlorophyte || Main.tile[i, j].type == TileID.DesertFossil)
								tileDestroyable = false;

							if (!Main.hardMode && Main.tile[i, j].type == TileID.Hellstone)
								tileDestroyable = false;

							if (tileDestroyable) {
								WorldGen.KillTile(i, j);
								if (!Main.tile[i, j].active() && Main.netMode != 0)
									NetMessage.SendData(17, -1, -1, null, 0, i, j);
							}
						}

						if (!tileDestroyable)
							continue;

						for (int iWall = i - 1; iWall <= i + 1; iWall++) {
							for (int jWall = j - 1; jWall <= j + 1; jWall++) {
								if (Main.tile[iWall, jWall] != null && Main.tile[iWall, jWall].wall > 0 && tilesPresent) {
									WorldGen.KillWall(iWall, jWall);
									if (Main.tile[iWall, jWall].wall == 0 && Main.netMode != 0)
										NetMessage.SendData(17, -1, -1, null, 2, iWall, jWall);
								}
							}
						}
					}
				}

				AchievementsHelper.CurrentlyMining = false;
				*/
		        
		        void ExplosionDamage()
		        {
			        int radius = 3;
			        if (type == ProjectileID.Bomb || type == ProjectileID.StickyBomb || type == ProjectileID.BouncyBomb || type == ProjectileID.BombFish)
				        radius = 4;

			        if (type == ProjectileID.Dynamite || type == ProjectileID.StickyDynamite || type == ProjectileID.BouncyDynamite)
				        radius = 7;

			        if (type == ProjectileID.GrenadeIV || type == ProjectileID.RocketIV || type == ProjectileID.ProximityMineIV || type == ProjectileID.RocketSnowmanIV)
				        radius = 5;

			        if (type == ProjectileID.Explosives)
				        radius = 10;
			        bool crit = false;
			        if (Main.player[projectile.owner].EE().ExplosiveCrit > Main.rand.Next(1, 101)) crit = true;
			        foreach (NPC npc in Main.npc)
			        {
				        float dist = Vector2.Distance(npc.Center, projectile.Center);
				        if (dist/16f <= radius)
				        {
					        int dir = (dist > 0) ? 1 : -1;
					        npc.StrikeNPC(projectile.damage, projectile.knockBack, dir, crit);
				        }
			        }

			        foreach (Player player in Main.player)
			        {
				        if (player == null || player.whoAmI == 255 || !player.active) return;
				        if (!CanHitPlayer(projectile, player)) continue;
				        if (player.EE().BlastShielding &&
				            player.EE().BlastShieldingActive) continue;
				        float dist = Vector2.Distance(player.Center, projectile.Center);
				        int dir = (dist > 0) ? 1 : -1;
				        if (dist/16f <= radius)
				        {
					        player.Hurt(PlayerDeathReason.ByProjectile(player.whoAmI, projectile.whoAmI), (int)(projectile.damage * (crit ? 1.5 : 1)), dir);
					        player.hurtCooldowns[0] += 15;
				        }
				        if (Main.netMode != 0)
				        {
					        NetMessage.SendPlayerHurt(projectile.owner, PlayerDeathReason.ByProjectile(player.whoAmI, projectile.whoAmI), (int)(projectile.damage * (crit ? 1.5 : 1)), dir, crit, pvp: true, 0);
				        }
			        }
            
		        }
		        
		        ExplosionDamage();
		        return false;
	        }
	        return base.PreKill(projectile, timeLeft);
        }

        public override void Kill(Projectile projectile, int timeLeft)
        {
	        if (projectile.type == ModContent.ProjectileType<NukeProjectileBomb>() ||
	            projectile.type == ModContent.ProjectileType<NukeProjectilePlane>()) return;
	        if (projectile.aiStyle == 16)
	        {
		        Vector2 npcPos = new Vector2();
		        ExtraExplosivesPlayer mp = Main.player[projectile.owner].EE();
		        if (mp.RandomFuel &&
		            mp.RandomFuelActive)	// Random fuel (working)
		        {
			        
			        if (!clone)
			        {
				        Projectile proj = Projectile.NewProjectileDirect
				        (
					        projectile.position,
					        projectile.velocity,
					        projectile.type,
					        projectile.damage,
					        projectile.knockBack,
					        projectile.owner,
					        projectile.ai[0],
					        projectile.ai[1]
					    );
				        proj.timeLeft = 10;
				        proj.velocity = projectile.velocity;
				        proj.GetGlobalProjectile<ExtraExplosivesGlobalProjectile>().clone = true;
			        }
			        /*int buffId = 0;
			        int dustId = 0;
			        switch (Main.rand.Next(3)) // get the buff id
			        {
				        case 0:
					        if (!mp.RandomFuelOnFire) break;
					        buffId = BuffID.OnFire;
					        dustId = 6;	// A red dust
					        break;
				        case 1:
					        if (!mp.RandomFuelFrostburn) break;
					        buffId = BuffID.Frostburn;
					        dustId = 211;	// a bluish dust
					        break;
				        case 2:
					        if (!mp.RandomFuelConfused) break;
					        buffId = BuffID.Confused;
					        dustId = 261;	// grey?
					        break;
				        default:	// if something breaks
					        break;
			        }
			        // Random Fuel
			        if(buffId != 0)GlobalMethods.InflictDubuff(buffId, 15, projectile.Center, projectile.owner, dustId, 300);*/
			        
		        }

		        if (mp.GlowingCompound && 
		            mp.GlowingCompoundActive)	// Glowing compound (working)
		        {
			        float projX = projectile.Center.X;
			        float projY = projectile.Center.Y;
			        for (float i = projX - 128; i <= projX + 128; i += 4) // Cycle X cords
			        {
				        for (float j = projY - 128; j <= projY + 128; j += 4) // Cycle Y cords
				        {
					        /*float dist = Vector2.Distance(new Vector2(i, j), projectile.Center);
					        if (dist < 127 && dist > 126 && WorldGen.TileEmpty((int)(i/16f), (int)(j/16f))) // Radius check
					        {
						        Dust dust = Dust.NewDustDirect(new Vector2(i, j), 1, 1, ModContent.DustType<GlowingDust>());
						        dust.position.X += i - projX;
						        dust.position.Y += j - projY;
					        }*/
					        if (Main.rand.Next(400) == 0) // Random Scattering
					        {
						        Dust dust = Dust.NewDustDirect(new Vector2(i, j), 1, 1, ModContent.DustType<GlowingDust>());
						        dust.position.X += i - projX;
						        dust.position.Y += j - projY;
					        }
				        }
			        }
		        }
		        
		        if (mp.Bombshroom &&
		            mp.AnarchistCookbook)
		        {
			        for (int n = 0; n < 3; n++)
			        {
				        Main.NewText("Spawn" + n);
				        Projectile.NewProjectileDirect(projectile.Center,
					        new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1)),
					        ModContent.ProjectileType<MushroomProjectile>(), 40, 0);
			        }
		        }
		        
		        bool immune = false;
		        if (projectile.type == ModContent.ProjectileType<NovaBoosterProjectile>() ||
		            projectile.type == ModContent.ProjectileType<BombCloakProjectile>()) immune = true;
		        if(mp.LihzahrdFuzeset) GlobalMethods.InflictDubuff(BuffID.OnFire, 15, projectile.Center, immune, projectile.owner, 6, 300);
		        if(mp.AlienExplosive) GlobalMethods.InflictDubuff(BuffID.Confused, 15, projectile.Center, immune, projectile.owner, 261, 300);
		        if(mp.SupernaturalBomb) GlobalMethods.InflictDubuff(BuffID.ShadowFlame, 15, projectile.Center, immune, projectile.owner, 179, 300);
		        if(mp.Bombshroom) GlobalMethods.InflictDubuff(BuffID.Venom, 15, projectile.Center, immune, projectile.owner, 173, 300);
	        }
        }
    }
}