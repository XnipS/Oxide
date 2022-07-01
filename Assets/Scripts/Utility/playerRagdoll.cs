using System.Collections;
using UnityEngine;
using Cinemachine;


public class playerRagdoll : MonoBehaviour
{
    public CinemachineVirtualCamera cam;

    void Start()
    {
        StartCoroutine(DeathCam());
    }

    IEnumerator DeathCam()
    {
        cam.m_Priority = 5;
        yield return new WaitForSeconds(2f);
        cam.m_Priority = -1;
    }
}
