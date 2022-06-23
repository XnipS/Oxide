using UnityEngine;
using UnityEngine.Rendering.Universal;
public class overlayCamera : MonoBehaviour
{
    void Start()
    {
        var cameraData = Camera.main.GetUniversalAdditionalCameraData();
        cameraData.cameraStack.Add(GetComponent<Camera>());
    }

}
