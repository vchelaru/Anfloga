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
            EndingScreenGumRuntime.MineralCountText = "Minerals Obtained: " + GlobalData.TotalCurrencyCollected;
            

            if (GlobalData.TotalCurrencyCollected > 60)
            {
                EndingScreenGumRuntime.EndingText = "You made it to the surface. You wish to share the secrets of Anfloga to prevent others from falling into this death trap. \n\nYou are resourceful. You fashion a distress beacon from parts of your ship. You hope a nearby trader notices it before the Mining Corporation sends a cleanup crew. \n\nIn the meantime, you hone your fishing skills.";
            }
            else
            {
                EndingScreenGumRuntime.EndingText = "Thank you for completing your contract with Anfloga Mining Corporation. \n\nUnfortunately, we have deciced against commuting your sentence, an option we retain as stated in Clause 6B. Perhaps if you had gathered more minerals, the situation would be different. \n\nPlease wait patiently and avoid hiding from the Anfloga Mining Corporation cleanup crew.";
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
