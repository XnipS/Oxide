using UnityEngine;
using TMPro;
public class dev_watermark : MonoBehaviour
{
    public bool minimal = false;
    void Start()
    {
        TMP_Text label = GetComponent<TMP_Text>();
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
