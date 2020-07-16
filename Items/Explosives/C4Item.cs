﻿using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace ExtraExplosives.Items.Explosives
{
	public class C4Item : ExplosiveItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("C4");
			Tooltip.SetDefault("Make sure to set the explosion button in the settings.\n" +
				"[c/AB40FF:Can destroy dungeon bricks and desert fossils]");
		}

		public override void SafeSetDefaults()
		{
			item.damage = 0;	 //The damage stat for the Weapon.
			item.width = 20;	//sprite width
			item.height = 20;   //sprite height
			item.maxStack = 999;   //This defines the items max stack
			item.consumable = true;  //Tells the game that this should be used up once fired
			item.useStyle = 1;   //The way your item will be used, 1 is the regular sword swing for example
			item.rare = 9;	 //The color the title of your item when hovering over it ingame
			item.UseSound = SoundID.Item1; //The sound played when using this item
			item.useAnimation = 50;  //How long the item is used for.
			item.useTime = 50;	 //How fast the item is used.
			item.value = Item.buyPrice(0, 6, 18, 0);   //How much the item is worth, in copper coins, when you sell it to a merchant. It costs 1/5th of this to buy it back from them. An easy way to remember the value is platinum, gold, silver, copper or PPGGSSCC (so this item price is 3 silver)
			item.noUseGraphic = true;
			item.noMelee = true;	  //Setting to True allows the weapon sprite to stop doing damage, so only the projectile does the damge
			item.shoot = mod.ProjectileType("C4Projectile"); //This defines what type of projectile this item will shoot
			item.shootSpeed = 5f; //This defines the projectile speed when shot
			//item.createTile = mod.TileType("ExplosiveTile");
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<LargeExplosiveItem>(), 1);
			recipe.AddIngredient(ItemID.LandMine, 1);
			recipe.AddIngredient(ItemID.ExplosivePowder, 1);
			recipe.AddIngredient(ItemID.Gel, 10);
			recipe.AddIngredient(ItemID.ClayBlock, 10);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}