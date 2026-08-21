using DTOModel;
using Microsoft.Extensions.Logging;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Operation
{
    public class EmployeeOperation  : IEmployeeOperation
    {
        private readonly IEmployeeRepository _employeeRepository;
        //private readonly ILogger<RoleOperation> _logger;

        public EmployeeOperation(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }
        public async Task<ApiResponse> GetAllEmployee()
        {
            try
            {
                return await _employeeRepository.GetAllEmply();
            }
            catch (Exception ex)
            {
                return new ApiResponse("500", false, null, "An error occured during fetching emlpoyee." + ex.Message);
            }
        }
    }
}
