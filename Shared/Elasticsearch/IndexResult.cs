using System;

namespace Shared.Elasticsearch
{
    public class IndexResult
    {
        public bool IsValid { get; set; }

        public string ErrorReason { get; set; }

        public Exception Exception { get; set; }
    }
}