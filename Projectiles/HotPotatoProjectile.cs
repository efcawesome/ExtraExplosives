using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using static ExtraExplosives.GlobalMethods;

namespace ExtraExplosives.Projectiles
{
    public class HotPotatoProjectile : ModProjectile
    {
        private int _damage = 100;
        private int _pickPower = 0;
        private int _lifeTime = 300 + Main.rand.Next(60);    // How long to keep alive in ticks (currently 5-6 seconds)
        private bool _thrown;

        private int _fuze = 30;
        //private float[] rgb = new[] {1.58f, 1.58f, 0.0f};

        public override bool CloneNewInstances => true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hot Potato");
        }

        public override void SetDefaults()
        {
            projectile.tileCollide = true;
            projectile.width = 20;
            projectile.height = 20;
            projectile.aiStyle = 16;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = _lifeTime + 5; // set higher than _lifeTime
            projectile.alpha = 255;    // Make it invisible
            projectile.velocity = Vector2.Zero;
            Lighting.AddLight(projectile.Center, 0.5f, 0.5f, 0.3f);
        }

        public override string Texture => "ExtraExplosives/Projectiles/HotPotatoProjectile";

        /*public override bool PreAI()
        {
            projectile.ai[0]++;
            if ((Mouse.GetState().LeftButton == ButtonState.Released && !_thrown) || projectile.ai[0] >= _lifeTime - 15)    // Add support for controllers
            {
                projectile.alpha = 0;
                projectile.ai[1] = projectile.ai[0];
                _thrown = true;
            }
            else if(!_thrown)
            {
                projectile.position = Main.player[projectile.owner].position;
            }
            return _thrown;
        }

        public override void AI()
        {
            projectile.ai[1]++;
            if (projectile.ai[0] >= _lifeTime - 15 && projectile.ai[1] >= _lifeTime)
            {
                Kill(_lifeTime - (int)projectile.ai[0]);
            }
            else if (projectile.ai[1] >= _lifeTime)
            {
                Kill(_lifeTime - (int)projectile.ai[0]);
            }
            else if(projectile.ai[1] < 2f)
            {
                projectile.velocity.X += 100;
            }
            projectile.velocity.Y += 0.2f;
            projectile.velocity.X *= 0.99f;
        }*/

        public override bool PreAI()
        {
            Player player = Main.player[projectile.owner];
            if (_thrown) return true;
            
            projectile.localAI[0]++;
            if (player.releaseUseItem || projectile.localAI[0] >= _lifeTime - _fuze)
            {
                _thrown = true;
                projectile.alpha = 0;
                projectile.localAI[1] = projectile.localAI[0];
                if (projectile.localAI[0] < _lifeTime - _fuze)
                {
                    float modifier = 0.075f;
                    float screenW = Main.screenWidth;
                    float mouseX = Main.MouseScreen.X - (screenW/2);
                    float screenH = Main.screenHeight;
                    float mouseY = Main.MouseScreen.Y - (screenH/2);
                    projectile.velocity.X = projectile.localAI[0] /_lifeTime * mouseX * modifier;
                    projectile.velocity.Y = projectile.localAI[0] /_lifeTime * mouseY * modifier;
                    //mod.Logger.DebugFormat("Potato thrown at {0} {1}, mouse {2} {3}, screen {4} {5}", projectile.velocity.X, projectile.velocity.Y,Main.MouseScreen.X,Main.MouseScreen.Y,screenW,screenH);
                }
                else
                {
                    projectile.velocity.Y = 3;
                }
            }
            else
            {
                projectile.position = player.position;
            }

            return _thrown;
        }

        public override void AI()
        {
            projectile.localAI[1]++;
            if (projectile.localAI[1] >= _lifeTime)
            {
                Kill(projectile.timeLeft);    // since this value will vary, just use localAI[0]
            }

            projectile.velocity.X *= 0.995f;
            if (projectile.velocity.Y < 20f) projectile.velocity.Y += 0.1f;
        }

        public override void PostAI()
        {
        }

        private void CreateExplosion(Vector2 position, int radius)    // Ripped from troll bomb, changed where needed
        {                                                                // comments removed
            for (int x = -radius; x <= radius; x++) 
            {
                for (int y = -radius; y <= radius; y++) 
                {
                    int xPosition = (int)(x + position.X / 16.0f);
                    int yPosition = (int)(y + position.Y / 16.0f);

                    if (Math.Sqrt(x * x + y * y) <= radius + 0.5) 
                    {
                        ushort tile = Main.tile[xPosition, yPosition].type;
                        if (!CanBreakTile(tile, _pickPower)) 
                        {
                        }
                        else //Breakable
                        {
                            if (CanBreakTiles) //User preferences dictates if this bomb can break tiles
                            {
                                WorldGen.KillTile(xPosition, yPosition, false, false, false); //This destroys Tiles
                                if (CanBreakWalls) WorldGen.KillWall(xPosition, yPosition, false); //This destroys Walls
                            }
                        }
                    }
                }
            }
        }
        
        public override void Kill(int ignore)
        {
            //Create Bomb Sound
                Main.PlaySound(SoundID.Item14, (int) projectile.Center.X, (int) projectile.Center.Y);

                //Create Bomb Damage
                ExplosionDamage(projectile.localAI[0] / 6, projectile.Center, (int)projectile.localAI[0], projectile.localAI[0]/4, projectile.owner);

                //Create Bomb Explosion
                CreateExplosion(projectile.Center, (int) projectile.localAI[0]/12);

                //Create Bomb Dust
                CreateDust(projectile.Center, (int) projectile.localAI[0]);
        }
        
        private void CreateDust(Vector2 position, int amount)    // TODO UPDATE DUST CODE THIS BIT ACTS STRANGE
        {
            Dust dust;
            Vector2 updatedPosition;

            for (int i = 0; i <= amount; i++)
            {
                if (Main.rand.NextFloat() < DustAmount)
                {
                    //---Dust 1---
                    if (Main.rand.NextFloat() < 0.1)    // dynamite gibs    // Standard
                    {
                        updatedPosition = new Vector2(position.X - 70 / 2, position.Y - 70 / 2);

                        dust = Main.dust[Terraria.Dust.NewDust(updatedPosition, 70, 70, 4, 0f, 0f, 154, new Color(255, 255, 255), 1.55f)];
                        dust.noGravity = false;
                        dust.fadeIn = 0.2763158f;
                    }
                    if (Main.rand.NextFloat() < 0.1)    // potato gibs    // change if a better dust exists
                    {
                        updatedPosition = new Vector2(position.X - 70 / 2, position.Y - 70 / 2);

                        dust = Main.dust[Terraria.Dust.NewDust(updatedPosition, 70, 70, 216, 0f, 0f, 154, new Color(255, 255, 255), 1.55f)];
                        dust.noGravity = false;
                        dust.fadeIn = 0.2763158f;
                    }
                    //------------
                }
            }
        }
        
    }
}