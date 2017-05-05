using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using FlatRedBall;
using FlatRedBall.Input;
using FlatRedBall.Instructions;
using FlatRedBall.AI.Pathfinding;
using FlatRedBall.Graphics.Animation;
using FlatRedBall.Graphics.Particle;
using FlatRedBall.Math.Geometry;
using FlatRedBall.Localization;
using FlatRedBall.TileGraphics;
using FlatRedBall.TileEntities;
using FlatRedBall.TileCollisions;
using Anfloga.Entities;
using Anfloga.Logic;
using Microsoft.Xna.Framework.Graphics;

namespace Anfloga.Screens
{
	public partial class GameScreen
	{
        #region Fields

        static string LevelNameToLoad = nameof(theMap);

        LayeredTileMap currentLevel;

        TileShapeCollection solidCollision;

        DialogLogic dialogLogic;

        WorldObjectEntity objectCollidingWith;

        RenderTarget2D darknessRenderTarget;

        #endregion

        #region Initialize Methods

        void CustomInitialize()
		{
            LoadLevel(LevelNameToLoad);

            InitializeCamera();

            InitializeCollision();

            InitializeDialogBoxLogic();

            InitializeHud();

            InitializeRenderTargets();

            MoveLightObjectsToRenderTargetLayer();
		}

        private void MoveLightObjectsToRenderTargetLayer()
        {
            foreach(var item in PlayerList)
            {
                item.InitializeLightLayer(DarknessRenderTargetLayer);
            }
        }

        private void InitializeRenderTargets()
        {
            this.darknessRenderTarget = new RenderTarget2D(FlatRedBallServices.GraphicsDevice, (int)Camera.Main.OrthogonalWidth/2, (int)Camera.Main.OrthogonalHeight/2);

            this.DarknessRenderTargetLayer.RenderTarget = darknessRenderTarget;

            this.DarknessSprite.Texture = darknessRenderTarget;
            // makes the sprite the same size regardless of the render target resolution;
            this.DarknessSprite.TextureScale = -1;
            this.DarknessSprite.Width = Camera.Main.OrthogonalWidth;
            this.DarknessSprite.Height = Camera.Main.OrthogonalHeight;

#if DEBUG
            if (DebuggingVariables.HideDarknessOverlay)
            {
                this.DarknessSprite.Visible = false;
            }
#endif
        }

        private void InitializeHud()
        {
            if(PlayerList.Count > 0)
            {
                //Get the first player for now to attatch the hud instance to.
                PlayerList[0].PlayerHud = this.PlayerHudInstance;
            }
        }

        private void InitializeDialogBoxLogic()
        {
            dialogLogic = new Logic.DialogLogic();
            dialogLogic.DialogBox = this.DialogBoxInstance;
            // todo: assign the action prompt:
            //dialogLogic.CheckActionPrompt = null;
        }

        private void InitializeCollision()
        {
            solidCollision = new TileShapeCollection();
            solidCollision.AddMergedCollisionFrom(currentLevel, (propertyList) =>
            {
                return propertyList.Any(item => item.Name == "SolidCollision");
            });

            solidCollision.Visible = true;
        }

        private void InitializeCamera()
        {
            if (PlayerList.Count > 0)
            {
                CameraControllerInstance.ObjectFollowing = PlayerList[0];
            }

            CameraControllerInstance.SetCameraBoundsFromTiledMap(currentLevel);
        }

        private void LoadLevel(string levelNameToLoad)
        {
            currentLevel = (LayeredTileMap)GetFile(levelNameToLoad);

            currentLevel.AddToManagers();

            TileEntityInstantiator.CreateEntitiesFrom(currentLevel);

#if DEBUG
            if(DebuggingVariables.ShowWorldObjectCollision)
            {
                foreach (var item in WorldObjectEntityList)
                {
                    item.Collision.Visible = true;
                }
            }
#endif
#if DEBUG
            if (DebuggingVariables.ShowReplenishZoneCollision)
            {
                foreach (var item in SafeZoneList)
                {
                    item.Collision.Visible = true;
                }
            }
#endif
#if DEBUG
            if(DebuggingVariables.ShowMineralDepositCollision)
            {
                foreach(var item in MineralDepositList)
                {
                    item.Collision.Visible = true;
                }
            }
#endif
            // todo: create collision:

            // todo: set camera bounds:
        }

        #endregion

        #region Activity Methods


        void CustomActivity(bool firstTimeCalled)
		{
            // This needs to happen before dialog box activity:
            PerformCollision();

            if (PlayerList.Count > 0)
            {
                dialogLogic.Update(PlayerList[0].DialogInput.WasJustPressed, objectCollidingWith);
            }

            ReloadScreenActivity();
		}

        private void ReloadScreenActivity()
        {
            var keyboard = InputManager.Keyboard;
            if(keyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.R))
            {
                RestartScreen(true);
            }
        }

        private void PerformCollision()
        {
            foreach(var player in PlayerList)
            {
                solidCollision.CollideAgainstSolid(player);

                objectCollidingWith = null;
                player.ObjectsToPerformCurrencyTransactionOn.Clear();

                foreach(var worldObject in WorldObjectEntityList)
                {
                    if(player.CollideAgainst(worldObject))
                    {
                        objectCollidingWith = worldObject;
                        break;
                    }
                }

                // We will assume the player is not in a replenish zone, then set to replenish if they are colliding with one.
                player.SetExplorationState(ExplorationState.Consume);
                foreach(var safeZone in SafeZoneList)
                {
                    if(player.CollideAgainst(safeZone))
                    {
                        if (safeZone.IsActive)
                        {
                            player.SetExplorationState(ExplorationState.Replenish);
                        }
                        else
                        {
                            //Let the playe know they can activate the safe zone?
                            player.ObjectsToPerformCurrencyTransactionOn.Add(safeZone);
                        }
                    }
                }
                foreach (var mineral in MineralDepositList)
                {
                    if (player.CollideAgainst(mineral))
                    {
                        player.ObjectsToPerformCurrencyTransactionOn.Add(mineral);
                    }
                }
            }
        }

        #endregion

        #region Destroy Methods

        void CustomDestroy()
		{
            solidCollision.RemoveFromManagers();

            darknessRenderTarget?.Dispose();
		}

        #endregion

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
