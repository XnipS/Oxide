using UnityEngine;

public class fireLight : MonoBehaviour
{
    Vector3 startPos;
    public float amount;
    public float scale;
    public Vector2 speed;
    void Start()
    {
        startPos = transform.localPosition;
    }

    void Update()
    {
        Vector2 spd = new Vector2( Time.time * speed.x, Time.time * speed.y);
        float noise1 = Mathf.PerlinNoise(transform.localPosition.x + spd.x, transform.localPosition.x + spd.y);
        float noise2 = Mathf.PerlinNoise(transform.localPosition.y + spd.x, transform.localPosition.y + spd.y);
        float noise3 = Mathf.PerlinNoise(transform.localPosition.z + spd.x, transform.localPosition.z + spd.y);

        noise1 -= .5f;
        noise2 -= .5f;
        noise3 -= .5f;

        noise1 *= amount;
        noise2 *= amount;
        noise3 *= amount;

        transform.localPosition = startPos + new Vector3(noise1, noise2, noise3);
    }
}
