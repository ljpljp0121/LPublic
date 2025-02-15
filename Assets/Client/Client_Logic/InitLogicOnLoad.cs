public class InitLogicOnLoad
{
    public static bool loaded = false;
    public static void Init()
    {
        if (!loaded)
        {
            InitOnLoadMethodManager.ProcessInitOnLoadMethods(typeof(InitLogicOnLoad));

            loaded = true;
        }
    }
}