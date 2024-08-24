using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;

public class Minos_GUI_StartScreen : MonoBehaviour
{
    [SerializeField]
    string m_strDungeonSceneName = "";

    [SerializeField]
    Button m_btnNewGame;






    private void Awake()
    {
        GameCommon.CHECK(!string.IsNullOrWhiteSpace(m_strDungeonSceneName));
        m_btnNewGame.onClick.AddListener(OnClick_NewGame);
    }

    void OnClick_NewGame()
    {
        LoadingSceneManager.LoadScene(m_strDungeonSceneName);
    }
}
