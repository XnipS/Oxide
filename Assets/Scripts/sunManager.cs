using UnityEngine;
using Mirror;

public class sunManager : NetworkBehaviour
{
    [SyncVar]
    public double sunAngle;
    public float speed;
    public Gradient day;
    public Color night;
    void FixedUpdate()
    {
        sunAngle = (double)NetworkTime.time * speed;
        float val = (float)sunAngle;
        float normal = Mathf.Sin(Mathf.Deg2Rad * (val % 360));
        //Debug.Log(normal);
        GetComponent<Light>().intensity = Mathf.Clamp(normal * 4f, 0, 2f);
        if (normal > 0)
        {
            RenderSettings.ambientLight = day.Evaluate(Mathf.Clamp01(normal));
        }
        else
        {
            RenderSettings.ambientLight = night;
        }
        // Debug.Log(GetComponent<Light>().intensity);
        transform.rotation = Quaternion.Euler(val, -30, 0);
    }
}
