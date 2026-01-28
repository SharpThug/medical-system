using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public interface IAuthService
    {
        public Task<ApiResponse<string>> LoginAsync(string login, string password);
    }
}
