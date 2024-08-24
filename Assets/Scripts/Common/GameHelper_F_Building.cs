using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum EM_F_BuildingType
{
    Invalid = -1,

    F_Homeland,
    F_PrimitivemanFactory,
    F_HammermanFactory,
    F_HammermanTotem,
    F_BowmanFactory,
    F_BowmanTotem,
    F_BowmanTower,
    F_FarmerFactory,
    F_FarmerTotem,
    F_Farmland,
    F_NearWarriorAFactory,
    F_NearWarriorATotem,
    F_KnightFactory,
    F_KnightTotem,
    F_Wall,
    F_Tree,
    F_RabbitFactory,
    
    Max,
};



static public class GameHelper_F_Building
{
    static public string GetBuildingModelName(
        EM_F_BuildingType emType, 
        int nLevel,
        bool bIncludePath = true
        )
    {
        GameCommon.CHECK(nLevel >= 0);

        switch (emType)
        {
            case EM_F_BuildingType.F_Homeland:
                {
                    if (bIncludePath)
                    {
                        return string.Format("Prefabs/BuildingModels/F_HomelandModel_Lev{0}", nLevel);
                    }
                    else
                    {
                        return string.Format("F_HomelandModel_Lev{0}", nLevel);
                    }
                }
            case EM_F_BuildingType.F_PrimitivemanFactory:
                {
                    if (bIncludePath)
                    {
                        return string.Format("Prefabs/BuildingModels/F_PrimitivemanFactoryModel_Lev{0}", nLevel);
                    }
                    else
                    {
                        return string.Format("F_PrimitivemanFactoryModel_Lev{0}", nLevel);
                    }
                }
            case EM_F_BuildingType.F_HammermanFactory:
                {
                    if (bIncludePath)
                    {
                        return string.Format("Prefabs/BuildingModels/F_HammermanFactoryModel_Lev{0}", nLevel);
                    }
                    else
                    {
                        return string.Format("F_HammermanFactoryModel_Lev{0}", nLevel);
                    }
                }
            case EM_F_BuildingType.F_HammermanTotem:
                {
                    if (bIncludePath)
                    {
                        return string.Format("Prefabs/BuildingModels/F_HammermanTotemModel_Lev{0}", nLevel);
                    }
                    else
                    {
                        return string.Format("F_HammermanTotemModel_Lev{0}", nLevel);
                    }
                }
            case EM_F_BuildingType.F_BowmanFactory:
                {
                    if (bIncludePath)
                    {
                        return string.Format("Prefabs/BuildingModels/F_BowmanFactoryModel_Lev{0}", nLevel);
                    }
                    else
                    {
                        return string.Format("F_BowmanFactoryModel_Lev{0}", nLevel);
                    }
                }
            case EM_F_BuildingType.F_BowmanTotem:
                {
                    if (bIncludePath)
                    {
                        return string.Format("Prefabs/BuildingModels/F_BowmanTotemModel_Lev{0}", nLevel);
                    }
                    else
                    {
                        return string.Format("F_BowmanTotemModel_Lev{0}", nLevel);
                    }
                }
            case EM_F_BuildingType.F_BowmanTower:
                {
                    if (bIncludePath)
                    {
                        return string.Format("Prefabs/BuildingModels/F_BowmanTowerModel_Lev{0}", nLevel);
                    }
                    else
                    {
                        return string.Format("F_BowmanTowerModel_Lev{0}", nLevel);
                    }
                }
            case EM_F_BuildingType.F_FarmerFactory:
                {
                    if (bIncludePath)
                    {
                        return string.Format("Prefabs/BuildingModels/F_FarmerFactoryModel_Lev{0}", nLevel);
                    }
                    else
                    {
                        return string.Format("F_FarmerFactoryModel_Lev{0}", nLevel);
                    }
                }
            case EM_F_BuildingType.F_FarmerTotem:
                {
                    if (bIncludePath)
                    {
                        return string.Format("Prefabs/BuildingModels/F_FarmerTotemModel_Lev{0}", nLevel);
                    }
                    else
                    {
                        return string.Format("F_FarmerTotemModel_Lev{0}", nLevel);
                    }
                }
            case EM_F_BuildingType.F_Farmland:
                {
                    if (bIncludePath)
                    {
                        return string.Format("Prefabs/BuildingModels/F_FarmlandModel_Lev{0}", nLevel);
                    }
                    else
                    {
                        return string.Format("F_FarmlandModel_Lev{0}", nLevel);
                    }
                }
            case EM_F_BuildingType.F_NearWarriorAFactory:
                {
                    if (bIncludePath)
                    {
                        return string.Format("Prefabs/BuildingModels/F_NearWarriorAFactoryModel_Lev{0}", nLevel);
                    }
                    else
                    {
                        return string.Format("F_NearWarriorAFactoryModel_Lev{0}", nLevel);
                    }
                }
            case EM_F_BuildingType.F_NearWarriorATotem:
                {
                    if (bIncludePath)
                    {
                        return string.Format("Prefabs/BuildingModels/F_NearWarriorATotemModel_Lev{0}", nLevel);
                    }
                    else
                    {
                        return string.Format("F_NearWarriorATotemModel_Lev{0}", nLevel);
                    }
                }
            case EM_F_BuildingType.F_KnightFactory:
                {
                    if (bIncludePath)
                    {
                        return string.Format("Prefabs/BuildingModels/F_KnightFactoryModel_Lev{0}", nLevel);
                    }
                    else
                    {
                        return string.Format("F_KnightFactoryModel_Lev{0}", nLevel);
                    }
                }
            case EM_F_BuildingType.F_KnightTotem:
                {
                    if (bIncludePath)
                    {
                        return string.Format("Prefabs/BuildingModels/F_KnightTotemModel_Lev{0}", nLevel);
                    }
                    else
                    {
                        return string.Format("F_KnightTotemModel_Lev{0}", nLevel);
                    }
                }
            case EM_F_BuildingType.F_Wall:
                {
                    if (bIncludePath)
                    {
                        return string.Format("Prefabs/BuildingModels/F_WallModel_Lev{0}", nLevel);
                    }
                    else
                    {
                        return string.Format("F_WallModel_Lev{0}", nLevel);
                    }
                }
            case EM_F_BuildingType.F_Tree:
                {
                    if (bIncludePath)
                    {
                        return string.Format("Prefabs/BuildingModels/F_TreeModel_Lev{0}", nLevel);
                    }
                    else
                    {
                        return string.Format("F_TreeModel_Lev{0}", nLevel);
                    }
                }
            case EM_F_BuildingType.F_RabbitFactory:
                {
                    if (bIncludePath)
                    {
                        return string.Format("Prefabs/BuildingModels/F_RabbitFactoryModel_Lev{0}", nLevel);
                    }
                    else
                    {
                        return string.Format("F_RabbitFactoryModel_Lev{0}", nLevel);
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