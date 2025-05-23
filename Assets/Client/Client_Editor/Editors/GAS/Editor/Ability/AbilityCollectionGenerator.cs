﻿#if UNITY_EDITOR
namespace GAS.Editor
{
    using Runtime;
    using UnityEditor;
    using System;
    using System.IO;
    using GAS;
    using Editor;
    using UnityEngine;

    public class AbilityCollectionGenerator
    {
        public static void Gen()
        {
            string pathWithoutAssets = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
            var filePath =
                $"{pathWithoutAssets}/{GASSettingAsset.CodeGenPath}/{GasDefine.GAS_ABILITY_LIB_CSHARP_SCRIPT_NAME}";
            GenerateAbilityCollection(filePath);
        }

        private static void GenerateAbilityCollection(string filePath)
        {
            using var writer = new IndentedWriter(new StreamWriter(filePath));
            writer.WriteLine("///////////////////////////////////");
            writer.WriteLine("//// This is a generated file. ////");
            writer.WriteLine("////     Do not modify it.     ////");
            writer.WriteLine("///////////////////////////////////");

            writer.WriteLine("");

            writer.WriteLine("using System;");
            writer.WriteLine("using System.Collections.Generic;");

            writer.WriteLine("");

            writer.WriteLine("namespace GAS.Runtime");
            writer.WriteLine("{");
            writer.Indent++;
            {
                writer.WriteLine("public static class GAbilityLib");
                writer.WriteLine("{");
                writer.Indent++;
                {
                    writer.WriteLine("public struct AbilityInfo");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        writer.WriteLine("public int ID;");
                        writer.WriteLine("public string Name;");
                        writer.WriteLine("public string AssetPath;");
                        writer.WriteLine("public Type AbilityClassType;");
                        //writer.WriteLine("public AbilityAsset Asset()");
                        // writer.WriteLine("{");
                        // writer.Indent++;
                        // {
                        //     string loadAbilityAssetCode = string.Format(loadMethodCodeString, "AssetPath");
                        //     writer.WriteLine($"return {loadAbilityAssetCode};");
                        // }
                        // writer.Indent--;
                        // writer.WriteLine("}");
                    }
                    writer.Indent--;
                    writer.WriteLine("}");

                    writer.WriteLine("");

                    var abilityAssets =
                        EditorUtil.FindAssetsByType<AbilityAsset>(GASSettingAsset.GameplayAbilityLibPath);
                    foreach (var ability in abilityAssets)
                    {
                        var path = AssetDatabase.GetAssetPath(ability);
#if true
                        writer.WriteLine(
                            $"public static AbilityInfo {ability.UniqueName} = " +
                            $"new AbilityInfo {{ " +
                            $"ID = {ability.ID}, " +
                            $"Name = \"{ability.UniqueName}\", " +
                            $"AssetPath = \"{path}\"," +
                            $"AbilityClassType = typeof({ability.InstanceAbilityClassFullName}) }};");
#else
                          writer.WriteLine($"public static AbilityInfo {ability.UniqueName} = new AbilityInfo");
                        writer.WriteLine("{");
                        writer.Indent++;
                        {
                            writer.WriteLine($"Name = \"{ability.UniqueName}\",");
                            writer.WriteLine($"AssetPath = \"{path}\",");
                            writer.WriteLine($"AbilityClassType = typeof({ability.InstanceAbilityClassFullName})");
                        }
                        writer.Indent--;
                        writer.WriteLine("};");
#endif
                        writer.WriteLine("");
                    }

                    writer.WriteLine("");

                    writer.WriteLine(
                        "public static Dictionary<string, AbilityInfo> AbilityMap = new Dictionary<string, AbilityInfo>");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        foreach (var ability in abilityAssets)
                        {
                            writer.WriteLine($"[\"{ability.UniqueName}\"] = {ability.UniqueName},");
                        }
                    }
                    writer.Indent--;
                    writer.WriteLine("};");

                    writer.WriteLine("");
                    writer.WriteLine(
                        "public static Dictionary<int, AbilityInfo> AbilityMapByID = new Dictionary<int, AbilityInfo>()");
                    writer.WriteLine("{");
                    writer.Indent++;
                    {
                        foreach (var ability in abilityAssets)
                        {
                            writer.WriteLine($"[{ability.ID}] = {ability.UniqueName},");
                        }
                    }
                    writer.Indent--;
                    writer.WriteLine("};");
                }
                writer.Indent--;
                writer.WriteLine("}");
            }
            writer.Indent--;
            writer.Write("}");

            Console.WriteLine($"Generated GTagLib at path: {filePath}");
        }
    }
}
#endif