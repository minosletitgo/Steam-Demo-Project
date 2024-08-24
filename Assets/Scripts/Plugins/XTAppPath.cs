using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;


public static class XTAppPath
{
    static string s_strAppName;
    static string s_strRoot;
	static string s_strStreamAssert;

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

    public static string AppName
    {
        get
        {
            if (string.IsNullOrEmpty(s_strAppName))
            { s_strAppName = "EW";/*UnityEditor.PlayerSettings.productName;*/ }
            return s_strAppName;
        }
    }

    public static string Resource
    {
        get { return Application.dataPath + "/Resources/"; }
    }

    public static string Root
    {
        get
        {
            if (string.IsNullOrEmpty(s_strRoot))
            {
                if (Application.platform == RuntimePlatform.Android)
                { s_strRoot = "/mnt/sdcard/" + AppName + "/"; }
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                { s_strRoot = Application.persistentDataPath + AppName + "/"; }
                else
                { s_strRoot = Application.dataPath + "/../"; }
                //TCFC.TDEBUG("platform:" + Application.platform + " s_strRoot set:" + s_strRoot);
            }
            return s_strRoot;
        }
    }

	public static string StreamAssert
	{
		get
		{
			if (string.IsNullOrEmpty(s_strRoot))
			{
				if (Application.platform == RuntimePlatform.Android)
				{ s_strStreamAssert = Application.streamingAssetsPath + "/LuaRoot/"; }
				else if (Application.platform == RuntimePlatform.IPhonePlayer)
				{ s_strStreamAssert = "file://" + Application.streamingAssetsPath + "/LuaRoot/"; }
				else
				{ s_strStreamAssert = "file://" + Application.streamingAssetsPath + "/LuaRoot/"; }
				//TCFC.TDEBUG("platform:" + Application.platform + " s_strRoot set:" + s_strRoot);
			}
			return s_strStreamAssert;
		}
	}

    public const string FolderWTF = "WTF/";
    public const string FolderLog = "Log/";
    public const string FolderConfig = "Config/";


    //原来是文件夹，如果路径的话需要区分，例如：
     //if ( Application.platform == RuntimePlatform.OSXEditor )
     //       return Application.dataPath + "/../ab_ios/" + _szPath;
     // else
     //       return Application.persistentDataPath + "/ab_ios/"+ _szPath;

    public static string StaticPlatformFolder
    {
        get
        {
#if UNITY_ANDROID
            return "ab_ad/";
#elif UNITY_IPHONE
            return "ab_ios/";
#else
            return "ab_pc/";
#endif
        }
    }

    public static string RunTimePlatformFolder
    {
        get
        {
            if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.WindowsPlayer)
                return "ab_pc/";
            else if (Application.platform == RuntimePlatform.Android)
                return "ab_ad/";
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
                return "ab_ios/";
            else
                return "ab_pc/";
        }
    }

    public static string AssetbundleURL
    {
        get
        {
            if (IsNet)
            {
                return IP + "/" + RunTimePlatformFolder;
                //if (Application.platform == RuntimePlatform.WindowsWebPlayer || Application.platform == RuntimePlatform.WindowsPlayer)
                //    return IP + "/ab_pc/";
                //else if (Application.platform == RuntimePlatform.Android)
                //    return IP + "/ab_ad/";
                //else if (Application.platform == RuntimePlatform.IPhonePlayer)
                //    return IP + "/ab_ios/";
                //else
                //    return IP + "/ab_pc/";
            }
            else
            {
                if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.WindowsPlayer)
                    return "file://" + Application.dataPath + "/../" + RunTimePlatformFolder;
                else if (Application.platform == RuntimePlatform.Android)
                    return "file:///mnt/sdcard/" + AppName + "/" + RunTimePlatformFolder;
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                    return "file:///mnt/sdcard/" + AppName + "/" + RunTimePlatformFolder;
                else
                    return "file://" + Application.dataPath + "/../" + RunTimePlatformFolder;

                //if (Application.platform == RuntimePlatform.WindowsWebPlayer || Application.platform == RuntimePlatform.WindowsPlayer)
                //    return "file://" + Application.dataPath + "/../ab_pc/";
                //else if (Application.platform == RuntimePlatform.Android)
                //    return "file:///mnt/sdcard/" + AppName + "/ab_ad/";
                //else if (Application.platform == RuntimePlatform.IPhonePlayer)
                //    return "file:///mnt/sdcard/" + AppName + "/ab_ios/";
                //else
                //    return "file://" + Application.dataPath + "/../ab_pc/";
            }
        }
    }

    public static string ResourcePath2RootPath(string strResourcePath)
    {
        return "Assets/Resources/" + strResourcePath;
    }

    public static string ResourcePath2AbsolutePath(string strResourcePath)
    {
        string strAbsoluteFilePath = Resource + strResourcePath;
        return strAbsoluteFilePath;
    }

    public static string RootPath2AbsolutePath(string strRootPath)
    {
        const string _start = "Assets";
        string strAbsolutePath = null;
        if (strRootPath.StartsWith(_start,StringComparison.OrdinalIgnoreCase))
        { strAbsolutePath = Application.dataPath + strRootPath.Substring(_start.Length); }
        else { strAbsolutePath = Application.dataPath + strRootPath;}
        return strAbsolutePath;
    }

    public static string Path2ResourcePath(string strPath)
    {
        const string _start = "Resources/";
        int idx = strPath.LastIndexOf(_start, StringComparison.OrdinalIgnoreCase);
        if (idx != -1) { return strPath.Substring(idx + _start.Length); }
        return strPath;
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
        return s.Replace("#", "%23");
    }

    public static string ConfigPath { get { return Root + FolderConfig; } }

    public static bool SpecailConfig(string _szSpecailConfig)
    {
        if (_szSpecailConfig.Contains("config/msgDef") ||
            _szSpecailConfig.Contains("config/Quest")  ||
            _szSpecailConfig.Contains("config/IFR") ||
            _szSpecailConfig.Contains("LuaRoot/AI"))
            return true;
        return false;
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

    public static string SpecailConfigPath
    {
        get
        {

            string strSpecailDir = GetLocalPath("SpecailConfig/");
            if (!XTFile.ExistDirectory(strSpecailDir))
            {
                XTFile.CreateDirectory(strSpecailDir);
            }
            return strSpecailDir + "SpecailConfig".GetHashCode() + ".txt";
        }
    }
}

