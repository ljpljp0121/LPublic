using UnityEngine;

public class InitEngineOnLoad 
{
    public static bool loaded = false;
    public static void Init()
    {
        if (!loaded)
        {
            InitOnLoadMethodManager.ProcessInitOnLoadMethods(typeof(InitEngineOnLoad));

            loaded = true;
        }
    }
}