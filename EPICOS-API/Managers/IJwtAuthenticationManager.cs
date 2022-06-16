using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Managers
{
    public interface IJwtAuthenticationManager
    {
        string Authenticate(int userID);
    }
}
