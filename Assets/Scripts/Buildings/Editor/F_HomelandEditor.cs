#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;



[CustomEditor(typeof(F_Homeland), true)]
[InitializeOnLoad]
public class F_HomelandEditor : IBase_Friend_BuildingEditor
{
    /// <summary>
    /// OnSceneGUI, draws repositionable handles at every point in the path, for easier setup
    /// </summary>
    protected override void OnSceneGUI()
    {
        base.OnSceneGUI();

        Handles.color = Color.green;
        F_Homeland stHost = (target as F_Homeland);

        //if (t.GetOriginalTransformPositionStatus() == false)
        //{
        //    return;
        //}

        for (int i = 0; i < stHost.m_lstPathDefendLeft.Count; i++)
        {
            EditorGUI.BeginChangeCheck();

            Vector3 oldPoint = stHost.transform.position + stHost.m_lstPathDefendLeft[i].PathElementPosition;
            GUIStyle style = new GUIStyle();

            // draws the path item number
            style.normal.textColor = Color.yellow;
            Handles.Label(stHost.transform.position + stHost.m_lstPathDefendLeft[i].PathElementPosition + (Vector3.down * 0.4f) + (Vector3.right * 0.4f), "" + i, style);

            // draws a movable handle
            Vector3 newPoint = Handles.FreeMoveHandle(oldPoint, Quaternion.identity, .5f, new Vector3(.25f, .25f, .25f), Handles.CircleHandleCap);

            // records changes
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Free Move Handle");
                stHost.m_lstPathDefendLeft[i].PathElementPosition = newPoint - stHost.transform.position;
                stHost.m_lstPathDefendLeft[i].PathElementPosition.y = 0;
            }
        }

        for (int i = 0; i < stHost.m_lstPathDefendRight.Count; i++)
        {
            EditorGUI.BeginChangeCheck();

            Vector3 oldPoint = stHost.transform.position + stHost.m_lstPathDefendRight[i].PathElementPosition;
            GUIStyle style = new GUIStyle();

            // draws the path item number
            style.normal.textColor = Color.yellow;
            Handles.Label(stHost.transform.position + stHost.m_lstPathDefendRight[i].PathElementPosition + (Vector3.down * 0.4f) + (Vector3.right * 0.4f), "" + i, style);

            // draws a movable handle
            Vector3 newPoint = Handles.FreeMoveHandle(oldPoint, Quaternion.identity, .5f, new Vector3(.25f, .25f, .25f), Handles.CircleHandleCap);

            // records changes
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Free Move Handle");
                stHost.m_lstPathDefendRight[i].PathElementPosition = newPoint - stHost.transform.position;
                stHost.m_lstPathDefendRight[i].PathElementPosition.y = 0;
            }
        }
    }
}

#endif