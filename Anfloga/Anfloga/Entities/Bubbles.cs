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
using Microsoft.Xna.Framework.Graphics;

namespace Anfloga.Entities
{
    public enum BubbleEmitterType
    {
        Geyser,
        Sub,
    }

    public static class BubbleEmitterTypeExtenstion
    {
        public static string ToFiendlyName(this BubbleEmitterType emitterType)
        {
            string toReturn = string.Empty;
            switch(emitterType)
            {
                case BubbleEmitterType.Geyser: toReturn = nameof(BubbleEmitterType.Geyser); break;
                case BubbleEmitterType.Sub: toReturn = nameof(BubbleEmitterType.Sub); break;
            }

            return toReturn;
        }
    }

	public partial class Bubbles
	{
        private List<Tuple<Sprite, double>> bubbles;
        public BubbleEmitterType CurrentBubbleEmitterType;

        private FlatRedBall.Screens.Screen currentScreen;

        private double timeSinceLastSpawn;
        private bool ShouldSpawnBubble => currentScreen.PauseAdjustedSecondsSince(timeSinceLastSpawn) >= BubbleSpawnFrequency;
        private Texture2D textureForBubbles;
        private AnimationChainList animations;

        private bool isEmitting
        {
            get
            { 
                //We are emitting it the emitter is a geyser or the parent's velocity is not 0.
                bool toReturn = CurrentBubbleEmitterType == BubbleEmitterType.Geyser;

                if(toReturn == false && Parent != null)
                {
                    toReturn = Parent.Velocity.Length() != 0;
                }

                return toReturn;
            }
        }

        /// <summary>
        /// Initialization logic which is execute only one time for this Entity (unless the Entity is pooled).
        /// This method is called when the Entity is added to managers. Entities which are instantiated but not
        /// added to managers will not have this method called.
        /// </summary>
		private void CustomInitialize()
		{
            bubbles = new List<Tuple<Sprite, double>>();

            currentScreen = FlatRedBall.Screens.ScreenManager.CurrentScreen;

            CurrentBubbleEmitterType = BubbleEmitterType.Geyser;

            textureForBubbles = (Texture2D)GetFile("anflogaSprites");
            animations = (AnimationChainList)GetFile("AnimationChainListFile");
		}

		private void CustomActivity()
		{
            if (isEmitting)
            {
                SpawnNewBubbleActivity();
            }

            UpdateBubbleActivity();

        }

        private void UpdateBubbleActivity()
        {
            for(int i = bubbles.Count -1; i > -1; i--)
            {
                var bubbleTuple = bubbles[i];
                if(currentScreen.PauseAdjustedSecondsSince(bubbleTuple.Item2) >= BubbleLifeTime)
                {
                    SpriteManager.RemoveSprite(bubbleTuple.Item1);
                    bubbles.Remove(bubbleTuple);
                }
            }
        }

        private void SpawnNewBubbleActivity()
        {
            if(ShouldSpawnBubble)
            {
                timeSinceLastSpawn = currentScreen.PauseAdjustedCurrentTime;

                SpawnBubble();
            }
        }

        private void SpawnBubble()
        {
            var newBubble = SpriteManager.AddParticleSprite(textureForBubbles);
            newBubble.AnimationChains = animations;

            //Set Random bubble chain.
            int bubbleIndex = FlatRedBallServices.Random.Next(3) + 1;
            newBubble.CurrentChainName = CurrentBubbleEmitterType.ToFiendlyName() + bubbleIndex;
            newBubble.TextureScale = 1;

            //Set the position to the parent, and the z so it draws behind the parent.
            if (Parent != null)
            {
                float positionOffset = FlatRedBallServices.Random.Between(-PositionOffset, PositionOffset);
                newBubble.Position = Parent.Position;
                newBubble.Z--;

                // Set the positional offsets
                // and velocity based on the type 
                switch (CurrentBubbleEmitterType)
                {
                    case BubbleEmitterType.Geyser:
                        newBubble.X += positionOffset;
                        newBubble.YVelocity = BubbleInitialVelocity;
                        newBubble.Acceleration.Y = BubbleAccelleration;
                        break;
                    case BubbleEmitterType.Sub:
                        newBubble.Y += positionOffset;
                        newBubble.Velocity = Parent.Velocity;
                        newBubble.Velocity.Normalize();
                        newBubble.Velocity *= -BubbleInitialVelocity;
                        break;
                }

                
            }

            bubbles.Add(new Tuple<Sprite, double>(newBubble, timeSinceLastSpawn));
        }
        private string GetBubbleAnimationChain()
        {
            return "Bubble6";
        }

        private void CustomDestroy()
		{
            currentScreen = null;

            SpriteManager.RemoveAllParticleSprites();

            bubbles.Clear();

		}

        private static void CustomLoadStaticContent(string contentManagerName)
        {


        }
	}
}
