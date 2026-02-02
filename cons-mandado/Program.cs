using api_mandado.services;
using core_mandado.Users;
using Services.Dapper.Interfaces;


IAppHost 
    host = DI_Console.Instance;
IQueries 
    query = host.CreateInstance<IQueries>();
var x = query.free.Query<dynamic>("show databases;");


var repoUser = host.CreateInstance<IRepo_Users>();

Console.WriteLine("username:");
string
    username = Console.ReadLine()!;
Console.WriteLine("pssword :");
string
    password = Console.ReadLine()!;

LoginInfo
    loginInfo = new() { username = username, password = password };

Console.WriteLine(repoUser.Login(loginInfo) ? "ok" : "ko");


Console.WriteLine("End of dialog");
Console.ReadKey();
