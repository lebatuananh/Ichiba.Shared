using System.Collections.Generic;

namespace Shared.Common.Algorithm
{
    public abstract class ChooseAlgorithm
    {
        public abstract T Get<T>(IReadOnlyCollection<T> items);
    }
}