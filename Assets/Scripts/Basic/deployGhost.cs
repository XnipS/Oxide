using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deployGhost : MonoBehaviour
{
    public LayerMask mask;

    void Start () {
        if(!GetComponent<Collider>().isTrigger) {
            Debug.LogError("Ghost collider is not a trigger!");
        }
    }

    public bool SpaceTestCanFit()
    {
        if (Test())
        {
            SetAllMaterialColours(GetComponentInChildren<MeshRenderer>(), Color.blue);
            return true;
        }
        else
        {
            SetAllMaterialColours(GetComponentInChildren<MeshRenderer>(), Color.red);

            return false;
        }
    }

    static void SetAllMaterialColours (MeshRenderer ren, Color color) {
        foreach(Material mat in ren.materials) {
            mat.color = color;
        }
    }

    public bool Test()
    {
        bool angle = Vector3.Angle(Vector3.up, transform.up) < 25f;
        if (GetComponent<CapsuleCollider>() && angle)
        {
            CapsuleCollider col = GetComponent<CapsuleCollider>();

            var direction = new Vector3 { [col.direction] = 1 };
            var offset = col.height / 2 - col.radius;
            var localPoint0 = col.center - direction * offset;
            var localPoint1 = col.center + direction * offset;

            var point0 = transform.TransformPoint(localPoint0);
            var point1 = transform.TransformPoint(localPoint1);

            Collider[] hits = Physics.OverlapCapsule(point0, point1, col.radius, mask);
            return hits.Length < 1;
        }
        if (GetComponent<BoxCollider>() && angle)
        {
            BoxCollider col = GetComponent<BoxCollider>();


            Collider[] hits = Physics.OverlapBox(transform.TransformPoint(col.center),col.size / 2, transform.rotation, mask);
            return hits.Length < 1;
        }
        return false;
    }

}
