using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReverieMod.Helpers;
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
        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>($"{AssetPath}Backgrounds/PlaceholderBG");

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Music/Reverie");
        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => null;
        public override Asset<Texture2D> SunTexture => null;
        public override Asset<Texture2D> MoonTexture => null;
        public override string DisplayName => "Reverie";
       
        public override void OnSelected() {
            SoundEngine.PlaySound(new SoundStyle("ReverieMod/Assets/SFX/Theme_Select"));
        }
        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black);
            Main.dayTime = false;
            logoRotation = 0f;
            logoScale *= 0.35f;
            return true;
        }
    }
}
