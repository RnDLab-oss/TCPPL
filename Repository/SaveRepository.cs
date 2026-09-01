using ERP_API.DTOs;
using ERP_API.Helpers;
using ERP_API.Repositories;
using Microsoft.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

namespace ERP_API.Repositories
{
    public interface ISaveRepository
    {
        ApiResponse SaveTask(SaveTaskRequest request);
        ApiResponse RoleMaster(SaveRoleReq request);  
        ApiResponse RolePermission(SaveRolePermissionReq request);
    }

    public class SaveRepository : ISaveRepository
    {
        private readonly DBHelper _db;
        public SaveRepository(DBHelper db)
        {
            _db = db;
        }

        public ApiResponse SaveTask(SaveTaskRequest _req)
        {
            SqlParameter[] param =
            {
                new SqlParameter("@Mode", _req.Mode),
                new SqlParameter("@EntryNo", _req.EntryNo),
                new SqlParameter("@TaskTitle", _req.TaskTitle),
                new SqlParameter("@Description", _req.Description),
                new SqlParameter("@DepartmentId", _req.DepartmentId),
                new SqlParameter("@TaskType", _req.TaskType),
                new SqlParameter("@Frequency", _req.Frequency),
                new SqlParameter("@AssignEmpId", _req.AssignEmpId),
                new SqlParameter("@Priority", _req.Priority),
                new SqlParameter("@Reminder", _req.Reminder),
                new SqlParameter("@StartDate", _req.StartDate),
                new SqlParameter("@DueDate", _req.DueDate)
            };

            DataTable dt = _db.ExecuteDataTable("Udp_Web_Save_Task", param);
            ApiResponse response = new ApiResponse();
            if (dt.Rows.Count > 0)
            {
                response.Success = Convert.ToBoolean(dt.Rows[0]["Success"]);
                response.Message = pvtUtilitiy.ToString( dt.Rows[0]["Message"]);
                response.Id = Convert.ToInt32(dt.Rows[0]["EntryNo"]);
            }
            return response;
        }

        public ApiResponse RoleMaster(SaveRoleReq _req)
        {
            SqlParameter[] param =
            {
                new SqlParameter("@cmid", _req.Cmid),
                new SqlParameter("@branchid", _req.BranchID),
                new SqlParameter("@RoleID", _req.RoleID),
                new SqlParameter("@RoleCode", _req.RoleCode),
                new SqlParameter("@RoleName", _req.RoleName),
                new SqlParameter("@Description", _req.Description),
                new SqlParameter("@DisplayOrder", _req.DisplayOrder),
                new SqlParameter("@IsActive", _req.IsActive),
                new SqlParameter("@UserToken", _req.UserToken)
            };

            DataTable dt = _db.ExecuteDataTable( "Udp_Web_Save_Role",param);
            ApiResponse response = new ApiResponse();
            if (dt.Rows.Count > 0)
            {
                response.Success =pvtUtilitiy.ToBool(dt.Rows[0]["Success"]);
                response.Message =pvtUtilitiy.ToString(dt.Rows[0]["Message"]);
                if (dt.Columns.Contains("RoleID"))
                {
                    response.Id =pvtUtilitiy.ToInt(dt.Rows[0]["RoleID"]);
                }
            }
            return response;
        }

        public ApiResponse RolePermission(SaveRolePermissionReq req)
        {
            SqlParameter[] param =
            {
            new SqlParameter("@cmid", req.Cmid),
            new SqlParameter("@RoleID", req.RoleID),
            new SqlParameter("@Permissions",JsonConvert.SerializeObject(req.Permissions)
            )
        };

            DataTable dt = _db.ExecuteDataTable("Udp_Web_Save_RolePermission",param);
            ApiResponse response = new ApiResponse();
            if (dt.Rows.Count > 0)
            {
                response.Success = pvtUtilitiy.ToBool(dt.Rows[0]["Success"]);
                response.Message = pvtUtilitiy.ToString(dt.Rows[0]["Message"]);
            }
            return response;
        }

    }
}