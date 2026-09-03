using ERP_API.DTOs;
using ERP_API.Helpers;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ERP_API.Repositories
{
    public interface IReportRepository
    {
        ApiResponse GetData(ReportRequest request);
        ApiResponse GetDataById(GetDataRequest request);

        DashboardKpiResponse GetDashboardKpi(DashboardKpiRequest request);

        //ApiResponse GetTaskList(TaskListRequest request);

    }
    public class ReportRepository : IReportRepository
    {
        private readonly DBHelper _db;
        public ReportRepository(DBHelper db)
        {
            _db = db;
        }
        public ApiResponse GetData(ReportRequest request)
        {
            SqlParameter[] param =
            {
                new SqlParameter("@CompId", request.CompId),
                new SqlParameter("@BranchId", request.BranchId),
                new SqlParameter("@AcYear", request.AcYear),
                new SqlParameter("@UserId", request.UserId),
                new SqlParameter("@FromDate", request.FromDate),
                new SqlParameter("@ToDate", request.ToDate),
                new SqlParameter("@RptType", request.RptType)
            };
            return _db.ExecuteJson("Udp_Web_Reports", param);
        }

        public ApiResponse GetDataById(GetDataRequest request)
        {
            SqlParameter[] param =
            {
                new SqlParameter("@CompId", request.CompId),
                new SqlParameter("@BranchId", request.BranchId),
                new SqlParameter("@AcYear", request.AcYear),
                new SqlParameter("@RptType", request.RptType),
                new SqlParameter("@Id", request.Id),
                new SqlParameter("@Value", request.Value ?? "")
            };

            return _db.ExecuteJson("Udp_Api_GetDataById", param);
        }

        public DashboardKpiResponse GetDashboardKpi(DashboardKpiRequest request)
        {
            SqlParameter[] param =
            {
                new SqlParameter("@CompId", request.CompId),
                new SqlParameter("@BranchId", request.BranchId),
                new SqlParameter("@AcYear", request.AcYear),
                new SqlParameter("@RptType", request.RptType),
                new SqlParameter("@ViewPeriod", request.ViewPeriod),
                new SqlParameter("@UserToken", request.UserToken ?? "")
            };

            DataSet ds = _db.ExecuteDataSet("udp_Web_GetDashboardKPIs",param);

            var response = new DashboardKpiResponse();
            // TABLE 1 - SUMMARY

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                response.Summary = new DashboardSummary
                {
                    TotalTasks = pvtUtilitiy.ToInt(row["TotalTasks"]),
                    Completed = pvtUtilitiy.ToInt(row["Completed"]),
                    Pending = pvtUtilitiy.ToInt(row["Pending"]),
                    Overdue = pvtUtilitiy.ToInt(row["Overdue"]),
                    ExtensionsUsed = pvtUtilitiy.ToInt(row["ExtensionsUsed"]),

                    CompletionRate = pvtUtilitiy.ToDecimal(row["CompletionRate"]),
                    PendingRate = pvtUtilitiy.ToDecimal(row["PendingRate"]),
                    OverdueRate = pvtUtilitiy.ToDecimal(row["OverdueRate"])
                };
            }

            // TABLE 2 - TASK STATUS
            if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
            {
                DataRow row = ds.Tables[1].Rows[0];
                response.TaskStatus = new DashboardTaskStatus
                {
                    Completed = pvtUtilitiy.ToInt(row["Completed"]),
                    Pending = pvtUtilitiy.ToInt(row["Pending"]),
                    Overdue = pvtUtilitiy.ToInt(row["Overdue"])
                };
            }

            // TABLE 3 - MONTHLY TREND
            response.MonthlyTrend = new List<object>();
            response.DepartmentPerformance = new List<DepartmentPerformance>();

            if (ds.Tables.Count > 3)
            {
                foreach (DataRow row in ds.Tables[3].Rows)
                {
                    response.DepartmentPerformance.Add(new DepartmentPerformance
                        {
                            DepartmentID = pvtUtilitiy.ToInt(row["DepartmentID"]),
                            DepartmentName = pvtUtilitiy.ToString(row["DepartmentName"]),
                            Assigned = pvtUtilitiy.ToInt(row["Assigned"]),
                            Completed = pvtUtilitiy.ToInt(row["Completed"]),
                            CompletionRate = pvtUtilitiy.ToDecimal(row["CompletionRate"])
                        }
                    );
                }
            }
            return response;
        }

        //public ApiResponse GetTaskList(TaskListRequest request)
        //{
        //    SqlParameter[] param =
        //    {
        //        new SqlParameter("@DepartmentID", request.DepartmentID ?? (object)DBNull.Value),
        //        new SqlParameter("@TaskType", request.TaskType ?? (object)DBNull.Value),
        //        new SqlParameter("@Frequency", request.Frequency ?? (object)DBNull.Value),
        //        new SqlParameter("@Priority", request.Priority ?? (object)DBNull.Value),
        //        new SqlParameter("@UserID", request.UserID ?? (object)DBNull.Value),
        //        new SqlParameter("@StatusKey", request.StatusKey ?? (object)DBNull.Value),
        //        new SqlParameter("@Search", request.Search ?? (object)DBNull.Value),
        //        new SqlParameter("@Tab", request.Tab ?? (object)DBNull.Value),
        //        new SqlParameter("@AsOfDate", request.AsOfDate ?? (object)DBNull.Value)
        //    };

        //    return _db.ExecuteJson("udp_Web_Tasks_GetList", param);
        //}


    }
}