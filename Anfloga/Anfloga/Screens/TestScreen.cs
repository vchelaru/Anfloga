using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Anfloga.Screens
{
	public partial class TestScreen
	{
        RenderTarget2D renderTarget;

		void CustomInitialize()
		{
            renderTarget = new RenderTarget2D(FlatRedBallServices.GraphicsDevice, 100, 100);
            DarknessRenderTargetLayer.RenderTarget = renderTarget;


            Camera.Main.BackgroundColor = Color.Orange;
            Camera.Main.XVelocity = 8;

            ShapeManager.AddCircle().Radius = 32;
		}

		void CustomActivity(bool firstTimeCalled)
		{


		}

		void CustomDestroy()
		{


		}

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
