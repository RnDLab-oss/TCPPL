using DTOModel;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Repository
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly EmpTaskMsDbContext _empTaskMsDbContext;
        public EmployeeRepository(EmpTaskMsDbContext empTaskMsDbContext)
        {
            _empTaskMsDbContext = empTaskMsDbContext;
        }

        
        public async Task<ApiResponse> GetAllEmply()
        {
            try
            {
                var data = await _empTaskMsDbContext.EmployeeMasters
                    .Where(x => x.Role != "Admin")
                    .ToListAsync();

                if (data.Count == 0)
                {
                    return new ApiResponse(
                        "404",
                        false,
                        null,
                        "No employee found."
                    );
                }

                return new ApiResponse(
                    "200",
                    true,
                    data,
                    "Employee fetched successfully."
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse(
                    "500",
                    false,
                    null,
                    "An error occurred during fetching employee. " + ex.Message
                );
            }
        }
    }
}
