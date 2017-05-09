using System;
using System.Collections.Generic;
using System.Reflection;

using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBall.Screens;
using Microsoft.Xna.Framework;

using System.Linq;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Anfloga.Rendering;

namespace Anfloga
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        // TODO: strip these out once post proc is implemented at lower level
        IEffectsRenderer effectRenderer;
        IEffectsRenderer passthroughRenderer;
        RenderTarget2D renderBuffer;

        public Game1() : base ()
        {
            graphics = new GraphicsDeviceManager(this);

#if WINDOWS_PHONE || ANDROID || IOS

			// Frame rate is 30 fps by default for Windows Phone,
            // so let's keep that for other phones too
            TargetElapsedTime = TimeSpan.FromTicks(333333);
            graphics.IsFullScreen = true;
#elif WINDOWS || DESKTOP_GL
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
#endif


#if WINDOWS_8
            FlatRedBall.Instructions.Reflection.PropertyValuePair.TopLevelAssembly = 
                this.GetType().GetTypeInfo().Assembly;
#endif

        }

        protected override void Initialize()
        {
#if IOS
			var bounds = UIKit.UIScreen.MainScreen.Bounds;
			var nativeScale = UIKit.UIScreen.MainScreen.Scale;
			var screenWidth = (int)(bounds.Width * nativeScale);
			var screenHeight = (int)(bounds.Height * nativeScale);
			graphics.PreferredBackBufferWidth = screenWidth;
			graphics.PreferredBackBufferHeight = screenHeight;
#endif

            this.IsMouseVisible = true;

            FlatRedBallServices.InitializeFlatRedBall(this, graphics);
            FlatRedBall.Localization.LocalizationManager.CurrentLanguage = 1;
            FlatRedBallServices.GraphicsOptions.TextureFilter = TextureFilter.Point;

            // init our post proc effects (camera init now happens in here)
            InitializePostProcessing();

			GlobalContent.Initialize();

			FlatRedBall.Screens.ScreenManager.Start(typeof(Anfloga.Screens.GameScreen));

            base.Initialize();
        }


        protected override void Update(GameTime gameTime)
        {
            FlatRedBallServices.Update(gameTime);

            FlatRedBall.Screens.ScreenManager.Activity();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            DrawWithFrbDefaults();

            //DrawWithShaders();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Draws the way FlatRedBall draws by default
        /// </summary>
        private void DrawWithFrbDefaults()
        {
            FlatRedBallServices.Draw();
        }

        /// <summary>
        /// Draws entire buffer using shaders
        /// </summary>
        private void DrawWithShaders()
        {
            // intercept draw call by setting a custom render target
            graphics.GraphicsDevice.SetRenderTarget(renderBuffer);

            // draw game to render target
            FlatRedBallServices.Draw();

            // Example implementation to apply post proc effects
            // NOTE: if you want to draw with no effects you can
            // draw using passthroughRenderer.Draw(renderBuffer)!
            effectRenderer.Draw(renderBuffer);
        }

        /// <summary>
        /// Initializes effects so they can be used in the draw cycle
        /// </summary>
        private void InitializePostProcessing()
        {
            CameraSetup.SetupCamera(SpriteManager.Camera, graphics);

            var device = graphics.GraphicsDevice;

            // instance of the bloom effect
            effectRenderer = new BloomRenderer();
            effectRenderer.Initialize(device, Camera.Main);

            // instance of the passthrough effect
            passthroughRenderer = new PassThroughRenderer();
            passthroughRenderer.Initialize(device, Camera.Main);

            var viewport = Camera.Main.GetViewport();

            renderBuffer = new RenderTarget2D(device, viewport.Width, viewport.Height, false, device.DisplayMode.Format, DepthFormat.Depth24);
        }
    }
}
