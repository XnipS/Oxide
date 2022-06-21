using UnityEngine;
using UnityEngine.UI;
public class dev_watermark : MonoBehaviour
{
	void Start()
	{
		Text label = GetComponent<Text>();
		label.text = "Dev v" + Application.version + "\n" + System.DateTime.Now.ToString("dd/MM/yyyy");
	}
}
