using DominatorHouseCore.Enums.QdQuery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.StartupActivity.Quora
{
    public class QuoraQuestionActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(QuestionQueryParameters);
        }

        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(QuestionQueryParameters)).ToList();
        }
    }
}
