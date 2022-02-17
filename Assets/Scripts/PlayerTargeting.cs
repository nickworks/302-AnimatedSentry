using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{

    public float visionDistance = 10;

    [Range(1, 20)]
    public int roundsPerSecond = 5;


    public Transform boneShoulderRight;
    public Transform boneShoulderLeft;

    public TargetableObject target { get; private set; }
    public bool playerWantsToAim { get; private set; }
    public bool playerWantsToAttack { get; private set; }

    private List<TargetableObject> validTargets = new List<TargetableObject>();
    private float cooldownScan = 0;
    private float cooldownPickTarget = 0;
    private float cooldownAttack = 0;

    void Update()
    {

        playerWantsToAttack = Input.GetButton("Fire1");
        playerWantsToAim = Input.GetButton("Fire2");

        cooldownScan -= Time.deltaTime;
        cooldownPickTarget -= Time.deltaTime;
        cooldownAttack -= Time.deltaTime;

        if (playerWantsToAim) {

            if(target != null) {
                if (!CanSeeThing(target)) {
                    target = null;
                }
            }

            if (cooldownScan <= 0) ScanForTargets();
            if (cooldownPickTarget <= 0) PickATarget();
        } else {
            target = null;
        }

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

        boneShoulderLeft.localEulerAngles += new Vector3(-30, 0, 0);
        boneShoulderRight.localEulerAngles += new Vector3(-30, 0, 0);

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
