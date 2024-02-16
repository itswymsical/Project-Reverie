using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

using Microsoft.Xna.Framework;

using ReverieMod.Helpers;

namespace ReverieMod.Common.Tiles
{
	public class ReverieTile : GlobalTile
	{
		private const float nutSpawnChance = 0.05f;
		private const float leafSpawnChance = 0.025f;
		private Player player;

		public override void FloorVisuals(int type, Player player)
		{
			player.GetModPlayer<Players.ReveriePlayer>().onSand =
				(TileID.Sets.Conversion.Sand[type] || TileID.Sets.Conversion.Sandstone[type] || TileID.Sets.Conversion.HardenedSand[type]);
		}
		public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
		{
			player = Main.LocalPlayer;
			if (!effectOnly)
			{
				if (player.ZoneForest())
				{
					if (Main.netMode != NetmodeID.MultiplayerClient && type == TileID.Trees)
					{
						TrySpawnLeaf(i, j);
						TrySpawnNut(i, j, fail);
					}
				}
			}
		}

		private void TrySpawnNut(int x, int y, bool fail)
		{
			if (fail || Main.rand.NextFloat() > nutSpawnChance)
			{
				return;
			}
			while (y > 10 && Main.tile[x, y].HasTile && Main.tile[x, y].TileType == TileID.Trees)
			{
				y--;
			}
			y++;
			if (!IsTileALeafyTreeTop(x, y) || Collision.SolidTiles(x - 2, x + 2, y - 2, y + 2))
			{
				return;
			}

			//Item.NewItem(pos: new Vector2(x * 16, y * 16), ModContent.ItemType<Nut>(), Main.rand.Next(5));
		}

		private void TrySpawnLeaf(int x, int y)
		{
			if (Main.rand.NextFloat() > leafSpawnChance)
			{
				return;
			}
			while (y > 10 && Main.tile[x, y].HasTile && Main.tile[x, y].TileType == TileID.Trees)
			{
				y--;
			}
			y++;

			if (!IsTileALeafyTreeTop(x, y) || Collision.SolidTiles(x - 2, x + 2, y - 2, y + 2))
			{
				return;
			}
			/*
			int velocityXDir = Main.rand.Next(2) * 2 - 1;
			int projectileType = ModContent.ProjectileType<Content.Projectiles.Typeless.FallingLeaf>();
			Projectile.NewProjectile(default, x * 16, y * 16, Main.rand.NextFloat(2f) * velocityXDir, 0f, projectileType, 0, 0f, Player.FindClosest(new Vector2(x * 16, y * 16), 16, 16));
			*/
		}

		private bool IsTileALeafyTreeTop(int i, int j)
		{
			Tile tileSafely = Framing.GetTileSafely(i, j);
			if (tileSafely.HasTile && tileSafely.TileType == TileID.Trees)
			{
				if (tileSafely.TileFrameX == 22 && tileSafely.TileFrameY >= 198 && tileSafely.TileFrameY <= 242)
				{
					return true;
				}
			}
			return false;
		}
	}
}
