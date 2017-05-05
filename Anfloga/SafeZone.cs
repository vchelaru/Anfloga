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
using Anfloga.Interfaces;

namespace Anfloga.Entities
{
	public partial class SafeZone: IPerformCurrencyTransactionOn
    {
        public bool IsActive { get; private set; }
        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
		private void CustomInitialize()
		{


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
        public bool PerformCurrencyTransaction(IPerformsCurrencyTransaction player)
        {
            bool toReturn = false;
            //If the player has enough currency, then we will activate this safe zone.
            if(player.CurrentCurrencyBalance >= ActivationCost)
            {
                toReturn = true;
                PerformActivation();
                player.SpendCurrency(ActivationCost);
            }

            return toReturn;
        }

        private void PerformActivation()
        {
            IsActive = true;
            //Activation animation?
        }
    }
}
