using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pr17Danilov.Models
{
    public static class SessionService
    {
        public static User CurrentUser { get; set; }

        public static bool IsAuthenticated => CurrentUser != null;

        public static void Logout()
        {
            CurrentUser = null;
        }
    }
}
