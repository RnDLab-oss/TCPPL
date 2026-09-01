using ERP_API.DTOs;
using ERP_API.Helpers;
using ERP_API.Helpers;
using ERP_API.Interface;
using Microsoft.Data.SqlClient;
using System.Data;
using System.IdentityModel.Tokens.Jwt;

namespace ERP_API.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly DBHelper _db;
        public ReportRepository(DBHelper db)
        {
            _db = db;
        }
        public DataTable GetData(ReportRequest request)
        {
            SqlParameter[] param =
            {
                new SqlParameter("@CompId", request.CompId),
                new SqlParameter("@BranchId", request.BranchId),
                new SqlParameter("@AcYear", request.AcYear),
                new SqlParameter("@RptType", request.RptType)
            };

            return _db.ExecuteDataTable("Udp_Api_Get_Data", param);
        }

    }
}