public class InitUIOnLoad
{
    public static bool loaded = false;
    public static void Init()
    {
        if (!loaded)
        {
            InitOnLoadMethodManager.ProcessInitOnLoadMethods(typeof(InitUIOnLoad));

            loaded = true;
        }
    }
}