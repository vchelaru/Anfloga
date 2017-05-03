using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anfloga.GumRuntimes
{
    public partial class StatusBarRuntime
    {
        private int MaxFillTextureHeight;
        private int MaxFillTextureTop;
        partial void CustomInitialize()
        {
            //To account for texture sampling bugs in render libraries, we will adjust the Y by a small amount.
            //We may need to do this on a case by case basis.
            StatusBarFill.Y -= .05f;

            MaxFillTextureHeight = StatusBarFill.TextureHeight;
            MaxFillTextureTop = StatusBarFill.TextureTop;
        }

        public void UpdateFillHeight(float percentage)
        { 
            var testValue = FlatRedBall.Math.MathFunctions.RoundFloat(MaxFillTextureHeight* percentage, 1);
            int newTextureHeight = FlatRedBall.Math.MathFunctions.RoundToInt(MaxFillTextureHeight * percentage);
            int topOffset = MaxFillTextureHeight - newTextureHeight;

            StatusBarFill.TextureTop = MaxFillTextureTop + topOffset;
            StatusBarFill.TextureHeight = newTextureHeight;
        }
    }
}
