using ERP_API.DTOs;
using ERP_API.Helpers;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ERP_API.Repositories
{
    public interface ICommonRepository
    {
        ApiResponse GetDataById(GetDataRequest request);
        ApiResponse GetUserPermissions(UserpermissionRequest request);

        ApiResponse GetDropdown(GetDropdownRequest request);
    }

    public class CommonRepository : ICommonRepository
    {
        private readonly DBHelper _db;
        public CommonRepository(DBHelper db)
        {
            _db = db;
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
            return _db.ExecuteJson("Udp_Web_GetData", param);
        }

        public ApiResponse GetUserPermissions(UserpermissionRequest request)
        {
            SqlParameter[] param =
            {
                new SqlParameter("@CompId", request.CompId),
                new SqlParameter("@BranchId", request.BranchId),
                new SqlParameter("@AcYear", request.AcYear),
                new SqlParameter("@RoleId", request.RoleId),
                new SqlParameter("@UserToken", request.UserToken ?? "")
            };
            return _db.ExecuteJson("udp_Web_GetUserPermission", param);
        }

        public ApiResponse GetDropdown(GetDropdownRequest _req)
        {
            SqlParameter[] param =
            {
                new SqlParameter("@CompId", _req.CompId),
                new SqlParameter("@BranchId", _req.BranchId),
                new SqlParameter("@AcYear", _req.AcYear),
                new SqlParameter("@FormKey", _req.FormKey)
            };


            DataSet ds = _db.ExecuteDataSet("Udp_Web_GetDropdown", param);
            ApiResponse response = new ApiResponse();

            if (ds.Tables.Count > 0)
            {
                Dictionary<string, object> dropdownData = new Dictionary<string, object>();
                foreach (DataTable dt in ds.Tables)
                {
                    if (dt.Rows.Count > 0)
                    {
                        string DDName = pvtUtilitiy.ToString(dt.Rows[0]["DropdownName"]);
                        var list = dt.AsEnumerable()
                            .Select(row => new
                            {
                                Id = Convert.ToInt32(row["Id"]),
                                Name = row["Name"].ToString()
                            })
                            .ToList();
                        dropdownData.Add(DDName, list);
                    }
                }
                response.Success = true;
                response.Message = "Dropdown loaded successfully";
                response.Data = dropdownData;
            }
            else
            {
                response.Success = false;
                response.Message = "No dropdown data found";
                response.Data = DBNull.Value;
            }

            return response;
        }
    }
}