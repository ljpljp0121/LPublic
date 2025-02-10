using System;
using System.IO;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using UnityEditor;
using UnityEngine;

public static class OssHandler
{
    static readonly string accessKeyId = OssConfig.AccessKeyId;
    static readonly string accessKeySecret = OssConfig.AccessKeySecret;
    static readonly string endpoint = OssConfig.Endpoint;
    static readonly string projectName = OssConfig.ProjectName;
    static string bucketName = OssConfig.BucketName;
    static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

    public static void PutObjectFromFolder(string packageName, string bundleVersion)
    {
        string path =
            $"{Environment.CurrentDirectory}\\Bundles\\{EditorUserBuildSettings.activeBuildTarget}\\{packageName}\\{bundleVersion}";
        var files = Directory.GetFiles(path);
        foreach (string file in files)
        {
            try
            {
                client.PutObject(bucketName,
                    $"{projectName}/{CoreEngineRoot.Version}/{packageName}/{Path.GetFileName(file)}", file);
                Console.WriteLine("Put object:{0} succeeded", file);
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
                    ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed with error info: {0}", ex.Message);
            }
        }
    }
}


internal class OssConfig
{
    public static string AccessKeyId = "LTAI5t68Js1vgZwDBwHeJ4WV";

    public static string AccessKeySecret = "kaLwz7HSxYYVVNR0VWoxdMqhT3pc9S";

    public static string Endpoint = "oss-cn-hangzhou.aliyuncs.com";

    public static string BucketName = "unity-2540297235";

    public static string ProjectName = "LPublic";
}