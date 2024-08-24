using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Minos_CTBLInfo //: ScriptableObject
{
    //singleton
    //-----------------
    public static Minos_CTBLInfo s_Inst = null;//故意public
    public static Minos_CTBLInfo Inst { get { if (s_Inst == null) s_Inst = new Minos_CTBLInfo(); /*ScriptableObject.CreateInstance<Minos_CTBLInfo>();*/ return s_Inst; } }
    public static void Destroy() { s_Inst = null; }
    public Minos_CTBLInfo() { }//故意public








    public class ST_IconInfo
    {
        public int id;
        public string strAtlas;
        public string strSprite;
        public Vector2 vDimension;
    }
    Dictionary<int, ST_IconInfo> m_mIconInfo = new Dictionary<int, ST_IconInfo>();

    public bool LoadIconInfo(string strPath)
    {
        CTBLLoader_Base loader = new CTBLLoader_Base();
        loader.LoadFromFile(strPath);
        int nLine = loader.GetLineCount();
        for (int i = 0; i < nLine; i++)
        {
            loader.GoToLineByIndex(i);

            ST_IconInfo objInfo = new ST_IconInfo();
            loader.GetIntByName("ID", out objInfo.id);
            loader.GetStringByName("Atlas", out objInfo.strAtlas);
            loader.GetStringByName("Sprite", out objInfo.strSprite);

            string strDimension;
            loader.GetStringByName("Dimension", out strDimension);
            if (!string.IsNullOrEmpty(strDimension))
            {
                string[] arrayStrDimension = strDimension.Split(':');
                TCK.CHECK(arrayStrDimension.Length == 3);
                objInfo.vDimension = new Vector2(
                    float.Parse(arrayStrDimension[1]),
                    float.Parse(arrayStrDimension[2]));
            }

            m_mIconInfo.Add(objInfo.id, objInfo);
        }

        return true;
    }







    public class ST_EverydayEnemy
    {
        public int nDayId;

        public class ST_EnemyGroup
        {
            public EM_E_CharacterType emType;
            public int nCount;
        };

        public List<ST_EnemyGroup> lstEnemyInNight;
    };
    Dictionary<int, ST_EverydayEnemy> m_mEverydayEnemy = new Dictionary<int, ST_EverydayEnemy>();
    
    public bool LoadEverydayEnemy(string strPath)
    {
        CTBLLoader_Base loader = new CTBLLoader_Base();
        loader.LoadFromFile(strPath);
        int nLine = loader.GetLineCount();
        for (int i = 0; i < nLine; i++)
        {
            loader.GoToLineByIndex(i);

            ST_EverydayEnemy stInfo = new ST_EverydayEnemy();

            loader.GetIntByName("DayId", out stInfo.nDayId);

            string strEnemyInNight;
            loader.GetStringByName("InNight", out strEnemyInNight);
            if (!string.IsNullOrEmpty(strEnemyInNight))
            {
                stInfo.lstEnemyInNight = new List<ST_EverydayEnemy.ST_EnemyGroup>();

                string[] arrayStrEnemy = strEnemyInNight.Split('|');
                TCK.CHECK(arrayStrEnemy.Length > 0);
                for (int iE = 0; iE < arrayStrEnemy.Length; iE++)
                {
                    string[] arrayStrCouple = arrayStrEnemy[iE].Split(':');
                    TCK.CHECK(arrayStrCouple.Length == 2);

                    ST_EverydayEnemy.ST_EnemyGroup stCouple = new ST_EverydayEnemy.ST_EnemyGroup();
                    stCouple.emType = (EM_E_CharacterType)(Enum.Parse(typeof(EM_E_CharacterType), arrayStrCouple[0]));
                    stCouple.nCount = int.Parse(arrayStrCouple[1]);
                    TCK.CHECK(stCouple.nCount > 0);

                    stInfo.lstEnemyInNight.Add(stCouple);
                }
            }

            m_mEverydayEnemy.Add(stInfo.nDayId, stInfo);
        }

        return m_mEverydayEnemy.Count > 0;
    }

    public ST_EverydayEnemy GetEverydayEnemy(int nDayId)
    {
        ST_EverydayEnemy stRet = null;
        if (m_mEverydayEnemy.TryGetValue(nDayId, out stRet))
        {
            return stRet;
        }
        return null;
    }

    public IEnumerable EnumEverydayEnemy()
    {
        foreach (KeyValuePair<int, ST_EverydayEnemy> _stPair in m_mEverydayEnemy)
        {
            yield return _stPair.Value;
        }
    }






    #region ----------------------------------------------------F_CharacterAttr----------------------------------------------------------------

    public class ST_F_CharacterAttr
    {
        public EM_F_CharacterType emCharType;
        public int nLv;//1开始计数

        public float[] aryAttr = new float[(int)(EM_F_CharacterAttr.Max)];

        public float GetAttr(EM_F_CharacterAttr emAttr)
        {
            GameCommon.CHECK(emAttr > EM_F_CharacterAttr.Invalid && emAttr < EM_F_CharacterAttr.Max);
            return aryAttr[(int)(emAttr)];
        }
    };
    Dictionary<EM_F_CharacterType, List<ST_F_CharacterAttr>> m_mF_CharacterAttr = new Dictionary<EM_F_CharacterType, List<ST_F_CharacterAttr>>();

    public bool LoadF_CharacterAttr(string strPath)
    {
        CTBLLoader_Base loader = new CTBLLoader_Base();
        loader.LoadFromFile(strPath);
        int nLine = loader.GetLineCount();

        EM_F_CharacterType emCharType_Last = EM_F_CharacterType.Invalid;
        int nLv_Last = 0;

        for (int i = 0; i < nLine; i++)
        {
            loader.GoToLineByIndex(i);

            ST_F_CharacterAttr stInfo = new ST_F_CharacterAttr();

            loader.GetEmByName<EM_F_CharacterType>("CharacterType", out stInfo.emCharType);
            GameCommon.CHECK(stInfo.emCharType > EM_F_CharacterType.Invalid && 
                stInfo.emCharType < EM_F_CharacterType.Max);

            loader.GetIntByName("Lv", out stInfo.nLv);
            GameCommon.CHECK(stInfo.nLv > 0);

            if (emCharType_Last > EM_F_CharacterType.Invalid && emCharType_Last == stInfo.emCharType)
            {
                GameCommon.CHECK(stInfo.nLv == nLv_Last + 1);
            }
            else
            {
                GameCommon.CHECK(stInfo.nLv == 1);
            }

            loader.GetFloatByName("Hp", out stInfo.aryAttr[(int)(EM_F_CharacterAttr.Hp)]);
            loader.GetFloatByName("HitRate", out stInfo.aryAttr[(int)(EM_F_CharacterAttr.HitRate)]);
            loader.GetFloatByName("Damage", out stInfo.aryAttr[(int)(EM_F_CharacterAttr.Damage)]);
            loader.GetFloatByName("DamageDelay", out stInfo.aryAttr[(int)(EM_F_CharacterAttr.DamageDelay)]);
            loader.GetFloatByName("Income", out stInfo.aryAttr[(int)(EM_F_CharacterAttr.Income)]);
            loader.GetFloatByName("RecoverHp", out stInfo.aryAttr[(int)(EM_F_CharacterAttr.RecoverHp)]);
            loader.GetFloatByName("WalkSpeed", out stInfo.aryAttr[(int)(EM_F_CharacterAttr.WalkSpeed)]);
            loader.GetFloatByName("RunSpeed", out stInfo.aryAttr[(int)(EM_F_CharacterAttr.RunSpeed)]);

            List<ST_F_CharacterAttr> lstInsert;
            if (!m_mF_CharacterAttr.TryGetValue(stInfo.emCharType, out lstInsert))
            {
                lstInsert = new List<ST_F_CharacterAttr>();
                m_mF_CharacterAttr.Add(stInfo.emCharType, lstInsert);
            }
            GameCommon.CHECK(lstInsert != null);

            lstInsert.Add(stInfo);

            emCharType_Last = stInfo.emCharType;
            nLv_Last = stInfo.nLv;
        }

        return m_mF_CharacterAttr.Count > 0;
    }

    public ST_F_CharacterAttr GetF_CharacterAttr(EM_F_CharacterType emCharType, int nLv)
    {
        List<ST_F_CharacterAttr> lstInsert;
        if (!m_mF_CharacterAttr.TryGetValue(emCharType, out lstInsert))
        {
            return null;
        }
        GameCommon.CHECK(lstInsert != null);

        return lstInsert.Find(v => v.nLv == nLv);
    }

    public int GetF_CharacterAttr_MinLv(EM_F_CharacterType emCharType)
    {
        List<ST_F_CharacterAttr> lstInsert;
        if (!m_mF_CharacterAttr.TryGetValue(emCharType, out lstInsert))
        {
            return 0;
        }
        GameCommon.CHECK(lstInsert != null);

        ST_F_CharacterAttr stMin = lstInsert[0];
        GameCommon.CHECK(stMin != null);
        return stMin.nLv;
    }

    public int GetF_CharacterAttr_MaxLv(EM_F_CharacterType emCharType)
    {
        List<ST_F_CharacterAttr> lstInsert;
        if (!m_mF_CharacterAttr.TryGetValue(emCharType, out lstInsert))
        {
            return 0;
        }
        GameCommon.CHECK(lstInsert != null);

        ST_F_CharacterAttr stMax = lstInsert[lstInsert.Count - 1];
        GameCommon.CHECK(stMax != null);
        return stMax.nLv;
    }
    #endregion ----------------------------------------------------F_CharacterAttr----------------------------------------------------------------


    #region ----------------------------------------------------E_CharacterAttr----------------------------------------------------------------

    public class ST_E_CharacterAttr
    {
        public EM_E_CharacterType emCharType;
        public int nLv;//1开始计数

        public float[] aryAttr = new float[(int)(EM_E_CharacterAttr.Max)];

        public float GetAttr(EM_E_CharacterAttr emAttr)
        {
            GameCommon.CHECK(emAttr > EM_E_CharacterAttr.Invalid && emAttr < EM_E_CharacterAttr.Max);
            return aryAttr[(int)(emAttr)];
        }
    };
    Dictionary<EM_E_CharacterType, List<ST_E_CharacterAttr>> m_mE_CharacterAttr = new Dictionary<EM_E_CharacterType, List<ST_E_CharacterAttr>>();

    public bool LoadE_CharacterAttr(string strPath)
    {
        CTBLLoader_Base loader = new CTBLLoader_Base();
        loader.LoadFromFile(strPath);
        int nLine = loader.GetLineCount();

        EM_E_CharacterType emCharType_Last = EM_E_CharacterType.Invalid;
        int nLv_Last = 0;

        for (int i = 0; i < nLine; i++)
        {
            loader.GoToLineByIndex(i);

            ST_E_CharacterAttr stInfo = new ST_E_CharacterAttr();

            loader.GetEmByName<EM_E_CharacterType>("CharacterType", out stInfo.emCharType);
            GameCommon.CHECK(stInfo.emCharType > EM_E_CharacterType.Invalid &&
                stInfo.emCharType < EM_E_CharacterType.Max);

            loader.GetIntByName("Lv", out stInfo.nLv);
            GameCommon.CHECK(stInfo.nLv > 0);

            if (emCharType_Last > EM_E_CharacterType.Invalid && emCharType_Last == stInfo.emCharType)
            {
                GameCommon.CHECK(stInfo.nLv == nLv_Last + 1);
            }
            else
            {
                GameCommon.CHECK(stInfo.nLv == 1);
            }

            loader.GetFloatByName("Hp", out stInfo.aryAttr[(int)(EM_E_CharacterAttr.Hp)]);
            loader.GetFloatByName("Damage", out stInfo.aryAttr[(int)(EM_E_CharacterAttr.Damage)]);
            loader.GetFloatByName("DamageDelay", out stInfo.aryAttr[(int)(EM_E_CharacterAttr.DamageDelay)]);
            loader.GetFloatByName("WalkSpeed", out stInfo.aryAttr[(int)(EM_E_CharacterAttr.WalkSpeed)]);
            loader.GetFloatByName("RunSpeed", out stInfo.aryAttr[(int)(EM_E_CharacterAttr.RunSpeed)]);

            List<ST_E_CharacterAttr> lstInsert;
            if (!m_mE_CharacterAttr.TryGetValue(stInfo.emCharType, out lstInsert))
            {
                lstInsert = new List<ST_E_CharacterAttr>();
                m_mE_CharacterAttr.Add(stInfo.emCharType, lstInsert);
            }
            GameCommon.CHECK(lstInsert != null);

            lstInsert.Add(stInfo);

            emCharType_Last = stInfo.emCharType;
            nLv_Last = stInfo.nLv;
        }

        return m_mE_CharacterAttr.Count > 0;
    }

    public ST_E_CharacterAttr GetE_CharacterAttr(EM_E_CharacterType emCharType, int nLv)
    {
        List<ST_E_CharacterAttr> lstInsert;
        if (!m_mE_CharacterAttr.TryGetValue(emCharType, out lstInsert))
        {
            return null;
        }
        GameCommon.CHECK(lstInsert != null);

        return lstInsert.Find(v => v.nLv == nLv);
    }

    public int GetE_CharacterAttr_MinLv(EM_E_CharacterType emCharType)
    {
        List<ST_E_CharacterAttr> lstInsert;
        if (!m_mE_CharacterAttr.TryGetValue(emCharType, out lstInsert))
        {
            return 0;
        }
        GameCommon.CHECK(lstInsert != null);

        ST_E_CharacterAttr stMin = lstInsert[0];
        GameCommon.CHECK(stMin != null);
        return stMin.nLv;
    }

    public int GetE_CharacterAttr_MaxLv(EM_E_CharacterType emCharType)
    {
        List<ST_E_CharacterAttr> lstInsert;
        if (!m_mE_CharacterAttr.TryGetValue(emCharType, out lstInsert))
        {
            return 0;
        }
        GameCommon.CHECK(lstInsert != null);

        ST_E_CharacterAttr stMax = lstInsert[lstInsert.Count - 1];
        GameCommon.CHECK(stMax != null);
        return stMax.nLv;
    }
    #endregion ----------------------------------------------------E_CharacterAttr----------------------------------------------------------------






    public class ST_F_Wall
    {
        public int nLv;
        public int nHp;
        public int nUpGradeNeedMoneyCoins;
    };
    List<ST_F_Wall> m_lstF_Wall = new List<ST_F_Wall>();

    public bool LoadF_Wall(string strPath)
    {
        CTBLLoader_Base loader = new CTBLLoader_Base();
        loader.LoadFromFile(strPath);
        int nLine = loader.GetLineCount();

        int nLv_Last = 0;

        for (int i = 0; i < nLine; i++)
        {
            loader.GoToLineByIndex(i);

            ST_F_Wall stInfo = new ST_F_Wall();

            loader.GetIntByName("Lv", out stInfo.nLv);

            if (nLv_Last > 0)
            {
                GameCommon.CHECK(nLv_Last + 1 == stInfo.nLv);
            }
            else
            {
                GameCommon.CHECK(stInfo.nLv == 1);
            }

            loader.GetIntByName("Hp", out stInfo.nHp);
            loader.GetIntByName("UpGradeNeedMoneyCoins", out stInfo.nUpGradeNeedMoneyCoins);

            m_lstF_Wall.Add(stInfo);

            nLv_Last = stInfo.nLv;
        }

        return m_lstF_Wall.Count > 0;
    }

    public ST_F_Wall GetF_Wall(int nLv)
    {
        return m_lstF_Wall.Find(v => v.nLv == nLv);
    }

    public int GetF_Wall_MaxLv()
    {
        ST_F_Wall stWall = m_lstF_Wall[m_lstF_Wall.Count - 1];
        GameCommon.CHECK(stWall != null);
        return stWall.nLv;
    }
}