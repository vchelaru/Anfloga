using System;
using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Specialized;
using FlatRedBall.Audio;
using FlatRedBall.Screens;
using Anfloga.Entities;
using Anfloga.Screens;
using static Anfloga.GumRuntimes.GameScreenGumRuntime;
using FlatRedBall.Glue.StateInterpolation;

namespace Anfloga.Screens
{
	public partial class GameScreen
	{
        void OnResolutionOrOrientationChanged (object sender, EventArgs e)
        {
            
        }
        void OnOkCancelWindowInstanceYesButtonClick (FlatRedBall.Gui.IWindow window)
        {
            OkCancelWindowInstance.Visible = false;

            isTransitioning = true;
            this.GameScreenGumRuntime.InterpolateTo(FadeoutCategory.Dark, FadeOutTime, InterpolationType.Linear, Easing.In);
            this.Call(() =>
            {
                UnpauseThisScreen();
                MoveToScreen(typeof(MainMenuScreen));

            })
                .After(FadeOutTime);
        }
        void OnOkCancelWindowInstanceNoButtonClick (FlatRedBall.Gui.IWindow window)
        {
            OkCancelWindowInstance.Visible = false;
            UnpauseThisScreen();
        }
		
	}
}
