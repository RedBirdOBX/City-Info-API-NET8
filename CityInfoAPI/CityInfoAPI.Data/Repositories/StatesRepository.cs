using CityInfoAPI.Data.DbContents;
using CityInfoAPI.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace CityInfoAPI.Data.Repositories
{
    public class StatesRepository : IStatesRepository
    {
        private readonly CityInfoDbContext _dbContext;
        private readonly ILogger<StatesRepository> _logger;

        public StatesRepository(CityInfoDbContext dbContext, ILogger<StatesRepository> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException();
            _logger = logger ?? throw new ArgumentNullException();
        }

        public async Task<IEnumerable<State>> GetStatesAsync()
        {
            try
            {
                var results = await _dbContext.States.AsNoTracking().ToListAsync();
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while getting states. {ex}");
                throw;
            }
        }

        public async Task<State?> GetStateAsync(string stateCode)
        {
            try
            {
                return await _dbContext.States.AsNoTracking().Where(s => s.StateCode.ToLower() == stateCode.ToLower()).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while getting state: {stateCode}. {ex}");
                throw;
            }
        }
    }
}
