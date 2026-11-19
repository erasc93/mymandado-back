using cons_mandado;
using core_mandado.Users;
using Services.Dapper.Interfaces;
using VsConsole;



IAppHost
    host = DI_Console.Instance;
IMenu
    main = host.CreateInstance<MenuHome>();
    //main = host.CreateInstance<MenuMain>();
main.Open();
