using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minos_3DGUI_BuildingMoneyCoinBar : MonoBehaviour
{
    [SerializeField]
    GameObject m_goGrid;
    [SerializeField]
    Image m_imgInstMoneyCoin;

    List<Image> m_lstMoneyCoin = new List<Image>();




    private void Awake()
    {
        m_imgInstMoneyCoin.gameObject.SetActive(false);
    }

    public void Show(int nMoneyCoinCount)
    {
        GameCommon.CHECK(nMoneyCoinCount > 0);

        gameObject.SetActive(true);

        foreach(Image _img in m_lstMoneyCoin)
        {
            Destroy(_img.gameObject);
        }
        m_lstMoneyCoin.Clear();

        for (int i = 0; i < nMoneyCoinCount; i++)
        {
            GameObject goImg = (GameObject)Instantiate(m_imgInstMoneyCoin.gameObject);
            GameCommon.CHECK(goImg != null);
            goImg.SetActive(true);
            Image img = goImg.GetComponent<Image>();
            GameCommon.CHECK(img != null);
            img.transform.SetParent(m_goGrid.transform);
            img.transform.localPosition = Vector3.zero;
            img.transform.rotation = Quaternion.identity;
            img.transform.localScale = Vector3.one;
            img.color = new Color32(103, 103, 103, 255);

            m_lstMoneyCoin.Add(img);
        }
    }

    public void UnShow()
    {
        gameObject.SetActive(false);
    }

    public bool IsShow() { return gameObject.activeSelf; }

    public int GetMoneyCoinCount() { return m_lstMoneyCoin.Count; }

    public Vector3 GetMoneyCoinPosition(int nIndex)
    {
        GameCommon.CHECK(nIndex >= 0 && nIndex < m_lstMoneyCoin.Count, "GetMoneyCoinPosition Index = " + nIndex);
        return m_lstMoneyCoin[nIndex].transform.position;
    }

    public void SetFillMoneyCoin(int nIndex, bool bIsFill = true)
    {
        GameCommon.CHECK(nIndex >= 0 && nIndex < m_lstMoneyCoin.Count, "GetMoneyCoinPosition Index = " + nIndex);

        if (bIsFill)
        {
            m_lstMoneyCoin[nIndex].color = new Color32(255, 255, 0, 255);
        }
        else
        {
            m_lstMoneyCoin[nIndex].color = new Color32(103, 103, 103, 255);
        }
    }
}