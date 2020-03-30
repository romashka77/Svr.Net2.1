using System;
using System.Collections.Generic;
using System.Text;

namespace Svr.Infrastructure.Extensions
{
    public static class Role
    {
        //private const string slash = "\\";
        private const string domain = @"0079PFRRU\";
        private const string users = @"UristUsers";
        private const string admin = @"UristAdmin";
        //private const string admin = @"Urist";
        public const string User = domain + users;
        public const string Admin = domain + admin;
    }
}
