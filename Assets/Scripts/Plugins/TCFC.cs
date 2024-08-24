using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System;

public static class TCFC 
{
    //flag 方便函数
    public static bool HasFlag(int a, int b) { return ((a & b) == b); }//包含
    public static bool ItrFlag(int a, int b) { return ((a & b) != 0); }//有交集
    public static int SetFlag(int a, int b) { return a | b; }
    public static void SetFlag(ref int a, int b) { a = SetFlag(a, b); }
    public static int ClsFlag(int a, int b) { return a & (~b); }
    public static void ClsFlag(ref int a, int b) { a = ClsFlag(a, b); }

    //32|32 <-> 64
    public static System.Int64 Make64(System.Int32 a, System.Int32 b) { return ((System.Int64)(a) << 32) | ((System.Int64)(b) & 0xffffffff); }
    public static System.Int32 Split32H(System.Int64 a) { return ((System.Int32)((System.Int64)(a) >> 32)); }
    public static System.Int32 Split32L(System.Int64 a) { return ((System.Int32)((System.Int64)(a) & 0xffffffff)); }

    public static string getMD5Str(string str)
    {
        StringBuilder sb = new StringBuilder();
        foreach (byte b in System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(str)))
        {
            sb.Append(b.ToString("X2"));
        }
        return sb.ToString().ToLower();
    }

    public static string CalculateByteSubstring(string strTarget, int nCutBytes)
    {
        if (nCutBytes <= 0)
        {
            return "";
        }

        int i = 0;
        int n = 0;
        byte[] arrayBytes = System.Text.Encoding.Unicode.GetBytes(strTarget);
        for (i = 0; i < arrayBytes.Length && n < nCutBytes; i++)
        {
            if (i % 2 == 0)
            {
                n++;
            }
            else
            {
                if (arrayBytes[i] > 0)
                {
                    n++;
                }
            }
        }

        if (i % 2 != 0)
        {
            if (arrayBytes[i] > 0)
            {
                i--;
            }
            else
            {
                i++;
            }
        }

        return System.Text.Encoding.Unicode.GetString(arrayBytes, 0, i);
    }


    public static int CalculateByteCount(string strTarget)
    {
        int nRet = 0;
        byte[] arrayBytes = System.Text.Encoding.Unicode.GetBytes(strTarget);
        for (int i = 0; i < arrayBytes.Length; i++)
        {
            if (i % 2 == 0)
            {
                nRet++;
            }
            else
            {
                if (arrayBytes[i] > 0)
                {
                    nRet++;
                }
            }
        }

        return nRet;
    }

    public static int CalculateByteCount(byte[] arrayBytes)
    {
        //针对于宽字节
        int nRet = 0;
        for (int i = 0; i < arrayBytes.Length; i++)
        {
            if (i % 2 == 0)
            {
                if (arrayBytes[i] == 0 && arrayBytes[i + 1] == 0 && i < arrayBytes.Length - 1)
                {
                    break;
                }
                else
                {
                    nRet++;
                }
            }
            else
            {
                //if (arrayBytes[i] > 0)
                {
                    nRet++;
                }
            }
        }

        return nRet;
    }

    public static void TimeTurnMonth(ref int nYear, ref int nMonth, bool bNext)
    {
        TCK.CHECK(nYear > 0);
        TCK.CHECK(nMonth >= 1 && nMonth <= 12);

        if (!bNext)
        {
            if (nMonth == 1)
            {
                nYear--;
                nMonth = 12;
                return;
            }
            else
            {
                nMonth--;
                return;
            }
        }
        else
        {
            if (nMonth == 12)
            {
                nYear++;
                nMonth = 1;
                return;
            }
            else
            {
                nMonth++;
                return;
            }
        }
    }

    public static void TimeTurnDay(ref int nYear, ref int nMonth, ref int nDay, bool bNext)
    {
        TCK.CHECK(nYear > 0);
        TCK.CHECK(nMonth >= 1 && nMonth <= 12);

        int nDaysInMonth = DateTime.DaysInMonth(nYear, nMonth);
        TCK.CHECK(nDay > 0 && nDay <= nDaysInMonth);

        if (!bNext)
        {
            if (nDay == 1)
            {
                if (nMonth == 1)
                {
                    nYear--;
                    nMonth = 12;
                    nDaysInMonth = DateTime.DaysInMonth(nYear, nMonth);
                    nDay = nDaysInMonth;
                    return;
                }
                else
                {
                    nMonth--;
                    nDaysInMonth = DateTime.DaysInMonth(nYear, nMonth);
                    nDay = nDaysInMonth;
                    return;
                }
            }
            else
            {
                nDay--;
                return;
            }
        }
        else
        {
            if (nDay == nDaysInMonth)
            {
                if (nMonth == 12)
                {
                    nYear++;
                    nMonth = 1;
                    nDay = 1;
                    return;
                }
                else
                {
                    nMonth++;
                    nDay = 1;
                    return;
                }
            }
            else
            {
                nDay++;
                return;
            }
        }
    }

    public static int MonthToID(int nYear, int nMonth)
    {
        TCK.CHECK(nYear > 0);
        TCK.CHECK(nMonth >= 1 && nMonth <= 12);

        return int.Parse(nYear.ToString() + nMonth.ToString("00"));
    }

    public static bool IDToMonth(int nMonthID, ref int nYear, ref int nMonth)
    {
        string strMonthID = nMonthID.ToString();

        string strYear = strMonthID.Substring(0, strMonthID.Length - 2);
        if (!int.TryParse(strYear, out nYear))
        {
            return false;
        }

        string strMonth = strMonthID.Substring(strMonthID.Length - 2, 2);
        if (!int.TryParse(strMonth, out nMonth))
        {
            return false;
        }

        return true;
    }

    public delegate void DGOnLog(string strLog);
    public static DGOnLog m_dgOnLog;
    public static void LOG(string strLog)
    {
        XTFile.Log(strLog);
        Debug.Log(strLog);
        if (m_dgOnLog != null) m_dgOnLog(strLog);
    }

    public static void TINFO(string strLog)
    {
        if (UnityEngine.Debug.isDebugBuild) TDEBUG(strLog);
        else LOG("[INFO]" + strLog);
    }

    public static void TDEBUG(object message)
    {
        string strLog = message.ToString();
        if (UnityEngine.Debug.isDebugBuild)
        {
            LOG("[DEBUG]" + strLog);
            //UnityEngine.Debug.Log(strLog);
        }
    }

    public static void TERRO(string strLog)
    {
        //LOG("[ERRO]" + strLog);
        UnityEngine.Debug.LogError(strLog);
    }

    public static void TWARNING(string strLog)
    {
        LOG("[WARNING]" + strLog);
        UnityEngine.Debug.LogWarning(strLog);
    }
}
