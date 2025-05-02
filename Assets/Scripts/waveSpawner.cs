using UnityEngine;

public class waveSpawner : MonoBehaviour
{
    public float speed = 5f;
    public float maxRadius = 20f;
    public float thickness = 1f;
    public float amplitude = 1f;
    public Color waveColor = Color.white;

    private Material mat;

    void Start()
    {
        mat = GetComponent<Renderer>().material;

        mat.SetFloat("_Speed", speed);
        mat.SetFloat("_MaxRadius", maxRadius);
        mat.SetFloat("_Thickness", thickness);
        mat.SetFloat("_Amplitude", amplitude);
        mat.SetColor("_WaveColor", waveColor);
    }
    void OnCollisionEnter(Collision collision)
    {
       // if(collision.contactCount > 0)
       // {
       //     Vector3 contactPoint = collision.GetContact(0).point;
       //     TriggerShock(contactPoint);
       // }
    }
    public void TriggerShock(Vector3 center)
    {
        mat.SetVector("_WaveCenter", new Vector4(center.x, center.y, center.z, 0));
        mat.SetFloat("_StartTime", Time.time);
    }
}
