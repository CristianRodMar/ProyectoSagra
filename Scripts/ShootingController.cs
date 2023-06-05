using UnityEngine;

public class ShootingController : MonoBehaviour
{
    //Punto de disparo
    public Transform point;
    //Referencia a la bala que dispara
    public GameObject bulletPrefab;
    //Velocidad de disparo
    public float bulletForce = 30f;
    public  float deviation = 0;
    public AudioSource audioSource;
    public AudioClip shootSound;
    public void pistolShoot()
    {
        //Generamos la bala
        GameObject bullet = Instantiate(bulletPrefab, point.position, point.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        //Añadimos desvío aleatorio a la bala
        float deviationAngle = Random.Range(-deviation, deviation);
        //Añadimos fuerza en la dirección de disparo
        Vector2 direction = Quaternion.Euler(0, 0, deviationAngle) * -point.up;
        rb.AddForce(direction * bulletForce, ForceMode2D.Impulse);
        audioSource.PlayOneShot(shootSound);
    }
}
