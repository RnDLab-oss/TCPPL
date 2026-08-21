using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.IRepository
{
    public interface IStoredProcedureRepository
    {
        Task<int> InsertDataAsync(string procedureName, SqlParameter[] sqlParameters);
        Task<int> UpdateDataAsync(string procedureName, SqlParameter[] sqlParameters);
        Task<int> PatchDataAsync(string procedureName, SqlParameter[] sqlParameters);
        Task<int> GetDataAsync(string procedureName, SqlParameter[] sqlParameters);
        Task<DataSet> GetDataSetAsync(string procedureName, SqlParameter[] sqlParameters);
        Task<DataSet> GetDataSetAsync(string procedureName);
        Task<int> DeleteDataAsync(int id);
        Task<object> InsertDataScalarAsync(string procedureName, SqlParameter[] parameters);
    }
}
