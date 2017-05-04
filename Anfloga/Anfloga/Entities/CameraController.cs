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

        private float cameraLeftXBound;
        private float cameraRightXBound;

        private float cameraTopYBound;
        private float cameraBottomYBound;

        private Camera mainCamera;

        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
		private void CustomInitialize()
		{
            Camera.Main.AttachTo(this, false);
            Camera.Main.RelativeZ = 40;

            mainCamera = Camera.Main;
		}

		private void CustomActivity()
		{
            PerformFollowingActivity();

            if(InputManager.Keyboard.KeyDown(Microsoft.Xna.Framework.Input.Keys.P))
            PerformClampCameraActivity();
        }

        private void PerformClampCameraActivity()
        {
            float cameraLeft = mainCamera.AbsoluteLeftXEdgeAt(0);
            float cameraRight = mainCamera.AbsoluteRightXEdgeAt(0);

            float cameraTop = mainCamera.AbsoluteTopYEdgeAt(0);
            float cameraBottom = mainCamera.AbsoluteBottomYEdgeAt(0);

            if (cameraLeft < cameraLeftXBound)
            {
                this.X += cameraLeftXBound - cameraLeft;
            }
            else if(cameraRight > cameraRightXBound)
            {
                this.X -= cameraRightXBound - cameraRight;
            }

            if (cameraTop > cameraTopYBound)
            {
                this.Y += cameraTopYBound - cameraTop;
            }
            else if(cameraBottom < cameraBottomYBound)
            {
                this.Y -= cameraBottomYBound - cameraBottom;
            }
             
        }

        private void PerformFollowingActivity()
        {
            if(ObjectFollowing != null)
            {
                this.X = ObjectFollowing.X;
                this.Y = ObjectFollowing.Y;
            }
        }

        public void SetCameraBoundsFromTiledMap(FlatRedBall.TileGraphics.LayeredTileMap mapToSetBoundsFrom)
        {
            cameraLeftXBound = mapToSetBoundsFrom.X;
            cameraRightXBound = mapToSetBoundsFrom.X + mapToSetBoundsFrom.Width;

            cameraTopYBound = mapToSetBoundsFrom.Y;
            cameraBottomYBound = mapToSetBoundsFrom.Y - mapToSetBoundsFrom.Height;
        }

        private void CustomDestroy()
		{

            mainCamera = null;
		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
