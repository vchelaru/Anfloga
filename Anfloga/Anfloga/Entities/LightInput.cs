using FlatRedBall;
using FlatRedBall.Gui;
using FlatRedBall.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anfloga.Entities
{
    public class LightInput : I2DInput
    {
        #region Fields/Properties

        float? lastAnalogStickAngle;
        float lastCursorScreenX;
        float lastCursorScreenY;

        public PositionedObject Player { get; set; }

        public float Magnitude
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public float X
        {
            get;
            private set;
        }

        public float XVelocity
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public float Y
        {
            get;
            private set;
        }

        public float YVelocity
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        public LightInput(Player player)
        {
            Player = player;
        }

        public void Activity()
        {
            var analogStick = (InputManager.Xbox360GamePads[0].LeftStick as I2DInput);
            lastAnalogStickAngle = analogStick.GetAngle();

            if(lastAnalogStickAngle != null)
            {
                X = analogStick.X;
                Y = analogStick.Y;
            }
            else
            {
                GetAngleFromCursor();
            }
        }

        private void GetAngleFromCursor()
        {
            bool didCursorChange = false;

            var cursor = GuiManager.Cursor;
            if (lastAnalogStickAngle == null)
            {
                if (lastCursorScreenX != cursor.ScreenX)
                {
                    didCursorChange = true;
                    lastCursorScreenX = cursor.ScreenX;
                }

                if (lastCursorScreenY != cursor.ScreenY)
                {
                    didCursorChange = true;
                    lastCursorScreenY = cursor.ScreenY;
                }
            }

            if (didCursorChange)
            {
                var worldX = cursor.WorldXAt(0);
                var worldY = cursor.WorldYAt(0);

                if (worldX != Player.X || worldY != Player.Y)
                {
                    var angle = (float)(Math.Atan2(worldY - Player.Y, worldX - Player.X));

                    X = (float)Math.Cos(angle);
                    Y = (float)Math.Sin(angle);
                }
            }
        }
    }
}
