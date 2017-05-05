using Anfloga.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anfloga.Interfaces
{
    public interface IPerformCurrencyTransactionOn
    {
        bool PerformCurrencyTransaction(IPerformsCurrencyTransaction player);
    }
}
