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

        private float displacementOffset = 0;

        #endregion

        #region Initialize
        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
        private void CustomInitialize()
		{
            spriteBatch = new SpriteBatch(FlatRedBallServices.GraphicsDevice);
            displacementOffset = DisplacementStart;
		}

        #endregion

        private void CustomActivity()
		{
            
            //if (InputManager.Keyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.Q))
            //    DisplacementStart--;
            //if (InputManager.Keyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.E))
            //    DisplacementStart++;
            //Effect.Parameters["DisplacementStart"].SetValue(DisplacementStart);
        }

		private void CustomDestroy()
		{
            // todo: make this codegen
            SpriteManager.RemoveDrawableBatch(this);
		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
        public void InitializeRenderVariables()
        {
            Effect.Parameters["BlurStrength"].SetValue(BlurStrength);
            Effect.Parameters["DisplacementStart"].SetValue(DisplacementStart);
        }

        public void Draw(Camera camera)
        {
            DrawWorld(camera);

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

                // All render targets are sized to match the camera's destination rectangle, so we don't need to perform
                // any offsets to account for weird aspect ratios. In other words, the render target resolution matches the
                // desired aspect ratio, guaranteed, so we 0-out the offset values (X and Y). See explanation below on when destinationRectangle
                // is re-assigned.
                destinationRectangle.X = 0;
                destinationRectangle.Y = 0;

                Effect.Parameters["CameraHeight"].SetValue(Camera.Main.OrthogonalHeight);

                float rightX = camera.AbsoluteRightXEdgeAt(Viewer.Z);
                float leftX = camera.AbsoluteLeftXEdgeAt(Viewer.Z);

                float bottomY = camera.AbsoluteBottomYEdgeAt(Viewer.Z);
                float topY = camera.AbsoluteTopYEdgeAt(Viewer.Z);

                float ratioX = (Viewer.X - leftX) / (rightX - leftX);
                float ratioY = 1 - (Viewer.Y - bottomY) / (topY - bottomY);


                Effect.Parameters["ViewerX"].SetValue(ratioX);
                Effect.Parameters["ViewerY"].SetValue(ratioY);
                Effect.Parameters["CameraTop"].SetValue(topY);
                Effect.Parameters["DisplacementTextureOffset"].SetValue((float)TimeManager.CurrentTime / DisplacementVelocity);
                // divide by 2 to account for the focus being applied center-out
                Effect.Parameters["FocusArea"].SetValue(FocusedRatio / 2.0f);

                FlatRedBallServices.GraphicsDevice.Textures[1] = WavyTexture;
                FlatRedBallServices.GraphicsDevice.Textures[2] = DisplacementRenderTarget;

                Effect.CurrentTechnique = Effect.Techniques["DistanceBlurTechnique"];
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                                    SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone,
                                    Effect);
                
                FlatRedBallServices.GraphicsDevice.SetRenderTarget(DisplacementRenderTarget);

                spriteBatch.Draw(WorldTexture, destinationRectangle, Color.White);
                FlatRedBallServices.GraphicsDevice.SetRenderTarget(null);

                spriteBatch.End();

                FlatRedBallServices.GraphicsDevice.Textures[1] = null;
                FlatRedBallServices.GraphicsDevice.Textures[2] = null;

                FlatRedBallServices.GraphicsDevice.SetRenderTarget(null);

                // This last draw call renders to the screen (note the render target is null), so the offsets do need to 
                // be applied. We'll just re-assign destinationRectangle, without setting the X and Ys to 0:
                destinationRectangle = camera.DestinationRectangle;

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);
                spriteBatch.Draw(DisplacementRenderTarget, destinationRectangle, Color.White);
                spriteBatch.End();
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

                spriteBatch.Begin(SpriteSortMode.Immediate, blendState, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);
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
