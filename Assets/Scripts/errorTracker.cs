using UnityEngine;
using UnityEngine.UI;
public class errorTracker : MonoBehaviour
{
    void Start()
    {
        Application.logMessageReceived += TrackError;
    }

    void TrackError(string cond, string stack, LogType type)
    {
        GetComponent<Text>().text += "\n" + cond;
    }
}
