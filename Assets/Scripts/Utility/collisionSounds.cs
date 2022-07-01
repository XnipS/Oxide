using UnityEngine;
[RequireComponent(typeof(reloadAudio))]
public class collisionSounds : MonoBehaviour
{
    public AudioClip[] sounds;
    public bool oneShot;
    bool shot = false;

    void OnCollisionEnter(Collision col)
    {
        //Check if moving
        if (GetComponent<Rigidbody>().velocity.magnitude > 0.1f)
        {
            if (oneShot && shot == false)
            {
                //Play sound
                GetComponent<reloadAudio>().PlayRandomReloadAudio(sounds);
                shot = true;
            }
        }
    }
}
