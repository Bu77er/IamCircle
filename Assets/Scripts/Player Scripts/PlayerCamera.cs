using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;             
    public Vector3 offset = new Vector3(0, 2, -4);
    public float sensitivity = 2f;       
    public float distance = 5f;          
    public float height = 2f;            
    public float minY = -30f;            
    public float maxY = 60f;

    private float yaw = 0f;
    private float pitch = 10f;

    private void Start()
    {
        Application.targetFrameRate = 100;
        QualitySettings.vSyncCount = 0;
        Cursor.lockState = CursorLockMode.Locked;

        if (target != null)
            yaw = target.eulerAngles.y;
    }
    void LateUpdate()
    {
        if (!target) return;

        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, minY, maxY);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 offset = rotation * new Vector3(0f, height, -distance);
         
        transform.position = target.position + offset;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}
