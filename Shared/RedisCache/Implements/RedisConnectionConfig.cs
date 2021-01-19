using System.ComponentModel.DataAnnotations;

namespace Shared.RedisCache.Implements
{
    public enum RedisConnectionConfig
    {
        [Display(Name = "Redis:Servers")] Server,
        [Display(Name = "Redis:Password")] Password,
        [Display(Name = "Redis:DbId")] DbId,
        [Display(Name = "Redis:LogDbId")] LogDbId
    }
}