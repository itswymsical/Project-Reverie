using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Trelamium.Content
{
    public class Menu : ModMenu
    {
        private const string AssetPath = "Trelamium/Assets/";
        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>($"{AssetPath}logo");
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/IlluminantInkiness");
        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => base.MenuBackgroundStyle;
        public override Asset<Texture2D> SunTexture => null;
        public override Asset<Texture2D> MoonTexture => null;
        public override void OnSelected() {
            SoundEngine.PlaySound(new SoundStyle(AssetPath + "SFX/MenuSweep"));
        }
    }
}
