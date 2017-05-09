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
	public partial class MineralDeposit: IPerformCurrencyTransactionOn
    {
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
            //For now we do not have a max limit to how much currency we can have.
            //So this will always return true.

            player.CollectCurrency(CurrencyValue);
            Destroy();

            return true;
        }
    }
}
