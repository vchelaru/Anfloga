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

        static string LevelNameToLoad = nameof(theMap);
        //static string LevelNameToLoad = nameof(anflogaTest);

        LayeredTileMap currentLevel;

        TileShapeCollection solidCollision;

        DialogLogic dialogLogic;

        WorldObjectEntity objectCollidingWith;

        RenderTarget2D worldRenderTarget;
        RenderTarget2D darknessRenderTarget;

        DarknessTrigger lastDarknessTriggerCollidedAgainst;
        Tweener lastDarknessTweener;

        #endregion

        #region Initialize Methods

        void CustomInitialize()
		{
            InitializeFactoryEvents();

            LoadLevel(LevelNameToLoad);

            InitializeCamera();

            InitializeCollision();

            InitializeDialogBoxLogic();

            InitializeHud();

            InitializeRenderTargets();

            MoveLightObjectsToRenderTargetLayer();

            InitializeRestartVariables();
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

            WorldObjectEntityList.CollectionChanged += (sender, args) =>
            {
                if (args.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    foreach (WorldObjectEntity e in args.NewItems) e.MoveToLayer(WorldLayer);
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


        }

        private void InitializeRestartVariables()
        {
            RestartVariables.Add("this.DarknessSprite.Alpha");
            
            RestartVariables.Add("this.PlayerList[0].X");
            RestartVariables.Add("this.PlayerList[0].XVelocity");
            RestartVariables.Add("this.PlayerList[0].Y");
            RestartVariables.Add("this.PlayerList[0].YVelocity");
        }

        private void MoveLightObjectsToRenderTargetLayer()
        {
            foreach(var item in PlayerList)
            {
                item.InitializeLightLayer(DarknessRenderTargetLayer);
            }

            foreach(var item in this.LightEntityList)
            {
                item.MoveToLayer(DarknessRenderTargetLayer);
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
            this.DarknessSprite.TextureFilter = TextureFilter.Anisotropic;

#if DEBUG
            if (DebuggingVariables.HideDarknessOverlay)
            {
                this.DarknessSprite.Visible = false;
            }
#endif

            worldRenderTarget = new RenderTarget2D(FlatRedBallServices.GraphicsDevice,
                (int)FlatRedBallServices.GraphicsOptions.ResolutionWidth, (int)FlatRedBallServices.GraphicsOptions.ResolutionHeight);
            this.WorldLayer.RenderTarget = worldRenderTarget;

            this.WorldSprite.Texture = worldRenderTarget;
            this.WorldSprite.TextureScale = -1;
            this.WorldSprite.Y = -.2f;
            this.WorldSprite.Y = .2f;
            //this.WorldSprite.ColorOperation = FlatRedBall.Graphics.ColorOperation.Color;
            this.WorldSprite.Blue = 1;

            this.WorldSprite.Width = Camera.Main.OrthogonalWidth;
            this.WorldSprite.Height = Camera.Main.OrthogonalHeight;

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
            solidCollision.AddMergedCollisionFrom(currentLevel, (propertyList) =>
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

            if(InputManager.Keyboard.KeyPushed(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                using (var stream = System.IO.File.OpenWrite("test.png"))
                {
                    worldRenderTarget.SaveAsPng(stream, worldRenderTarget.Width, worldRenderTarget.Height);
                }

            }
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

            float currentValue = (float)DarknessSprite.Alpha;

            Tweener tweener = new Tweener(currentValue, darknessTrigger.DarknessValue, darknessTrigger.DarknessInterpolationTime, InterpolationType.Linear, Easing.In);

            tweener.PositionChanged = HandlePositionSet;

            tweener.Owner = this;

            TweenerManager.Self.Add(tweener);
            tweener.Start();
        }

        private void HandlePositionSet(float value)
        {
            DarknessSprite.Alpha = value;
        }

        #endregion

        #region Destroy Methods

        void CustomDestroy()
		{
            solidCollision.RemoveFromManagers();

            darknessRenderTarget.Dispose();
            worldRenderTarget.Dispose();
		}

        #endregion

        static void CustomLoadStaticContent(string contentManagerName)
        {


        }

	}
}
