using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anfloga.GumRuntimes
{
    public struct HudUpdateData
    {
        public float OxygenFill;
        public float HealthFill;
    }
    public partial class PlayerHudRuntime
    {
        public void UpdateHud(HudUpdateData data)
        {
            //Update status bars
            OxygenStatusBar.UpdateFillHeight(data.OxygenFill);
            HealthStatusBar.UpdateFillHeight(data.HealthFill);
        }
    }
}
