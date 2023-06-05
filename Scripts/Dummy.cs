using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    public float dummyLife = 5f;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Bullet enterBullet = collision.gameObject.GetComponent<Bullet>();
        if (enterBullet != null)
        {
            dummyLife -= enterBullet.damange;
        }
    }

    private void Update()
    {
        if (dummyLife < 1)
        {
            Debug.Log("Dummy a muerto");
            Destroy(gameObject);
        }
    }
}
