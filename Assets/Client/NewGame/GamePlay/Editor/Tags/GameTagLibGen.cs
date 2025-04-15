#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using Game.RunTime;
using UnityEngine;

namespace Game.Editor
{
    public static class GameTagLibGen
    {
        public static void Gen()
        {
            var asset = GameTagsAsset.LoadOrCreate();
            string pathWithoutAssets = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            var filePath =
                $"{pathWithoutAssets}/{GASSettingAsset.CodeGenPath}/{GameDefine.GAME_TAG_LIB_CS_SCRIPT_NAME}";
            var tags = asset.Tags;
            GenerateGTagLib(tags, filePath);
        }

        private static void GenerateGTagLib(List<GameTag> tags, string filePath)
        {
            using var writer = new IndentedWriter(new StreamWriter(filePath));
            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("//// This is a generated file. ////");
            writer.WriteLine("////     Do not modify it.     ////");
            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("");

            writer.WriteLine("using System.Collections.Generic;");

            writer.WriteLine("");
            writer.WriteLine("namespace Game.RunTime");
            writer.WriteLine("{");
            writer.Indent++;
            {
                writer.WriteLine("public static class GTagLib");
                writer.WriteLine("{");
                writer.Indent++;
                {
                    foreach (var tag in tags)
                    {
                        var validName = MakeValidIdentifier(tag.Name);
                        writer.WriteLine(
                            $"public static GameTag {validName} {{ get; }} = new GameTag(\"{tag.Name}\");");
                    }
                    writer.WriteLine("");
                    writer.WriteLine("public static Dictionary<string, GameTag> TagMap = new Dictionary<string, GameTag>");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        foreach (var tag in tags)
                        {
                            var validName = MakeValidIdentifier(tag.Name);
                            writer.WriteLine($"[\"{tag.Name}\"] = {validName},");
                        }
                    }
                    writer.Indent--;
                    writer.WriteLine("};");
                }
                writer.Indent--;
                writer.WriteLine("}");
            }
            writer.Indent--;
            writer.WriteLine("}");

            Console.WriteLine($"Generated GTagLib at path: {filePath}");
        }

        private static string MakeValidIdentifier(string name)
        {
            name = name.Replace('.', '_');

            if (char.IsDigit(name[0])) name = "_" + name;

            return string.Join("",
                name.Split(
                    new[]
                    {
                        ' ', '-', '.', ':', ',', '!', '?', '#', '$', '%', '^', '&', '*', '(', ')', '[', ']', '{', '}',
                        '/', '\\', '|'
                    }, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
#endif