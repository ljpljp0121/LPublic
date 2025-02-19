using System.IO;
using OfficeOpenXml;

public static class ExcelManager
{
    public static ExcelPackage LoadExcel(string path)
    {
        FileInfo fileInfo = new FileInfo(path);
        return new ExcelPackage(fileInfo);
    }
}