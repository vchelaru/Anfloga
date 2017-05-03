using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anfloga.GumRuntimes
{
    public struct HudUpdateData
    {
        public float ExplorationLimitFill;
    }
    public partial class PlayerHudRuntime
    {
        public void UpdateHud(HudUpdateData data)
        {
            //Update status bars
            ExplorationLimitStatusBar.UpdateFillHeight(data.ExplorationLimitFill);
        }
    }
}
