using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Anfloga.Rendering
{
    public class PassThroughRenderer : IEffectsRenderer
    {
        public int Height { get; set; }
        public int Width { get; set; }

        SpriteBatch spriteBatch;
        GraphicsDevice device;
        bool initialized = false;

        public void Initialize(GraphicsDevice device, Camera camera)
        {
            Width = device.PresentationParameters.BackBufferWidth;
            Height = device.PresentationParameters.BackBufferHeight;
            spriteBatch = new SpriteBatch(device);
            this.device = device;
            initialized = true;
        }

        public void Draw(RenderTarget2D src, RenderTarget2D dest = null)
        {
            if (!initialized)
            {
                throw new Exception("Draw was called before effect was initialized!");
            }

            int destWidth = dest != null ? dest.Width : Width;
            int destHeight = dest != null ? dest.Height : Height;

            device.SetRenderTarget(dest);
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null);
            spriteBatch.Draw(src, new Rectangle(0, 0, destWidth, destHeight), Color.White);
            spriteBatch.End();
        }

        
    }
}
