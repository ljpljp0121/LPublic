using cfg;
using SimpleJSON;
using UnityEngine;

public static class TableSystem
{
    public static Tables Table;

    public static void Init()
    {
        // Table = new Tables(LoadTable);
    }

    private static JSONNode LoadTable(string tableName)
    {
        var _textAsset = AssetSystem.LoadAsset<TextAsset>(string.Format("TableData/{0}.json", tableName));
        return JSON.Parse(_textAsset.text);
    }
}