using System;
using System.Collections.Generic;

namespace DominatorHouseCore.StartupActivity
{
    public abstract class BaseActivity
    {
        public abstract List<string> GetQueryType();
        public abstract Type GetEnumType();
    }
}
