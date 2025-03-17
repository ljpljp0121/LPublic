#if UNITY_EDITOR
namespace GAS.Editor.General
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// 自定义目录信息类，用于管理 GAS 资源在编辑器中的目录结构
    /// （区别于 System.IO.DirectoryInfo，专为 Odin 菜单树结构定制）
    /// </summary>
    public class DirectoryInfo
    {
        /// <summary>
        /// 根目录路径（如 "Assets/GAS/MMC"）
        /// </summary>
        public string RootDirectory { get; }

        public DirectoryInfo(string rootMenuName, string rootDirectory, string directory, Type assetType, bool isRoot)
        {
            RootDirectory = rootDirectory;
            Directory = directory;
            SubDirectory = new List<string>();
            AssetType = assetType;
            Root = isRoot;

            var dirs = directory.Replace("\\", "/");
            MenuName = dirs.Replace(RootDirectory + '/', "");
            MenuName = $"{rootMenuName}/{MenuName}";

            GetAllSubDir(Directory, SubDirectory);
        }
        /// <summary>
        /// 标识是否为根目录节点
        /// </summary>
        public bool Root { get; }
        /// <summary>
        /// 在 Odin 菜单树中显示的路径名称
        /// </summary>
        public string MenuName { get; }
        /// <summary>
        /// 该目录处理的资源类型
        /// </summary>
        public Type AssetType { get; }
        /// <summary>
        /// 当前目录的物理路径
        /// </summary>
        public string Directory { get; }
        /// <summary>
        /// 所有子目录路径列表（包含嵌套子目录）
        /// </summary>
        public List<string> SubDirectory { get; }
        /// <summary>
        /// 递归获取目录下所有子目录
        /// </summary>
        /// <param name="path">起始目录路径</param>
        /// <param name="subDirs">用于存储结果的列表</param>
        private void GetAllSubDir(string path, List<string> subDirs)
        {
            var dirs = System.IO.Directory.GetDirectories(path);
            foreach (var dir in dirs)
            {
                subDirs.Add(dir);
                GetAllSubDir(dir, subDirs);
            }
        }
    }
}
#endif