using System.ComponentModel.DataAnnotations;
using Shared.EF.Interfaces;

namespace Shared.EF.Implement
{
    public abstract class Entity<T> : IEntity
    {
        [Key] public T Id { get; set; }
    }
}