using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace Shared.Services
{
    public class SequenceService : ISequenceService
    {
        private readonly IDbConnection _dbConnection;

        public SequenceService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<long> GetNotifyDataNewId()
        {
            var result = await _dbConnection.ExecuteScalarAsync<long>(@"SELECT (NEXT VALUE FOR DataNotifySequence)",
                null,
                null, 120, CommandType.Text);
            return result;
        }

        public async Task TurnOnInsertIdentityWithEmailNotify()
        {
            var result = await _dbConnection.ExecuteScalarAsync(@"SET IDENTITY_INSERT EMAIL_NOTIFY ON",
                null,
                null, 120, CommandType.Text);
        }

        public async Task TurnOnInsertIdentityWithSubcribes()
        {
            var result = await _dbConnection.ExecuteScalarAsync(@"SET IDENTITY_INSERT SUBSCRIBES ON",
                null,
                null, 120, CommandType.Text);
        }
    }
}