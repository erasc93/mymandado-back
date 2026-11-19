using core_mandado.Users;
using Services.Dapper.Interfaces;
using VsConsole;

public class TrashMenu(
    IQueries _query,
    IRepo_Users _repoUser
    ): AMenu
{
    //public TrashMenu(IQueries query) { }
    protected override void SetupCommands()
    {
        

var x = _query.free.Query<dynamic>("show databases;");


//var repoUser = host.CreateInstance<IRepo_Users>();

//Console.WriteLine("username:");
//string
//    username = Console.ReadLine()!;
//Console.WriteLine("pssword :");
//string
//    password = Console.ReadLine()!;

//LoginInfo
//    loginInfo = new() { username = username, password = password };

//Console.WriteLine(repoUser.Login(loginInfo) ? "ok" : "ko");


    }
}