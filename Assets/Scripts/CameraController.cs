using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform target;

    public Vector3 targetOffset;

    public float mouseSensitivityX = 5;
    public float mouseSensitivityY = -5;
    public float mouseSensitivityScroll = 5;

    private Camera cam;

    private float pitch = 0;
    private float yaw = 0;
    private float dollyDis = 10;

    void Start()
    {
        cam = GetComponentInChildren<Camera>();
    }
    void Update()
    {
        // 1. ease position:

        if (target) {
            transform.position = AnimMath.Ease(transform.position, target.position + targetOffset, .01f);
        }

        // 2. set rotation (TODO: ease):

        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        yaw += mx * mouseSensitivityX;
        pitch += my * mouseSensitivityY;

        pitch = Mathf.Clamp(pitch, -10, 89);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0);

        // 3. dolly camera in/out:

        dollyDis += Input.mouseScrollDelta.y * mouseSensitivityScroll;
        dollyDis = Mathf.Clamp(dollyDis, 3, 20);

        // ease towards dolly position:
        cam.transform.localPosition = AnimMath.Ease(
            cam.transform.localPosition,
            new Vector3(0, 0, -dollyDis),
            .02f);


    }

    private void OnDrawGizmos() {

        if(!cam) cam = GetComponentInChildren<Camera>();
        if (!cam) return;

        Gizmos.DrawWireCube(transform.position, Vector3.one);
        Gizmos.DrawLine(transform.position, cam.transform.position);
    }
}
