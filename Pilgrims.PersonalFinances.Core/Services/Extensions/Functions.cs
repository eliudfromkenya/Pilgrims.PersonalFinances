using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pilgrims.PersonalFinances.Core.Services.Extensions
{
    public static class Functions
    {
        public static decimal? Max(decimal? a, decimal? b)
        {
            if (a == null && b == null) return null;
            if (a == null) return b;
            if (b == null) return a;
            return Math.Max(a.Value, b.Value);
        }
    }
}
