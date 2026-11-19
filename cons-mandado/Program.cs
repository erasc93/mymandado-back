using cons_mandado;
using VsConsole;

IMenu 
    menuHome = DI_Console.Instance.CreateInstance<MenuHome>(); // host.CreateInstance<MenuMain>();
menuHome.Open();
