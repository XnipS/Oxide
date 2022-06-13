using System.Collections;
using UnityEngine;

public class WaitTillDestory : MonoBehaviour {
    public float seconds;
	void Start () {
        StartCoroutine(Die());
	}
    IEnumerator Die () {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
