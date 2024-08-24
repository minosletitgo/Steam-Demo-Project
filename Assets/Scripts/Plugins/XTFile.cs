using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

public static class XTFile
{
    public static MemoryStream ReadCfgFile(string strFile)
    {
        MemoryStream ms = new MemoryStream();
        FileStream fs = new FileStream(strFile, FileMode.Open);
        if (fs==null)
        {
            Debug.LogWarning(strFile + " read failed");
            return null;
        }
        int nFileLen = (int)fs.Length;
        ms.SetLength(nFileLen);
        int nCount = fs.Read(ms.GetBuffer(), 0, nFileLen);
        fs.Close();
        if (nCount != nFileLen)
        {
            Debug.LogWarning(strFile + " read failed");
            return null;
        }
        //Log("Test Log 1");
        return ms;
    }

    public static void Log(string szLog)
    {
        //szLog += "\r\n";
        //WriteFileTxt(XTAppPath.FolderLog + "Log.txt", szLog);

        //获取当前系统时间
        DateTime now = DateTime.Now;
        string strPath = Path.Combine(AppPath.LogPath, now.Year + "-" + now.Month + "-" + now.Day);

        if (!Directory.Exists(strPath))
        {
            XTFile.CreateDirectory(strPath);
        }

        StreamWriter sw = new StreamWriter(Path.Combine(strPath, "Log.txt"), true, Encoding.Unicode);
        if (sw == null)
        {
            Debug.LogWarning("Log" + " Write failed");
            return;
        }
        //byte[] byteLog = System.Text.Encoding.Unicode.GetBytes(szLog + "\r\n");
        //fs.Write(byteLog, 0, byteLog.Length);
        //fs.Close();
        szLog += "--" + now + "\r\n";
        sw.Write(szLog.ToCharArray(), 0, szLog.Length);
        sw.Close();

    }

    public static void LogFindMaterialExist(string strPath, string strTxtName, string strLog)
    {
        if (!Directory.Exists(strPath))
        {
            XTFile.CreateDirectory(strPath);
        }
        
        StreamWriter sw = new StreamWriter(Path.Combine(strPath, strTxtName), true, Encoding.Unicode);
        if (sw == null)
        {
            Debug.LogWarning("Log" + " Write failed");
            return;
        }

        strLog += "\r\n";
        sw.Write(strLog.ToCharArray(), 0, strLog.Length);
        sw.Close();
    }

    public static void LogFindShaderReference(string strPath, string strTxtName, string strLog)
    {
        if (!Directory.Exists(strPath))
        {
            XTFile.CreateDirectory(strPath);
        }

        StreamWriter sw = new StreamWriter(Path.Combine(strPath, strTxtName), true, Encoding.Unicode);
        if (sw == null)
        {
            Debug.LogWarning("Log" + " Write failed");
            return;
        }

        strLog += "\r\n";
        sw.Write(strLog.ToCharArray(), 0, strLog.Length);
        sw.Close();
    }

    public static string GetMD5(string strPath)
    {
        string ret = null;
        StringBuilder sb = new StringBuilder();
        byte[] buff = ReadFileBin(strPath, false);
        //TCFC.TDEBUG("GetMD5 strPath:" + strPath + " ret:" + (buff != null ? "not null" : "null"));
        if (buff != null)
        {
            //foreach (byte b in System.Security.Cryptography.MD5.Create().ComputeHash(buff))
            Byte[] pByte = System.Security.Cryptography.MD5.Create().ComputeHash(buff);
            for (int iByte = 0; iByte < pByte.Length; iByte++)
            {
                byte b = pByte[iByte];
                sb.Append(b.ToString("X2"));
            }
            ret = sb.ToString().ToLower();
        }
        return ret;
    }

    public static void Log_GMTool(string szLog)
    {
        //szLog += "\r\n";
        //WriteFileTxt(XTAppPath.FolderLog + "Log.txt", szLog);

        //获取当前系统时间
        DateTime now = DateTime.Now;
        string strPath = Path.Combine(AppPath.GMToolLogPath, now.Year + "-" + now.Month + "-" + now.Day);

        if (!Directory.Exists(strPath))
        {
            XTFile.CreateDirectory(strPath);
        }

        StreamWriter sw = new StreamWriter(Path.Combine(strPath, "Log.txt"), true, Encoding.Unicode);
        if (sw == null)
        {
            Debug.LogWarning("Log" + " Write failed");
            return;
        }
        //byte[] byteLog = System.Text.Encoding.Unicode.GetBytes(szLog + "\r\n");
        //fs.Write(byteLog, 0, byteLog.Length);
        //fs.Close();
        szLog += "--" + now + "\r\n";
        sw.Write(szLog.ToCharArray(), 0, szLog.Length);
        sw.Close();

    }

    public static void WriteFileTxt(string strPath, string strText, bool bRelativeRoot = true, bool bAppend = true)
    {
        string strFileFullPath = (bRelativeRoot ? XTAppPath.Root : null) + strPath;
        string strDirectoryFullPath = strFileFullPath.Substring(0, strFileFullPath.LastIndexOf('/'));
        CreateDirectory(strDirectoryFullPath);

        StreamWriter sw = new StreamWriter(strFileFullPath, bAppend, Encoding.Unicode);
        TCK.CHECK(sw != null);
        if (!string.IsNullOrEmpty(strText)) sw.Write(strText.ToCharArray(), 0, strText.Length);
        //else { sw.Write("1"); TCFC.TDEBUG("write null"); }
        sw.Close();
    }

    public static string ReadFileText(string strPath, bool bRelativeRoot = true)
    {
        string strFileFullPath = (bRelativeRoot ? XTAppPath.Root : null) + strPath;
        if (!File.Exists(strFileFullPath)) { Debug.LogWarning("ReadFileText File Not Exist:" + strFileFullPath); return null; }
        StreamReader sr = new StreamReader(strFileFullPath);
        TCK.CHECK(sr != null);
        return sr.ReadToEnd();
    }

    public static void WriteFileBin(string strPath, byte[] buff, bool bRelativeRoot = true)
    {
        string strFileFullPath = (bRelativeRoot ? XTAppPath.Root : null) + strPath;
        string strDirectoryFullPath = strFileFullPath.Substring(0,strFileFullPath.LastIndexOf('/'));
        CreateDirectory(strDirectoryFullPath);

        FileStream fs = new FileStream(strFileFullPath, FileMode.Create);
        TCK.CHECK(fs != null);
        fs.Write(buff, 0, buff.Length);
        fs.Flush();
        fs.Close();
        
    }

    public static byte[] ReadFileBin(string strPath, bool bRelativeRoot = true)
    {
        string strFileFullPath = (bRelativeRoot ? XTAppPath.Root : null) + strPath;
        if (!File.Exists(strFileFullPath)) { /*Debug.LogWarning("ReadFileBin File Not Exist:" + strFileFullPath);*/ return null; }
        FileStream fs = new FileStream(strFileFullPath, FileMode.Open);
        TCK.CHECK(fs != null);
        byte[] ret = new byte[fs.Length];
        fs.Read(ret, 0, ret.Length);
        fs.Close();
        return ret;
    }

    public static void DeleteFile(string strPath)
    { File.Delete(strPath); }

    public static void CreateDirectory(DirectoryInfo info)
    {
        if (info.Exists) return;

        CreateDirectory(info.Parent);//递归尝试创建父文件夹
        info.Create();
        //TCFC.TDEBUG("CreateFolder:" + info.FullName);
    }

    public static void CreateDirectory(string strPath)
    { CreateDirectory(new DirectoryInfo(strPath)); }

    public static bool ExistDirectory(string strPath)
    { return Directory.Exists(strPath); }

    public static bool ExistFile(string strPath)
    { return File.Exists(strPath); }

    public static void DeleteDirectory(string strPath, bool bRecursive = false)
    {
        DirectoryInfo info = new DirectoryInfo(strPath);
        if (!info.Exists) return;
        info.Delete(bRecursive);
    }
}
