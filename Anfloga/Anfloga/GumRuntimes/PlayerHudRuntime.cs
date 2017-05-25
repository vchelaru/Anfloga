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
        public bool IsFillingUp;

        public bool IsLow;
    }
    public partial class PlayerHudRuntime
    {
        public void UpdateHud(HudUpdateData data)
        {
            //Update status bars
            ExplorationLimitStatusBar.UpdateFillHeight(data.ExplorationLimitFill);
            //Update currency display
            ResourceDisplayInstance.ResourceDisplayText = data.MineralText.ToString();

            if(data.IsFillingUp)
            {
                this.ExplorationLimitStatusBar.CurrentRecoverySpendingTypeState = StatusBarRuntime.RecoverySpendingType.Charging;
            }
            else if(data.IsLow)
            {
                this.ExplorationLimitStatusBar.CurrentRecoverySpendingTypeState = StatusBarRuntime.RecoverySpendingType.DrainingLow;
            }
            else
            {
                this.ExplorationLimitStatusBar.CurrentRecoverySpendingTypeState = StatusBarRuntime.RecoverySpendingType.Draining;
            }
        }
    }
}
