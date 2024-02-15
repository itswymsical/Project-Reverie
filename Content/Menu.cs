using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace ReverieMod.Content
{
    public class Menu : ModMenu
    {
        private const string AssetPath = "ReverieMod/Assets/Textures/";
        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>($"{AssetPath}logo");
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/IlluminantInkiness");
        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => base.MenuBackgroundStyle;
        public override Asset<Texture2D> SunTexture => null;
        public override Asset<Texture2D> MoonTexture => null;
        public override string DisplayName => "Reverie";
        public override void Update(bool isOnTitleScreen)
        {
            Main.time = 1;
        }
        public override void OnSelected() {
            SoundEngine.PlaySound(new SoundStyle("ReverieMod/Assets/SFX/Theme_Select"));
        }
        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            logoRotation = 0f;
            logoScale *= 0.75f;
            return true;
        }
    }
}
