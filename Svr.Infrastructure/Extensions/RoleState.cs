using System.ComponentModel;

namespace Svr.Infrastructure.Extensions
{
    /// <summary>
    /// Роли пользователей
    /// </summary>
    public enum RoleState : byte
    {
        [Description(RoleBase.Administrator)]
        Administrator,
        [Description(RoleBase.AdminOPFR)]
        AdministratorOPFR,
        [Description(RoleBase.AdminUPFR)]
        AdministratorUPFR,
        [Description(RoleBase.UserOPFR)]
        UserOPFR,
        [Description(RoleBase.UserUPFR)]
        UserUPFR
        //,
        //[Description("Пользователь")]
        //User
    }

}
