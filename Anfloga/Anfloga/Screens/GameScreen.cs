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
using FlatRedBall.Glue.StateInterpolation;
using StateInterpolationPlugin;

namespace Anfloga.Screens
{
	public partial class GameScreen
	{
        #region Fields

        static string LevelNameToLoad = nameof(smallerMap);
        //static string LevelNameToLoad = nameof(anflogaTest);

        LayeredTileMap currentLevel;

        TileShapeCollection solidCollision;

        DialogLogic dialogLogic;

        WorldObjectEntity objectCollidingWith;

        DarknessTrigger lastDarknessTriggerCollidedAgainst;
        Tweener lastDarknessTweener;

        #endregion

        #region Initialize Methods

        void CustomInitialize()
		{
            AdjustLayerOrthoValues();

            InitializeFactoryEvents();

            LoadLevel(LevelNameToLoad);

            InitializeCamera();

            InitializeLastCheckpoint();

            InitializeCollision();

            InitializeDialogBoxLogic();

            InitializeHud();

            InitializeRenderTargets();

            InitializeShaders();

            MoveObjectsToCorrectRenderLayer();

            InitializeRestartVariables();
		}

        private void AdjustLayerOrthoValues()
        {
            // Adjust any layer which has its destination rectangle set in glue:
            WorldLayer.LayerCameraSettings.OrthogonalWidth = Camera.Main.OrthogonalWidth;
            WorldLayer.LayerCameraSettings.OrthogonalHeight = Camera.Main.OrthogonalHeight;

            DarknessRenderTargetLayer.LayerCameraSettings.OrthogonalWidth = Camera.Main.OrthogonalWidth;
            DarknessRenderTargetLayer.LayerCameraSettings.OrthogonalHeight = Camera.Main.OrthogonalHeight;
        }

        private void InitializeShaders()
        {
            ShaderRendererInstance.Effect = TestShader;
            ShaderRendererInstance.Viewer = PlayerList[0];

            ShaderRendererInstance.InitializeRenderVariables();
        }

        private void InitializeFactoryEvents()
        {
            PlayerList.CollectionChanged += (sender, args) =>
            {
                if (args.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    foreach (Player p in args.NewItems) p.MoveToLayer(WorldLayer);
                }
            };

            SafeZoneList.CollectionChanged += (sender, args) =>
            {
                if (args.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    foreach (SafeZone s in args.NewItems) s.MoveToLayer(WorldLayer);
                }
            };

            MineralDepositList.CollectionChanged += (sender, args) =>
            {
                if (args.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    foreach (MineralDeposit m in args.NewItems) m.MoveToLayer(WorldLayer);
                }
            };

            BubblesList.CollectionChanged += (sender, args) =>
            {
                if (args.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    foreach (Bubbles b in args.NewItems) b.MoveToLayer(WorldLayer);
                }
            };


            DarknessTriggerList.CollectionChanged += (sender, args) =>
            {
                if (args.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    foreach (DarknessTrigger d in args.NewItems) d.MoveToLayer(WorldLayer);
                }
            };

            SeaLifeList.CollectionChanged += (sender, args) =>
            {
                if(args.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    foreach (SeaLife s in args.NewItems) s.MoveToLayer(WorldLayer);
                }
            };
        }

        private void InitializeRestartVariables()
        {
            RestartVariables.Add("this.PlayerList[0].X");
            RestartVariables.Add("this.PlayerList[0].XVelocity");
            RestartVariables.Add("this.PlayerList[0].Y");
            RestartVariables.Add("this.PlayerList[0].YVelocity");
        }

        private void MoveObjectsToCorrectRenderLayer()
        {
            foreach(var item in PlayerList)
            {
                item.InitializeLightLayer(DarknessRenderTargetLayer);
            }

            foreach(var item in this.LightEntityList)
            {
                item.MoveToLayer(DarknessRenderTargetLayer);
            }

            foreach(var item in WorldObjectEntityList)
            {
                var layerToPlaceOn = item.ShouldPlaceOnUiLayer ? AboveEverythingLayer : WorldLayer;
                item.MoveToLayer(layerToPlaceOn);
            }
        }

        private void InitializeRenderTargets()
        {
            bool shouldExecute = true;
#if DEBUG
            shouldExecute = DebuggingVariables.RenderWithNoShaders == false;
#endif
            if(shouldExecute)
            {
                this.WorldLayer.RenderTarget = WorldRenderTarget;

                this.ShaderRendererInstance.WorldTexture = WorldRenderTarget;
            }
            this.DarknessRenderTargetLayer.RenderTarget = DarknessRenderTarget;
            this.ShaderRendererInstance.DarknessTexture = DarknessRenderTarget;
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
            solidCollision.Visible = false;

            var layer = currentLevel.MapLayers.First(item =>item.Name == "ForegroundLevel");

            solidCollision.AddMergedCollisionFromLayer(layer, currentLevel, (propertyList) =>
            {
                return propertyList.Any(item => item.Name == "SolidCollision");
            });

#if DEBUG
            if(DebuggingVariables.ShowTerrainCollision)
            {
                solidCollision.Visible = true;
            }
#endif
        }



        private void InitializeCamera()
        {
            if (PlayerList.Count > 0)
            {
                CameraControllerInstance.ObjectFollowing = PlayerList[0];
            }
            else
            {
                throw new Exception("The map must contain a tile or object with EntityToCreate set to Player");
            }

            CameraControllerInstance.SetCameraBoundsFromTiledMap(currentLevel);
        }

        private void InitializeLastCheckpoint()
        {
            // This is the only spot I could find that was "late" enough to initialize this info.
            // Feel free to do it somewhere better if there is one.
            if (PlayerList.Count > 0)
            {
               PlayerList[0].UpdateLastCheckpointPosition(PlayerList[0].X, PlayerList[0].Y);
            }
        }

        private void LoadLevel(string levelNameToLoad)
        {
            TMXGlueLib.DataTypes.ReducedTileMapInfo.FastCreateFromTmx = true;

            currentLevel = (LayeredTileMap)GetFile(levelNameToLoad);

            currentLevel.AddToManagers(WorldLayer);

            TileEntityInstantiator.CreateEntitiesFrom(currentLevel);

#if DEBUG
            if(DebuggingVariables.ShowWorldObjectCollision)
            {
                foreach (var item in WorldObjectEntityList)
                {
                    item.Collision.Visible = true;
                }
            }

            if (DebuggingVariables.ShowReplenishZoneCollision)
            {
                foreach (var item in SafeZoneList)
                {
                    item.Collision.Visible = true;
                }
            }

            if (DebuggingVariables.ShowMineralDepositCollision)
            {
                foreach(var item in MineralDepositList)
                {
                    item.Collision.Visible = true;
                }
            }


            if(DebuggingVariables.ShowDarknessTriggerCollision)
            {
                foreach(var item in DarknessTriggerList)
                {
                    item.Collision.Visible = true;
                }
            }

#endif
            // todo: create collision:
            
            foreach(var item in SafeZoneList)
            {
                item.SetupCollision();
            }
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

                    worldObject.SetIsVisible(true);
                    if(player.CollideAgainst(worldObject))
                    {
                        objectCollidingWith = worldObject;
                        worldObject.SetIsVisible(false);
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

                foreach (var darknessTrigger in DarknessTriggerList)
                {
                    if(player.CollideAgainst(darknessTrigger) && lastDarknessTriggerCollidedAgainst != darknessTrigger)
                    {
                        lastDarknessTriggerCollidedAgainst = darknessTrigger;
                        RespondToDarknessTriggerCollision(darknessTrigger);
                    }
                }
            }
        }

        private void RespondToDarknessTriggerCollision(DarknessTrigger darknessTrigger)
        {
            if(lastDarknessTweener != null && lastDarknessTweener.Running)
            {
                lastDarknessTweener.Stop();
                lastDarknessTweener = null;
            }

            float currentValue = (float)ShaderRendererInstance.DarknessAlpha;

            Tweener tweener = new Tweener(currentValue, darknessTrigger.DarknessValue, darknessTrigger.DarknessInterpolationTime, InterpolationType.Linear, Easing.In);

            tweener.PositionChanged = HandlePositionSet;

            tweener.Owner = this;

            TweenerManager.Self.Add(tweener);
            tweener.Start();
        }

        private void HandlePositionSet(float value)
        {
            this.ShaderRendererInstance.DarknessAlpha = value;
        }

        #endregion

        #region Destroy Methods

        void CustomDestroy()
		{
            solidCollision.RemoveFromManagers();
		}

        #endregion

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
