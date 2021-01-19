using System;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Common.Algorithm
{
    public class ChooseRandomAlgorithm : ChooseAlgorithm
    {
        public override T Get<T>(IReadOnlyCollection<T> items)
        {
            var random = new Random();
            var length = items.Count;
            var index = random.Next(0, length);

            return items.ElementAt(index);
        }
    }
}