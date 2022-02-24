using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{

    public float visionDistance = 10;

    [Range(1, 20)]
    public int roundsPerSecond = 5;

    public PointAt boneShoulderRight;
    public PointAt boneShoulderLeft;

    public TargetableObject target { get; private set; }
    public bool playerWantsToAim { get; private set; }
    public bool playerWantsToAttack { get; private set; }

    private List<TargetableObject> validTargets = new List<TargetableObject>();
    private float cooldownScan = 0;
    private float cooldownPickTarget = 0;
    private float cooldownAttack = 0;

    private CameraController cam;


    private void Start() {

        cam = FindObjectOfType<CameraController>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {

        playerWantsToAttack = Input.GetButton("Fire1");
        playerWantsToAim = Input.GetButton("Fire2");

        cooldownScan -= Time.deltaTime;
        cooldownPickTarget -= Time.deltaTime;
        cooldownAttack -= Time.deltaTime;

        if (playerWantsToAim) {

            if(target != null) {

                // turn towards it...

                Vector3 toTarget = target.transform.position - transform.position;
                toTarget.y = 0;

                if (toTarget.magnitude > 3 && !CanSeeThing(target)) {
                    target = null;
                }
            }

            if (cooldownScan <= 0) ScanForTargets();
            if (cooldownPickTarget <= 0) PickATarget();
        } else {
            target = null;
        }

        if (boneShoulderLeft) boneShoulderLeft.target = target ? target.transform : null;
        if (boneShoulderRight) boneShoulderRight.target = target ? target.transform : null;


        DoAttack();
    }

    void DoAttack() {
        if (cooldownAttack > 0) return;
        if (!playerWantsToAim) return;
        if (!playerWantsToAttack) return;
        if (target == null) return;
        if (!CanSeeThing(target)) return;

        cooldownAttack = 1f / roundsPerSecond;

        // spawn projectiles...
        // or take health away from target

        boneShoulderLeft.transform.localEulerAngles += new Vector3(-30, 0, 0);
        boneShoulderRight.transform.localEulerAngles += new Vector3(-30, 0, 0);

        if(cam) cam.Shake(.25f);

    }

    void ScanForTargets() {
        cooldownScan = .5f;

        validTargets.Clear();

        TargetableObject[] things = GameObject.FindObjectsOfType<TargetableObject>();
        foreach(TargetableObject thing in things) {
            if (CanSeeThing(thing)) {
                validTargets.Add(thing);
            }
        }

    }

    private bool CanSeeThing(TargetableObject thing) {
        Vector3 vToThing = thing.transform.position - transform.position;

        // is too far to see?
        if (vToThing.sqrMagnitude > visionDistance * visionDistance) return false;

        // how much is in-front of player?
        float alignment = Vector3.Dot(transform.forward, vToThing.normalized);

        // is within so-many degrees of forward direction?
        if (alignment < .4f) return false;

        // check for occlusion...

        Ray ray = new Ray();
        ray.origin = transform.position;
        ray.direction = vToThing;

        
        Debug.DrawRay(ray.origin, ray.direction * visionDistance, Color.red);

        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, visionDistance)) {

            bool canSee = false;
            Transform xform = hit.transform;
            do {
                if (xform.gameObject == thing.gameObject) {
                    canSee = true;
                    break;
                }
                xform = xform.parent;
            } while (xform != null);

            if (!canSee) return false;
        }


        return true;
    }

    void PickATarget() {

        if (target) return;
        float closestDistanceSoFar = 0;

        foreach(TargetableObject thing in validTargets) {
            Vector3 vToThing = thing.transform.position - transform.position;

            float dis = vToThing.sqrMagnitude;
            if(dis < closestDistanceSoFar || target == null) {
                closestDistanceSoFar = dis;
                target = thing;
            }
        }

    }

}
