using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mirror;
//[AddComponentMenu("Camera-Control/Smooth Mouse Look")]
public class smoothMouseLook : MonoBehaviour
{
    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    [Header("Basic Settings")]
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 1F;
    public float sensitivityY = 1F;
    [Header("Restrictions")]
    public float minimumX = -360F;
    public float maximumX = 360F;
    public float minimumY = -90F;
    public float maximumY = 90F;
    [Header("Read Only")]
    public float rotationX = 0F;
    public float rotationY = 0F;

    float rotX = 0F;
    float rotY = 0F;

    Quaternion originalRotation;

    private void LateUpdate()
    {

    }
    void Update()
    {
        if (GetComponent<NetworkIdentity>() != null)
        {
            if (!GetComponent<NetworkIdentity>().hasAuthority) { return; }
        }
        if (GetComponentInParent<NetworkIdentity>() != null)
        {
            if (!GetComponentInParent<NetworkIdentity>().hasAuthority) { return; }
        }


        if (axes == RotationAxes.MouseXAndY)
        {
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationX += Input.GetAxis("Mouse X") * sensitivityX;

            rotationY = ClampAngle(rotationY, minimumY, maximumY);
            rotationX = ClampAngle(rotationX, minimumX, maximumX);

            Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);
            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);

            transform.localRotation = originalRotation * xQuaternion * yQuaternion;
        }
        else if (axes == RotationAxes.MouseX)
        {
            rotationX += Input.GetAxis("Mouse X") * sensitivityX;

            rotationX = ClampAngle(rotationX, minimumX, maximumX);

            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            transform.localRotation = originalRotation * xQuaternion;
        }
        else
        {
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

            rotationY = ClampAngle(rotationY, minimumY, maximumY);

            Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);
            transform.localRotation = originalRotation * yQuaternion;
        }
        //rotationX = Mathf.Clamp(rotationX, minimumX, maximumX);        
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb)
            rb.freezeRotation = true;
        originalRotation = transform.localRotation;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        angle = angle % 360;
        if ((angle >= -360F) && (angle <= 360F))
        {
            if (angle < -360F)
            {
                angle += 360F;
            }
            if (angle > 360F)
            {
                angle -= 360F;
            }
        }
        return Mathf.Clamp(angle, min, max);
    }
}
