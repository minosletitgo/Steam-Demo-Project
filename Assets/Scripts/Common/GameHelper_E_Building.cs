using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EM_E_BuildingType
{
    Invalid = -1,

    /*
        目前的设定就是【怪物神庙】
        01.在夜晚-【EverydayEnemy.txt - InNight】
        01.在白天-【EverydayEnemy.txt - InDay】，初步暂定【主角方靠近后，按一定CD刷出，AI有回巢表现等】
    */
    E_MonsterFactory,

    Max,
};

static public class GameHelper_E_Building
{
    static public string GetBuildingModelName(
        EM_E_BuildingType emType,
        int nLevel,
        bool bIncludePath = true
        )
    {
        GameCommon.CHECK(nLevel >= 0);

        switch (emType)
        {
            case EM_E_BuildingType.E_MonsterFactory:
                {
                    if (bIncludePath)
                    {
                        return string.Format("Prefabs/BuildingModels/E_MonsterFactoryModel_Lev{0}", nLevel);
                    }
                    else
                    {
                        return string.Format("E_MonsterFactoryModel_Lev{0}", nLevel);
                    }
                }
            default:
                {
                    Debug.LogError("GetBuildingModelName Failed : " + emType.ToString());
                    return null;
                }
        }
    }
}