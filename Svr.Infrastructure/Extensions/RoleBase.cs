using System;
using System.Collections.Generic;
using System.Text;

namespace Svr.Infrastructure.Extensions
{
    public class RoleBase
    {
        //private const string slash = "\\";
        private const string urist = "Urist";
        private const string domain = @"0079PFRRU\";
        private const string users = urist + @"Users";
        private const string admin = urist + @"Admin";
        private const string manager = urist + @"Manager";
        private const string administrator = @"Администратор";
        private const string user = @"Пользователь";
        private const string opfr = @"ОПФР";
        private const string upfr = @"УПФР";

        public const string Urist = urist;
        public const string Users = domain + users;
        public const string Admin = domain + admin;
        public const string Manager = domain + manager;

        public const string UserOPFR = user + " " + opfr;
        public const string UserUPFR = user + " " + upfr;
        public const string AdminOPFR = administrator + " " + opfr;
        public const string AdminUPFR = administrator + " " + upfr;
        public const string Administrator = administrator;
        public virtual string Lord { get; }
    }
}
