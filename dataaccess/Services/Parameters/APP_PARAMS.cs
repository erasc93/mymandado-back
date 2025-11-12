using System.ComponentModel;

namespace core_mandado.parameters;

internal sealed class APP_PARAMS
{
    private static APP_PARAMS? _instance;
    public static APP_PARAMS instance { get { if (_instance == null) { _instance = new APP_PARAMS(); } return _instance; } }
    private APP_PARAMS() { }


    public int UNDEFINED { get; } = -13;
}
