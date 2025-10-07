// /*
// Created by Darsan
// */

using System;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ObjectIconCreator : MonoBehaviour
{

    [SerializeField] private string _iconName;

    public Camera Camera { get; private set; }

    private void Awake()
    {
        Camera = GetComponent<Camera>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            TakeScreenShot();
        }

    }

    private string TakeScreenShot()
    {
        try
        {
            var height = Camera.pixelHeight;
            var width = (int)(Camera.aspect * height);
            var rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);

            Camera.targetTexture = rt;
            var screenShot = new Texture2D(width, height, TextureFormat.ARGB32, false);
            Camera.Render();

            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            Camera.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            rt.Release();
            Destroy(rt);

            //            screenShot.Resize(width , height , TextureFormat.ARGB32, true);
            //            screenShot.Apply();

            var bytes = screenShot.EncodeToPNG();
            File.WriteAllBytes(_iconName, bytes);
            Destroy(screenShot);



            return _iconName;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            return "";
        }
    }
}