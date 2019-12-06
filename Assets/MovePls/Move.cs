using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{

    public float speed;
    public bool cyclic;
    public float waitTime;
    [Range(0, 2)]
    public float easeAmount;
    float nextMoveTime;
    //index of waypoint moving away from
    int fromWaypointIndex;
    //% between 0 and 1
    float percentBetweenWaypoints;
    public bool stop;
    //public GameObject gb;

    public Vector3[] localWaypoints;
    Vector3[] globalWaypoints;
    public void Start()
    {
        Debug.Log("hit");
        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
    }
    public void Update()
    {
        if(!stop)
        {
            Vector3 velocity = CalculatePlatformMovement();
            transform.Translate(velocity);
        }
    }
    float Ease(float x)
    {
        float a = easeAmount + 1;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }


    Vector3 CalculatePlatformMovement()
    {
        if (Time.time < nextMoveTime)
        {
            //Debug.Log("hit");
            return Vector3.zero;
            
        }
        if (globalWaypoints.Length != 0)
        {
            //Debug.Log("hit");
            fromWaypointIndex %= globalWaypoints.Length;
            int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
            float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
            percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;
            percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
            float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

            Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);
            //wut happens when % is greater than or = to 1 like u at a point
            //we do this 
            if (percentBetweenWaypoints >= 1)
            {
                percentBetweenWaypoints = 0;
                fromWaypointIndex++;
                if (!cyclic)
                {
                    if (fromWaypointIndex >= globalWaypoints.Length - 1)
                    {
                        fromWaypointIndex = 0;
                        //bruh this is like the easiest thing to flip an array y dis no exist elsewhere why we no b told bout dis
                        System.Array.Reverse(globalWaypoints);
                    }
                }
                // = current time + amount of time 2 wait
                nextMoveTime = Time.time + waitTime;
            }
            return newPos - transform.position;
        }
        
        return new Vector3(0, 0, 0);
    }
    private void OnDrawGizmos()
    {

        if (localWaypoints != null)
        {
            Gizmos.color = Color.black;
            float size = .3f;
            for (int i = 0; i < localWaypoints.Length; i++)
            {
                //the next line here is so that the waypoints move when moving around the object while game is not playing but 
                //dont move while the game is not playing
                Vector3 globalWaypointPos = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + transform.position;
                Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);

            }
        }
    }
}

