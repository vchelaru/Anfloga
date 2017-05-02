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
using Anfloga.GumRuntimes;

namespace Anfloga.Entities
{
	public partial class Player
	{
        private PlayerHudRuntime PlayerHud { get; set; }
        public I2DInput MovementInput { get; set; }
        public IPressableInput DashInput { get; set; }
        public IPressableInput DialogInput { get; set; }

        private float currentOxygenSupply;

        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
		private void CustomInitialize()
		{
            // We may end up calling this in a screen in case we want the screen to control this assignment
            AssignInput();

            //If we end up having player data move this to the event.
            InitializeHudVariables();
		}

        private void InitializeHudVariables()
        {
            currentOxygenSupply = MaxOxygenSupply;
        }

        private void AssignInput()
        {
            var movementInput = new Multiple2DInputs();
            movementInput.Inputs.Add(InputManager.Keyboard.Get2DInput(
                Microsoft.Xna.Framework.Input.Keys.A,
                Microsoft.Xna.Framework.Input.Keys.D,
                Microsoft.Xna.Framework.Input.Keys.W,
                Microsoft.Xna.Framework.Input.Keys.S));
            movementInput.Inputs.Add(InputManager.Xbox360GamePads[0].LeftStick);
            MovementInput = movementInput;

            var dashInput = new MultiplePressableInputs();
            dashInput.Inputs.Add(InputManager.Keyboard.GetKey(Microsoft.Xna.Framework.Input.Keys.Space));
            dashInput.Inputs.Add(InputManager.Xbox360GamePads[0].GetButton(Xbox360GamePad.Button.A));
            DashInput = dashInput;

            var dialogInput = new MultiplePressableInputs();
            dialogInput.Inputs.Add(InputManager.Keyboard.GetKey(Microsoft.Xna.Framework.Input.Keys.Enter));
            dashInput.Inputs.Add(InputManager.Xbox360GamePads[0].GetButton(Xbox360GamePad.Button.X));
            DialogInput = dialogInput;
        }

        private void CustomActivity()
		{
            PerformMovementInput();
            ConsumeOxygenActivity();

            //Perform hud update at the end. Incase we have abilities that consume oxygen.
            UpdateHudActivity();

		}

        private void ConsumeOxygenActivity()
        {
            currentOxygenSupply -= OxygenConsumptionRate;
        }

        private void UpdateHudActivity()
        {
            
        }

        private void PerformMovementInput()
        {
            // todo : make this accelerate up instead of be immediate
            this.XVelocity = MovementInput.X * MaxSpeed;
            this.YVelocity = MovementInput.Y * MaxSpeed;
        }

        private void CustomDestroy()
		{


		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
