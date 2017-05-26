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



namespace Anfloga.Screens
{
	public partial class MainMenuScreen
	{

		void CustomInitialize()
		{


		}

		void CustomActivity(bool firstTimeCalled)
		{
            ProceedActivity();

		}

        private void ProceedActivity()
        {
            bool shouldStartLoad = this.AsyncLoadingState == FlatRedBall.Screens.AsyncLoadingState.NotStarted &&
                InputManager.Keyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.Space);
            if (shouldStartLoad)
            {
                MainMenuScreenGumRuntime.UnderLogoText = "Loading...";

                StartAsyncLoad(typeof(GameScreen), () => IsActivityFinished = true);
            }
        }

        void CustomDestroy()
		{


		}

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
