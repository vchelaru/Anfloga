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
using System.Linq;

namespace Anfloga.Entities
{
	public partial class DarkRegion
	{
        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
		private void CustomInitialize()
		{

            SpriteInstance.Width = 100;
            SpriteInstance.Height = 100;
		}

		private void CustomActivity()
		{


		}

		private void CustomDestroy()
		{


		}

        public void SetSpriteDimentionsFromCollision()
        {
            var polygon = Collision.Polygons.FirstOrDefault();
            if(polygon != null)
            {
                //Subract Right point minus left point.
                //The polygon points are relative which are centered at 0,0.
                var width = (float)(polygon.Points[1].X -polygon.Points[0].X);
                var height = (float)(polygon.Points[1].Y - polygon.Points[2].Y);

                SpriteInstance.Width = width;
                SpriteInstance.Height = height;
            }
            polygon.Visible = true;
        }

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
