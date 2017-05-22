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
        public Effect DarknessEffect { get; set; }

        public Texture2D WorldTexture { get; set; }
        public Texture2D DarknessTexture { get; set; }
        public Texture2D LightTexture { get; set; }
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


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
        public void SetShaderParameters()
        {
            DarknessEffect.Parameters["textureHeight"].SetValue(DarknessTexture.Height);
            DarknessEffect.Parameters["darknessStart"].SetValue(DarknessStart);
            DarknessEffect.Parameters["alphaPerPixel"].SetValue(DarknessIncreaseRate);
            Effect.Parameters["BlurStrength"].SetValue(BlurStrength);
        }

        public void Draw(Camera camera)
        {
            DrawWorld(camera);

            DrawDarkness(camera);
            DrawLight(camera);
        }


        private void DrawWorld(Camera camera)
        {
            var destinationRectangle = camera.DestinationRectangle;

            FlatRedBallServices.GraphicsDevice.Textures[1] = WavyTexture;

            Effect.CurrentTechnique = Effect.Techniques["BlurTechnique"];

            float rightX = camera.AbsoluteRightXEdgeAt(Viewer.Z);
            float leftX = camera.AbsoluteLeftXEdgeAt(Viewer.Z);

            float bottomY = camera.AbsoluteBottomYEdgeAt(Viewer.Z);
            float topY = camera.AbsoluteTopYEdgeAt(Viewer.Z);

            float ratioX = (Viewer.X - leftX) / (rightX - leftX);
            float ratioY = 1 - (Viewer.Y - bottomY) / (topY - bottomY);

            Effect.Parameters["ViewerX"].SetValue(ratioX);
            Effect.Parameters["ViewerY"].SetValue(ratioY);

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

        private void DrawDarkness(Camera camera)
        {
            var destinationRectangle = camera.DestinationRectangle;

            var darknessColor = new Color(1, 1, 1, DarknessAlpha);
            BlendState blendState = GetMultiplyBlendOperation();

            DarknessEffect.CurrentTechnique = DarknessEffect.Techniques["DarknessTechnique"];
            DarknessEffect.Parameters["cameraTop"].SetValue(camera.AbsoluteTopYEdgeAt(Viewer.Z));
            var ytop = camera.AbsoluteTopYEdgeAt(Viewer.Z);

            bool shaderOn = true;
            if (shaderOn)
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, blendState,
                    SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone,
                        DarknessEffect);
            }
            else
            {
                spriteBatch.Begin(SpriteSortMode.Immediate, blendState);
            }
            var colorToUse = shaderOn ? Color.White : darknessColor;
            spriteBatch.Draw(DarknessTexture, destinationRectangle, colorToUse);

            spriteBatch.End();
        }
        private void DrawLight(Camera camera)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            spriteBatch.Draw(LightTexture, camera.DestinationRectangle, Color.White);
            spriteBatch.End();
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
