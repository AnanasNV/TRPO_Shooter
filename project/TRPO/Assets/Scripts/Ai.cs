using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ai : MonoBehaviour
{
    public Transform destination;

    public float Angle;
    public float Radius;
    public float HearRadius;
    public float heightMultiplayer = 1.5f;

    NavMeshAgent navMeshAgent;

    private bool isInFOV = false;
    private bool isHearing = false;
    private bool isStealth = false;
    private bool Heared = false;

    private void OnDrawGizmos()
    {
        Vector3 fovLine1 = Quaternion.AngleAxis(Angle, transform.up) * transform.forward * Radius;
        Vector3 fovLine2 = Quaternion.AngleAxis(-Angle, transform.up) * transform.forward * Radius;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, fovLine1);
        Gizmos.DrawRay(transform.position, fovLine2);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Radius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, HearRadius);



       

        if (!isInFOV)
            Gizmos.color = Color.red;
        else
            Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, (destination.position - transform.position).normalized * Radius);

        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, transform.forward * Radius);
    }


    public bool inFOV(Transform checkingObject, Transform target,ref bool isHearing, float maxAngle, float maxRadius, float maxHearRadius)
    {
        Vector3 directionBetween = (target.position - checkingObject.position).normalized;
        directionBetween.y *= 0;
        RaycastHit hit;
        if (Physics.Raycast(checkingObject.position, target.position - checkingObject.position, out hit, maxHearRadius))
        {            
            if (LayerMask.LayerToName(hit.transform.gameObject.layer) == "Player")
            {
                isHearing = true;
                float angle = Vector3.Angle(checkingObject.forward, directionBetween);
                if (angle <= maxAngle && hit.distance<=maxRadius)
                {
                    return true;
                }
            }
        }
        return false;
    }﻿

    void Start()
    {
        navMeshAgent = this.GetComponent<NavMeshAgent>();
        if (navMeshAgent == null)
        {
            Debug.LogError("The nav mesh agent is not attached to " + gameObject.name);
        }

    }        

    void Update()
    {        
        Quaternion rotToPlayer = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(destination.position - transform.position), Time.deltaTime * 3);
        if (navMeshAgent != null)
        {
            if (isStealth)
            {
                isHearing = false;
            }
            if (isHearing)
            {
               transform.rotation = rotToPlayer;
            }
            if (isInFOV)
            {
                transform.rotation = rotToPlayer;
                SetDestination();
            }
            if (Heared && !isStealth)
            {
                if (isInFOV)
                    transform.rotation = rotToPlayer;
                SetDestination();
            }
            isStealth = GameObject.Find("Cube").GetComponent<PlayerController>().Stealth;
            float dist = navMeshAgent.remainingDistance;
            if (dist != Mathf.Infinity && navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete && navMeshAgent.remainingDistance == 0)
                isHearing = false;
            isInFOV = inFOV(transform, destination, ref isHearing, Angle, Radius, HearRadius);   
            if (isInFOV)
            {
                Heared = true;
            }
            if (!isHearing && !isInFOV)
            {
                Heared = false;
            }
        }
        
    }

    void SetDestination()
    {
        if (destination != null)
        {
            Vector3 targetVector = destination.transform.position;
            navMeshAgent.SetDestination(targetVector);
        }
    }
}
