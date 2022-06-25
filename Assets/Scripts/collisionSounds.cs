using UnityEngine;
[RequireComponent(typeof(reloadAudio))]
public class collisionSounds : MonoBehaviour
{
    public AudioClip[] sounds;
    public bool oneShot;
    bool shot = false;

    void OnCollisionEnter(Collision col)
    {
        
        if(GetComponent<Rigidbody>().velocity.magnitude > 0.1f) {
        if (oneShot && shot == false)
        {
            Debug.Log("COLLISIOn");
            GetComponent<reloadAudio>().PlayRandomReloadAudio(sounds);
            shot = true;
        }
        }
    }
}
