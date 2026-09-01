using ERP_API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace ERP_API.Helpers
{
    public class DBHelper
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly IHttpContextAccessor? _httpContextAccessor;


        // ============================================================
        // CONSTRUCTOR
        // ============================================================

        public DBHelper(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString =_configuration.GetConnectionString("DefaultConnection");
        }

        public DBHelper(IConfiguration configuration,IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _connectionString =_configuration.GetConnectionString("DefaultConnection");
            _httpContextAccessor = httpContextAccessor;
        }


        // ============================================================
        // SET DATABASE OBJECT NAME
        // ============================================================

        private void SetDbObjectName(string procedureName)
        {
            if (procedureName.Equals(
                "Udp_Web_InsertApiLog",
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (_httpContextAccessor != null &&
                _httpContextAccessor.HttpContext != null)
            {
                _httpContextAccessor.HttpContext.Items["DbObjectName"]
                    = procedureName;
            }
        }


        // EXECUTE JSON

        public ApiResponse ExecuteJson( string procedureName,params SqlParameter[] parameters)
        {
            DataTable dt =ExecuteDataTable(procedureName, parameters);
            List<Dictionary<string, object>> data =new List<Dictionary<string, object>>();

            foreach (DataRow row in dt.Rows)
            {
                Dictionary<string, object> item =
                    new Dictionary<string, object>();

                foreach (DataColumn col in dt.Columns)
                {
                    item[col.ColumnName] =
                        row[col] == DBNull.Value? null: row[col];
                }
                data.Add(item);
            }
            return new ApiResponse
            {
                Success = true,
                Message = "Data Loaded Successfully",
                Count = data.Count,
                Data = data
            };
        }

        // EXECUTE JSON DATASET

        public ApiResponse ExecuteJsonDataSet( string procedureName,params SqlParameter[] parameters)
        {
            DataSet ds =ExecuteDataSet(procedureName, parameters);
            List<object> tables = new List<object>();
            foreach (DataTable dt in ds.Tables)
            {
                List<Dictionary<string, object>> data =new List<Dictionary<string, object>>();
                foreach (DataRow row in dt.Rows)
                {
                    Dictionary<string, object> item = new Dictionary<string, object>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        item[col.ColumnName] = row[col] == DBNull.Value ? null: row[col];
                    }
                    data.Add(item);
                }
                tables.Add(data);
            }

            return new ApiResponse
            {
                Success = true,
                Message = "Data Loaded Successfully",
                Count = tables.Count,
                Data = tables
            };
        }

        // EXECUTE NON QUERY

        public int ExecuteNonQuery(string procedureName, params SqlParameter[] parameters)
        {
        
            SetDbObjectName(procedureName);

            using SqlConnection con = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(procedureName, con);
            cmd.CommandType = CommandType.StoredProcedure;
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }
            con.Open();
            return cmd.ExecuteNonQuery();
        }

        // ============================================================
        // EXECUTE SCALAR
        // ============================================================
        public object ExecuteScalar(string procedureName, params SqlParameter[] parameters)
        {
            SetDbObjectName(procedureName);

            using SqlConnection con = new SqlConnection(_connectionString);
            using SqlCommand cmd =new SqlCommand(procedureName, con);

            cmd.CommandType =
                CommandType.StoredProcedure;

            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }

            con.Open();

            return cmd.ExecuteScalar();
        }


        // ============================================================
        // EXECUTE READER
        // ============================================================

        public SqlDataReader ExecuteReader(
            string procedureName,
            params SqlParameter[] parameters)
        {
            // Capture Stored Procedure Name
            SetDbObjectName(procedureName);


            SqlConnection con =
                new SqlConnection(_connectionString);

            SqlCommand cmd =
                new SqlCommand(procedureName, con);

            cmd.CommandType =
                CommandType.StoredProcedure;

            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }

            con.Open();

            return cmd.ExecuteReader(
                CommandBehavior.CloseConnection);
        }


        // ============================================================
        // EXECUTE DATA TABLE
        // ============================================================

        public DataTable ExecuteDataTable(
            string procedureName,
            params SqlParameter[] parameters)
        {
            // Capture Stored Procedure Name
            SetDbObjectName(procedureName);


            using SqlConnection con =
                new SqlConnection(_connectionString);

            using SqlCommand cmd =
                new SqlCommand(procedureName, con);

            cmd.CommandType =
                CommandType.StoredProcedure;

            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }

            using SqlDataAdapter da =
                new SqlDataAdapter(cmd);

            DataTable dt =
                new DataTable();

            da.Fill(dt);

            return dt;
        }


        // ============================================================
        // EXECUTE DATA SET
        // ============================================================

        public DataSet ExecuteDataSet(
            string procedureName,
            params SqlParameter[] parameters)
        {
            // Capture Stored Procedure Name
            SetDbObjectName(procedureName);


            using SqlConnection con =
                new SqlConnection(_connectionString);

            using SqlCommand cmd =
                new SqlCommand(procedureName, con);

            cmd.CommandType =
                CommandType.StoredProcedure;

            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }

            using SqlDataAdapter da =
                new SqlDataAdapter(cmd);

            DataSet ds =
                new DataSet();

            da.Fill(ds);

            return ds;
        }
    }
}