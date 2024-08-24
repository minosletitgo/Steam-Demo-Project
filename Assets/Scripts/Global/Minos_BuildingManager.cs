using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minos_BuildingManager : GameHelper.Singleton<Minos_BuildingManager>
{
    //我方
    F_Homeland m_stPlayerHomeland;
    List<IBase_Friend_Building> m_lstAll_F_Building = new List<IBase_Friend_Building>();

    //敌方
    List<IBase_Enemy_Building> m_lstAll_E_Building = new List<IBase_Enemy_Building>();




    public Minos_BuildingManager()
    {
        
    }


    #region******************************我方**************************************
    public void Push_F_BuildingAtAwake(IBase_Friend_Building _stBuilding)
    {
        GameCommon.CHECK(_stBuilding != null);
        m_lstAll_F_Building.Add(_stBuilding);

        switch(_stBuilding.GetBuildingType())
        {
            case EM_F_BuildingType.F_Homeland:
                {
                    GameCommon.CHECK(m_stPlayerHomeland == null);
                    m_stPlayerHomeland = _stBuilding as F_Homeland;
                    GameCommon.CHECK(m_stPlayerHomeland != null);
                }
                break;
        }
    }

    public IEnumerable<IBase_Friend_Building> EnumAll_F_Building()
    {
        foreach (IBase_Friend_Building _stBuilding in m_lstAll_F_Building)
        {
            yield return _stBuilding;
        }
    }

    public F_Homeland GetPlayerHomeland() { return m_stPlayerHomeland; }

    public F_Wall Get_F_DefendLeftWall()
    {
        /*
            找出当前“左防守”的Wall，即最外围“基地左侧”的“已修建的Wall”   
            01.要么返回“距离最远Lv>0的墙”，要么返回"距离最近的Lv=0的墙"
            02.优先返回前者
        */

        float fDis_Lv = float.MinValue;
        F_Wall stRet_Lv = null;

        float fDis_LvZero = float.MaxValue;
        F_Wall stRet_LvZero = null;

        foreach (IBase_Friend_Building _stBuilding in m_lstAll_F_Building)
        {
            if (_stBuilding.GetBuildingType() != EM_F_BuildingType.F_Wall)
            {
                continue;
            }

            F_Wall _stWall = _stBuilding as F_Wall;
            GameCommon.CHECK(_stWall != null);

            if (m_stPlayerHomeland.transform.position.x < _stBuilding.transform.position.x)
            {
                continue;
            }

            float fDisTmp = Vector2.Distance(
                new Vector2(m_stPlayerHomeland.transform.position.x, m_stPlayerHomeland.transform.position.z),
                new Vector2(_stBuilding.transform.position.x, _stBuilding.transform.position.z)
                );

            if (_stWall.GetCurLev() > 0)
            {
                if (fDisTmp >= fDis_Lv)
                {
                    fDis_Lv = fDisTmp;
                    stRet_Lv = _stWall;
                }
            }
            else
            {
                if (fDisTmp <= fDis_LvZero)
                {
                    fDis_LvZero = fDisTmp;
                    stRet_LvZero = _stWall;
                }
            }
        }
        
        if (stRet_Lv != null)
        {
            return stRet_Lv;
        }

        GameCommon.CHECK(stRet_LvZero != null);
        return stRet_LvZero;
    }

    public F_Wall Get_F_DefendRightWall()
    {
        /*
            找出当前“右防守”的Wall，即最外围“基地右侧”的“已修建的Wall”   
            01.要么返回“距离最远Lv>0的墙”，要么返回"距离最近的Lv=0的墙"
            02.优先返回前者
        */

        float fDis_Lv = float.MinValue;
        F_Wall stRet_Lv = null;

        float fDis_LvZero = float.MaxValue;
        F_Wall stRet_LvZero = null;

        foreach (IBase_Friend_Building _stBuilding in m_lstAll_F_Building)
        {
            if (_stBuilding.GetBuildingType() != EM_F_BuildingType.F_Wall)
            {
                continue;
            }

            F_Wall _stWall = _stBuilding as F_Wall;
            GameCommon.CHECK(_stWall != null);

            if (m_stPlayerHomeland.transform.position.x > _stBuilding.transform.position.x)
            {
                continue;
            }

            float fDisTmp = Vector2.Distance(
                new Vector2(m_stPlayerHomeland.transform.position.x, m_stPlayerHomeland.transform.position.z),
                new Vector2(_stBuilding.transform.position.x, _stBuilding.transform.position.z)
                );

            if (_stWall.GetCurLev() > 0)
            {
                if (fDisTmp >= fDis_Lv)
                {
                    fDis_Lv = fDisTmp;
                    stRet_Lv = _stWall;
                }
            }
            else
            {
                if (fDisTmp <= fDis_LvZero)
                {
                    fDis_LvZero = fDisTmp;
                    stRet_LvZero = _stWall;
                }
            }
        }

        if (stRet_Lv != null)
        {
            return stRet_Lv;
        }

        GameCommon.CHECK(stRet_LvZero != null);
        return stRet_LvZero;
    }
    #endregion******************************我方**************************************











    #region******************************敌方**************************************
    public void Push_E_BuildingAtAwake(IBase_Enemy_Building _stBuilding)
    {
        GameCommon.CHECK(_stBuilding != null);
        m_lstAll_E_Building.Add(_stBuilding);

        switch (_stBuilding.GetBuildingType())
        {
            case EM_E_BuildingType.E_MonsterFactory:
                {
                    E_MonsterFactory stFactory = _stBuilding as E_MonsterFactory;
                    GameCommon.CHECK(stFactory != null);
                }
                break;
        }
    }

    public IEnumerable<IBase_Enemy_Building> EnumAll_E_Building()
    {
        foreach (IBase_Enemy_Building _stBuilding in m_lstAll_E_Building)
        {
            yield return _stBuilding;
        }
    }
    #endregion******************************敌方**************************************

}