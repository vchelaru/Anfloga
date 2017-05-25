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
using FlatRedBall.Glue.StateInterpolation;
using StateInterpolationPlugin;

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
            if(TweenerManager.Self.IsObjectReferencedByTweeners(this) == false)
            {
                SetNewTargetPosition();
            }

		}

        private void CheckChangePatrolPoint()
        {
            var posDiff = positionToMoveTo - this.Position;
            posDiff.Z = 0;
            if(posDiff.Length() < Velocity.Length() * TimeManager.SecondDifference)
            {
                SetNewTargetPosition();
            }
        }

        private void CustomDestroy()
		{


		}

        public void SetPatrolArea()
        {
            minPosition = new Vector2 { X = this.X - XPatrolRange, Y = this.Y - YPatrolRange };
            maxPosition = new Vector2 { X = this.X + XPatrolRange, Y = this.Y + YPatrolRange };
            SetNewTargetPosition();
        }

        private void SetNewTargetPosition()
        {
            float xToMoveTo = FlatRedBallServices.Random.Between(minPosition.X, maxPosition.X);
            float yToMoveTo = FlatRedBallServices.Random.Between(minPosition.Y, maxPosition.Y);

            positionToMoveTo = new Vector3 { X = xToMoveTo, Y = yToMoveTo };

            var velocityVector = positionToMoveTo - this.Position;

            float interpolateTime = velocityVector.Length() / SwimVelocity;

            this.Tween(nameof(X), positionToMoveTo.X, interpolateTime, InterpolationType.Quadratic, Easing.InOut);
            this.Tween(nameof(Y), positionToMoveTo.Y, interpolateTime, InterpolationType.Quadratic, Easing.InOut);

            this.SpriteInstance.FlipHorizontal = velocityVector.X > 0;
        }

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
