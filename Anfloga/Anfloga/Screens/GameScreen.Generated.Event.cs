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
namespace Anfloga.Screens
{
	public partial class GameScreen
	{
		void OnOkCancelWindowInstanceYesButtonClickTunnel (FlatRedBall.Gui.IWindow window)
		{
			if (this.OkCancelWindowInstanceYesButtonClick != null)
			{
				OkCancelWindowInstanceYesButtonClick(window);
			}
		}
		void OnOkCancelWindowInstanceNoButtonClickTunnel (FlatRedBall.Gui.IWindow window)
		{
			if (this.OkCancelWindowInstanceNoButtonClick != null)
			{
				OkCancelWindowInstanceNoButtonClick(window);
			}
		}
	}
}
