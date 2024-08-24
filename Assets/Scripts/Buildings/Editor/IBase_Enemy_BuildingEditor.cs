#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;



[CustomEditor(typeof(IBase_Enemy_Building), true)]
[InitializeOnLoad]
public class IBase_Enemy_BuildingEditor : Editor
{
    /// <summary>
    /// OnSceneGUI, draws repositionable handles at every point in the path, for easier setup
    /// </summary>
    protected virtual void OnSceneGUI()
    {
        Handles.color = Color.green;
        IBase_Enemy_Building stHost = (target as IBase_Enemy_Building);

        //if (t.GetOriginalTransformPositionStatus() == false)
        //{
        //    return;
        //}

        for (int i = 0; i < stHost.m_lstPathAroundSelf.Count; i++)
        {
            EditorGUI.BeginChangeCheck();

            Vector3 oldPoint = stHost.transform.position + stHost.m_lstPathAroundSelf[i].PathElementPosition;
            GUIStyle style = new GUIStyle();

            // draws the path item number
            style.normal.textColor = Color.yellow;
            Handles.Label(stHost.transform.position + stHost.m_lstPathAroundSelf[i].PathElementPosition + (Vector3.down * 0.4f) + (Vector3.right * 0.4f), "" + i, style);

            // draws a movable handle
            Vector3 newPoint = Handles.FreeMoveHandle(oldPoint, Quaternion.identity, .5f, new Vector3(.25f, .25f, .25f), Handles.CircleHandleCap);

            // records changes
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Free Move Handle");
                stHost.m_lstPathAroundSelf[i].PathElementPosition = newPoint - stHost.transform.position;
                stHost.m_lstPathAroundSelf[i].PathElementPosition.y = 0;
            }
        }
    }
}

#endif