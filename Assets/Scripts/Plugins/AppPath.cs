using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

public class AppPath
{
    public static bool IsNet
    {
        get;
        set;
    }

    public static string IP
    {
        get;
        set;
    }

    public static string ReplaceString(string s)
    {
        if (s == null)
        {
            return null;
        }
        if (IsNet)
        {
            s = s.Replace(" ", "%20");
        }

#if UNITY_IPHONE
        s = s.Replace(" ", "%20");
#endif

        return s.Replace("#", "%23");
    }

    private static string m_DataApp = "";
    public static string DataApp
    {
        get
        {
            if (string.IsNullOrEmpty(m_DataApp)) m_DataApp = Application.dataPath + "/";
            return m_DataApp;
        }   
    }

    private static string m_StreamFiles = "";
    public static string StreamFiles
    {
        get
        {
            if (string.IsNullOrEmpty(m_StreamFiles)) m_StreamFiles = Application.streamingAssetsPath + "/";
            return m_StreamFiles;
        }
    }

    //供Read的路径
    //注意!与Write的路径是不同的.
    //比如考虑Anriod Jar环境效率低下,会将所有streamingAssetsPath下文件放置到独立文件夹
    public static string StreamFilesForRead
    {
        get
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            return "file://" + StreamFiles;
#elif UNITY_ANDROID 
            return "file://" + AssetbundleLocal;
#elif UNITY_IPHONE
            return StreamFiles;
#else
            return StreamFiles;
#endif
        }
    }

    private static string m_DataFiles = "";
    public static string DataFiles
    {
        get
        {
            if (string.IsNullOrEmpty(m_DataFiles))
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                m_DataFiles = Application.dataPath + "/../";
#elif UNITY_ANDROID || UNITY_IPHONE
                m_DataFiles =  Application.persistentDataPath + "/";
#else
                m_DataFiles = Application.dataPath + "/../";
#endif
            }
            return m_DataFiles;
        }
    }

    private static string m_DataCache = "";
    public static string DataCache
    {
        get
        {

            if (string.IsNullOrEmpty(m_DataCache))
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                m_DataCache = Application.dataPath + "/../";
#elif UNITY_ANDROID || UNITY_IPHONE
                m_DataCache = Application.temporaryCachePath + "/";
#else
                m_DataCache = Application.dataPath + "/../";
#endif
            }

            return m_DataCache;
        }
    }

    public static string GetAssetbundleDir(RuntimePlatform emPlatform)
    {
        switch(emPlatform)
        {
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.WindowsPlayer:
                return "ab_pc/";
            case RuntimePlatform.Android:
                return "ab_ad/";
            case RuntimePlatform.IPhonePlayer:
                return "ab_ios/";
            default:
                return "ab_pc/";
        }
    }

    public static string AssetbundleDir
    {
        get
        {
#if UNITY_ANDROID
            return GetAssetbundleDir(RuntimePlatform.Android);
#elif UNITY_IPHONE
            return GetAssetbundleDir(RuntimePlatform.IPhonePlayer);
#else
            return GetAssetbundleDir(RuntimePlatform.WindowsPlayer);
#endif
        }
    }

    public static string AssetbundleLocal
    {
        get
        {
            return DataFiles + AssetbundleDir;
        }
    }

    public static string AssetbundleStream
    {
        get
        {
            return StreamFiles + AssetbundleDir;
        }
    }

    public static string ConfigPath
    {
        get 
        {
            return DataFiles + "/Config/";
        }
    }

    public static string LogPath
    {
        get
        {
            return DataCache + "/Log/";
        }
    }

    public static string GMToolLogPath
    {
        get
        {
            return DataCache + "/Log_GMTool/";
        }
    }

    public static string ResPath
    {
        get
        {
            return DataCache;
        }
    }

    public static string ObbPath
    {
        get
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            //if (Application.platform == RuntimePlatform.WindowsWebPlayer
            //   || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                //Debug.LogWarning("AssetbundleDir : /ab_pc/");
                return "/ab_pc/";
            }
#elif UNITY_ANDROID
            //else if (Application.platform == RuntimePlatform.Android)
            {

                AndroidJavaClass Environment;
                Environment = new AndroidJavaClass("android.os.Environment");
                if (Environment.CallStatic<string>("getExternalStorageState") == "mounted")
                {
                    using (AndroidJavaObject externalStorageDir = Environment.CallStatic<AndroidJavaObject>("getExternalStorageDirectory"))
                    {
                        string root = externalStorageDir.Call<string>("getPath");
                        return String.Format("{0}/Android/obb/", root);
                    }
                }

                return "";
 
            }
#elif UNITY_IPHONE
            //else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                //Debug.LogWarning("AssetbundleDir : /ab_ios/");
                return "/ab_ios/";
            }
#else
            //else
            {
                return "/ab_pc/";
                //Debug.LogWarning("Application.platform = " + Application.platform.ToString());
                //Debug.LogWarning("AssetbundleDir : /Assets/StreamingAssets/ab_ad/");
                //return IP + "/Assets/StreamingAssets/ab_ad/";
            }
#endif
        }
    }
}

public static class CResPath
{
    /* 说明
    * LocalPath    本地系统文件路径
    * LocalUrl     WWW 本地文件路径
    * RemoteUrl    WWW Http文件路径
    */
    static string m_szLocalVersionFile = "LocalVersion.txt";
    static string m_szStreamVersionFile = "StreamVersion.txt";
    static string m_szLocalPackages = "LocalPackages.txt";
    static string m_szVersionListFile = "VersionList.txt";
    static string m_szUpdateListFile = "UpdateList.txt";
    static string m_szTempCacheFolder = "TempCache";
    static string m_szRelationFolder = "AssetRelation";

    static string m_szVersionListFolder = "Version";

    static string m_szResRemoteUrl = string.Empty;


    public static void InitRemoteUrl(string _szResUrl)
    {
        m_szResRemoteUrl = _szResUrl;
    }

    public static string GetUpdateListFileName()
    {
        return m_szUpdateListFile;
    }

    public static string GetVersionListFileName()
    {
        return m_szVersionListFile;
    }

    public static string GetRelationFolder()
    {
        return m_szRelationFolder;
    }
    /// <summary>
    /// 获得本地文件系统路径
    /// </summary>
    /// <param name="_szPath"></param>
    /// <returns></returns>
    public static string GetLocalPath(string _szPath)
    {  
#if UNITY_ANDROID
    if ( Application.platform == RuntimePlatform.WindowsEditor )
        return Application.dataPath + "/../ab_ad/" + _szPath;
    else
        return Application.persistentDataPath + "/ab_ad/" + _szPath;
#elif UNITY_IPHONE
        if ( Application.platform == RuntimePlatform.OSXEditor )
            return Application.dataPath + "/../ab_ios/" + _szPath;
        else
            return Application.persistentDataPath + "/ab_ios/"+ _szPath;
#else
         return Application.dataPath + "/../ab_pc/" + _szPath;
#endif
    }

    public static string GetLocalRoot()
    {
#if (UNITY_EDITOR || UNITY_STANDALONE)
        return Application.dataPath + "/../";
#else
        return Application.persistentDataPath;
#endif
    }

    public static string GetLocalZipPath()
    {
#if (UNITY_EDITOR || UNITY_STANDALONE)
        return GetLocalRoot() + "ab_pc.txt";
#elif UNITY_ANDROID
        return GetLocalRoot() + '/' + "ab_ad.zip";
#elif UNITY_IPHONE
        return GetLocalRoot()  + '/' + "ab_ios.zip";;
#else
       return "UNKNOW";
#endif

    }

    public static string GetRemoteResDir()
    {
#if (UNITY_EDITOR || UNITY_STANDALONE)
        return "/ab_pc/Res/";
#elif UNITY_ANDROID
        return "/ab_ad/Res/";
#elif UNITY_IPHONE
       return "/ab_ios/Res/";
#else
        return "/ab_other/Res/";
#endif
    }


    /// <summary>
    /// 获得本地文件Url路径
    /// </summary>
    /// <param name="_szPath"></param>
    /// <returns></returns>
    public static string GetLocalUrl(string _szPath)
    {
        return Correct("file://" + _szPath);
    }

    /// <summary>
    /// 获得远程资源目录下文件的Url路径
    /// </summary>
    /// <param name="_szPath"></param>
    /// <returns></returns>
    public static string GetResources_RemoteUrl(string _szPath)
    {
        return Correct(Path.Combine(m_szResRemoteUrl + GetRemoteResDir(), _szPath));
    }

    public static string GetConfig_RemoteUrl(string _szPath)
    {
        return Correct(Path.Combine(m_szResRemoteUrl, _szPath));
    }

    static string Correct(string _szPath)
    {
        return _szPath.Replace('\\', '/');
    }

    /// <summary>
    /// 获得StreamAsset文件绝对路径
    /// </summary>
    /// <param name="_szPath"></param>
    /// <returns></returns>

    public static string GetStreamAsset_LocalPath(string _szPath)
    {
        return Path.Combine(Application.streamingAssetsPath, _szPath);
    }

    public static string GetAssetBundle_LocalPath(string _szPath)
    {
        return GetLocalPath(_szPath);
    }

    /// <summary>
    /// 获得临时缓存文件路径
    /// </summary>
    /// <param name="_szPath"></param>
    /// <returns></returns>
    public static string GetTempCache_LocalPath()
    {
        return GetLocalPath(m_szTempCacheFolder);
    }

    public static string GetVersionList_RemoteUrl()
    {
        return GetConfig_RemoteUrl(m_szVersionListFile);
    }

    public static string GetVersionList_LocalPath()
    {
        return GetLocalPath(m_szVersionListFile);
    }

    public static string GetLocalVersionFile()
    {
        return m_szLocalVersionFile;
    }

    public static string GetLocalPackages()
    {
        return GetLocalPath(m_szLocalPackages);
    }

    public static string GetUpdateList_RemoteUrl(string _szVersion)
    {
        return GetResources_RemoteUrl(Path.Combine(m_szVersionListFolder, _szVersion + ".txt"));
    }

    public static string GetUpdateListRemoteUrl_Version(string _szPath)
    {
        int iIndex = _szPath.LastIndexOf('/');
        string szTemp = _szPath.Remove(iIndex, _szPath.Length - iIndex);
        return szTemp.Remove(0, szTemp.LastIndexOf('/') + 1);
    }

    public static string GetUpdateList_LocalPath(string _szVersion)
    {
        return GetLocalPath(Path.Combine(_szVersion, m_szUpdateListFile));
    }

    public static string GetPackageRelation_LocalPath()
    {
        return GetLocalPath(m_szRelationFolder);
    }

    /// <summary>
    /// 获得系统文件夹的文件路径
    /// </summary>
    /// <param name="_szPath"></param>
    /// <returns></returns>
    public static string GetStreamFile(string _szPath,bool _bLoadLocal = false)
    {
        string szFileFlag = (_bLoadLocal == true ? "file://" : "");
#if (UNITY_EDITOR || UNITY_STANDALONE)
        return szFileFlag + Application.streamingAssetsPath + "/ab_pc/" + _szPath;
#elif UNITY_ANDROID
        return szFileFlag + Application.streamingAssetsPath + "/ab_ad/" + _szPath;
#elif UNITY_IPHONE
        return szFileFlag + Application.streamingAssetsPath + "/ab_ios/" + _szPath;
#else
        return szFileFlag + Application.streamingAssetsPath + "/ab_Other/" + _szPath;
#endif
    }

    /// <summary>
    /// 获取Stream
    /// </summary>
    /// <returns></returns>
    public static string GetStream_LocalVersion()
    {
#if (UNITY_EDITOR || UNITY_STANDALONE || UNITY_IPHONE)
        return "file://" + GetStreamAsset_LocalPath(m_szStreamVersionFile);
#else
        return GetStreamAsset_LocalPath(m_szStreamVersionFile);
#endif
    }

    public static string GetStreamAsset_LocalPath_Version()
    {
        return GetStreamAsset_LocalPath(m_szStreamVersionFile);
    }

    /// <summary>
    /// 得到明文依赖项的路径
    /// </summary>
    /// <returns></returns>
    public static string GetRealRelationDir()
    {

#if UNITY_ANDROID
         return GetLocalPath("AssetRelation_Android");
#elif UNITY_IPHONE
         return GetLocalPath("AssetRelation_IOS");
#else
         return GetLocalPath("AssetRelation_PC");
#endif
    }


    public static string GetCompressAssetPath()
    {

#if UNITY_ANDROID
         return GetLocalPath("ab_ad.zip");
#elif UNITY_IPHONE
         return GetLocalPath("ab_ios.zip");
#else
        return GetLocalPath("ab_pc.zip");
#endif 
    }

    public static string StreamAssetZipPath()
    {
        return ExportAssetZipPath();
// #if (UNITY_EDITOR || UNITY_STANDALONE || UNITY_IPHONE)
//         return "file://" + ExportAssetZipPath();
// #else
//         return ExportAssetZipPath();
// #endif
    }

    public static string ExportAssetZipPath()
    {
#if UNITY_ANDROID
         return Application.streamingAssetsPath + "/ab_ad.zip";
#elif UNITY_IPHONE
         return Application.streamingAssetsPath + "/ab_ios.zip";
#else
        return Application.streamingAssetsPath + "/ab_pc.zip";
#endif 
    }

    public static string GetAssetFlag()
    {
#if UNITY_ANDROID
         return "ab_ad";
#elif UNITY_IPHONE
        return "ab_ios";
#else
        return "ab_pc";
#endif
    }

   
}
