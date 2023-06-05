using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Bullet enterBullet = collision.gameObject.GetComponent<Bullet>();
        if (enterBullet != null)
        {
            gameManager.GameOver();
        }
    }
}

