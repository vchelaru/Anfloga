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
            MaxFillTextureHeight = StatusBarFill.TextureHeight;
            MaxFillTextureTop = StatusBarFill.TextureTop;
        }

        public void UpdateFillHeight(float percentage)
        {
            int newTextureHeight = (int)(MaxFillTextureHeight * percentage);
            int topOffset = MaxFillTextureHeight - newTextureHeight;

            StatusBarFill.TextureTop = MaxFillTextureTop + topOffset;
            StatusBarFill.TextureHeight = newTextureHeight;
        }
    }
}
