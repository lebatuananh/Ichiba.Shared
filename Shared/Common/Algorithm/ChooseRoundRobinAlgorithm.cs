using System.Collections.Generic;
using System.Linq;

namespace Shared.Common.Algorithm
{
    public class ChooseRoundRobinAlgorithm : ChooseAlgorithm
    {
        private readonly object lockObject = new object();
        private int currentIndex;

        public override T Get<T>(IReadOnlyCollection<T> items)
        {
            lock (lockObject)
            {
                var length = items.Count;

                if (currentIndex >= length) currentIndex = 0;

                return items.ElementAt(currentIndex++);
            }
        }
    }
}