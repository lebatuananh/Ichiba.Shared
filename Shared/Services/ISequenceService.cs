using System.Threading.Tasks;

namespace Shared.Services
{
    public interface ISequenceService
    {
        Task<long> GetNotifyDataNewId();
        Task TurnOnInsertIdentityWithEmailNotify();
        Task TurnOnInsertIdentityWithSubcribes();
    }
}