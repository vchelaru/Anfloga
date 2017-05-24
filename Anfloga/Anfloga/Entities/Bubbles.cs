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
using Microsoft.Xna.Framework;

namespace Anfloga.Entities
{
    #region Enums

    public enum BubbleEmitterType
    {
        Geyser,
        Sub,
    }

    #endregion

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
        private List<Sprite> bubbles;
        public BubbleEmitterType CurrentBubbleEmitterType;

        private FlatRedBall.Screens.Screen currentScreen;

        private double timeSinceLastSpawn;
        private bool ShouldSpawnBubble => currentScreen.PauseAdjustedSecondsSince(timeSinceLastSpawn) >= BubbleSpawnFrequency;
        private Texture2D textureForBubbles;
        private AnimationChainList animations;

        public Vector3 ThrustVector;
        public float SpawnOffset;

        private bool isEmitting
        {
            get
            { 
                //We are emitting if the type is a Geyser of if the thrust vector is !0.
                bool toReturn = CurrentBubbleEmitterType == BubbleEmitterType.Geyser;

                if(toReturn == false)
                {
                    toReturn = ThrustVector.LengthSquared() != 0;
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
            bubbles = new List<Sprite>();

            currentScreen = FlatRedBall.Screens.ScreenManager.CurrentScreen;

            CurrentBubbleEmitterType = BubbleEmitterType.Geyser;

            SpawnOffset = 0f;

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
                var bubble = bubbles[i];
                if (currentScreen.PauseAdjustedSecondsSince(bubble.TimeCreated) >= BubbleLifeTime)
                {
                    DestroyBubble(bubble);
                }
                else if(bubble.Y > YToDestroyAt)
                {
                    DestroyBubble(bubble);
                }
                else
                {
                    ApplyVelocityAdjustments(bubble);
                }
            }
        }

        private void DestroyBubble(Sprite bubble)
        {
            SpriteManager.RemoveSprite(bubble);
            bubbles.Remove(bubble);
        }

        private void ApplyVelocityAdjustments(Sprite bubble)
        {
            //Drag calculations
            var calculatedCoefficient = bubble.Width * DragCoefficient;
            bubble.Velocity -= bubble.Velocity * calculatedCoefficient * TimeManager.SecondDifference;

            //Random X calculations
            bubble.XVelocity += FlatRedBallServices.Random.Between(-RandomXVelocityAdjustments, RandomXVelocityAdjustments);

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
            if(LayerProvidedByContainer != null)
            {
                SpriteManager.AddToLayer(newBubble, this.LayerProvidedByContainer);
            }
            newBubble.AnimationChains = animations;

            //Set Random bubble chain.
            int bubbleIndex = FlatRedBallServices.Random.Next(3) + 1;
            newBubble.CurrentChainName = CurrentBubbleEmitterType.ToFiendlyName() + bubbleIndex;
            newBubble.TextureScale = 1;

            var bubbleArea = newBubble.Width * newBubble.Height;

            //Set the position to the parent, and the z so it draws behind the parent.
            if (Parent != null)
            {
                float positionOffset = FlatRedBallServices.Random.Between(-RandomSpawnOffset, RandomSpawnOffset);
                newBubble.Position = this.Position; 

                // Set the positional offsets
                // and velocity based on the type 
                switch (CurrentBubbleEmitterType)
                {
                    case BubbleEmitterType.Geyser:
                        newBubble.X += positionOffset;
                        newBubble.YVelocity = BubbleInitialVelocity;
                        newBubble.YAcceleration = bubbleArea * AccellerationCoefficient;
                        break;
                    case BubbleEmitterType.Sub:
                        ThrustVector.Normalize();
                        newBubble.X += positionOffset * ThrustVector.Y;
                        newBubble.Y += positionOffset * -ThrustVector.X;
                        newBubble.YAcceleration = bubbleArea *AccellerationCoefficient;

                        newBubble.Velocity = ThrustVector * -BubbleInitialVelocity;
                        break;
                }

                
            }
            float lifeTimeOffset = FlatRedBallServices.Random.Between(-RandomLifeTimeOffset, RandomLifeTimeOffset);

            //Adjusting the timeSinceLastSpawn will increase/decrease the lifetime of the bubble.
            //We do this so each spawn has a slightly different life time
            newBubble.TimeCreated = timeSinceLastSpawn + RandomLifeTimeOffset;
            bubbles.Add(newBubble);
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
