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
	public partial class EndingScreen
	{

		void CustomInitialize()
		{
            if(GlobalData.TotalCurrencyCollected > 10)
            {
                EndingScreenGumRuntime.EndingText = "You got so much!";
            }
            else
            {
                EndingScreenGumRuntime.EndingText = "You so poor!";
            }
        }

		void CustomActivity(bool firstTimeCalled)
		{
            if(InputManager.Keyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.Space) || 
                InputManager.Xbox360GamePads[0].ButtonPushed(Xbox360GamePad.Button.A))
            {
                MoveToScreen(typeof(Screens.MainMenuScreen));
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
