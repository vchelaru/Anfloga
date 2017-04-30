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
using FlatRedBall.Math.Geometry;
using System.Linq;

namespace Anfloga.Entities
{
	public partial class WorldObjectEntity
	{
        void OnAfterCircleCollisionRadiusSet (object sender, EventArgs e)
        {
            Circle circle = this.Collision.Circles.FirstOrDefault();
            if(circle == null)
            {
                circle = new Circle();
                circle.AttachTo(this,false);
                this.Collision.Circles.Add(circle);
#if DEBUG
                if(DebuggingVariables.ShowWorldObjectCollision)
                {
                    circle.Visible = true;
                }
#endif
            }

            circle.Radius = this.CircleCollisionRadius;
        }
		
	}
}
