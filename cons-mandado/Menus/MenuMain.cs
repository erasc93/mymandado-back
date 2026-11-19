using VoiceControl;
using VsConsole;

public class MenuMain : AMenuMain,IMenuMain
{
    public MenuMain(MenuHome _menuHome)
    {
        this._subMenus.Add(_menuHome);
    }
}
