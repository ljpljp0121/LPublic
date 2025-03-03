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
    private const string SKILL_CLIP_PATH = "Assets/GameRes/SkillConfig";
    private const string XML_FILE_PATH = "Datas/Defines/skill.xml"; //Assets下之后的路径
    private static XmlDocument xmlDoc;
    private static List<SkillClip> clipList = new List<SkillClip>();

    [MenuItem("Project/技能系统/导出SkillClip")]
    public static void ExportToExcel()
    {
        if (!LoadSkillClips()) return;
        GetSkillXml();
        ExportExcel();
    }

    private static void ExportExcel()
    {
        using (var package = new ExcelPackage())
        {
            var workSheet = package.Workbook.Worksheets.Add("SkillClips");

            XmlNode root = xmlDoc.DocumentElement;
            var skillClipNode = root?.SelectSingleNode("//bean[@name='SkillClip']");
            InitExcelHead(skillClipNode, workSheet);
            InitExcelData(workSheet);
            SaveExcel(package);
        }
    }

    /// <summary>
    /// 加载所有SkillClip资产
    /// </summary>
    private static bool LoadSkillClips()
    {
        clipList.Clear();
        clipList = AssetDatabase
            .FindAssets("t:SkillClip", new string[] { SKILL_CLIP_PATH })
            .Select(guid => AssetDatabase.LoadAssetAtPath<SkillClip>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(clip => clip != null)
            .ToList();

        if (clipList == null || clipList.Count == 0)
        {
            Debug.Log("未找到任何SkillClip资产");
            return false;
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
        return true;
    }

    /// <summary>
    /// 获取skill.xml文件
    /// </summary>
    private static void GetSkillXml()
    {
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
    }

    /// <summary>
    /// 生成Excel表头
    /// </summary>
    private static void InitExcelHead(XmlNode skillClipNode, ExcelWorksheet workSheet)
    {
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
    }

    /// <summary>
    /// 生成Excel数据
    /// </summary>
    private static void InitExcelData(ExcelWorksheet workSheet)
    {
        for (int i = 0; i < clipList.Count; i++)
        {
            SkillClip clip = clipList[i];
            workSheet.Cells[5 + i, 2].Value = clip.SkillID;
            workSheet.Cells[5 + i, 3].Value = clip.SkillName;
            workSheet.Cells[5 + i, 4].Value = clip.FrameCount;
            workSheet.Cells[5 + i, 5].Value = clip.FrameRate;
            StringBuilder sb = new StringBuilder();
            //自定义事件
            foreach (var data in clip.SkillCustomEventData.FrameData)
            {
                sb.Append($"{data.Key};{data.Value.EventType}|");
            }
            workSheet.Cells[5 + i, 6].Value = sb.ToString();
            //动画事件
            sb.Clear();
            foreach (var data in clip.SkillAnimationData.FrameData)
            {
                sb.Append($"{data.Key};{AssetDatabase.GetAssetPath(data.Value.AnimationClip)};{data.Value.DurationFrame}|");
            }
            workSheet.Cells[5 + i, 7].Value = sb.ToString();
            //音频事件
            sb.Clear();
            foreach (var data in clip.SkillAudioData.FrameData)
            {
                sb.Append($"{data.FrameIndex};{AssetDatabase.GetAssetPath(data.AudioClip)}|");
            }
            workSheet.Cells[5 + i, 8].Value = sb.ToString();
            //特效事件
            sb.Clear();
            foreach (var data in clip.SkillEffectData.FrameData)
            {
                sb.Append($"{data.FrameIndex};{AssetDatabase.GetAssetPath(data.EffectPrefab)};" +
                          $"{data.Position.x},{data.Position.y},{data.Position.z};" +
                          $"{data.Rotation.x},{data.Rotation.y},{data.Rotation.z};" +
                          $"{data.Scale.x},{data.Scale.y},{data.Scale.z};" +
                          $"{data.Duration};{data.AutoDestroy}|");
            }
            workSheet.Cells[5 + i, 9].Value = sb.ToString();
            //碰撞事件
            sb.Clear();
            foreach (var data in clip.SkillColliderData.FrameData)
            {
                sb.Append($"{data.FrameIndex};{data.DurationFrame};");
                if (data.SkillColliderData is WeaponCollider weaponCollider)
                {
                    sb.Append($"{nameof(WeaponCollider)}/{weaponCollider.weaponName}|");
                }
                else if (data.SkillColliderData is BoxSkillCollider boxCollider)
                {
                    sb.Append($"{nameof(BoxSkillCollider)}/" +
                              $"{boxCollider.Position.x},{boxCollider.Position.y},{boxCollider.Position.z}/" +
                              $"{boxCollider.Rotation.x},{boxCollider.Rotation.y},{boxCollider.Rotation.z}/" +
                              $"{boxCollider.Scale.x},{boxCollider.Scale.y},{boxCollider.Scale.z}|");
                }
                else if (data.SkillColliderData is SphereSkillCollider sphereCollider)
                {
                    sb.Append($"{nameof(SphereSkillCollider)}/" +
                              $"{sphereCollider.Position.x},{sphereCollider.Position.y},{sphereCollider.Position.z}/" +
                              $"{sphereCollider.Radius}|");
                }
                else if (data.SkillColliderData is FanSkillCollider fanCollider)
                {
                    sb.Append($"{nameof(FanSkillCollider)}/" +
                              $"{fanCollider.Position.x},{fanCollider.Position.y},{fanCollider.Position.z}/" +
                              $"{fanCollider.Rotation.x},{fanCollider.Rotation.y},{fanCollider.Rotation.z}/" +
                              $"{fanCollider.InsideRadius}/{fanCollider.OutsideRadius}/" +
                              $"{fanCollider.Height}/{fanCollider.Angle}");
                }
            }
            workSheet.Cells[5 + i, 10].Value = sb.ToString();
        }
    }

    /// <summary>
    /// 保存Excel文件
    /// </summary>
    private static void SaveExcel(ExcelPackage package)
    {
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