using UnityEngine;

public class viewmodelSway : MonoBehaviour
{
    public float sensitivity;
    public float lerp;
    ui_inventory inv;

    void Start()
    {
        inv = FindObjectOfType<ui_inventory>();
    }
    void LateUpdate()
    {
        //Check if inv open
        if (inv.inventoryStatus) { return; }
        //Get input
        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X") * sensitivity, Input.GetAxisRaw("Mouse Y") * sensitivity);
        //Calculate
        Quaternion target = (Quaternion.AngleAxis(-mouseInput.y, transform.right)) * (Quaternion.AngleAxis(mouseInput.x, transform.up));
        //Apply
        transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * lerp);
    }
}
