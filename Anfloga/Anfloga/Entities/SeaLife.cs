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
using Microsoft.Xna.Framework;

namespace Anfloga.Entities
{
	public partial class SeaLife
	{
        private Vector2 minPosition;
        private Vector2 maxPosition;

        private Vector3 positionToMoveTo;
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

        public void SetPatrolArea()
        {
            minPosition = new Vector2 { X = this.X - PatrolRadius, Y = this.Y - PatrolRadius };
            maxPosition = new Vector2 { X = this.X + PatrolRadius, Y = this.Y + PatrolRadius };
            SetNewTargetPosition();
        }

        private void SetNewTargetPosition()
        {
            float xToMoveTo = FlatRedBallServices.Random.Between(minPosition.X, maxPosition.X);
            float yToMoveTo = FlatRedBallServices.Random.Between(minPosition.Y, maxPosition.Y);

            positionToMoveTo = new Vector3 { X = xToMoveTo, Y = yToMoveTo };

            var velocityVector = positionToMoveTo - this.Position;
            //this.Velocity = velocityVector.Normalize() * ExploreVelocity;
        }

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
