using UnityEngine;
using UnityEditor;
using System.IO;

public class screenshotTaker : MonoBehaviour
{
    public Vector2Int resolution;

    public static string ScreenShotName()
    {
        return string.Format("{0}/Screenshots/{1}.png",
            Application.persistentDataPath,
            System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }
    public void Generate()
    {
        Camera mainCam = GetComponent<Camera>();
        RenderTexture rt = new RenderTexture(resolution.x, resolution.y, 24);
        mainCam.targetTexture = rt;
        Texture2D screenShot = new Texture2D(resolution.x, resolution.y, TextureFormat.RGB24, false);
        mainCam.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resolution.x, resolution.y), 0, 0);
        mainCam.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        DestroyImmediate(rt);
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = ScreenShotName();
        if (!Directory.Exists(Application.persistentDataPath + "/Screenshots/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Screenshots/");

        }
        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.Log(string.Format("Took screenshot to: {0}", filename));
    }
}

[CustomEditor(typeof(screenshotTaker))]
public class screenshotTaker_editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        screenshotTaker script = (screenshotTaker)target;

        if (GUILayout.Button("Generate"))
        {
            script.Generate();
        }
    }
}
