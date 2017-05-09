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
namespace Anfloga.Entities
{
	public partial class SafeZone
	{
        void OnAfterActivationCostSet (object sender, EventArgs e)
        {
            //The bool defaults to false, we will set it to true if the
            //Cost of the safeZone is 0.
            if(ActivationCost < 1)
            {
                IsActive = true;
            }
        }
		
	}
}
