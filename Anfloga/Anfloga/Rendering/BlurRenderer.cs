using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Anfloga.Rendering
{
    public class BlurRenderer : IEffectsRenderer
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public float BlurAmount { get; set; } = 1.25f;

        Effect blur;
        SpriteBatch spriteBatch;
        bool initialized = false;
        GraphicsDevice device;
        RenderTarget2D target2;
        float[] kernal;
        LinearOffsets offsets;

        /// <summary>
        /// An easy way to calculate and save horizontal
        /// and vertical offsets in one pass but
        /// use them in two separate render passes
        /// </summary>
        private class LinearOffsets
        {
            public Vector2[] X;
            public Vector2[] Y;
        }

        public void Initialize(GraphicsDevice device, Camera camera)
        {
            Width = device.PresentationParameters.BackBufferWidth;
            Height = device.PresentationParameters.BackBufferHeight;
            this.device = device;
            spriteBatch = new SpriteBatch(this.device);

            // load our fx
            blur = FlatRedBallServices.Load<Effect>(@"Content\Shaders\Blur");

            // calculate the effect blur radius using the array length
            var radius = (blur.Parameters["Weights"].Elements.Count - 1) / 2;

            // calculate kernal and offsets
            kernal = ComputeKernal(radius, BlurAmount);
            offsets = ComputeOffsets(Width, Height, radius);

            blur.Parameters["Weights"].SetValue(kernal);

            target2 = new RenderTarget2D(device, Width, Height);

            initialized = true;
        }

        public void Draw(RenderTarget2D src, RenderTarget2D dest = null)
        {
            if(!initialized)
            {
                throw new Exception("Draw called before effect initialized!");
            }

            blur.Parameters["Weights"].SetValue(kernal);

            // perform horizontal blur, this pass renders to our secondary target
            blur.Parameters["Offsets"].SetValue(offsets.X);
            device.SetRenderTarget(target2);
            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, blur);
            spriteBatch.Draw(src, new Rectangle(0, 0, Width, Height), Color.White);
            spriteBatch.End();

            // perform vertical blur, this pass renders to destination
            int destWidth = dest != null ? dest.Width : Width;
            int destHeight = dest != null ? dest.Height : Height;

            blur.Parameters["Offsets"].SetValue(offsets.Y);
            device.SetRenderTarget(dest);
            spriteBatch.Begin(0, BlendState.Opaque, null, null, null, blur);
            spriteBatch.Draw(target2, new Rectangle(0, 0, destWidth, destHeight), Color.White);
            spriteBatch.End();
        }

        /// <summary>
        /// Calculates the weights of each step in the kernal
        /// </summary>
        /// <param name="radius">The blur radius</param>
        /// <param name="blurAmount">The blur strength</param>
        /// <returns>An array of weights to apply in blur shader</returns>
        private float[] ComputeKernal(int radius, float blurAmount)
        {
            var kernal = new float[radius * 2 + 1];
            var sigma = radius / blurAmount;
            float twoSigmaSquare = 2.0f * sigma * sigma;
            float sigmaRoot = (float)Math.Sqrt(twoSigmaSquare * Math.PI);
            float total = 0f;
            float distance = 0f;
            int index = 0;

            for(int i = -radius; i <= radius; i++)
            {
                distance = i * i;
                index = i + radius;
                kernal[index] = (float)Math.Exp(-distance / twoSigmaSquare) / sigmaRoot;
                total += kernal[index];
            }

            // normalize to sum to 1
            for(int i = 0; i < kernal.Length; i++)
            {
                kernal[i] /= total;
            }

            return kernal;
        }

        /// <summary>
        /// Calculates all coordinate offsets for
        /// both the X and Y blur pass in a single
        /// pass and returns an object that allows
        /// each pass to be handed to the render cycle
        /// individually.
        /// </summary>
        /// <param name="textureWidth">The width of the texture to be blurred</param>
        /// <param name="textureHeight">The height of the texture to be blurred</param>
        /// <param name="radius">The blur radius</param>
        /// <returns></returns>
        private LinearOffsets ComputeOffsets(float textureWidth, float textureHeight, int radius)
        {
            var linearOffsets = new LinearOffsets()
            {
                X = new Vector2[radius * 2 + 1],
                Y = new Vector2[radius * 2 + 1]
            };

            var index = 0;
            float xOffset = 1.0f / textureWidth;
            float yOffset = 1.0f / textureHeight;

            for(int i = -radius; i <= radius; i++)
            {
                index = i + radius;
                linearOffsets.X[index] = new Vector2(i * xOffset, 0f);
                linearOffsets.Y[index] = new Vector2(0f, i * yOffset);
            }

            return linearOffsets;
        }
    }
}
