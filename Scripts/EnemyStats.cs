using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Bullet enterBullet = collision.gameObject.GetComponent<Bullet>();
        if (enterBullet != null)
        {
            Destroy(transform.parent.gameObject);
        }
    }
}
