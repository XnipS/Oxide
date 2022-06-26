using UnityEngine;
[RequireComponent(typeof(reloadAudio))]
public class playReloadAudio : MonoBehaviour
{
    public AudioClip clip;
    void Start()
    {
        GetComponent<reloadAudio>().PlayReloadAudio(clip);
    }
}