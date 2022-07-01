using UnityEngine;
using Mirror;

public class sunManager : MonoBehaviour
{
    public float totalLength;
    public float timeSpeed;
    public float nightLength;
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
        double timeOfDay = ((double)NetworkTime.time * timeSpeed) % totalLength;//Value between 0 and total length
        double sunAngle;
        if (timeOfDay > (totalLength - nightLength))
        {
            //nighttime
            sunAngle = 180 + ((180 / (nightLength)) * (timeOfDay - (totalLength - nightLength)));
        }
        else
        {
            //daytime
            sunAngle = (180 / (totalLength - nightLength)) * timeOfDay;
        }

        float normal = Mathf.Sin(Mathf.Deg2Rad * ((float)sunAngle % 360));
        //Debug.Log("[ENV] Time: " + timeOfDay + " SunAngle: " + sunAngle + " Sine: " + normal);
        //Apply intensity
        sun.intensity = Mathf.Clamp(normal * 4f, 0, 1.5f);
        //Apply ambient lighting and fog
        if (timeOfDay > (totalLength - nightLength))
        {
            RenderSettings.ambientLight = night;
            RenderSettings.fogColor = night;
        }

        else
        {
            RenderSettings.ambientLight = day.Evaluate(Mathf.Clamp01(normal));
            RenderSettings.fogColor = day.Evaluate(Mathf.Clamp01(normal));
        }
        //Rotate sun
        transform.rotation = Quaternion.Euler((float)sunAngle, -30, 0);
    }
}
