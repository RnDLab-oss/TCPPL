using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Model;
using Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class StoredProcedureRepository : IStoredProcedureRepository
    {
        private readonly DatabaseHelper _databaseHelper;
        private readonly Logger<StoredProcedureRepository> _logger;
        public StoredProcedureRepository(DatabaseHelper databaseHelper)
        {
            _databaseHelper = databaseHelper;
            // _logger = logger;
        }

        //public async Task<DataTable> GetDataAsync(int id)
        //{
        //    var parameters = new SqlParameter[]
        //    {
        //        new SqlParameter("@Id", SqlDbType.Int) { Value = id }
        //    };
        //    return await _databaseHelper.ExecuteStoredProcedureAsync("GetProcedureName", parameters);
        //}

        public async Task<int> InsertDataAsync(string procedureName, SqlParameter[] parameters)
        {
            return await _databaseHelper.ExecuteNonQueryStoredProcedureAsync(procedureName, parameters);
        }

        public async Task<object> InsertDataScalarAsync(string procedureName, SqlParameter[] parameters)
        {
            return await _databaseHelper.ExecuteScalarQueryStoredProcedureAsync(procedureName, parameters);
        }



        public async Task<int> UpdateDataAsync(string procedureName, SqlParameter[] parameters)
        {
            return await _databaseHelper.ExecuteNonQueryStoredProcedureAsync(procedureName, parameters);
        }

        public async Task<int> PatchDataAsync(string procedureName, SqlParameter[] parameters)
        {
            return await _databaseHelper.ExecuteNonQueryStoredProcedureAsync(procedureName, parameters);
        }

        public async Task<int> GetDataAsync(string procedureName, SqlParameter[] parameters)
        {
            try
            {
                return await _databaseHelper.ExecuteNonQueryStoredProcedureAsync(procedureName, parameters);
            }
            catch (Exception ex)
            {
                return -2;
            }
        }

        public async Task<DataSet> GetDataSetAsync(string procedureName, SqlParameter[] parameters)
        {
            try
            {
                return await _databaseHelper.ExecuteDataSetStoredProcedureAsync(procedureName, parameters);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<DataSet> GetDataSetAsync(string procedureName)
        {
            try
            {
                return await _databaseHelper.ExecuteDataSetStoredProcedureAsync(procedureName);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        //public async Task<IEnumerable<T>> GetDataAsync<T>(string storedProcedure, SqlParameter[] parameters)
        //{
        //    using (SqlConnection connection = new SqlConnection(_connectionString))
        //    {
        //        await connection.OpenAsync();
        //        var dynamicParameters = new DynamicParameters();

        //        foreach (var param in parameters)
        //        {
        //            dynamicParameters.Add(param.ParameterName, param.Value, param.DbType, param.Direction, param.Size);
        //        }

        //        return await connection.QueryAsync<T>(
        //            storedProcedure,
        //            dynamicParameters,
        //            commandType: CommandType.StoredProcedure
        //        );
        //    }
        //}

        public async Task<int> DeleteDataAsync(int id)
        {
            var parameters = new SqlParameter[]
            {
                new SqlParameter("@Id", SqlDbType.Int) { Value = id }
            };

            return await _databaseHelper.ExecuteNonQueryStoredProcedureAsync("DeleteProcedureName", parameters);
        }
    }
}
