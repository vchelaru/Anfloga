﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RenderingLibrary.Graphics
{
    public struct StateChangeInfo
    {
        public Texture2D Texture;
        public SpriteFont SpriteFont;
        public object ObjectRequestingChange;
    }

    public enum BeginType
    {
        Begin,
        Push
    }

    #region BeginParameters class

    public struct BeginParameters
    {
        public bool IsDefault
        {
            get;
            set;
        }

        public SpriteSortMode SortMode { get; set; }
        public BlendState BlendState { get; set; }
        public SamplerState SamplerState { get; set; }
        public DepthStencilState DepthStencilState { get; set; }
        public RasterizerState RasterizerState { get; set; }
        public Effect Effect { get; set; }
        public Matrix TransformMatrix { get; set; }
        public Rectangle ScissorRectangle { get; set; }


        public List<StateChangeInfo> ChangeRecord
        {
            get; set;
        }

        public SpriteFont SpriteFont { get; set; }

        public BeginParameters Clone()
        {
            return (BeginParameters)this.MemberwiseClone();
        }

        public override string ToString()
        {
            return $"{ChangeRecord.Count} calls";
        }
    }

    #endregion

    public class SpriteBatchStack
    {
        enum SpriteBatchBeginEndState
        {
            Ended,
            Began
        }


        #region Fields

        SpriteBatchBeginEndState beginEndState;

        List<BeginParameters> beginParametersUsedThisFrame = new List<BeginParameters>();

        List<BeginParameters?> mStateStack = new List<BeginParameters?>();
        BeginParameters? currentParameters;

        #endregion

        #region Properties

        public SpriteBatch SpriteBatch
        {
            get;
            private set;
        }

        public GraphicsDevice GraphicsDevice
        {
            get
            {
                return SpriteBatch.GraphicsDevice;
            }
        }

        public List<BeginParameters> LastFrameDrawStates
        {
            get
            {
                List<BeginParameters> toReturn = new List<BeginParameters>();

                toReturn.AddRange(beginParametersUsedThisFrame);

                // The last parameters used for draw will not be part of beginParametersUsedThisFrame, so add it here:
                if (currentParameters != null)
                {
                    toReturn.Add(currentParameters.Value);
                }

                return toReturn;
            }
        }

        #endregion

        public SpriteBatchStack(GraphicsDevice graphicsDevice)
        {
            SpriteBatch = new SpriteBatch(graphicsDevice);
        }



        public void Begin()
        {
            var beginParams = new BeginParameters();
            beginParams.ChangeRecord = new List<StateChangeInfo>();

            beginParams.IsDefault = true;
            currentParameters = beginParams;

            if (beginEndState == SpriteBatchBeginEndState.Began)
            {
                SpriteBatch.End();
            }

            beginEndState = SpriteBatchBeginEndState.Began;
            SpriteBatch.Begin();
        }

        public void PushRenderStates(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState,
            DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect,
            Matrix transformMatrix, Rectangle scissorRectangle)
        {


            mStateStack.Add(currentParameters);

            // begin will end 
            ReplaceRenderStates(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix, scissorRectangle);
        }

        public void ReplaceRenderStates(SpriteSortMode sortMode, BlendState blendState, SamplerState samplerState,
            DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix transformMatrix,
            Rectangle scissorRectangle)
        {
            bool isNewRender = currentParameters.HasValue == false;

            var newParameters = new BeginParameters();
            newParameters.ChangeRecord = new List<StateChangeInfo>();

            newParameters.SortMode = sortMode;
            newParameters.BlendState = blendState;
            newParameters.SamplerState = samplerState;
            newParameters.DepthStencilState = depthStencilState;
            newParameters.RasterizerState = rasterizerState;
            newParameters.Effect = effect;
            newParameters.TransformMatrix = transformMatrix;

            try
            {
                newParameters.ScissorRectangle = scissorRectangle;
            }
            catch(Exception e)
            {
                throw new Exception("Could not set scissor rectangle to:" + scissorRectangle.ToString(), e);
            }
            if (currentParameters != null)
            {
                beginParametersUsedThisFrame.Add(currentParameters.Value);
            }

            currentParameters = newParameters;

            if (beginEndState == SpriteBatchBeginEndState.Began)
            {
                SpriteBatch.End();
            }

            try
            {
                SpriteBatch.GraphicsDevice.ScissorRectangle = scissorRectangle;
            }
            catch(Exception e)
            {
                throw new Exception("Error trying to set scissor rectangle:" + scissorRectangle.ToString());
            }
            beginEndState = SpriteBatchBeginEndState.Began;
            SpriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);

        }

        internal void Draw(Texture2D texture2D, Rectangle destinationRectangle, Rectangle sourceRectangle, Color color, object objectRequestingChange)
        {
            AdjustCurrentParametersDrawCall(texture2D, null, objectRequestingChange);

            SpriteBatch.Draw(texture2D, destinationRectangle, sourceRectangle, color);
        }

        internal void Draw(Texture2D texture2D, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color, float rotation, Vector2 vector2, SpriteEffects effects, int layerDepth, object objectRequestingChange)
        {
            AdjustCurrentParametersDrawCall(texture2D, null, objectRequestingChange);


            SpriteBatch.Draw(texture2D, destinationRectangle, sourceRectangle, color, rotation, vector2, effects, layerDepth);
        }

        internal void Draw(Texture2D texture2D, Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 vector22, Vector2 scale, SpriteEffects effects, float depth, object objectRequestingChange)
        {
            AdjustCurrentParametersDrawCall(texture2D, null, objectRequestingChange);

            SpriteBatch.Draw(texture2D, position, sourceRectangle, color, rotation, vector22, scale, effects, depth);
        }

        internal void DrawString(SpriteFont font, string line, Vector2 offset, Color color, object objectRequestingChange)
        {
            AdjustCurrentParametersDrawCall(null, font, objectRequestingChange);

            SpriteBatch.DrawString(font, line, offset, color);
        }

        private void AdjustCurrentParametersDrawCall(Texture2D texture, SpriteFont spriteFont, object objectRequestingChange)
        {
            var paramsValue = currentParameters.Value;

            bool shouldRecordChange = paramsValue.ChangeRecord.Count == 0;

            if(!shouldRecordChange)
            {
                var last = paramsValue.ChangeRecord.Last();

                shouldRecordChange = last.Texture != texture || last.SpriteFont != spriteFont;
            }

            if (shouldRecordChange)
            {
                var newChange = new StateChangeInfo();
                newChange.Texture = texture;
                newChange.SpriteFont = spriteFont;
                newChange.ObjectRequestingChange = objectRequestingChange;

                paramsValue.ChangeRecord.Add(newChange);
                currentParameters = paramsValue;
            }
        }

        void TryEnd()
        {
            if (currentParameters != null)
            {
                End();
            }
        }

        internal void End()
        {

            if (currentParameters != null)
            {
                RecordCurrentParameters();

                if (beginEndState == SpriteBatchBeginEndState.Began)
                {
                    SpriteBatch.End();
                    beginEndState = SpriteBatchBeginEndState.Ended;
                }
            }
            else
            {

                if (beginEndState == SpriteBatchBeginEndState.Began)
                {
                    SpriteBatch.End();
                    beginEndState = SpriteBatchBeginEndState.Ended;

                }
            }
        }

        public void PopRenderStates()
        {

            var parameters = mStateStack.Last();
            mStateStack.RemoveAt(mStateStack.Count - 1);


            if (parameters.HasValue)
            {
                ReplaceRenderStates(parameters.Value.SortMode, parameters.Value.BlendState,
                    parameters.Value.SamplerState, parameters.Value.DepthStencilState,
                    parameters.Value.RasterizerState, parameters.Value.Effect,
                    parameters.Value.TransformMatrix, parameters.Value.ScissorRectangle);
            }
            else
            {
                if (currentParameters != null)
                {
                    beginParametersUsedThisFrame.Add(currentParameters.Value);
                }
                // this is the end
                currentParameters = null;
                End();
            }
        }

        private void RecordCurrentParameters()
        {
            if (currentParameters != null)
            {
                beginParametersUsedThisFrame.Add(currentParameters.Value);
            }
        }

        internal void ClearPerformanceRecordingVariables()
        {
            currentParameters = null;
            beginParametersUsedThisFrame.Clear();
        }
    }
}
