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
        private const string manager = @"UristManager";
        private const string administrator = @"Администратор";
        private const string user = @"Пользователь";
        private const string opfr = @"ОПФР";
        private const string upfr = @"УПФР";
        //private const string admin = @"Urist";
        public const string User = domain + users;
        public const string Admin = domain + admin;
        public const string Manager = domain + manager;

        public const string UserOPFR = user + " " + opfr;
        public const string UserUPFR = user + " " + upfr;
        public const string AdminOPFR = administrator + " " + opfr;
        public const string AdminUPFR = administrator + " " + upfr;
        public const string Administrator = administrator;

    }
}
