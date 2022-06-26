using UnityEngine;
using Mirror;

public class sunManager : NetworkBehaviour
{
    [SyncVar]
    public double sunAngle;
    public float speed;
    public Gradient day;
    public Color night;
    Light sun;
    void Start()
    {
        sun = GetComponent<Light>();
    }
    void FixedUpdate()
    {
        //Assign reflection brightness
        RenderSettings.reflectionIntensity = RenderSettings.ambientLight.r;
        //Calculate sun angle from server time
        sunAngle = (double)NetworkTime.time * speed;
        float val = (float)sunAngle;
        float normal = Mathf.Sin(Mathf.Deg2Rad * (val % 360));
        //Apply intensity
        sun.intensity = Mathf.Clamp(normal * 4f, 0, 2f);
        //Apply ambient lighting
        if (normal > 0)
        {
            RenderSettings.ambientLight = day.Evaluate(Mathf.Clamp01(normal));
        }
        else
        {
            RenderSettings.ambientLight = night;
        }
        //Rotate sun
        transform.rotation = Quaternion.Euler(val, -30, 0);
    }
}
