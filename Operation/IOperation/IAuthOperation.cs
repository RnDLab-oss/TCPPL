using DTOModel.LoginDTO;
using DTOModel.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Operation.IOperation
{
    public interface IAuthOperation
    {
        Task<ApiResponse> Login(Login_Req_Dto dto);
    }
}
