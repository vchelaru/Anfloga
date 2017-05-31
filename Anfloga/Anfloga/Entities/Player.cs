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
using Microsoft.Xna.Framework;
using FlatRedBall.Audio;
using Microsoft.Xna.Framework.Audio;

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

        float facingLeftLightXOffset;

        public PlayerHudRuntime PlayerHud { get; set; }
        public I2DInput MovementInput { get; set; }
        public LightInput LightInput { get; set; }
        public IPressableInput DashInput { get; set; }
        public IPressableInput DialogInput { get; set; }

        public IPressableInput ActionInput { get; set; }
        public IPressableInput LightToggleInput { get; set; }


        public List<IPerformCurrencyTransactionOn> ObjectsToPerformCurrencyTransactionOn { get; private set; }

        public int CurrentCurrencyBalance { get; private set; }
        public int TotalCurrencyCollected { get; private set; }

        private float explorationDurationLeft;

        public bool IsDead => explorationDurationLeft <= 0;

        public ExplorationState CurrentExplorationState { get; set; }

        private Vector2 lastCheckpointPosition;

        private float actionIconOffset;

        private bool didPlayEngineLoop;

        #endregion

        #region Initialize

        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
        private void CustomInitialize()
		{
#if DEBUG
            CurrentCurrencyBalance = DebuggingVariables.StartingCurrency;
#endif

            facingLeftLightXOffset = AlwaysOnLightSprite.RelativeX;

            // We may end up calling this in a screen in case we want the screen to control this assignment
            AssignInput();

            //If we end up having player data move this to the event.
            InitializeExplorationVariables();

            //
            InitializeCollidingObjectList();

            InitializeEmitter();

            InitializeAcionIcon();

		}

        private void InitializeAcionIcon()
        {
            //Possible bug? The pixel coorinates are not updating on initialize.
            ActionIconInstance.VisualName = "CollectMineral";
            ActionIconInstance.VisualName = "PurchaseCollector";
            
            actionIconOffset = AxisAlignedRectangleInstance.Height / 2;

            ActionIconInstance.RelativeY = actionIconOffset + ActionIconInstance.SpriteInstanceHeight;
            ActionIconInstance.Visible = false;
        }

        private void InitializeEmitter()
        {

            BubblesInstance.CurrentBubbleEmitterType = BubbleEmitterType.Sub;
        }

        private void InitializeCollidingObjectList()
        {
            ObjectsToPerformCurrencyTransactionOn = new List<IPerformCurrencyTransactionOn>();
        }

        private void InitializeExplorationVariables()
        {
            explorationDurationLeft = MaxExplorationTime;
            CurrentExplorationState = ExplorationState.Idle;
        }

        public void InitializeLightLayer(Layer lightLayer)
        {
            this.LightBeamInstance.MoveToLayer(lightLayer);

            SpriteManager.AddToLayer(AlwaysOnLightSprite, lightLayer);
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
            dialogInput.Inputs.Add(InputManager.Keyboard.GetKey(Microsoft.Xna.Framework.Input.Keys.Space));
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

        #region Activity

        private void CustomActivity()
		{
            UpdateActionIconActivity();

            PerformMovementInput();

            PerformActionInput();

            PerformLightLogic();

            UpdateExplorationDurationActivity();

            UpdateFacingDirection();
            //Perform hud update at the end. Incase we have abilities that consume oxygen.
            UpdateHudActivity();
		}

        private void UpdateActionIconActivity()
        {
            if(ObjectsToPerformCurrencyTransactionOn.Count > 0)
            {
                ActionIconInstance.Visible = true;

                if(ObjectsToPerformCurrencyTransactionOn[0] is MineralDeposit)
                {
                    ActionIconInstance.VisualName = "CollectMineral";
                }
                else
                {
                    ActionIconInstance.VisualName = "PurchaseCollector";
                }

                ActionIconInstance.RelativeY = actionIconOffset + ActionIconInstance.SpriteInstanceHeight / 2;
            }
            else
            {
                ActionIconInstance.Visible = false;
            }
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

        private void UpdateFacingDirection()
        {
            if(XVelocity < 0)
            {
                this.SpriteInstance.CurrentChainName = "FaceLeft";
                this.AlwaysOnLightSprite.RelativeX = facingLeftLightXOffset;
                this.AlwaysOnLightSprite.FlipHorizontal = false;
            }
            else if(XVelocity > 0)
            {
                this.SpriteInstance.CurrentChainName = "FaceRight";
                this.AlwaysOnLightSprite.RelativeX = -facingLeftLightXOffset;
                this.AlwaysOnLightSprite.FlipHorizontal = true;
            }
        }

        public void UpdateLastCheckpointPosition(float x, float y)
        {
            lastCheckpointPosition.X = x;
            lastCheckpointPosition.Y = y;
        }

        public void RespawnFromLastCheckpointPosition()
        {
            this.X = lastCheckpointPosition.X;
            this.Y = lastCheckpointPosition.Y;
            InitializeExplorationVariables();
            this.LightBeamInstance.Visible = false;
        }

        private void UpdateExplorationDurationActivity()
        {
#if DEBUG
            if(InputManager.Keyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.O))
            {
                explorationDurationLeft = MaxExplorationTime;
            }
#endif

            if (CurrentExplorationState == ExplorationState.Consume)
            {
                bool wasLow = (explorationDurationLeft / MaxExplorationTime) < PercentageForLowEnergyNotification / 100.0f;
                explorationDurationLeft -= ExplorationConsumptionRate * TimeManager.SecondDifference;
                
                if(this.LightBeamInstance.Visible)
                {
                    explorationDurationLeft -= AdditionalLightConsumption * TimeManager.SecondDifference;
                }

                bool isLow = (explorationDurationLeft / MaxExplorationTime) < PercentageForLowEnergyNotification / 100.0f;

                if(!wasLow && isLow)
                {
                    warningAlert.Play();
                }

                if (explorationDurationLeft < 0)
                {
                    explorationDurationLeft = 0;


                }
            }
            else if (CurrentExplorationState == ExplorationState.Replenish)
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

            var updateValues = new HudUpdateData()
            {
                ExplorationLimitFill = currentOxygenPercentage,
                MineralText = CurrentCurrencyBalance,
                IsFillingUp = CurrentExplorationState == ExplorationState.Replenish,
                IsLow = currentOxygenPercentage < PercentageForLowEnergyNotification/100.0f
            };

            PlayerHud.UpdateHud(updateValues);
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

            if (desiredYVelocity != 0 || desiredXVelocity != 0)
            {
                BubblesInstance.ThrustVector = new Microsoft.Xna.Framework.Vector3(desiredXVelocity, desiredYVelocity, 0);
            }
            else if (BubblesInstance.ThrustVector.Length() != 0)
            {
                BubblesInstance.ThrustVector = Microsoft.Xna.Framework.Vector3.Zero;
            }

            if(desiredXVelocity != 0 || desiredYVelocity != 0)
            {
                EngineLoop.Play();
            }
            else
            {
                EngineLoop.Pause();
            }

            if((desiredXVelocity == 0 && desiredYVelocity == 0) && Velocity.LengthSquared() > 0)
            {
                SubLoop.Play();
            }
            else
            {
                SubLoop.Pause();
            }
        }

        public void CollectCurrency(int increase)
        {
            TotalCurrencyCollected += increase;
            CurrentCurrencyBalance += increase;
        }

        public void SpendCurrency(int cost)
        {
            CurrentCurrencyBalance -= cost;
        }
        #endregion

        private void CustomDestroy()
		{

		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }

    }
}
