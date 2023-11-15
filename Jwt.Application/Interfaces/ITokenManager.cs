using Jwt.Application.Model;
using Jwt.WebUI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jwt.Application.Interfaces
{
    public interface ITokenManager
    {
        public string GenerateToken(User user);
        public RefreshToken GenerateRefreshToken();
    }
}
