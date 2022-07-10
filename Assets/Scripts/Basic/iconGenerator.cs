using UnityEngine;
using System.Collections;
using UnityEditor;
#if UNITY_EDITOR
public class iconGenerator : MonoBehaviour
{
    public Camera mainCam;
    public int resolution;
    public GameObject[] obs;

    public static string ScreenShotName(int id)
    {
        //return string.Format("{0}/UI/Icons/ID_{1}_{2}.png",
        return string.Format("{0}/UI/Icons/ID_{1}.png",
            Application.dataPath,
            id,
            System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }


    public void IconGen(bool all, string str)
    {
        if (all)
        {
            StartCoroutine(GenerateAll());
        }
        else
        {
            StartCoroutine(Generate(str));
        }
    }

    IEnumerator GenerateAll()
    {
        for (int i = 0; i < obs.Length; i++)
        {
            if (i != 0 && i!=30&& i!=31&& i!=32&& i!=33)
            {
                DisableAll();
                obs[i].SetActive(true);
                yield return Generate(i.ToString());
            }
        }
    }

    void DisableAll()
    {
        foreach (GameObject g in obs)
        {
            if (g != null)
            {
                g.SetActive(false);
            }
        }
    }

    IEnumerator Generate(string str)
    {
        int id = int.Parse(str);
        RenderTexture rt = new RenderTexture(resolution, resolution, 24);
        mainCam.targetTexture = rt;
        Texture2D screenShot = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
        mainCam.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
        mainCam.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        DestroyImmediate(rt);
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = ScreenShotName(id);
        yield return System.IO.File.WriteAllBytesAsync(filename, bytes);
        //Debug.Log(string.Format("Took screenshot to: {0}", filename));
    }
}

[CustomEditor(typeof(iconGenerator))]
public class iconGenerator_editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        iconGenerator script = (iconGenerator)target;

        if (GUILayout.Button("Generate"))
        {
            iconGenerator_editor_popup window = (iconGenerator_editor_popup)EditorWindow.GetWindow(typeof(iconGenerator_editor_popup));
            window.script = script;
            window.Show();
        }
        if (GUILayout.Button("Generate ALL"))
        {
            script.IconGen(true, "");
        }
    }
}

public class iconGenerator_editor_popup : EditorWindow
{
    public string input_id;
    public iconGenerator script;
    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        input_id = EditorGUILayout.TextField("", input_id);
        if (GUILayout.Button("Done!"))
        {
            script.IconGen(false, input_id);
            this.Close();
        }
    }
}
#endif