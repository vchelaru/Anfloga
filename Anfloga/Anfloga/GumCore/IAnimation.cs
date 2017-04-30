﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace RenderingLibrary.Graphics
{
    public interface IAnimation
    {
        bool FlipHorizontal
        {
            get;
        }

        bool FlipVertical
        {
            get;
        }

        Texture2D CurrentTexture
        {
            get;
        }

        Rectangle? SourceRectangle
        {
            get;
        }

        void AnimationActivity(double currentTime);

        int CurrentFrameIndex
        {
            get;
        }
    }
}
