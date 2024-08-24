using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;

public class Minos_GUI_PauseWindow : MonoBehaviour
{
    [SerializeField]
    Button m_btnResume;


    private void Awake()
    {
        m_btnResume.onClick.AddListener(OnClick_Resume);
    }

    void OnClick_Resume()
    {
        TopDownEngineEvent.Trigger(TopDownEngineEventTypes.Pause, null);
    }
}
