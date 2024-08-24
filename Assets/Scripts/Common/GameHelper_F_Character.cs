using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EM_F_CharacterType
{
    Invalid = -1,
    
    F_Primitiveman,
    F_Villager,
    F_Hammerman,
    F_Bowman,
    F_Farmer,
    F_NearWarriorA,
    F_Knight,
    F_Rabbit,

    Max,
};

public enum EM_F_CharacterAttr
{
    Invalid = -1,

    Hp,//血量
    HitRate,//命中率
    Damage,//伤害(攻击力)
    DamageDelay,//伤害延迟(攻击间隔)
    Income,//收益(种田、砍树)
    RecoverHp,//回血量(建筑修理，兵种回血)
    WalkSpeed,//行走速度
    RunSpeed,//奔跑速度

    Max,
};

static public class GameHelper_F_Character
{
    static public string GetCharacterModelName(
          EM_F_CharacterType emType,
          bool bIncludePath = true
          )
    {
        switch (emType)
        {
            case EM_F_CharacterType.F_Primitiveman:
                {
                    if (bIncludePath)
                    {
                        return string.Format("Prefabs/Characters/F_PrimitivemanCharacter");
                    }
                    else
                    {
                        return string.Format("F_PrimitivemanCharacter");
                    }
                }
            case EM_F_CharacterType.F_Villager:
                {
                    if (bIncludePath)
                    {
                        return string.Format("Prefabs/Characters/F_VillagerCharacter");
                    }
                    else
                    {
                        return string.Format("F_VillagerCharacter");
                    }
                }
            case EM_F_CharacterType.F_Hammerman:
                {
                    if (bIncludePath)
                    {
                        return string.Format("Prefabs/Characters/F_HammermanCharacter");
                    }
                    else
                    {
                        return string.Format("F_HammermanCharacter");
                    }
                }
            case EM_F_CharacterType.F_Bowman:
                {
                    if (bIncludePath)
                    {
                        return string.Format("Prefabs/Characters/F_BowmanCharacter");
                    }
                    else
                    {
                        return string.Format("F_BowmanCharacter");
                    }
                }
            case EM_F_CharacterType.F_Farmer:
                {
                    if (bIncludePath)
                    {
                        return string.Format("Prefabs/Characters/F_FarmerCharacter");
                    }
                    else
                    {
                        return string.Format("F_FarmerCharacter");
                    }
                }
            case EM_F_CharacterType.F_NearWarriorA:
                {
                    if (bIncludePath)
                    {
                        return string.Format("Prefabs/Characters/F_NearWarriorACharacter");
                    }
                    else
                    {
                        return string.Format("F_NearWarriorACharacter");
                    }
                }
            case EM_F_CharacterType.F_Knight:
                {
                    if (bIncludePath)
                    {
                        return string.Format("Prefabs/Characters/F_KnightCharacter");
                    }
                    else
                    {
                        return string.Format("F_KnightCharacter");
                    }
                }
            case EM_F_CharacterType.F_Rabbit:
                {
                    if (bIncludePath)
                    {
                        return string.Format("Prefabs/Characters/F_RabbitCharacter");
                    }
                    else
                    {
                        return string.Format("F_RabbitCharacter");
                    }
                }
            default:
                {
                    Debug.LogError("GetCharacterModelName Failed : " + emType.ToString());
                    return null;
                }
        }
    }

    static public T InstantiateCharacters<T>(
        EM_F_CharacterType emType,
        int nOnlyId,
        Transform trParent,
        Vector3 v3LocalPosition,
        Quaternion qtRotation,
        Vector3 v3LocalScale
        )
    {
        GameCommon.CHECK(emType > EM_F_CharacterType.Invalid && emType < EM_F_CharacterType.Max);

        string strLoadPath = GameHelper_F_Character.GetCharacterModelName(emType);
        UnityEngine.Object objSrc = Resources.Load(strLoadPath);
        GameCommon.CHECK(objSrc != null, "Resources.Load Failed: " + strLoadPath);
        GameObject goItem = (GameObject)UnityEngine.Object.Instantiate(objSrc);
        GameCommon.CHECK(goItem != null);
        goItem.name = emType.ToString() + ":" + nOnlyId.ToString("00");
        goItem.transform.SetParent(trParent);
        goItem.transform.localPosition = v3LocalPosition;
        goItem.transform.rotation = qtRotation;
        goItem.transform.localScale = v3LocalScale;
        return goItem.GetComponent<T>();
    }
}