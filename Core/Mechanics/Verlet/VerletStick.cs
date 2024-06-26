using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

namespace ReverieMod.Core.Mechanics.Verlet
{
    public class VerletStick
    {
        public Texture2D Texture;

        public VerletPoint Start;

        public VerletPoint End;

        public float Length;

        public VerletStick(VerletPoint start, VerletPoint end, float length, Texture2D texture = null)
        {
            Start = start;
            End = end;
            Length = length;
            Texture = texture ?? TextureAssets.MagicPixel.Value;
        }
    }
}
