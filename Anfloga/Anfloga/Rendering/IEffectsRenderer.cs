using FlatRedBall;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anfloga.Rendering
{
    public interface IEffectsRenderer
    {
        /// <summary>
        /// The width of the render target, often set automatically in Initialize
        /// when applying a fullscreen effect
        /// </summary>
        int Width { get; set; }

        /// <summary>
        /// The height of the render target, often set automatically in Initialize
        /// when applying a fullscreen effect
        /// </summary>
        int Height { get; set; }

        /// <summary>
        /// This method should set up effects, render targets and other things required
        /// to apply its effect
        /// </summary>
        /// <param name="device">The graphics device to use</param>
        /// <param name="camera">The camera object to use</param>
        void Initialize(GraphicsDevice device, Camera camera);

        /// <summary>
        /// This method should draw the provided render target with the effect
        /// </summary>
        /// <param name="src">The source texture to draw from</param>
        /// <param name="dest">The destination texture to draw to, 
        /// defaults to null, drawing to the screen</param>
        void Draw(RenderTarget2D src, RenderTarget2D dest = null);
    }
}
