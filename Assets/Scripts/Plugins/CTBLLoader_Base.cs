
/********************************************************************
    created:	2011/11/03
    created:	1
    filename: 	E:\Unity_SVN\XianTao\Assets\Script\Global\CTBLLoader.cs
    file path:	E:\Unity_SVN\XianTao\Assets\Script\Global
    file base:	
    file ext:	cs
    author:		Realizz
	
    purpose:	用来读取Tab分割格式的配置文件
*********************************************************************/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class CTBLLoader_Base
{

    public class SLine
    {
        public List<string> vCells = new List<string>();
    };

    List<SLine> m_vecValues = new List<SLine>();
    Dictionary<string, int> m_mapColName = new Dictionary<string, int>();
    Dictionary<int, int> m_mapLineIDs = new Dictionary<int, int>();
    Dictionary<string, int> m_mapLineStrings = new Dictionary<string, int>();

    int m_nLineCount = 0;
    int m_nColCount = 0;
    SLine m_pCurrentLine = null;
    string m_strFileName;

    public int GetLineCount() { return m_nLineCount; }

    public void LoadFromFileCfg(string strFileName)
    {
        //TCFC.TDEBUG(strFileName);
        LoadFromMem(XTFile.ReadCfgFile(XTAppPath.ConfigPath + strFileName));
    }

    public void LoadFromFileEx(string strFileName)//例如strFileName = "ActivityDone"，不需要后缀名
    {
        LoadFromMem(XTFile.ReadCfgFile(strFileName));
    }

    public void LoadFromMem(MemoryStream ms, bool bBuildID = false)
    {
        if (ms == null)
        {
            return;
        }
        string line = "";
        byte[] bytes = ms.GetBuffer();
        if ((bytes[0] == 0xFF && bytes[1] == 0xFE) ||
            (bytes[0] == 0xFE && bytes[1] == 0xFF)
            )
        {
            line = System.Text.Encoding.Unicode.GetString(bytes, 2, (int)ms.Length - 2);
        }
        else
        {
            line = System.Text.Encoding.Unicode.GetString(bytes, 0, (int)ms.Length);
        }
        //TCFC.TDEBUG(line);
        string[] vals = line.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        int nLineNumber = vals.Length;
        for (int n = 0; n < nLineNumber; n++)
        {
            if (!vals[n].StartsWith("//"))
                LoadFromString(vals[n], n);
        }
        m_pCurrentLine = GetLineByIndex(0);
        //默认使用第0行和第0列来构建2个词典
        if (bBuildID) BuildIDLineMap(0);
    }

    public void LoadFileNormal(string strFileName)
    {
        FileStream fs = new FileStream(strFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        StreamReader sr = new StreamReader(fs);
        m_strFileName = strFileName;
        string line = sr.ReadToEnd();
        LoadFromData(line);
    }

    public void LoadFromData(string strFileData)
    {
        string[] vals = strFileData.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        int nLineNumber = vals.Length;

        //return;

        for (int n = 0; n < nLineNumber; n++)
        {
            if (!vals[n].StartsWith("//"))
                LoadFromString(vals[n], n);
        }

        //return;

        m_pCurrentLine = GetLineByIndex(0);
        //默认使用第0行和第0列来构建2个词典
        //if (bBuildID) BuildIDLineMap(0);
    }

    public void LoadFromFile(string strFileName, bool bBuildID = false)//例如strFileName = "ActivityDone"，不需要后缀名
    {
        //using(Perf p = new Perf("CTBLLoader_Base::LoadFromFile:" + strFileName))
        {
            {
                string line = CFileMiddle.LoadFromFile(strFileName);
                m_strFileName = strFileName;
                //string line = file.text;
                string[] vals = line.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                int nLineNumber = vals.Length;

                //return;

                for (int n = 0; n < nLineNumber; n++)
                {
                    if (!vals[n].StartsWith("//"))
                        LoadFromString(vals[n], n);
                }

                //return;

                m_pCurrentLine = GetLineByIndex(0);
                //默认使用第0行和第0列来构建2个词典
                if (bBuildID) BuildIDLineMap(0);

                //Resources.UnloadAsset(file); 
            }
        }
    }

    public void LoadFromString(string strLine, int nLineNumber)
    {
        //TCFC.TDEBUG("strLine:" + strLine);
        string[] vals = strLine.Split(new char[] { '\t' });
        //TCFC.TDEBUG("vals.Length:" + vals.Length);
        if (nLineNumber == 0)
        {
            SLine firstLine = new SLine();
            //第一行剔除掉所有空的string
            for (int i = 0; i < vals.Length; i++)
            {
                vals[i].Trim();

                if (vals[i].Length > 0)
                {
                    firstLine.vCells.Add(vals[i]);
                }
            }
            //第一行需要记录非空的列数
            m_nColCount = firstLine.vCells.Count;
            //构建列的名字map
            BuildColNameMap(firstLine);
        }
        else//非第一行
        {
            //当此行的第一列非空时才会进行数据记录
            if (vals[0].Length > 0)
            {
                SLine newLine = new SLine();

                //以第一行的列数为标准来记录数据
                for (int i = 0; i < m_nColCount; i++)
                {
                    if (i < vals.Length)
                    {
                        vals[i].Trim();
                        newLine.vCells.Add(vals[i]);
                    }
                    else
                        newLine.vCells.Add("");
                }
                m_vecValues.Add(newLine);
                m_nLineCount++;
            }
        }
    }
    //按ID来获取行号
    public int GetLineIndexByID(int nId)
    {
        int nIndex = -1;
        bool bExist = m_mapLineIDs.TryGetValue(nId, out nIndex);
        if (bExist)
            return nIndex;
        else
            return -1;
    }
    //按String来获取行号
    public int GetLineIndexByString(string str)
    {
        int nIndex = -1;
        bool bExist = m_mapLineStrings.TryGetValue(str, out nIndex);
        if (bExist)
            return nIndex;
        else
            return -1;
    }
    //按Index来获取行
    public SLine GetLineByIndex(int nIndex)
    {
        if ((nIndex < m_nLineCount) && (nIndex >= 0))
            return m_vecValues[nIndex];
        else
            return null;
    }

    //按ID来获取行
    public SLine GetLineByID(int nID)
    {
        int nIndex = GetLineIndexByID(nID);
        return GetLineByIndex(nIndex);
    }
    //用ID来转移当前行
    public void GoToLineByID(int nID)
    {
        int nIndex = GetLineIndexByID(nID);
        GoToLineByIndex(nIndex);
    }

    //按string来获取行
    public SLine GetLineByString(string str)
    {
        int nIndex = GetLineIndexByString(str);
        return GetLineByIndex(nIndex);
    }
    //用string来转移当前行
    public void GoToLineByString(string str)
    {
        int nIndex = GetLineIndexByString(str);
        GoToLineByIndex(nIndex);
    }

    //用index来转移当前行
    public void GoToLineByIndex(int nIndex)
    {
        m_pCurrentLine = GetLineByIndex(nIndex);
    }
    //用某一行来进行名字索引构建
    //nLineIndex表示哪一行是名字
    public void BuildColNameMap(SLine indexLine)
    {
        m_mapColName.Clear();
        int index = 0;
        //foreach (string strName in indexLine.vCells)
        for (int iLine = 0; iLine < indexLine.vCells.Count; iLine++)
        {
            string strName = indexLine.vCells[iLine];
            m_mapColName.Add(strName, index);
            index++;
        }
    }

    //用某一列来进行ID索引构建
    //nColIndex表示哪一列是ID
    public bool BuildIDLineMap(int nColIndex)
    {
        int index = 0;
        if (nColIndex >= m_nColCount)
            return false;

        m_mapLineIDs.Clear();

        //foreach (SLine indexLine in m_vecValues)
        for(int iLine = 0; iLine < m_vecValues.Count; iLine++)
        {
            SLine indexLine = m_vecValues[iLine];
            try
            {
                int id = int.Parse(indexLine.vCells[nColIndex]);
                if (m_mapLineIDs.ContainsKey(id)) continue;//重复id,直接丢弃.
                m_mapLineIDs.Add(id, index);
            }
            catch (System.Exception)
            {
                TCFC.TDEBUG("LoadFile=" + m_strFileName + " Failed in Info=" + indexLine.vCells[nColIndex]);
            }
            index++;
        }
        return true;
    }
    //用某一列来进行string索引构建
    //nColIndex表示哪一列是ID
    public bool BuildStringLineMap(int nColIndex)
    {
        int index = 0;
        if (nColIndex >= m_nColCount)
            return false;

        m_mapLineStrings.Clear();

        //foreach (SLine indexLine in m_vecValues)
        for(int iLine = 0; iLine < m_vecValues.Count; iLine++)
        {
            SLine indexLine = m_vecValues[iLine];
            m_mapLineStrings.Add(indexLine.vCells[nColIndex], index);
            index++;
        }
        return true;
    }
    //获取当前行的查找值_String
    public bool GetStringByName(string strFieldName, out string strFieldValue)
    {
        int nIndex = -1;

        if (m_pCurrentLine == null)
        {
            strFieldValue = null;
            return false;
        }
        else
        {
            bool bExist = m_mapColName.TryGetValue(strFieldName, out nIndex);
            if (bExist)
            {
                strFieldValue = m_pCurrentLine.vCells[nIndex];
                bExist = !string.IsNullOrEmpty(strFieldValue);
            }
            else
            {
                Debug.LogWarning("TBL:" + m_strFileName + " invalid title:" + strFieldName);
                strFieldValue = "";
            }
            return bExist;
        }

    }
    //获取当前行的查找值_int
    public bool GetIntByName(string strFieldName, out int nFieldValue)
    {
        string strValue;
        bool bExist = GetStringByName(strFieldName, out strValue);
        if (bExist) { bExist = int.TryParse(strValue, out nFieldValue); }
        else { nFieldValue = 0; }
        return bExist;
    }
    //获取当前行的查找值_float
    public bool GetFloatByName(string strFieldName, out float fFieldValue)
    {
        string strValue;
        bool bExist = GetStringByName(strFieldName, out strValue);
        if (bExist) { bExist = float.TryParse(strValue, out fFieldValue); }
        else { fFieldValue = 0; }
        return bExist;
    }
    //获取当前行的查找值_int64
    public bool GetInt64ByName(string strFieldName, out Int64 i64FieldValue)
    {
        string strValue;
        bool bExist = GetStringByName(strFieldName, out strValue);
        if (bExist) { bExist = Int64.TryParse(strValue, out i64FieldValue); }
        else { i64FieldValue = 0; }
        return bExist;
    }
    //获取当前行的查找值 Bool
    public bool GetBoolByName(string strFieldName, out bool bFieldValue)
    {
        string strValue;
        bool bExist = GetStringByName(strFieldName, out strValue);
        if (bExist)
        {
            int nFieldValue = 0;
            bExist = int.TryParse(strValue, out nFieldValue);
            bFieldValue = (nFieldValue != 0);
        }
        else { bFieldValue = false; }
        return bExist;
    }
    //获取枚举
    public bool GetEmByName<T>(string strFieldName, out T emValue)
    {
        string strValue;
        bool bExist = GetStringByName(strFieldName, out strValue);
        if (bExist)
        { emValue = (T)System.Enum.Parse(typeof(T), strValue); }
        else { emValue = default(T); }

        return bExist;
    }
}


