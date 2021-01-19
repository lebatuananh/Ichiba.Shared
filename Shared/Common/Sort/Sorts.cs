using System.Collections.Generic;
using System.Linq;

namespace Shared.Common.Sort
{
    public class Sorts : List<Sort>
    {
        public Sorts()
        {
        }

        public Sorts(IEnumerable<Sort> collection)
            : base(collection)
        {
        }

        public Sorts(int capacity)
            : base(capacity)
        {
        }

        public string SortExpression
        {
            get
            {
                var sortExpressions = this.Select(m => m.SortExpression);

                return string.Join(',', sortExpressions);
            }
        }
    }
}