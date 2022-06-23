using UnityEngine;
using Mirror;

public class sunManager : NetworkBehaviour
{
    [SyncVar]
    public double sunAngle;
    public float speed;
    void FixedUpdate()
    {
        sunAngle = (double)NetworkTime.time * speed;
        float val = (float)sunAngle;
        Debug.LogError(Mathf.Sin(Mathf.Deg2Rad * 360f));
        Debug.Log(Mathf.Sin(Mathf.Deg2Rad * (val * speed)));
        GetComponent<Light>().intensity = Mathf.Clamp(Mathf.Deg2Rad * (val * speed),0f,1f) * 1f;
        transform.rotation = Quaternion.Euler(val, -30, 0);//Quaternion.Lerp(transform.rotation, Quaternion.Euler(val, -30, 0), Time.fixedDeltaTime * 10f);
    }
}
