using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingColliderTrigger: MonoBehaviour
{
    public delegate void OnTriggerCollider(Collider other);
    public OnTriggerCollider m_dgOnTriggerEnter;
    public OnTriggerCollider m_dgOnTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        if (m_dgOnTriggerEnter != null)
        {
            m_dgOnTriggerEnter(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (m_dgOnTriggerExit != null)
        {
            m_dgOnTriggerExit(other);
        }
    }
}