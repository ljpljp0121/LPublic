using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using OfficeOpenXml;
using UnityEditor;
using UnityEngine;

public class SkillClipExporter : EditorWindow
{
    private static List<SkillClip> clipList = new List<SkillClip>();
    private const string SKILL_CLIP_PATH = "Assets/GameRes/SkillConfig";
    private const string XML_FILE_PATH = "Datas/Defines/skill.xml"; //Assets下之后的路径
    private static XmlDocument xmlDoc;

    [MenuItem("Project/技能系统/导出SkillClip")]
    public static void ExportToExcel()
    {
        #region 加载所有Clip

        clipList.Clear();
        clipList = AssetDatabase
            .FindAssets("t:SkillClip", new string[] { SKILL_CLIP_PATH })
            .Select(guid => AssetDatabase.LoadAssetAtPath<SkillClip>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(clip => clip != null)
            .ToList();

        if (clipList == null || clipList.Count == 0)
        {
            Debug.Log("未找到任何SkillClip资产");
            return;
        }
        else
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("找到SkillClip资产,总数为:" + clipList.Count);
            foreach (var clip in clipList)
            {
                sb.AppendLine($"找到技能片段：{clip.name}");
            }
            Debug.Log(sb.ToString());
        }

        #endregion

        #region 获取skill.xml表配置

        string skillXmlPath = Path.Combine(Application.dataPath, "..", XML_FILE_PATH).Replace("\\", "/");

        if (File.Exists(skillXmlPath))
        {
            string xmlContent = File.ReadAllText(skillXmlPath);
            xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);
            Debug.Log($"XML文件路径：{skillXmlPath}");
        }
        else
        {
            Debug.LogError($"XML文件不存在：{skillXmlPath}");
        }

        #endregion

        using (var package = new ExcelPackage())
        {
            var workSheet = package.Workbook.Worksheets.Add("SkillClips");

            XmlNode root = xmlDoc.DocumentElement;
            var skillClipNode = root?.SelectSingleNode("//bean[@name='SkillClip']");
            XmlNodeList varNodes = skillClipNode?.SelectNodes("var");
            workSheet.Cells[1, 1].Value = "##var";
            workSheet.Cells[2, 1].Value = "##type";
            workSheet.Cells[3, 1].Value = "##group";
            workSheet.Cells[4, 1].Value = "##";
            for (var i = 0; i < varNodes?.Count; i++)
            {
                var varNode = varNodes[i];
                string name = varNode.Attributes?["name"].Value;
                string type = varNode.Attributes?["type"].Value;
                string alias = varNode.Attributes?["alias"].Value;
                workSheet.Cells[1, i + 2].Value = name;
                workSheet.Cells[2, i + 2].Value = type;
                workSheet.Cells[4, i + 2].Value = alias;
            }

            for (int i = 0; i < clipList.Count; i++)
            {
                SkillClip clip = clipList[i];
                workSheet.Cells[5 + i, 2].Value = clip.SkillID;
                workSheet.Cells[5 + i, 3].Value = clip.SkillName;
                workSheet.Cells[5 + i, 4].Value = clip.FrameCount;
            }

            string exportPath = EditorUtility.SaveFilePanel(
                "保存Excel文件",
                Path.Combine(Application.dataPath, "..", "Datas/Tables").Replace("\\", "/"),
                "SkillClips.xlsx",
                "xlsx");

            if (string.IsNullOrEmpty(exportPath))
            {
                Debug.Log("用户取消了保存操作");
                return;
            }
            else
            {
                File.WriteAllBytes(exportPath, package.GetAsByteArray());
                Debug.Log($"成功导出到: {exportPath}");
                AssetDatabase.Refresh();
            }
        }
    }
}