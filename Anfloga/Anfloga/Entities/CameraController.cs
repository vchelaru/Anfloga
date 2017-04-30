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

namespace Anfloga.Entities
{
	public partial class CameraController
	{
        public PositionedObject ObjectFollowing { get; set; }

        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
		private void CustomInitialize()
		{
            Camera.Main.AttachTo(this, false);
            Camera.Main.RelativeZ = 40;

		}

		private void CustomActivity()
		{
            PerformFollowingActivity();

		}

        private void PerformFollowingActivity()
        {
            if(ObjectFollowing != null)
            {
                this.X = ObjectFollowing.X;
                this.Y = ObjectFollowing.Y;
            }
        }

        private void CustomDestroy()
		{


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
