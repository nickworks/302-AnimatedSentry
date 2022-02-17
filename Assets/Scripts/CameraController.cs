using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public PlayerTargeting player;

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
        if(player == null) {
            PlayerTargeting script = FindObjectOfType<PlayerTargeting>();
            if (script != null) player = script;
        }
    }
    void Update()
    {

        // is the player aiming?
        bool isAiming = (player && player.target && player.playerWantsToAim);

        // 1. ease rig position:

        if (player) {
            transform.position = AnimMath.Ease(transform.position, player.transform.position + targetOffset, .01f);
        }

        // 2. set rig rotation:

        float playerYaw = AnimMath.AngleWrapDegrees(yaw, player.transform.eulerAngles.y);

        if (isAiming) {


            Quaternion tempTarget = Quaternion.Euler(0, playerYaw, 0);

            transform.rotation = AnimMath.Ease(transform.rotation, tempTarget, .001f);

        } else {
            float mx = Input.GetAxis("Mouse X");
            float my = Input.GetAxis("Mouse Y");

            yaw += mx * mouseSensitivityX;
            pitch += my * mouseSensitivityY;

            //if (yaw > 360) yaw -= 360;
            //if (yaw < 0) yaw += 360;

            pitch = Mathf.Clamp(pitch, -10, 89);
            transform.rotation = AnimMath.Ease(transform.rotation, Quaternion.Euler(pitch, yaw, 0), .001f);
        }



        // 3. dolly camera in/out:
        
        dollyDis += Input.mouseScrollDelta.y * mouseSensitivityScroll;
        dollyDis = Mathf.Clamp(dollyDis, 3, 20);

        // ease towards dolly position:

        float tempZ = isAiming ? 2 : dollyDis;

        cam.transform.localPosition = AnimMath.Ease(
            cam.transform.localPosition,
            new Vector3(0, 0, -tempZ),
            .02f);

        // 4. rotate the camera object

        if (isAiming) {
            Vector3 vToAimTarget = player.target.transform.position - cam.transform.position;
            Vector3 euler = Quaternion.LookRotation(vToAimTarget).eulerAngles;

            euler.y = AnimMath.AngleWrapDegrees(playerYaw, euler.y);

            Quaternion temp = Quaternion.Euler(euler.x, euler.y, 0);

            cam.transform.rotation = AnimMath.Ease(cam.transform.rotation, temp, .001f);

        } else {
            cam.transform.localRotation = AnimMath.Ease(cam.transform.localRotation, Quaternion.identity, .001f);
        }


    }

    private void OnDrawGizmos() {

        if(!cam) cam = GetComponentInChildren<Camera>();
        if (!cam) return;

        Gizmos.DrawWireCube(transform.position, Vector3.one);
        Gizmos.DrawLine(transform.position, cam.transform.position);
    }
}
