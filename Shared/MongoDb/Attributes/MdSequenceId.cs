using System;

namespace Shared.MongoDb.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MdSequenceId : Attribute
    {
        public string Id { get; set; }
    }
}