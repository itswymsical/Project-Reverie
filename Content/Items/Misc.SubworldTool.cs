﻿using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using ReverieMod.Common.Systems.Subworlds.Archaea;

namespace ReverieMod.Content.Items
{
    public class SubworldTool : ModItem
    {
        public override string Texture => Assets.PlaceholderTexture;
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 38;
            Item.rare = 12;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.UseSound = SoundID.Item1;
        }

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
                SubworldSystem.Enter<ArchaeaSubworld>();

            if (SubworldSystem.IsActive<ArchaeaSubworld>())
                SubworldSystem.Exit();

            return true;
        }
    }
}