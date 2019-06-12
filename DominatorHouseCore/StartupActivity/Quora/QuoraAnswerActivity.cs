using DominatorHouseCore.Enums.QdQuery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.StartupActivity.Quora
{
    public class QuoraAnswerActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(AnswerQueryParameters);
        }

        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(AnswerQueryParameters)).ToList();
        }
    }
}
