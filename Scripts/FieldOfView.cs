using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FieldOfView : MonoBehaviour
{

    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    public LayerMask targetMask;
    public LayerMask obstacleMask1;
    public Color ColorFOV;
    //public LayerMask obstacleMask2;
    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();
    Collider2D[] targetsInViewRadius;


    private void Start()
    {
        StartCoroutine("FindTargetsWithDelay", .2f);
    }
    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibileTargets();
        }
    }

    void FindVisibileTargets()
    {
        visibleTargets.Clear();
        targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(-transform.up, dirToTarget) < viewAngle / 2)
            {
                float dsToTarget = Vector3.Distance(transform.position, target.position);
                if (!Physics2D.Raycast(transform.position, dirToTarget, dsToTarget, obstacleMask1))
                {
                    visibleTargets.Add(target);
                }
            }
        }
    }
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.z;
        }

        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

        float x = -Mathf.Sin(angleInRadians);
        float y = -Mathf.Cos(angleInRadians);

        Quaternion rotation = transform.rotation;

        Vector3 direction = rotation * new Vector3(x, y, 0);

        return direction;
    }

}
