using core_mandado.Users;
using Services.Dapper.Interfaces;
using VsConsole;

namespace cons_mandado.Menus;

public class TrashMenu(
    IQueries _query,
    IRepo_Users _repoUser
    ) : AMenu
{
    protected override void SetupCommands()
    {
        IEnumerable<dynamic>
            query = _query.free.Query<dynamic>("show databases;");

        string username = AskUserValue("username :");
        string password = AskUserValue("password :");

        LoginInfo
            loginInfo = new() { username = username, password = password };

        AddFooterMessage(_repoUser.Login(loginInfo) ? "ok" : "ko");
    }
}