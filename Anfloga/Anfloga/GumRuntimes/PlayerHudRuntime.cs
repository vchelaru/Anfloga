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
        public int MineralText;
    }
    public partial class PlayerHudRuntime
    {
        public void UpdateHud(HudUpdateData data)
        {
            //Update status bars
            ExplorationLimitStatusBar.UpdateFillHeight(data.ExplorationLimitFill);
            //Update currency display
            ResourceDisplayInstance.ResourceDisplayText = data.MineralText.ToString();
        }
    }
}
