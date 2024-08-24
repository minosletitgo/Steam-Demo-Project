using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EM_E_CharacterType
{
    Invalid = -1,

    E_MonsterNearA,
    E_MonsterNearB,
    E_MonsterFarA,
    E_MonsterFarB,
    E_BossNearA,
    E_BossNearB,
    E_BossFar,

    Max,
};

public enum EM_E_CharacterAttr
{
    Invalid = -1,

    Hp,//血量
    Damage,//伤害(攻击力)
    DamageDelay,//伤害延迟(攻击间隔)
    WalkSpeed,//行走速度
    RunSpeed,//奔跑速度

    Max,
};


static public class GameHelper_E_Character
{
    static public string GetCharacterModelName(
          EM_E_CharacterType emType,
          bool bIncludePath = true
          )
    {
        switch (emType)
        {
            case EM_E_CharacterType.E_MonsterNearA:
                {
                    if (bIncludePath)
                    {
                        return string.Format("Prefabs/Characters/E_MonsterNearACharacter");
                    }
                    else
                    {
                        return string.Format("E_MonsterNearACharacter");
                    }
                }
            case EM_E_CharacterType.E_MonsterNearB:
                {
                    if (bIncludePath)
                    {
                        return string.Format("Prefabs/Characters/E_MonsterNearBCharacter");
                    }
                    else
                    {
                        return string.Format("E_MonsterNearBCharacter");
                    }
                }
            case EM_E_CharacterType.E_BossNearA:
                {
                    if (bIncludePath)
                    {
                        return string.Format("Prefabs/Characters/E_BossNearACharacter");
                    }
                    else
                    {
                        return string.Format("E_BossNearACharacter");
                    }
                }
            case EM_E_CharacterType.E_BossNearB:
                {
                    if (bIncludePath)
                    {
                        return string.Format("Prefabs/Characters/E_BossNearBCharacter");
                    }
                    else
                    {
                        return string.Format("E_BossNearBCharacter");
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
        EM_E_CharacterType emType,
        int nOnlyId,
        Transform trParent,
        Vector3 v3LocalPosition,
        Quaternion qtRotation,
        Vector3 v3LocalScale
        )
    {
        GameCommon.CHECK(emType > EM_E_CharacterType.Invalid && emType < EM_E_CharacterType.Max);

        string strLoadPath = GameHelper_E_Character.GetCharacterModelName(emType);
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