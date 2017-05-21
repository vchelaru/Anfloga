using System;
using System.Collections.Generic;
using System.Text;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;
using FlatRedBall.Math.Geometry;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Anfloga.Rendering;

namespace Anfloga.Entities
{
	public partial class ShaderRenderer
	{
        #region Fields/Properties

        SpriteBatch spriteBatch;

        public Effect Effect { get; set; }

        public Texture2D WorldTexture { get; set; }
        public Texture2D DarknessTexture { get; set; }
        public Texture2D BlowoutTexture { get; set; }

        public float DarknessAlpha { get; set; }

        public PositionedObject Viewer { get; set; }


        #endregion

        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
        private void CustomInitialize()
		{
            spriteBatch = new SpriteBatch(FlatRedBallServices.GraphicsDevice);

		}

        private void CustomActivity()
		{


		}

		private void CustomDestroy()
		{
            // todo: make this codegen
            SpriteManager.RemoveDrawableBatch(this);
		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }

        public void Draw(Camera camera)
        {
            DrawWorld(camera);

            //DrawBloom(camera);

            DrawDarkness(camera);
        }
        private void DrawWorld(Camera camera)
        {
            bool shouldExecute = true;
#if DEBUG
            shouldExecute = DebuggingVariables.RenderWithNoShaders == false;
#endif
            if(shouldExecute)
            {
                var destinationRectangle = camera.DestinationRectangle;

                FlatRedBallServices.GraphicsDevice.Textures[1] = WavyTexture;

                Effect.CurrentTechnique = Effect.Techniques["DistanceBlurTechnique"];
            
                float rightX = camera.AbsoluteRightXEdgeAt(Viewer.Z);
                float leftX = camera.AbsoluteLeftXEdgeAt(Viewer.Z);

                float bottomY = camera.AbsoluteBottomYEdgeAt(Viewer.Z);
                float topY = camera.AbsoluteTopYEdgeAt(Viewer.Z);

                float ratioX = (Viewer.X - leftX) / (rightX - leftX);
                float ratioY = 1 - (Viewer.Y - bottomY) / (topY - bottomY);

                Effect.Parameters["ViewerX"].SetValue(ratioX);
                Effect.Parameters["ViewerY"].SetValue(ratioY);
                Effect.Parameters["BlurStrength"].SetValue(BlurStrength);

                bool blurOn = true;
                if (blurOn)
                {
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                        SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone,
                        Effect);
                }
                else
                {
                    spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                }
                spriteBatch.Draw(WorldTexture, destinationRectangle, Color.White);
                spriteBatch.End();
                FlatRedBallServices.GraphicsDevice.Textures[1] = null;
            }
        }

        private void DrawBloom(Camera camera)
        {
            FlatRedBallServices.GraphicsDevice.SetRenderTarget(BloomRenderTarget);
            FlatRedBallServices.GraphicsDevice.Clear(Color.Black);
            {
                var destinationRectangle = new Rectangle(0,0, BloomRenderTarget.Width, BloomRenderTarget.Height);

                Effect.CurrentTechnique = Effect.Techniques["BloomTechnique"];

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                    SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone,
                    Effect);

                spriteBatch.Draw(WorldTexture, destinationRectangle, Color.White);
                spriteBatch.End();
            }

            if(this.LayerProvidedByContainer != null)
            {
                FlatRedBallServices.GraphicsDevice.SetRenderTarget(LayerProvidedByContainer.RenderTarget);
            }
            else
            {
                FlatRedBallServices.GraphicsDevice.SetRenderTarget(null);
            }

            {
                var destinationRectangle = camera.DestinationRectangle;

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive,
                    SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone,
                    Effect);

                spriteBatch.Draw(WorldTexture, destinationRectangle, Color.White);
                spriteBatch.End();
            }

            if (InputManager.Keyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                using (var stream = System.IO.File.OpenWrite("RenderTarget1.png"))
                {
                    BloomRenderTarget.SaveAsPng(stream, BloomRenderTarget.Width, BloomRenderTarget.Height);
                }
            }

        }


        private void DrawDarkness(Camera camera)
        {
            bool shouldExecute = true;
#if DEBUG
            shouldExecute = DebuggingVariables.RenderWithNoShaders == false;
#endif
            if(shouldExecute)
            {
                var destinationRectangle = camera.DestinationRectangle;

                var darknessColor = new Color(1, 1, 1, DarknessAlpha);
                BlendState blendState = GetMultiplyBlendOperation();

                spriteBatch.Begin(SpriteSortMode.Immediate, blendState);
                spriteBatch.Draw(DarknessTexture, destinationRectangle, darknessColor);

                spriteBatch.End();
            }
        }

        private static BlendState GetMultiplyBlendOperation()
        {
            BlendState blendState = new BlendState();
            blendState.AlphaSourceBlend = Blend.DestinationColor;
            blendState.ColorSourceBlend = Blend.DestinationColor;

            blendState.AlphaDestinationBlend = Blend.Zero;
            blendState.ColorDestinationBlend = Blend.InverseSourceAlpha;
            blendState.ColorBlendFunction = BlendFunction.Add;
            return blendState;
        }
    }
}
