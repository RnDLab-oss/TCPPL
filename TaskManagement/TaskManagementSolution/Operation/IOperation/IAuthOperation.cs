using DTOModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Operation
{
    public interface IAuthOperation
    {
        Task<ApiResponse> Login(LoginRequestDto dto);
    }
}
