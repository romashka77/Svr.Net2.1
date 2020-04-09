using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Svr.AD.Extensions
{
    public static class ControllerExtensions
    {
        public static string GetLord(this Controller controller, string lord = null)
        {
            if (controller.User.IsInRole(Role.Manager) || controller.User.IsInRole(Role.Users))
                return "1";
            return lord;
        }
        public static string GetOwner(this Controller controller, string owner = null)
        {
            if (controller.User.IsInRole(Role.Users))
                for (int i = 1; i < 100; i++)
                {
                    string s = Role.Urist + i.ToString();
                    if (controller.User.IsInRole(s))
                    {
                        owner = $"{i}";
                        break;
                    }
                }
            return owner;
        }
    }
}
