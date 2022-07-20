using UnityEngine;

public class buildingSnapPoint : MonoBehaviour
{
   public int id;
   //0 = foundation snap
   //1 = ceiling/top of wall snap

   void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(1, 1, 1));
    }
}
