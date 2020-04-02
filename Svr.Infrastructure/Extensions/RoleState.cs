using System.ComponentModel;

namespace Svr.Infrastructure.Extensions
{
    /// <summary>
    /// Роли пользователей
    /// </summary>
    public enum RoleState : byte
    {
        [Description(Role.Administrator)]
        Administrator,
        [Description(Role.AdminOPFR)]
        AdministratorOPFR,
        [Description(Role.AdminUPFR)]
        AdministratorUPFR,
        [Description(Role.UserOPFR)]
        UserOPFR,
        [Description(Role.UserUPFR)]
        UserUPFR
        //    ,
        //[Description("Пользователь")]
        //User
    }

}
