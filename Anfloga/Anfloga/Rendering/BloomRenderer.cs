using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Anfloga.Rendering
{
    public class BloomRenderer : IEffectsRenderer
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public float BloomIntensity { get; set; } = 2f;
        public float BaseIntensity { get; set; } = 0.9f;
        public float BloomSaturation { get; set; } = 1.5f;
        public float BaseSaturation { get; set; } = 1.0f;

        SpriteBatch spriteBatch;
        RenderTarget2D target1;
        RenderTarget2D target2;

        Effect bloomEffect;
        BloomExtractRenderer extractRenderer;
        BlurRenderer blurRenderer;
        GraphicsDevice device;
        bool initialized = false;

        public BloomRenderer(Effect bloomCombineEffect)
        {
            this.bloomEffect = bloomCombineEffect;
        }

        public void Initialize(GraphicsDevice device, Camera camera)
        {
            Height = device.PresentationParameters.BackBufferHeight;
            Width = device.PresentationParameters.BackBufferWidth;
            this.device = device;
            spriteBatch = new SpriteBatch(this.device);

            // use half size texture for extract and blur
            target1 = new RenderTarget2D(device, Width / 2, Height / 2);
            target2 = new RenderTarget2D(device, Width, Height);

            extractRenderer = new BloomExtractRenderer();
            extractRenderer.Initialize(this.device, camera);

            blurRenderer = new BlurRenderer();
            blurRenderer.Initialize(this.device, camera);

            bloomEffect.Parameters["BloomIntensity"].SetValue(BloomIntensity);
            bloomEffect.Parameters["BaseIntensity"].SetValue(BaseIntensity);
            bloomEffect.Parameters["BloomSaturation"].SetValue(BloomSaturation);
            bloomEffect.Parameters["BaseSaturation"].SetValue(BaseSaturation);

            initialized = true;
        }

        public void Draw(RenderTarget2D src, RenderTarget2D dest)
        {
            if(!initialized)
            {
                throw new Exception("Draw was called before effect was initialized!");
            }

            // draw src buffer with extract effect to target1 buffer
            extractRenderer.Draw(src, target1);

            // draw extract buffer to target2 with blur
            blurRenderer.Draw(target1, target2);

            int destWidth = dest != null ? dest.Width : Width;
            int destHeight = dest != null ? dest.Height : Height;

            // finally perform our bloom combine
            device.Textures[1] = src; // setting this allows the shader to operate on the src texture
            device.SetRenderTarget(dest);
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, bloomEffect);
            spriteBatch.Draw(target2, new Rectangle(0, 0, destWidth, destHeight), Color.White);
            spriteBatch.End();
        }

        
    }
}
