using UnityEngine;

public class reloadAudio : MonoBehaviour {
    public float volumeMod = 1f;
	public Vector3 offset;

	public void PlayRandomReloadAudio(AudioClip[] clip)
	{
		PlayReloadAudio(clip[Random.Range(0, clip.Length)]);
	}

	public void PlayReloadAudio(AudioClip clip)
	{
		if(!Application.isPlaying)
		{ return; }
		GameObject gam = new GameObject();
		gam.AddComponent<AudioSource>();
		gam.GetComponent<AudioSource>().clip = clip;
		gam.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Linear;
		gam.GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
		gam.GetComponent<AudioSource>().spatialBlend = 1f;
		gam.GetComponent<AudioSource>().minDistance = 1f;
		gam.GetComponent<AudioSource>().volume = volumeMod;
		gam.GetComponent<AudioSource>().maxDistance = 50f;
		gam.AddComponent<waitUntilDestroy>().seconds = clip.length;

		gam.transform.position = transform.position;
		gam.transform.position += offset;

		gam.transform.rotation = gam.transform.rotation;
		gam.GetComponent<AudioSource>().Play();
	}

	public void PlayNotReloadAudio(AudioClip clip)
	{
		if(!Application.isPlaying)
		{ return; }
		GameObject gam = new GameObject();
		gam.AddComponent<AudioSource>();

		gam.GetComponent<AudioSource>().clip = clip;
		gam.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Linear;
		gam.GetComponent<AudioSource>().spatialBlend = 1f;
		gam.GetComponent<AudioSource>().minDistance = 1f;
		gam.GetComponent<AudioSource>().volume = volumeMod;
		gam.GetComponent<AudioSource>().maxDistance = 50;
		gam.AddComponent<waitUntilDestroy>().seconds = clip.length;

		gam.transform.position = transform.position;
		gam.transform.position += offset;

		gam.transform.rotation = gam.transform.rotation;
		gam.GetComponent<AudioSource>().Play();
	}
}
