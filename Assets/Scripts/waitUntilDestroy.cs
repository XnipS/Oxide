using System.Collections;
using UnityEngine;

public class waitUntilDestroy : MonoBehaviour {
    public float seconds;
    public GameObject destroyEffects;
	void Start () {
        StartCoroutine(Die());
	}
    IEnumerator Die () {
        yield return new WaitForSeconds(seconds);
        if(destroyEffects != null) {
            Instantiate(destroyEffects, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }
}
