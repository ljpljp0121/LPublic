public class InitGamePlayOnLoad
{
    public static bool loaded = false;
    
    public static void Init()
    {
        if (!loaded)
        {
            InitOnLoadMethodManager.ProcessInitOnLoadMethods(typeof(InitGamePlayOnLoad));

            loaded = true;
        }
    }
}