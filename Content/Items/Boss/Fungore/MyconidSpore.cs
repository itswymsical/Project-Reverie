using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Trelamium.Content.Items.Boss.Fungore
{
    public class MyconidSpore : ModItem
    {
        public override string Texture => Assets.Items.Fungore + "MyconidSpore";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 12;
        }
        public override void SetDefaults()
        {
            Item.useTime = Item.useAnimation = 25;
            Item.width = Item.height = 32;

            Item.maxStack = 20;
            Item.consumable = true;

            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.ForceRoar;

            Item.value = Item.sellPrice(copper: 0);
        }
        public override bool CanUseItem(Player player) => !NPC.AnyNPCs(ModContent.NPCType<NPCs.Boss.Fungore.Fungore>());
        public override bool? UseItem(Player player)
        {
            NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<NPCs.Boss.Fungore.Fungore>());
            SoundEngine.PlaySound(SoundID.Roar, player.position);
            return true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Mushroom, 8);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}