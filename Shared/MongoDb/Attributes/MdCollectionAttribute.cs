using System;

namespace Shared.MongoDb.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MdCollectionAttribute : Attribute
    {
        public string Name { get; set; }
    }
}