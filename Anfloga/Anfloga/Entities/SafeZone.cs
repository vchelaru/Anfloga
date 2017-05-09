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
using System.Linq;

namespace Anfloga.Entities
{
	public partial class SafeZone: IPerformCurrencyTransactionOn
    {
        public bool IsActive { get; private set; }
        private Polygon tilePolygonReference;
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

        public void SetupCollision()
        {
            
            //We have 2 versions of the collision for the safe zone.
            //1. When it is a geyser and needs to be purchased. This is a circle who's radius is set in a glue variaable.
            //2. When it is a battery. The collsion is determined by the shape declared in tiled.

            if(ActivationCost <= 0)
            {
                SetupFreeSafeZone();
            }
            else
            {
                SetupSafeZoneForPurchase();
            }
        }
        private void SetupSafeZoneForPurchase()
        {
            //Vic - When you get on again I'd like you to review this funtion and another.
            //If needed I can get on a call.
            //
            //I'm trying to swap between 2 different collision instances.
            //The first when the safe zone is available for purchase. This will be a circle.
            //The second when the safe zone has been purchased. It will use the tiled polygon.
            //I'm able to remove them from managers, but I don't like havinng to call remove from managers
            //on collision. 

            SpriteInstance.CurrentChainName = "Geyser";

            tilePolygonReference = Collision.Polygons.FirstOrDefault();

            Collision.RemoveFromManagers();

            Collision.Polygons.Clear();

            Circle circle = new Circle();

            circle.AttachTo(this, false);
            Collision.Circles.Add(circle);

            Collision.AddToManagers();

#if DEBUG
            if(DebuggingVariables.ShowReplenishZoneCollision)
            {
                circle.Visible = true;
            }
#endif

            circle.Radius = this.GeyserCollisionRadius;
        }
        private void PerformActivation()
        {
            //Vic - the other place I had a question on.
            IsActive = true;
            Collision.RemoveFromManagers();
            Collision.Circles.Clear();

            Collision.Polygons.Add(tilePolygonReference);
            Collision.AddToManagers();

            SpriteInstance.CurrentChainName = "Battery";
            
            //Activation animation?
        }

        private void SetupFreeSafeZone()
        {
            IsActive = true;
            SpriteInstance.CurrentChainName = "Battery";
        }

    }
}