using UnityEngine;

public class ui_notifyManager : MonoBehaviour
{
    public Vector3 posToSpawn;
    public GameObject prefab;

    public void Notify(string msg, ui_notification.NotifyColourType colourType, ui_notification.NotifyIconType iconType)
    {
        foreach (ui_notification not in FindObjectsOfType<ui_notification>())
        {
            not.transform.position += Vector3.up * 35f * GetComponentInParent<Canvas>().scaleFactor;
        }
        GameObject gam = Instantiate(prefab, transform);
        gam.GetComponent<ui_notification>().Setup(msg, colourType, iconType);
    }
}
