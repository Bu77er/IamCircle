using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public float bufferScale = 1.0f;
    public LayerMask effectLayer;
    public Shader shader;
    public Material material;

    private Camera cam;
    private Camera effectCam;
    private int scaledWidth;
    private int scaledHeight;
    void Start()
    {
        Application.targetFrameRate = 120;
        cam = GetComponent<Camera>();
       // effectCam = new GameObject().AddComponent<Camera>(); ;
        UpdateBufferSize();
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
       //effectCam.CopyFrom(cam);
       //effectCam.transform.parent = this.transform;
       //effectCam.gameObject.hideFlags = HideFlags.HideAndDontSave;
        cam.transform.position = transform.position;
        cam.transform.rotation = transform.rotation;
        cam.cullingMask = effectLayer;

        RenderTexture uvBuffer = RenderTexture.GetTemporary(scaledWidth, scaledHeight, 24, RenderTextureFormat.ARGBFloat);
        cam.SetTargetBuffers(uvBuffer.colorBuffer, uvBuffer.depthBuffer);
        cam.RenderWithShader(shader, "");
        material.SetTexture("_UVBuffer", uvBuffer);
        Graphics.Blit(source, destination, material);
        
        RenderTexture.ReleaseTemporary(uvBuffer);
    }
    void Update()
    {
        UpdateBufferSize();
    }
    void UpdateBufferSize()
    {
        bufferScale = Mathf.Clamp01(bufferScale);
        scaledWidth = (int)(Screen.width * bufferScale);
        scaledHeight = (int)(Screen.height * bufferScale);
    }
}
