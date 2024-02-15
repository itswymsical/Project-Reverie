using Microsoft.Xna.Framework.Graphics;
using Terraria;
using ReverieMod.Core.Abstraction.Interfaces;

namespace ReverieMod.Core.Detours
{
    public class DrawAdditiveDetours : Detour
    {
        public override void LoadDetours() => Terraria.On_Main.DrawDust += DrawAdditive;

        public override void UnloadDetours() => Terraria.On_Main.DrawDust -= DrawAdditive;

        private void DrawAdditive(Terraria.On_Main.orig_DrawDust orig, Main self)
        {
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);

            foreach (var projectile in Main.projectile)
            {
                var modProjectile = projectile.ModProjectile;

                if (modProjectile is IDrawAdditive && projectile.active)
                {
                    (modProjectile as IDrawAdditive).DrawAdditive(Main.spriteBatch);
                }
            }

            foreach (var npc in Main.npc)
            {
                var modNPC = npc.ModNPC;

                if (modNPC is IDrawAdditive && npc.active)
                {
                    (modNPC as IDrawAdditive).DrawAdditive(Main.spriteBatch);
                }
            }

            Main.spriteBatch.End();

            orig(self);
        }
    }
}
