using Microsoft.Xna.Framework;
using ReverieMod.Common.Players;
using ReverieMod.Content.Items.Shovels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace ReverieMod.Common.Global
{
    public class ReverieGlobalProjectile : GlobalProjectile
    {
        public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
        {
            Player player = Main.LocalPlayer;
            if (player.HeldItem is Craterstrike && projectile.type >= 424 && projectile.type <= 426)
            {
                if (projectile.owner == player.whoAmI)
                {
                    player.GetModPlayer<ShovelPlayer>().DigBlocks((int)projectile.Center.X, (int)projectile.Center.Y);
                }
            }
            return true;
        }
    }
}
