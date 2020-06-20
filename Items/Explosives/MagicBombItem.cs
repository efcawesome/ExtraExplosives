using System.Collections.Generic;
using ExtraExplosives.Projectiles;
using Microsoft.Xna.Framework;
using On.Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Main = Terraria.Main;
using Player = Terraria.Player;
using Projectile = Terraria.Projectile;

namespace ExtraExplosives.Items.Explosives
{
    public class MagicBombItem : ModItem
    {
        private int _pickPower = 0;
        private int timeLeft = 0;

        public override bool CloneNewInstances => true;
        public override string Texture => "Terraria/Item_" + ItemID.StickyBomb;
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magic Bomb");
            Tooltip.SetDefault("It can be imbued with mana\n" +    // Not all mages cast spells lol
                               "Right Click to increase its power");
        }

        public override void SetDefaults()
        {
            item.useTurn = true;
            item.height = 20;
            item.width = 20;
            item.shoot = ModContent.ProjectileType<MagicBombProjectile>();
            item.shootSpeed = 10f;
            item.consumable = true;
            item.rare = ItemRarityID.Orange;
            item.value = 1000;
            item.maxStack = 99;
            item.useAnimation = 40;
            item.useTime = 40;
            item.useStyle = 4;
            //item.mana = 10;
            item.channel = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage,
            ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY),
                ModContent.ProjectileType<SmallExplosiveProjectile>(), item.damage, 0);   //ModContent.ProjectileType<MagicBombProjectile>()
            item.damage = 40;
            return false;
        }

        public override void HoldItem(Player player)
        {
            if (timeLeft != 0) timeLeft--;
            base.HoldItem(player);
        }


        public override bool AltFunctionUse(Player player)
        {
            if (player.statMana < 20 || timeLeft != 0 || item.damage >= 1000) return false;
            item.damage += 20;    // TODO add indicator showing failed or successful mana addition
            player.statMana -= 20;
            timeLeft = 30;
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            var currentDamageTooltip = new TooltipLine(mod, "CurrentDamage", $"Its damage has been increased by {item.damage - 40}");
            currentDamageTooltip.overrideColor = Color.Aquamarine;
            tooltips.Add(currentDamageTooltip);
        }
        
        
    }
}