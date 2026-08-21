using DTOModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Model;
using Operation;
using System.Data;
using System.Security.Cryptography;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeOperation _employeeOperation;
        private readonly EmpTaskMsDbContext _empTaskMsDbContext;
        private readonly IConfiguration _configuration;
        public EmployeeController(IEmployeeOperation employeeOperation, IConfiguration configuration , EmpTaskMsDbContext empTaskMsDbContext)
        {
            _employeeOperation = employeeOperation;
            _empTaskMsDbContext = empTaskMsDbContext;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("GetAllEmployee")]
        public async Task<ApiResponse> GetAllEmployee()
        {
            try
            {
                var key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
                Console.WriteLine(key);
                return await _employeeOperation.GetAllEmployee();

            }
            catch (Exception ex)
            {
                return new ApiResponse("500", false, null, "An error occured during fetching employee." + ex.Message);

            }
        }
        [HttpPost("GetEmployeeCount")]
        public async Task<ApiResponse> GetEmployeeCount()
        {
            try
            {
                int count = await _empTaskMsDbContext.EmployeeMasters
                    .Where(x => x.Role != "Admin")
                    .CountAsync();

                return new ApiResponse(
                    "200",
                    true,
                    count,
                    "Total number of employees found."
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse(
                    "500",
                    false,
                    null,
                    "An error occurred during fetching employee count. " + ex.Message
                );
            }
        }


        [HttpGet("GetEmployeeTaskList/{employeeId}")]
        public async Task<ApiResponse> GetEmployeeTaskList(int employeeId)
        {
            try
            {
                var tasks = await _empTaskMsDbContext.TaskManagements
                    .Where(x => x.AssignedEmployeeId == employeeId)
                    .OrderByDescending(x => x.CreatedDate)
                    .ToListAsync();

                return new ApiResponse(
                    "200",
                    true,
                    tasks,
                    "Employee task list found."
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse(
                    "500",
                    false,
                    null,
                    "Error while fetching employee tasks. " + ex.Message
                );
            }
        }


        [HttpPost("UpdateEmployeeTaskStatus")]
        public async Task<ApiResponse> UpdateEmployeeTaskStatus([FromForm] EmployeeTaskStatusDTO model)
        {
            try
            {
                // ==========================================
                // 1. Basic Validation
                // ==========================================

                if (model.TaskId <= 0)
                {
                    return new ApiResponse(
                        "400",
                        false,
                        null,
                        "Invalid Task Id."
                    );
                }

                if (string.IsNullOrWhiteSpace(model.EmployeeStatus))
                {
                    return new ApiResponse(
                        "400",
                        false,
                        null,
                        "Employee status is required."
                    );
                }


                // ==========================================
                // 2. Find Task
                // ==========================================

                var task = await _empTaskMsDbContext.TaskManagements
                    .FirstOrDefaultAsync(x => x.TaskId == model.TaskId);

                if (task == null)
                {
                    return new ApiResponse(
                        "404",
                        false,
                        null,
                        "Task not found."
                    );
                }


                // ==========================================
                // 3. Status Validation
                // ==========================================

                var allowedStatus = new[]
                {
            "Pending",
            "In Processing",
            "Completed"
        };

                if (!allowedStatus.Contains(model.EmployeeStatus))
                {
                    return new ApiResponse(
                        "400",
                        false,
                        null,
                        "Invalid employee status."
                    );
                }


                // ==========================================
                // 4. Completed => Remarks Required
                // ==========================================

                if (model.EmployeeStatus == "Completed" &&
                    string.IsNullOrWhiteSpace(model.EmployeeRemarks))
                {
                    return new ApiResponse(
                        "400",
                        false,
                        null,
                        "Employee remarks are required to complete the task."
                    );
                }


                // ==========================================
                // 5. APPROVED Task Cannot Be Changed
                // ==========================================

                if (task.AdminStatus == "Approved")
                {
                    return new ApiResponse(
                        "400",
                        false,
                        null,
                        "Approved task cannot be updated."
                    );
                }


                // ==========================================
                // 6. Update Employee Status
                // ==========================================

                if (model.EmployeeStatus == "Pending")
                {
                    task.EmployeeStatus = "Pending";

                    task.EmployeeRemarks = model.EmployeeRemarks;

                    task.AdminStatus = "Pending";
                }

                else if (model.EmployeeStatus == "In Processing")
                {
                    task.EmployeeStatus = "In Processing";

                    task.EmployeeRemarks = model.EmployeeRemarks;

                    task.AdminStatus = "Pending";
                }

                else if (model.EmployeeStatus == "Completed")
                {
                    task.EmployeeStatus = "Completed";

                    task.EmployeeRemarks = model.EmployeeRemarks;

                    // Employee ne task complete kiya.
                    // Ab Admin review karega.
                    task.AdminStatus = "Pending";
                }


                // ==========================================
                // 7. Updated Date
                // ==========================================

                task.UpdatedDate = DateTime.Now;


                // ==========================================
                // 8. Save
                // ==========================================

                await _empTaskMsDbContext.SaveChangesAsync();


                // ==========================================
                // 9. Success Response
                // ==========================================

                string message;

                if (model.EmployeeStatus == "Completed")
                {
                    message =
                        "Task completed successfully and sent for admin review.";
                }
                else if (model.EmployeeStatus == "In Processing")
                {
                    message =
                        "Task is now in processing.";
                }
                else
                {
                    message =
                        "Task status updated to pending.";
                }


                return new ApiResponse(
                    "200",
                    true,
                    null,
                    message
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse(
                    "500",
                    false,
                    null,
                    "Something went wrong: " + ex.Message
                );
            }
        }


        [HttpPost("UpdateAdminTaskStatus")]
        public async Task<ApiResponse> UpdateAdminTaskStatus([FromForm] AdminTaskStatusDTO model)
        {
            try
            {
                // Find Task
                var task = await _empTaskMsDbContext.TaskManagements
                    .FirstOrDefaultAsync(x => x.TaskId == model.TaskId);

                if (task == null)
                {
                    return new ApiResponse(
                        "404",
                        false,
                        null,
                        "Task not found."
                    );
                }

                // Validate Admin Status
                if (model.AdminStatus != "Approved" &&
                    model.AdminStatus != "Rework")
                {
                    return new ApiResponse(
                        "400",
                        false,
                        null,
                        "Admin status must be Approved or Rework."
                    );
                }


                // ==========================
                // APPROVED
                // ==========================
                if (model.AdminStatus == "Approved")
                {
                    task.AdminStatus = "Approved";
                    task.AdminRemarks = model.AdminRemarks;

                    task.EmployeeStatus = "Completed";
                    task.CompletionDate = DateTime.Now;
                }


                // ==========================
                // REWORK
                // ==========================
                else
                {
                    task.AdminStatus = "Rework";
                    task.AdminRemarks = model.AdminRemarks;

                    task.EmployeeStatus = "Pending";
                    task.CompletionDate = null;
                }


                task.UpdatedDate = DateTime.Now;

                await _empTaskMsDbContext.SaveChangesAsync();


                string message = model.AdminStatus == "Approved"
                    ? "Task approved successfully."
                    : "Task sent for rework successfully.";


                return new ApiResponse(
                    "200",
                    true,
                    null,
                    message
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse(
                    "500",
                    false,
                    null,
                    "An error occurred while updating task status. " + ex.Message
                );
            }
        }

        [HttpGet("GetInProcessingTaskCount")]
        public async Task<ApiResponse> GetInProcessingTaskCount()
        {
            try
            {
                var count = await _empTaskMsDbContext.TaskManagements
                    .CountAsync(x => x.EmployeeStatus == "In Processing");

                return new ApiResponse(
                    "200",
                    true,
                    count,
                    "In Processing task count found."
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse(
                    "500",
                    false,
                    null,
                    ex.Message
                );
            }
        }


        [HttpGet("GetApprovedTaskCount")]
        public async Task<ApiResponse> GetApprovedTaskCount()
        {
            try
            {
                var count = await _empTaskMsDbContext.TaskManagements
                    .CountAsync(x => x.AdminStatus == "Approved");

                return new ApiResponse(
                    "200",
                    true,
                    count,
                    "Approved task count found."
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse(
                    "500",
                    false,
                    null,
                    ex.Message
                );
            }
        }


        [HttpGet("GetReworkTaskCount")]
        public async Task<ApiResponse> GetReworkTaskCount()
        {
            try
            {
                var count = await _empTaskMsDbContext.TaskManagements
                    .CountAsync(x => x.AdminStatus == "Rework");

                return new ApiResponse(
                    "200",
                    true,
                    count,
                    "Rework task count found."
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse(
                    "500",
                    false,
                    null,
                    ex.Message
                );
            }
        }


        [HttpGet("GetUpcomingTasks")]
        public async Task<ApiResponse> GetUpcomingTasks()
        {
            try
            {
                var today = DateTime.Today;
                var nextSevenDays = today.AddDays(7);

                var taskList = await (
                    from task in _empTaskMsDbContext.TaskManagements

                    join emp in _empTaskMsDbContext.EmployeeMasters
                        on task.AssignedEmployeeId equals emp.EmployeeId

                    where task.Deadline.Date >= today
                          && task.Deadline.Date <= nextSevenDays
                          && task.AdminStatus != "Approved"

                    select new UpcomingTaskDTO
                    {
                        TaskId = task.TaskId,
                        TaskTitle = task.TaskTitle,
                        TaskDescription = task.TaskDescription,
                        Priority = task.Priority,
                        Deadline = task.Deadline,
                        EmployeeName = emp.EmployeeName,
                        AdminStatus = task.AdminStatus,
                        EmployeeStatus = task.EmployeeStatus
                    }
                ).ToListAsync();


                // Priority Sorting
                var result = taskList
                    .OrderBy(x =>
                        x.Priority == "High" ? 1 :
                        x.Priority == "Medium" ? 2 :
                        x.Priority == "Low" ? 3 : 4
                    )
                    .ThenBy(x => x.Deadline)
                    .ToList();


                return new ApiResponse(
                    "200",
                    true,
                    result,
                    "Upcoming tasks found successfully."
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse(
                    "500",
                    false,
                    null,
                    ex.Message
                );
            }
        }

        [HttpGet("GetPendingTaskCount")]
        public async Task<ApiResponse> GetPendingTaskCount()
        {
            try
            {
                var count = await _empTaskMsDbContext.TaskManagements
                    .CountAsync(x => x.EmployeeStatus == "Pending");

                return new ApiResponse(
                    "200",
                    true,
                    count,
                    "Pending task count found."
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse(
                    "500",
                    false,
                    null,
                    ex.Message
                );
            }
        }


        // =========================================================
        // 1. EMPLOYEE TOTAL TASK COUNT
        // =========================================================

        [HttpGet("GetEmployeeTotalTaskCount/{employeeId}")]
        public async Task<ApiResponse> GetEmployeeTotalTaskCount(int employeeId)
        {
            try
            {
                int count = await _empTaskMsDbContext.TaskManagements
                    .CountAsync(x => x.AssignedEmployeeId == employeeId);

                return new ApiResponse(
                    "200",
                    true,
                    count,
                    "Employee total task count found."
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse(
                    "500",
                    false,
                    null,
                    "An error occurred while fetching employee task count. "
                    + ex.Message
                );
            }
        }


        // =========================================================
        // 2. EMPLOYEE PENDING TASK COUNT
        // =========================================================

        [HttpGet("GetEmployeePendingTaskCount/{employeeId}")]
        public async Task<ApiResponse> GetEmployeePendingTaskCount(int employeeId)
        {
            try
            {
                int count = await _empTaskMsDbContext.TaskManagements
                    .CountAsync(x =>
                        x.AssignedEmployeeId == employeeId &&
                        x.EmployeeStatus == "Pending"
                    );

                return new ApiResponse(
                    "200",
                    true,
                    count,
                    "Employee pending task count found."
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse(
                    "500",
                    false,
                    null,
                    "An error occurred while fetching pending task count. "
                    + ex.Message
                );
            }
        }


        // =========================================================
        // 3. EMPLOYEE IN PROCESSING TASK COUNT
        // =========================================================

        [HttpGet("GetEmployeeInProcessingTaskCount/{employeeId}")]
        public async Task<ApiResponse> GetEmployeeInProcessingTaskCount(
            int employeeId)
        {
            try
            {
                int count = await _empTaskMsDbContext.TaskManagements
                    .CountAsync(x =>
                        x.AssignedEmployeeId == employeeId &&
                        x.EmployeeStatus == "In Processing"
                    );

                return new ApiResponse(
                    "200",
                    true,
                    count,
                    "Employee in processing task count found."
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse(
                    "500",
                    false,
                    null,
                    "An error occurred while fetching in processing task count. "
                    + ex.Message
                );
            }
        }


        // =========================================================
        // 4. EMPLOYEE APPROVED TASK COUNT
        // =========================================================

        [HttpGet("GetEmployeeApprovedTaskCount/{employeeId}")]
        public async Task<ApiResponse> GetEmployeeApprovedTaskCount(
            int employeeId)
        {
            try
            {
                int count = await _empTaskMsDbContext.TaskManagements
                    .CountAsync(x =>
                        x.AssignedEmployeeId == employeeId &&
                        x.AdminStatus == "Approved"
                    );

                return new ApiResponse(
                    "200",
                    true,
                    count,
                    "Employee approved task count found."
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse(
                    "500",
                    false,
                    null,
                    "An error occurred while fetching approved task count. "
                    + ex.Message
                );
            }
        }


        // =========================================================
        // 5. EMPLOYEE PRIORITY WISE TASK LIST
        // =========================================================

        [HttpGet("GetEmployeePriorityTaskList/{employeeId}")]
        public async Task<ApiResponse> GetEmployeePriorityTaskList(
            int employeeId)
        {
            try
            {
                var taskList = await _empTaskMsDbContext.TaskManagements

                    .Where(x => x.AssignedEmployeeId == employeeId)

                    .OrderBy(x =>
                        x.Priority == "High" ? 1 :
                        x.Priority == "Medium" ? 2 :
                        x.Priority == "Low" ? 3 : 4
                    )

                    .ThenBy(x => x.Deadline)

                    .Select(x => new
                    {
                        x.TaskId,
                        x.TaskTitle,
                        x.TaskDescription,

                        x.AssignedDate,
                        x.Deadline,

                        x.Priority,

                        x.EmployeeStatus,
                        x.EmployeeRemarks,

                        x.AdminStatus,
                        x.AdminRemarks,

                        x.CompletionDate,
                        x.DelayDays
                    })

                    .ToListAsync();


                return new ApiResponse(
                    "200",
                    true,
                    taskList,
                    taskList.Count > 0
                        ? "Employee priority task list found."
                        : "No task found for this employee."
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse(
                    "500",
                    false,
                    null,
                    "An error occurred while fetching employee priority task list. "
                    + ex.Message
                );
            }
        }


        [HttpPost("AssignEmployeeTask")]
        public async Task<ApiResponse> AssignEmployeeTask([FromForm] AssignEmployeeTaskDTO model)
        {
            try
            {
                // ==============================
                // VALIDATION
                // ==============================

                if (model == null)
                {
                    return new ApiResponse(
                        "400",
                        false,
                        null,
                        "Invalid request."
                    );
                }

                if (model.AssignedEmployeeId <= 0)
                {
                    return new ApiResponse(
                        "400",
                        false,
                        null,
                        "Please select employee."
                    );
                }

                if (string.IsNullOrWhiteSpace(model.TaskTitle))
                {
                    return new ApiResponse(
                        "400",
                        false,
                        null,
                        "Task title is required."
                    );
                }

                if (model.Deadline.Date < DateTime.Today)
                {
                    return new ApiResponse(
                        "400",
                        false,
                        null,
                        "Deadline cannot be in the past."
                    );
                }

                if (string.IsNullOrWhiteSpace(model.Priority))
                {
                    return new ApiResponse(
                        "400",
                        false,
                        null,
                        "Priority is required."
                    );
                }


                // ==============================
                // CONNECTION STRING
                // ==============================

                string connectionString =
                    _configuration.GetConnectionString("defaultString");


                // ==============================
                // SQL CONNECTION
                // ==============================

                using SqlConnection connection =
                    new SqlConnection(connectionString);


                // ==============================
                // SQL COMMAND
                // ==============================

                using SqlCommand command =
                    new SqlCommand(
                        "Usp_AssignEmployeeTask",
                        connection
                    );

                command.CommandType =
                    CommandType.StoredProcedure;


                // ==============================
                // PARAMETERS
                // ==============================

                command.Parameters.Add(
                    "@TaskTitle",
                    SqlDbType.NVarChar,
                    200
                ).Value = model.TaskTitle;


                command.Parameters.Add(
                    "@TaskDescription",
                    SqlDbType.NVarChar,
                    -1
                ).Value =
                    string.IsNullOrWhiteSpace(model.TaskDescription)
                        ? DBNull.Value
                        : model.TaskDescription;


                command.Parameters.Add(
                    "@AssignedEmployeeId",
                    SqlDbType.Int
                ).Value = model.AssignedEmployeeId;


                command.Parameters.Add(
                    "@AssignedBy",
                    SqlDbType.Int
                ).Value = model.AssignedBy;


                command.Parameters.Add(
                    "@Deadline",
                    SqlDbType.DateTime
                ).Value = model.Deadline;


                command.Parameters.Add(
                    "@Priority",
                    SqlDbType.NVarChar,
                    20
                ).Value = model.Priority;


                // ==============================
                // OPEN CONNECTION
                // ==============================

                await connection.OpenAsync();


                // ==============================
                // EXECUTE PROCEDURE
                // ==============================

                using SqlDataReader reader =
                    await command.ExecuteReaderAsync();


                // ==============================
                // READ PROCEDURE RESPONSE
                // ==============================

                if (await reader.ReadAsync())
                {
                    int status =
                        Convert.ToInt32(reader["Status"]);

                    string message =
                        Convert.ToString(reader["Message"]) ?? "";


                    // ==========================
                    // PROCEDURE FAILED
                    // ==========================

                    if (status == 0)
                    {
                        return new ApiResponse(
                            "400",
                            false,
                            null,
                            message
                        );
                    }


                    // ==========================
                    // GET TASK ID
                    // ==========================

                    int taskId =
                        Convert.ToInt32(reader["TaskId"]);


                    // ==========================
                    // SUCCESS RESPONSE
                    // ==========================

                    return new ApiResponse(
                        "200",
                        true,
                        taskId,
                        message
                    );
                }


                // ==============================
                // NO RESPONSE FROM PROCEDURE
                // ==============================

                return new ApiResponse(
                    "500",
                    false,
                    null,
                    "Unable to assign task."
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse(
                    "500",
                    false,
                    null,
                    "An error occurred while assigning task. "
                    + ex.Message
                );
            }
        }


        [HttpGet("GetAllAssignedTasks")]
        public async Task<ApiResponse> GetAllAssignedTasks()
        {
            try
            {
                var taskList = await _empTaskMsDbContext.TaskManagements

                    .Join(
                        _empTaskMsDbContext.EmployeeMasters,
                        task => task.AssignedEmployeeId,
                        emp => emp.EmployeeId,
                        (task, emp) => new
                        {
                            task.TaskId,

                            task.TaskTitle,

                            task.TaskDescription,

                            EmployeeName = emp.EmployeeName,

                            emp.Role,

                            task.Priority,

                            task.AssignedDate,

                            task.Deadline,

                            task.EmployeeStatus,

                            task.AdminStatus
                        }
                    )

                    .OrderByDescending(x => x.AssignedDate)

                    .ToListAsync();


                return new ApiResponse(
                    "200",
                    true,
                    taskList,
                    taskList.Count > 0
                        ? "Task list found."
                        : "No task found."
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse(
                    "500",
                    false,
                    null,
                    "An error occurred while fetching task list. " + ex.Message
                );
            }
        }


        [HttpGet("GetTaskByTaskId/{taskId}")]
        public async Task<ApiResponse> GetTaskByTaskId(int taskId)
        {
            try
            {
                if (taskId <= 0)
                {
                    return new ApiResponse(
                        "400",
                        false,
                        null,
                        "Invalid Task Id."
                    );
                }

                var task = await _empTaskMsDbContext.TaskManagements

                    .Where(x => x.TaskId == taskId)

                    .Select(x => new
                    {
                        x.TaskId,

                        x.TaskTitle,

                        x.TaskDescription,

                        x.AssignedEmployeeId,

                        x.AssignedBy,

                        x.AssignedDate,

                        x.Deadline,

                        x.Priority,

                        x.EmployeeStatus,

                        x.EmployeeRemarks,

                        x.AdminStatus,

                        x.AdminRemarks,

                        x.CompletionDate,

                        x.DelayDays
                    })

                    .FirstOrDefaultAsync();


                if (task == null)
                {
                    return new ApiResponse(
                        "404",
                        false,
                        null,
                        "Task not found."
                    );
                }


                return new ApiResponse(
                    "200",
                    true,
                    task,
                    "Task found successfully."
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse(
                    "500",
                    false,
                    null,
                    "An error occurred while fetching task. "
                    + ex.Message
                );
            }
        }


        [HttpPost("EditTask")]
    public async Task<ApiResponse> EditTask(
    [FromForm] EditTaskDTO model)
    {
        try
        {
            // ==========================
            // VALIDATION
            // ==========================

            if (model == null)
            {
                return new ApiResponse(
                    "400",
                    false,
                    null,
                    "Invalid request."
                );
            }

            if (model.TaskId <= 0)
            {
                return new ApiResponse(
                    "400",
                    false,
                    null,
                    "Invalid Task Id."
                );
            }

            if (string.IsNullOrWhiteSpace(model.TaskTitle))
            {
                return new ApiResponse(
                    "400",
                    false,
                    null,
                    "Task title is required."
                );
            }

            if (model.AssignedEmployeeId <= 0)
            {
                return new ApiResponse(
                    "400",
                    false,
                    null,
                    "Please select employee."
                );
            }

            if (string.IsNullOrWhiteSpace(model.Priority))
            {
                return new ApiResponse(
                    "400",
                    false,
                    null,
                    "Priority is required."
                );
            }

            if (model.Deadline.Date < DateTime.Today)
            {
                return new ApiResponse(
                    "400",
                    false,
                    null,
                    "Deadline cannot be in the past."
                );
            }


            // ==========================
            // CONNECTION STRING
            // ==========================

            string connectionString =
                _configuration.GetConnectionString("defaultString");


            // ==========================
            // SQL CONNECTION
            // ==========================

            using SqlConnection connection =
                new SqlConnection(connectionString);

            await connection.OpenAsync();


            // ==========================
            // SQL COMMAND
            // ==========================

            using SqlCommand command =
                new SqlCommand(
                    "Usp_EditEmployeeTask",
                    connection
                );

            command.CommandType =
                CommandType.StoredProcedure;


            // ==========================
            // PARAMETERS
            // ==========================

            command.Parameters.Add(
                "@TaskId",
                SqlDbType.Int
            ).Value =
                model.TaskId;


            command.Parameters.Add(
                "@TaskTitle",
                SqlDbType.NVarChar,
                200
            ).Value =
                model.TaskTitle;


            command.Parameters.Add(
                "@TaskDescription",
                SqlDbType.NVarChar
            ).Value =
                (object?)model.TaskDescription
                ?? DBNull.Value;


            command.Parameters.Add(
                "@AssignedEmployeeId",
                SqlDbType.Int
            ).Value =
                model.AssignedEmployeeId;


            command.Parameters.Add(
                "@Deadline",
                SqlDbType.DateTime
            ).Value =
                model.Deadline;


            command.Parameters.Add(
                "@Priority",
                SqlDbType.NVarChar,
                20
            ).Value =
                model.Priority;


            command.Parameters.Add(
                "@EmployeeStatus",
                SqlDbType.NVarChar,
                50
            ).Value =
                model.EmployeeStatus;


            command.Parameters.Add(
                "@EmployeeRemarks",
                SqlDbType.NVarChar
            ).Value =
                (object?)model.EmployeeRemarks
                ?? DBNull.Value;


            command.Parameters.Add(
                "@AdminStatus",
                SqlDbType.NVarChar,
                50
            ).Value =
                model.AdminStatus;


            command.Parameters.Add(
                "@AdminRemarks",
                SqlDbType.NVarChar
            ).Value =
                (object?)model.AdminRemarks
                ?? DBNull.Value;


            // ==========================
            // EXECUTE SP
            // ==========================

            using SqlDataReader reader =
                await command.ExecuteReaderAsync();


            // ==========================
            // RESPONSE
            // ==========================

            if (await reader.ReadAsync())
            {
                int status =
                    Convert.ToInt32(
                        reader["Status"]
                    );

                int taskId =
                    Convert.ToInt32(
                        reader["TaskId"]
                    );

                string message =
                    Convert.ToString(
                        reader["Message"]
                    ) ?? "";


                if (status == 0)
                {
                    return new ApiResponse(
                        "400",
                        false,
                        null,
                        message
                    );
                }


                return new ApiResponse(
                    "200",
                    true,
                    taskId,
                    message
                );
            }


            return new ApiResponse(
                "500",
                false,
                null,
                "Unable to update task."
            );
        }
        catch (Exception ex)
        {
            return new ApiResponse(
                "500",
                false,
                null,
                ex.Message
            );
        }
    }

        [HttpPost("DeleteTask")]
        public async Task<ApiResponse> DeleteTask(
    [FromForm] int taskId)
        {
            try
            {
                // ==========================
                // VALIDATION
                // ==========================

                if (taskId <= 0)
                {
                    return new ApiResponse(
                        "400",
                        false,
                        null,
                        "Invalid Task Id."
                    );
                }


                // ==========================
                // CONNECTION STRING
                // ==========================

                string connectionString =
                    _configuration.GetConnectionString(
                        "defaultString"
                    );


                // ==========================
                // SQL CONNECTION
                // ==========================

                using SqlConnection connection =
                    new SqlConnection(connectionString);

                await connection.OpenAsync();


                // ==========================
                // SQL COMMAND
                // ==========================

                using SqlCommand command =
                    new SqlCommand(
                        "Usp_DeleteEmployeeTask",
                        connection
                    );

                command.CommandType =
                    CommandType.StoredProcedure;


                // ==========================
                // PARAMETER
                // ==========================

                command.Parameters.Add(
                    "@TaskId",
                    SqlDbType.Int
                ).Value = taskId;


                // ==========================
                // EXECUTE SP
                // ==========================

                using SqlDataReader reader =
                    await command.ExecuteReaderAsync();


                // ==========================
                // RESPONSE
                // ==========================

                if (await reader.ReadAsync())
                {
                    int status =
                        Convert.ToInt32(
                            reader["Status"]
                        );

                    string message =
                        Convert.ToString(
                            reader["Message"]
                        ) ?? "";


                    // ==========================
                    // FAILED
                    // ==========================

                    if (status == 0)
                    {
                        return new ApiResponse(
                            "400",
                            false,
                            null,
                            message
                        );
                    }


                    // ==========================
                    // SUCCESS
                    // ==========================

                    return new ApiResponse(
                        "200",
                        true,
                        taskId,
                        message
                    );
                }


                return new ApiResponse(
                    "500",
                    false,
                    null,
                    "Unable to delete task."
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse(
                    "500",
                    false,
                    null,
                    ex.Message
                );
            }
        }


        [HttpGet("GetAllAdminApprovedTasks")]
        public async Task<ApiResponse> GetAllAdminApprovedTasks()
        {
            try
            {
                var taskList = await _empTaskMsDbContext.TaskManagements
                    .Where(x => x.AdminStatus == "Approved")
                    .OrderByDescending(x => x.UpdatedDate)
                    .ThenBy(x => x.Deadline)
                    .Select(x => new
                    {
                        x.TaskId,
                        x.TaskTitle,
                        x.TaskDescription,

                        x.AssignedEmployeeId,
                        x.AssignedBy,

                        x.AssignedDate,
                        x.Deadline,
                        x.CompletionDate,

                        x.Priority,

                        x.EmployeeStatus,
                        x.EmployeeRemarks,

                        x.AdminStatus,
                        x.AdminRemarks,

                        x.DelayDays,

                        x.CreatedDate,
                        x.UpdatedDate
                    })
                    .ToListAsync();

                return new ApiResponse(
                    "200",
                    true,
                    taskList,
                    taskList.Count > 0
                        ? "All approved tasks found."
                        : "No approved tasks found."
                );
            }
            catch (Exception ex)
            {
                return new ApiResponse(
                    "500",
                    false,
                    null,
                    "Error while fetching approved tasks. " + ex.Message
                );
            }
        }
    }
}
