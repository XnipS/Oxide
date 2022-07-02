using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR
public class wireController : MonoBehaviour
{
    public LineRenderer[] wire;
    public wireController target;
    
    public void UpdateWires () {
        for (int i = 0; i < target.wire.Length; i++)
        {
            Vector3 local = target.wire[i].GetPosition(0);
            Vector3 world = target.wire[i].transform.TransformPoint(local);
            Vector3 myLocal = wire[i].transform.InverseTransformPoint(world);
            wire[i].SetPosition(1, myLocal);
        }
    }
}
[CustomEditor(typeof(wireController))]
public class wireController_editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        wireController man = (wireController)target;
        if (GUILayout.Button("Apply Wires"))
        {
            man.UpdateWires();

        }
    }
}
#endif