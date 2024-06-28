using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using Terraria;
using Terraria.GameContent;
using static Terraria.ModLoader.ModContent;

namespace ReverieMod.Common.Systems.Subworlds
{
    public abstract class Otherworld : Subworld
    {
        private int currentFrame;
        private double frameTime;
        private const int totalFrames = 19; // Total number of frames in the texture
        private const double timePerFrame = 0.6; // Time per frame in seconds
        private float fadeInDuration = 5f; // Duration for the fade-in effect in seconds
        private double elapsedTime;
        public Texture2D loadingScreen = Request<Texture2D>("ReverieMod/Assets/Textures/Backgrounds/Otherworlds/Archaea").Value;
        public Texture2D otherworldTitle = (Texture2D)TextureAssets.Logo;
        public Texture2D loadingIcon = (Texture2D)TextureAssets.LoadingSunflower;
        public override void DrawSetup(GameTime gameTime)
        {
            float opacity = MathHelper.Clamp((float)(elapsedTime / fadeInDuration), 0f, 1f);
            // Update elapsed time
            elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;

            // Update animation frame
            frameTime += gameTime.ElapsedGameTime.TotalSeconds;
            if (frameTime >= timePerFrame)
            {
                frameTime -= timePerFrame;
                currentFrame = (currentFrame + 1) % totalFrames;
            }

            int frameWidth = loadingIcon.Width;
            int frameHeight = loadingIcon.Height / totalFrames;

            Rectangle sourceRectangle = new Rectangle(0, currentFrame * frameHeight, frameWidth, frameHeight);


            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
            // Draw background texture to fill the screen
            Main.spriteBatch.Draw(loadingScreen, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * opacity);

            // Calculate position for loading icon to be at the bottom right
            Vector2 iconPosition = new Vector2(Main.screenWidth - loadingIcon.Width - 10, Main.screenHeight - frameHeight - 10);

            // Draw loading icon
            Main.spriteBatch.Draw(loadingIcon, iconPosition, sourceRectangle, Color.White * opacity);

            // Draw subworld name texture in the top left corner
            Vector2 nameTexturePosition = new Vector2(10, 10);
            Main.spriteBatch.Draw(otherworldTitle, nameTexturePosition, Color.White * opacity);

            Main.DrawCursor(Main.DrawThickCursor());
            Main.spriteBatch.End();
        }
    }
}