using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pr17Danilov.Services
{

    public static class AuthService
    {
        public static User CurrentUser { get; set; }

        public static bool IsAuthenticated => CurrentUser != null && !CurrentUser.IsFrozen;

        public static void Logout()
        {
            CurrentUser = null;
        }
    }
}
