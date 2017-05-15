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

        public Texture2D WorldTexture { get; set; }
        public Texture2D DarknessTexture { get; set; }
        public Texture2D BlowoutTexture { get; set; }

        public float DarknessAlpha { get; set; }

        SpriteBatch spriteBatch;

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

        public void Draw(Camera camera)
        {
            var destinationRectangle = Camera.Main.DestinationRectangle;

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            spriteBatch.Draw(WorldTexture, destinationRectangle, Color.White);
            spriteBatch.End();

            var darknessColor = new Color(1, 1, 1, DarknessAlpha);
            BlendState blendState = new BlendState();
            blendState.AlphaSourceBlend = Blend.DestinationColor;
            blendState.ColorSourceBlend = Blend.DestinationColor;

            blendState.AlphaDestinationBlend = Blend.Zero;
            blendState.ColorDestinationBlend = Blend.InverseSourceAlpha;
            blendState.ColorBlendFunction = BlendFunction.Add;

            spriteBatch.Begin(SpriteSortMode.Immediate, blendState);
            spriteBatch.Draw(DarknessTexture, destinationRectangle, darknessColor);

            spriteBatch.End();
        }
	}
}
