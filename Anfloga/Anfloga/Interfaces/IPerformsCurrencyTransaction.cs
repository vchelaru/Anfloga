using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anfloga.Interfaces
{
    public interface IPerformsCurrencyTransaction
    {
        int CurrentCurrencyBalance { get; }
        void CollectCurrency(int increase);
        void SpendCurrency(int cost);

    }
}
