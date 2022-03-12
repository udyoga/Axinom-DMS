using Dapper;
using DMS_recipient.Logics.Interface;
using DMS_recipient.Models;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace DMS_recipient.Logics.Implementation
{
    public class DatabaseLogic : IDatabaseLogic
    {

        private readonly IOptionsMonitor<Configs> _config;
        public DatabaseLogic(IOptionsMonitor<Configs> optionsMonitor)
        {
            _config = optionsMonitor;
        }
        public async Task<int> SaveFileContent(string fileData)
        {
            string sql = @"INSERT INTO json_data(`data`)VALUES(@data);";

            try
            {
                using (var c = new MySqlConnection(_config.CurrentValue.BDconnection))
                {
                    return await c.ExecuteAsync(sql, new { data = fileData });
                }
            }
            catch (System.Exception)
            {
                throw;
            }            
        }
    }
}
