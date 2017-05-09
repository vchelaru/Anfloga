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
using FlatRedBall.Graphics;
using Anfloga.Interfaces;

namespace Anfloga.Entities
{
    public enum ExplorationState
    {
        Idle,
        Consume,
        Replenish
    }

	public partial class Player: IPerformsCurrencyTransaction
	{
        #region Fields

        public PlayerHudRuntime PlayerHud { get; set; }
        public I2DInput MovementInput { get; set; }
        public LightInput LightInput { get; set; }
        public IPressableInput DashInput { get; set; }
        public IPressableInput DialogInput { get; set; }

        public IPressableInput ActionInput { get; set; }
        public IPressableInput LightToggleInput { get; set; }


        public List<IPerformCurrencyTransactionOn> ObjectsToPerformCurrencyTransactionOn { get; private set; }

        public int CurrentCurrencyBalance { get; private set; }

        private float explorationDurationLeft;

        private ExplorationState currentExplorationState;

        #endregion

        #region Initialize

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
            InitializeExplorationVariables();

            //
            InitializeCollidingObjectList();
		}

        private void InitializeCollidingObjectList()
        {
            ObjectsToPerformCurrencyTransactionOn = new List<IPerformCurrencyTransactionOn>();
        }

        private void InitializeExplorationVariables()
        {
            explorationDurationLeft = MaxExplorationTime;
            currentExplorationState = ExplorationState.Idle;
        }

        public void InitializeLightLayer(Layer lightLayer)
        {
            this.LightBeamInstance.MoveToLayer(lightLayer);
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
            dashInput.Inputs.Add(InputManager.Keyboard.GetKey(Microsoft.Xna.Framework.Input.Keys.E));
            dashInput.Inputs.Add(InputManager.Xbox360GamePads[0].GetButton(Xbox360GamePad.Button.B));
            DashInput = dashInput;

            var dialogInput = new MultiplePressableInputs();
            dialogInput.Inputs.Add(InputManager.Keyboard.GetKey(Microsoft.Xna.Framework.Input.Keys.Enter));
            dashInput.Inputs.Add(InputManager.Xbox360GamePads[0].GetButton(Xbox360GamePad.Button.X));
            DialogInput = dialogInput;

            var actionInput = new MultiplePressableInputs();
            actionInput.Inputs.Add(InputManager.Keyboard.GetKey(Microsoft.Xna.Framework.Input.Keys.Space));
            actionInput.Inputs.Add(InputManager.Xbox360GamePads[0].GetButton(Xbox360GamePad.Button.A));
            ActionInput = actionInput;

            var lightToggleInput = new MultiplePressableInputs();
            lightToggleInput.Inputs.Add(InputManager.Mouse.GetButton(Mouse.MouseButtons.RightButton));
            lightToggleInput.Inputs.Add(InputManager.Xbox360GamePads[0].GetButton(Xbox360GamePad.Button.RightShoulder));
            LightToggleInput = lightToggleInput;

            LightInput = new LightInput(this);
        }

        #endregion

        private void CustomActivity()
		{
            PerformMovementInput();

            PerformActionInput();

            PerformLightLogic();

            UpdateExplorationDurtionActivity();

            PerformAnimationMovement();
            //Perform hud update at the end. Incase we have abilities that consume oxygen.
            UpdateHudActivity();
		}

        private void PerformActionInput()
        {
            //Only perform command action on one object per frame.
            //These objects should not be added back to
            if(ActionInput.WasJustPressed && ObjectsToPerformCurrencyTransactionOn.Count > 0)
            {
                foreach (var item in ObjectsToPerformCurrencyTransactionOn)
                {
                    //If the command is successful break out of the loop.
                    //Else we will try on the next object. 
                    //We do this so that the player will still collect minerals if they are colliding
                    //with a mineral and a geyser at the same time, but the player does not hae enough
                    //currency to purchase the safe zone.
                    if (item.PerformCurrencyTransaction(this))
                    {
                        break;
                    }
                }
            }
        }

        private void PerformLightLogic()
        {
            if(this.LightToggleInput.WasJustPressed)
            {
                this.LightBeamInstance.Visible = !this.LightBeamInstance.Visible;
            }

            if(this.LightBeamInstance.Visible)
            {
                LightInput.Activity();

                var angle = LightInput.GetAngle();

                if(angle != null)
                {
                    this.LightBeamInstance.RelativeRotationZ = angle.Value;
                }
            }
        }

        private void PerformAnimationMovement()
        {
            if(XVelocity < 0)
            {
                this.SpriteInstance.CurrentChainName = "FaceLeft";
            }
            else if(XVelocity > 0)
            {
                this.SpriteInstance.CurrentChainName = "FaceRight";
            }
        }

        private void UpdateExplorationDurtionActivity()
        {
#if DEBUG
            if(InputManager.Keyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.O))
            {
                explorationDurationLeft = MaxExplorationTime;
            }
#endif

            if (currentExplorationState == ExplorationState.Consume)
            {
                explorationDurationLeft -= ExplorationConsumptionRate * TimeManager.SecondDifference;
                
                if(this.LightBeamInstance.Visible)
                {
                    explorationDurationLeft -= AdditionalLightConsumption * TimeManager.SecondDifference;
                }

                if(explorationDurationLeft < 0)
                {
                    explorationDurationLeft = 0;
                }
            }
            else if (currentExplorationState == ExplorationState.Replenish)
            {
                explorationDurationLeft += ExplorationReplenishRate * TimeManager.SecondDifference;

                if(explorationDurationLeft > MaxExplorationTime)
                {
                    explorationDurationLeft = MaxExplorationTime;
                }
            }
        }

        private void UpdateHudActivity()
        {
            //We are not worried about 
            var currentOxygenPercentage = explorationDurationLeft / MaxExplorationTime;

            PlayerHud.UpdateHud(new HudUpdateData() { ExplorationLimitFill =  currentOxygenPercentage});
        }

        private void PerformMovementInput()
        {
            float desiredXVelocity = MovementInput.X * MaxSpeed;
            float desiredYVelocity = MovementInput.Y * MaxSpeed;

            var xSign = Math.Sign( desiredXVelocity - XVelocity);
            XVelocity += xSign * MaxSpeed * TimeManager.SecondDifference / AccelerationTime;
            if(xSign != Math.Sign(desiredXVelocity - XVelocity))
            {
                XVelocity = desiredXVelocity;
        }

            var ySign = Math.Sign(desiredYVelocity - YVelocity);
            YVelocity += ySign * MaxSpeed * TimeManager.SecondDifference / AccelerationTime;
            if(ySign != Math.Sign(desiredYVelocity - YVelocity))
            {
                YVelocity = desiredYVelocity;
            }

        }

        private void CustomDestroy()
		{


		}

        public void SetExplorationState(ExplorationState state)
        {
            currentExplorationState = state;
        }

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }

        public void CollectCurrency(int increase)
        {
            CurrentCurrencyBalance += increase;
        }

        public void SpendCurrency(int cost)
        {
            CurrentCurrencyBalance -= cost;
        }
    }
}
