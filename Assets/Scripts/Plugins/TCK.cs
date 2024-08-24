/********************************************************************
	created:	2012/10/18
	created:	18:10:2012   16:54
	filename: 	E:\UN_SVN\ShuShan\ShuShan\Assets\Script\Global\TCK.cs
	file path:	E:\UN_SVN\ShuShan\ShuShan\Assets\Script\Global
	file base:	CTCheck
	file ext:	cs
	author:		ash
	
	purpose:	类似C++里的CHECK宏
 *              在C#里的变通实现
 *              暂时还不能达到同C++一样强大的效果.需要后续改进.
 *              先定接口
*********************************************************************/


using UnityEngine;
using System;
using System.Collections;
using System.Diagnostics;

public static class TCK 
{
    public static void RegException()
    {
        Application.RegisterLogCallback(ProcessException);
        System.AppDomain.CurrentDomain.UnhandledException += _OnUnresolvedExceptionHandler;
    }

    private static void _OnUnresolvedExceptionHandler (object sender, System.UnhandledExceptionEventArgs args)
    {
        if (args == null || args.ExceptionObject == null)
        {
            return;
        }

        if (args.ExceptionObject.GetType() != typeof(System.Exception))
        {
            return;
        }

        doLogError((System.Exception)args.ExceptionObject);
    }

    private static void doLogError(System.Exception e)
    {
        StackTrace stackTrace = new StackTrace(e, true);
        string[] classes = new string[stackTrace.FrameCount];
        string[] methods = new string[stackTrace.FrameCount];
        string[] files = new string[stackTrace.FrameCount];
        int[] lineNumbers = new int[stackTrace.FrameCount];

        String message = "";

        for (int i = 0; i < stackTrace.FrameCount; i++)
        {
            StackFrame frame = stackTrace.GetFrame(i);
            classes[i] = frame.GetMethod().DeclaringType.Name;
            methods[i] = frame.GetMethod().Name;
            files[i] = frame.GetFileName();
            lineNumbers[i] = frame.GetFileLineNumber();

            message += classes[i] + ".";
            message += methods[i] + "() (at ";
            message += files[i] + ":";
            message += lineNumbers[i] + ")";
        }

        TCFC.LOG("[ERRO]" + "[" + e.GetType().Name + "]:" + message);
        //TestinCrashHelper.LogHandledException(e);
    }

    public static void ProcessException(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
        {
            TCFC.LOG("[ERRO]" + "[" + type.ToString() + "]:" + condition + " Trace:" + stackTrace);
            //TestinCrashHelper._OnDebugLogCallbackHandler(condition, stackTrace, type);
        }
        //else { TINFO("[" + type.ToString() + "]:" + condition); }
    }

    public static bool CHECK(bool bCheck,string strErro = null)
    {
        if (!bCheck)
        {
            throw new SystemException("[CHECK]" + strErro);
        }

        return bCheck;
    }
    

    //public delegate void DGOnLog(string strLog);
    //public static DGOnLog m_dgOnLog;
    public static void LOG(string strLog)
    {
        TCFC.LOG(strLog);
    }

    public static void TINFO(string strLog)
    {
        TCFC.TINFO(strLog);
    }

    public static void TDEBUG(string strLog)
    {
        TCFC.TDEBUG(strLog);
    }

    public static void TERRO(string strLog)
    {
        TCFC.TERRO(strLog);
    }

    public static void TWARNING(string strLog)
    {
        TCFC.TWARNING(strLog);
    }
}
