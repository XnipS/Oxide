using UnityEngine;
using UnityEngine.UI;
public class dev_watermark : MonoBehaviour
{
    public bool minimal = false;
    void Start()
    {
        Text label = GetComponent<Text>();
        if (!minimal)
        {
            label.text = "Dev v" + Application.version + "\n" + System.DateTime.Now.ToString("dd/MM/yyyy");
        }
        else
        {
            label.text = "v" + Application.version;
        }
    }
}
