using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class GameCommon
{
    static public bool CHECK(bool bCheck, string strErro = null)
    {
        if (!bCheck)
        {
            throw new SystemException("[CHECK]: " + strErro);
        }

        return bCheck;
    }
}
