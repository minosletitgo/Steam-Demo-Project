using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System;
//using ProtoBuf;

public class CFileMiddle {

    public static byte[] Code_ShiftByte(ref byte[] in1, int nInLength)
    {
        for (int i = 0; i < nInLength; i++)
        {
            in1[i] += 1;
            in1[i] ^= 0xff;
        }
        return in1;
    }
    public static byte[] Decode_ShiftByte(ref byte[] in1, int nStart, int nInLength)
    {
        for (int i = nStart; i < nStart + nInLength; i++)
        {
            in1[i] ^= 0xff;
            in1[i] -= 1;
        }
        return in1;
    }

    public static string getMD5Str(string str)
    {
        StringBuilder sb = new StringBuilder();
        //foreach (byte b in System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(str)))
        Byte[] pByte = System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(str));
        for (int iByte = 0; iByte < pByte.Length; iByte++)
        {
            byte b = pByte[iByte];
            sb.Append(b.ToString("X2"));
        }
        return sb.ToString().ToLower();
    }

    public static bool CHECK(bool bCheck, string strErro = null)
    {
        if (!bCheck)
        {
            throw new System.SystemException("[CHECK]" + strErro);
        }

        return bCheck;
    }


    static bool m_bAssertBundle = false;
    public static string LoadFromFile(string strFileName)
    {
        string line = null;
        if (m_bAssertBundle)
        {
            if (XTAppPath.SpecailConfig(strFileName))
            {
                line = CFileMiddle.LoadTextFromMerge(strFileName);
            }
            else
            {
                line = CFileMiddle.LoadTextFromFile(strFileName);
            }
        }
        else
        {
            //Debug.Log("LoadFromFile Resources.Load:" + strFileName);
            if (Path.HasExtension(strFileName)) 
            {
                strFileName = Path.Combine(Path.GetDirectoryName(strFileName), Path.GetFileNameWithoutExtension(strFileName)); //去掉后缀
                strFileName = strFileName.Replace('\\', '/');
            }

            TextAsset file = (TextAsset)Resources.Load(strFileName, typeof(TextAsset));
            //TCK.CHECK(file != null/*, strFileName*/);
            if (file == null) { Debug.LogWarning("Faild LoadFromFile:" + strFileName); }
            line = file.text;
            //Debug.Log("LoadFromFile Resources.Load:" + strFileName + "\n" + line);
            Resources.UnloadAsset(file);

        }


        return line;
    }

    public static byte[] LoadFromFileByte(string strFileName)
    {
        string line = LoadFromFile(strFileName);
        return System.Text.Encoding.UTF8.GetBytes(line);
//         TextAsset file = null;
//         if (!IsAssertBundle) file = (TextAsset)Resources.Load(strFileName, typeof(TextAsset));
//         string line = null;
//         if (file == null)
//         {
//             line = CFileMiddle.GetFileText(strFileName);
//             if (string.IsNullOrEmpty(line))
//             {
//                 //尝试 bundle 读取
//                 line = CFileMiddle.ReadFileText(strFileName);
//                 if (line == null)
//                 {
//                     Debug.LogError("CTBLLoader LoadFromFile Error strFileName = " + strFileName);
//                 }
//             }
//         }
//         else
//         {
//             line = file.text;
//             Resources.UnloadAsset(file);
//         }
// 
//         return System.Text.Encoding.UTF8.GetBytes(line);
    }

    public static byte[] ReadFileTextByte(string strPath, bool bRelativeRoot = true)
    {
        string s = LoadTextFromFile(strPath, bRelativeRoot ? EM_ReadFlag._Default : (EM_ReadFlag)TCFC.ClsFlag((int)EM_ReadFlag._Default, (int)EM_ReadFlag.RelativeRoot));
        if (string.IsNullOrEmpty(s))
        {
            return null;
        }
        return System.Text.Encoding.UTF8.GetBytes(s);
    }

    public static string LoadTextFromMerge(string strPath)
    {
        int nHashCode = strPath.ToLower().GetHashCode();
        if (!dicMerge2.ContainsKey(nHashCode))
        {
            return string.Empty;
        }

        return dicMerge2[nHashCode];
    }

    //public static string WritePNFile<T>(T t, string strPath, bool bMD5Name = true)
    //{
    //    strPath = strPath.ToLower();
    //    string conExPath = CResPath.GetLocalPath("");
    //    string PathName = Path.GetDirectoryName(strPath);

    //    string strFileFullPath = Path.Combine(conExPath, PathName);
    //    //Debug.Log("Combine strFileFullPath:" + strFileFullPath + " conExPath:" + conExPath + " PathName:" + PathName);
    //    Directory.CreateDirectory(strFileFullPath);

    //    string strFileName = Path.GetFileName(strPath).ToLower();
    //    if (bMD5Name) { strFileName = TCFC.getMD5Str(strFileName); }
    //    string EncrypFile = Path.Combine(strFileFullPath, strFileName + ".pn");
    //    //Debug.Log("WritePNFile EncrypFile:" + EncrypFile + " strFileFullPath:" + strFileFullPath + " conExPath:" + conExPath + " PathName:" + PathName);
    //    using (MemoryStream ms = new MemoryStream())
    //    {
    //        Serializer.Serialize<T>(ms, t);
    //        FileStream fsEncryp = File.Open(EncrypFile, FileMode.Create, FileAccess.Write, FileShare.Write);
    //        BinaryWriter bw = new BinaryWriter(fsEncryp, Encoding.UTF8);
    //        byte[] pEncrypt = ms.GetBuffer();
    //        Code_ShiftByte(ref pEncrypt, (int)ms.Length);
    //        bw.Write(pEncrypt, 0, (int)ms.Length);
    //        bw.Close();
    //        ms.Dispose();
    //        fsEncryp.Close();
    //    }
    //    return EncrypFile;
    //}

    //public static T ReadPNFile<T>(string strPath, bool bRelativeRoot = true, bool bMD5Name = true)
    //{
    //    strPath = strPath.ToLower();
    //    string conExPath = CResPath.GetLocalPath("");
    //    string PathName = Path.GetDirectoryName(strPath);

    //    //使用Root 路径
    //    string strFileFullPath = bRelativeRoot ? Path.Combine(conExPath, PathName) : PathName;
    //    string strFileName = Path.GetFileName(strPath).ToLower();
    //    if (bMD5Name) { strFileName = TCFC.getMD5Str(strFileName); }
    //    strFileFullPath = Path.Combine(strFileFullPath, strFileName + ".pn");

    //    //文件名 MD5
    //    if (!File.Exists(strFileFullPath))
    //    { Debug.LogWarning("ReadPNFile File Not Exist:" + strFileFullPath + " Real:" + strPath); return default(T); }

    //    FileStream fs = new FileStream(strFileFullPath, FileMode.Open, FileAccess.Read);
    //    if (fs == null) { return default(T); }
    //    byte[] ret = new byte[fs.Length];
    //    fs.Read(ret, 0, ret.Length);
    //    fs.Close();

    //    //内容解密开启
    //    Decode_ShiftByte(ref ret, 0, ret.Length);

    //    using (MemoryStream ms = new MemoryStream(ret))
    //    {
    //        T t = Serializer.Deserialize<T>(ms);
    //        return t;
    //    }

    //    //return default(T);
    //}

    public enum EM_ReadFlag
    {
        NULL = 0,
        MD5Name         = 1 << 0,
        RelativeRoot    = 1 << 1,
        Decode          = 1 << 2,

        _Default = EM_ReadFlag.MD5Name | EM_ReadFlag.RelativeRoot | EM_ReadFlag.Decode,
    }

    const int nMd5L_UTF8 = 32;
    const int nMd5L_Unicode = 64;
    const string conFileKey = "Test";

    public static int GetMD5Lenth(Encoding enc)
    {
        int ret = nMd5L_UTF8;
        if (enc == Encoding.Unicode)
        {
            ret = nMd5L_Unicode;
        }

        return ret;
    }

    public static string CalcFileFullPath(string strPath, bool bRelativeRoot, bool bMD5Name)
    {
        strPath = strPath.ToLower();

        string conExPath = CResPath.GetLocalPath("");//Path.Combine(XTAppPath.ConfigPath, "System");

        string PathName = Path.GetDirectoryName(strPath);
        //使用Root 路径
        string strFileFullPath = bRelativeRoot ? Path.Combine(conExPath, PathName) : PathName;
        if (bMD5Name)
        { strFileFullPath = Path.Combine(strFileFullPath, getMD5Str(Path.GetFileNameWithoutExtension(strPath)) + ".proto"); }
        else { strFileFullPath = Path.Combine(strFileFullPath, Path.GetFileName(strPath)); }

        //文件名 MD5
        if (!File.Exists(strFileFullPath)) 
        { Debug.LogWarning("ReadFileText File Not Exist:" + strFileFullPath + " Real:" + strPath); return null; }
        return strFileFullPath;
    }

    //读取文件
    public static string LoadTextFromFile(string strPath, EM_ReadFlag emFlag = EM_ReadFlag._Default)
    {
        string strFileFullPath = CalcFileFullPath(strPath, TCFC.HasFlag((int)emFlag, (int)EM_ReadFlag.RelativeRoot), TCFC.HasFlag((int)emFlag, (int)EM_ReadFlag.MD5Name));
        bool bIsTBL = IsTBL(strFileFullPath);

        FileStream fs = new FileStream(strFileFullPath, FileMode.Open, FileAccess.Read);
        if (fs == null) { return null; }
        byte[] ret = new byte[fs.Length];
        fs.Read(ret, 0, ret.Length);
        fs.Close();

        //内容解密开启
        bool bDecode = TCFC.HasFlag((int)emFlag, (int)EM_ReadFlag.Decode);
        if (bDecode) { Decode_ShiftByte(ref ret, 0, ret.Length); }

        MemoryStream ms = new MemoryStream(ret);
        BinaryReader br = new BinaryReader(ms);
        int nLen = bDecode ? br.ReadInt32() : ret.Length;
        if (ret.Length != nLen) { Debug.LogWarning("ret.Length != nLen ret.Length=" + ret.Length + " nLen=" + nLen + " " + strPath); }
        System.Text.Encoding enc = bDecode ? System.Text.Encoding.UTF8 : System.Text.Encoding.Unicode;
        enc = bIsTBL ? Encoding.Unicode : enc;
        int offset = bDecode ? GetMD5Lenth(enc) + 4 : 0;
        //byte[] pMD5 = br.ReadBytes(conMd5L);
        //string sMD5 = System.Text.Encoding.UTF8.GetString(pMD5);
        //byte[] pContext = br.ReadBytes(nLen - conMd5L);

        
        string s = enc.GetString(ret, offset, nLen - offset);
        br.Close();

        ////MD5 CHECK
        //string md5 = getMD5Str(s + conFileKey);
        //CHECK(md5 == sMD5);

        //Debug.Log("LoadTextFromFile:[" + s + "]");

        return s;
    }

    static Dictionary<string, string> dicMerge = null;
    static Dictionary<int, string> dicMerge2 = null;

    public static void ResetMerge()
    {
        if (dicMerge == null)
            dicMerge = new Dictionary<string, string>();
        dicMerge.Clear();
    }

    public static bool CollectFileText(string strPath)
    {
        strPath = strPath.Replace('\\', '/');
        if (!File.Exists(strPath)) return false;
        strPath = XTAppPath.Path2ResourcePath(strPath);
        strPath = strPath.Substring(0, strPath.Length - Path.GetExtension(strPath).Length);
        TextAsset file = (TextAsset)Resources.Load(strPath, typeof(TextAsset));
        TCK.CHECK(file != null);
        if (!dicMerge.ContainsKey(strPath))
            dicMerge.Add(strPath,file.text);
        return true;
    }

    //[int headlength] 4
    //( [byte path length] [int data begin] [int data end] 9 ) * count
    public static bool MergeFileText(string strPath)
    {
        TCFC.TDEBUG("MergeFileText:" + dicMerge.Count);
        if (dicMerge.Count <= 0) return false;

        FileStream fsEncryp = File.Open(strPath, FileMode.Create, FileAccess.Write, FileShare.Write);
        BinaryWriter bw = new BinaryWriter(fsEncryp, Encoding.UTF8);
        MemoryStream ms = new MemoryStream();

        int nHeadLen = dicMerge.Count * 9 + 4;
        byte[] pbHeader = System.BitConverter.GetBytes(nHeadLen);
        ms.Write(pbHeader, 0, pbHeader.Length);

        int nNextStartIdx = nHeadLen;

        //foreach (KeyValuePair<string, string> kvp in dicMerge)
        Dictionary<string, string>.Enumerator itKVP = dicMerge.GetEnumerator();
        while(itKVP.MoveNext())
        {
            KeyValuePair<string, string> kvp = itKVP.Current;
            string szBody = kvp.Key + kvp.Value;
            byte[] pbBody = System.Text.Encoding.UTF8.GetBytes(szBody);
            int nStartIdx = nNextStartIdx;
            byte[] pbStartIdx = System.BitConverter.GetBytes(nStartIdx);
            nNextStartIdx = nStartIdx + pbBody.Length;
            int nEndIdx = nNextStartIdx - 1;
            byte[] pbEndIdx = System.BitConverter.GetBytes(nEndIdx);
            ms.WriteByte((byte)kvp.Key.Length);
            ms.Write(pbStartIdx, 0, pbStartIdx.Length);
            ms.Write(pbEndIdx, 0, pbEndIdx.Length);
        }


        //foreach (KeyValuePair<string, string> kvp in dicMerge)
        itKVP = dicMerge.GetEnumerator();
        while (itKVP.MoveNext())
        {
            KeyValuePair<string, string> kvp = itKVP.Current;
            string szBody = kvp.Key + kvp.Value;
            byte[] pbBody = System.Text.Encoding.UTF8.GetBytes(szBody);
            ms.Write(pbBody, 0, pbBody.Length);
        }

        byte[] pEncrypt = ms.GetBuffer();
        Code_ShiftByte(ref pEncrypt, (int)ms.Length);
        bw.Write(pEncrypt, 0, (int)ms.Length);
        //byte[] pData = ms.GetBuffer();
        //bw.Write(pData, 0, (int)ms.Length);
        bw.Close();
        fsEncryp.Close();

        return true;
    }
    public class DDTTick
    {
        public System.Int64 m_dt;
        public DDTTick() { m_dt = System.DateTime.Now.ToBinary(); }
        public float time { get { return (float)((double)(System.DateTime.Now.ToBinary() - m_dt) / 10000000); } }
    }


    public static bool BuildDictionaries(bool bAssetBundle)
    {
        m_bAssertBundle = bAssetBundle;
        if (!File.Exists(XTAppPath.SpecailConfigPath))
        {
            if (m_bAssertBundle)
            {
                Debug.LogError("既然是assetbundle版本，就请先打包config文件");
            }
            return false;
        }

        //避免多次初始化
        if (dicMerge2 != null) { return true; }

        DDTTick ti = new DDTTick();
        FileStream fs = new FileStream(XTAppPath.SpecailConfigPath, FileMode.Open, FileAccess.Read);
        if (fs == null) { return false; }
        byte[] ret = new byte[fs.Length];
        fs.Read(ret, 0, ret.Length);
        fs.Close();

        //内容解密开启
        Decode_ShiftByte(ref ret, 0, ret.Length);

        MemoryStream ms = new MemoryStream(ret);
        BinaryReader br = new BinaryReader(ms);

        byte[] byHeadSize = br.ReadBytes(4);
        int nSize = System.BitConverter.ToInt32(byHeadSize, 0);
        int nHeadCout = nSize / 9;
        for (int i = 0; i < nHeadCout; ++i)
        {
            int filePathLen = br.ReadByte();
            byte[] byFileBegin = br.ReadBytes(4);
            byte[] byFileEnd = br.ReadBytes(4);
            int nBeginIdx = System.BitConverter.ToInt32(byFileBegin, 0);
            int nEndIdx = System.BitConverter.ToInt32(byFileEnd, 0);

            //byte[] byFilePath = new byte[filePathLen];
            //Array.Copy(ret, nBeginIdx, byFilePath, 0, filePathLen);
            string strFilePath = System.Text.Encoding.UTF8.GetString(ret, nBeginIdx, filePathLen);
            //TCFC.TDEBUG(strFilePath);

            //byte[] byFileData = new byte[nEndIdx - nBeginIdx - filePathLen + 1];
            //Array.Copy(ret, nBeginIdx + filePathLen, byFileData, 0, nEndIdx - nBeginIdx - filePathLen + 1);
            string strFileData = System.Text.Encoding.UTF8.GetString(ret, nBeginIdx + filePathLen, nEndIdx - nBeginIdx - filePathLen + 1);
            //TCFC.TDEBUG(strFileData);

            try
            {
                if (dicMerge2 == null)
                    dicMerge2 = new Dictionary<int, string>();
                //dicMerge.Add(strFilePath, strFileData);
                int nHashCode = strFilePath.ToLower().GetHashCode();
                dicMerge2.Add(nHashCode, strFileData);
                //TCFC.TDEBUG(strFilePath);
                //if (strFilePath == "config/msgDef/MsynGarden")
                //{
                //    TCFC.TDEBUG("**************************************");
                //    //TCFC.TDEBUG(strFilePath);
                //    TCFC.TDEBUG(strFilePath + " Code:" + nHashCode);
                //    TCFC.TDEBUG(strFileData);
                //}
            }
            catch(System.Exception ex)
            {
                TCFC.TDEBUG(ex.ToString());
            }
            
        }

        ret = null;
        br.Close();

        XTFile.Log("解析完成 耗时: " + ti.time.ToString("0.00") + "秒");
        return true;
    }

    public static bool IsBuildTBLSeriz(string strPath)
    {
#if PN
        if (
            strPath.Contains("config/tbl/Talk2D") ||
            strPath.Contains("config/tbl/Control") ||
            strPath.Contains("config/tbl/Action") ||
            strPath.Contains("config/tbl/Behavior")||
            strPath.Contains("config/tbl/String") ||
            strPath.Contains("config/tbl/GlobalPararm") ||
            strPath.Contains("config/tbl/PayRate") 
            //strPath.Contains("config/tbl/Action") ||
            //strPath.Contains("config/tbl/Action") ||
        )
            return true;
#endif
        return false;
    }

    public static bool IsTBL(string strPath)
    {
        return strPath.Contains("config/tbl");
    }

    //转化文件
    public static bool TransFileText(string strPath)
    {
        if (IsBuildTBLSeriz(strPath)) { return true; }

        strPath = strPath.ToLower();

        if (!File.Exists(strPath)) return false;

        bool bTBL = IsTBL(strPath);

        string conPackPath = CResPath.GetLocalPath("");//Path.Combine(XTAppPath.ConfigPath,"System");

        string PathName = Path.GetDirectoryName(strPath);
        int nStartbase = PathName.IndexOf("assets/resources/");
        if (nStartbase < 0) { Debug.LogWarning("TransFileText File Not Resources File:" + strPath); return false; }
        PathName = PathName.Substring(nStartbase + "assets/resources/".Length);

        string strFileName = Path.GetFileNameWithoutExtension(strPath);

		string strFile = Path.Combine(PathName,strFileName );
		//Debug.LogWarning ("strFile1 = " + strFile);
        strFile = strFile.Replace("\\","/");
		//Debug.LogWarning ("strFile2 = " + strFile);
        //外部理应排除
        //if (strFile.Contains(".svn")) { /*TCFC.TDEBUG("[" + strFile + "]is .svn,skip.");*/ return false; }//svn目录。跳过
        //if (Directory.Exists(XTAppPath.ResourcePath2AbsolutePath(strFile))) { /*TCFC.TDEBUG("[" + strFile + "]is Directory,skip.");*/ return false; }//目录。跳过

        //Res 下资源打包
        TextAsset file = (TextAsset)Resources.Load(strFile, typeof(TextAsset));
        if (file == null) { Debug.LogWarning("TransFileText File Not Exist:" + strFile); return false; }//所有文件都来试一下，所以会有不必要的日志
		//Debug.Log ("strFile suc = " + strFile);
        string line = file.text;

        string md5 = getMD5Str(line + conFileKey);

        string EncrypPath = Path.Combine(conPackPath, PathName);
        Directory.CreateDirectory(EncrypPath);

        string EncrypFile = Path.Combine(EncrypPath, getMD5Str(Path.GetFileNameWithoutExtension(strPath)) + ".proto");
		//Debug.LogWarning ("EncrypFile = " + EncrypFile);
        FileStream fsEncryp = File.Open(EncrypFile, FileMode.Create, FileAccess.Write, FileShare.Write);

        Encoding enc = bTBL ? Encoding.Unicode : Encoding.UTF8;

        BinaryWriter bw = new BinaryWriter(fsEncryp, enc);

        byte[] pMD5 = enc.GetBytes(md5);
        byte[] pContext = enc.GetBytes(line);
        TCK.CHECK(pMD5.Length == GetMD5Lenth(enc));

        MemoryStream ms = new MemoryStream();
        int nLenth = pMD5.Length + pContext.Length + 4;
        byte[] pLength = System.BitConverter.GetBytes(nLenth);
        ms.Write(pLength, 0, pLength.Length);
        ms.Write(pMD5, 0, pMD5.Length);
        ms.Write(pContext, 0, pContext.Length);

        byte[] pEncrypt = ms.GetBuffer();
        Code_ShiftByte(ref pEncrypt, (int)ms.Length);
        bw.Write(pEncrypt, 0, (int)ms.Length);
        bw.Close();
        fsEncryp.Close();

        //TCFC.TDEBUG(EncrypFile + " " + ms.Length);

        //TCFC.TDEBUG(ms.Length + " " + nLenth);
        CHECK(ms.Length == nLenth, "ret.Length=" + ms.Length + " nLen=" + nLenth + " " + strPath);

        Resources.UnloadAsset(file);
        return true;
    }

    public static void SaveLocalVer(string strPath,string szVersion)
    {
        FileStream fsEncryp = File.Open(strPath, FileMode.Create, FileAccess.Write, FileShare.Write);
        BinaryWriter bw = new BinaryWriter(fsEncryp, Encoding.UTF8);
        MemoryStream ms = new MemoryStream();
        string szVerHead = "abcdef\n";
        byte[] pbHeader = System.Text.Encoding.UTF8.GetBytes(szVerHead);
        ms.Write(pbHeader, 0, pbHeader.Length);
        string szVerBody = string.Format("version:{0}", szVersion);
        byte[] pbVerBody = System.Text.Encoding.UTF8.GetBytes(szVerBody);
        ms.Write(pbVerBody, 0, pbVerBody.Length);

        byte[] pData = ms.GetBuffer();
        Code_ShiftByte(ref pData, pData.Length);
        bw.Write(pData, 0, (int)ms.Length);
       
        bw.Close();
        fsEncryp.Close();
    }

    public static string ParseLocalVer(string strPath)
    {
        string szVersion = "0.0.0";

        if (File.Exists(strPath))
        {
            FileStream fs = new FileStream(strPath, FileMode.Open, FileAccess.Read);
            if (fs == null) { return szVersion; }
            byte[] ret = new byte[fs.Length];
            fs.Read(ret, 0, ret.Length);
            fs.Close();

            //内容解密开启
            Decode_ShiftByte(ref ret, 0, ret.Length);
            string szFileData = System.Text.Encoding.UTF8.GetString(ret);
            fs.Close();

            string[] arrLine = szFileData.Split('\n');
            TCK.CHECK(arrLine.Length >= 2);

            string szLine = arrLine[1];
            szLine = szLine.TrimEnd('\r', '\n');
            string[] array = szLine.Split(':');
            if (array.Length == 1)
                szVersion = array[0];
            else
                szVersion = array[1];
        }

        return szVersion;
    }
}
