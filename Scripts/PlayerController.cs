using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //Velocidad de movimento del personaje
    float speed = 5.0f;

    //Referencia a las animaciones
    Animator anim;

    //Referencia a la posición del ratón
    Vector3 mousePosition;
    public Vector3 mouseWorldPosition;
    Vector3 direction;

    //Almacenamiento de los inputs
    Vector3 inputDirection;

    //Angulo de dirección
    float angleBetween;

    //Variable para comprobar si está en movimiento
    bool isMoving = false;

    //Variable para comprobar si se está apuntando

    public bool isAiming = false;

    //Variable para comprobar si la animación de apuntar terminó

    public bool isAimAnimationEnded = false;
    public bool isReverseAimAnimationEnded = false;

    //Calcular el producto cruzado entre los vectores direction y input direction
    float crossProduct;

    float angle;
    float distance;

    public float rotationSpeed = 4;

    ShootingController shootingController;

    public Quaternion rotation;

    void Start()
    {
        anim = GetComponent<Animator>();
        shootingController = GetComponent<ShootingController>();
    }

    void Update()
    {
        if (!GameManager.instance.isPaused)
        {
            rotate();
            move();
            aim();
        }

    }
    void rotate()
    {
        // Obtener la posición del ratón en pantalla
        mousePosition = Input.mousePosition;
        // Establecer la distancia de la cámara al plano del ratón
        distance = Vector3.Distance(Camera.main.transform.position, transform.position);
        // Convertir la posición del ratón de pantalla a mundo
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, distance));
        // Establecer el valor del eje Z a 0 para la posición del ratón en mundo
        mouseWorldPosition.z = 0;
        // Calcular la dirección de rotación del personaje
        direction = (mouseWorldPosition - transform.position).normalized;
        //Calcular la rotación en grados
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // Rotar el personaje hacia la dirección calculada
        rotation = Quaternion.Euler(new Vector3(0, 0, angle + 90));
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
    }

    void move()
    {
        //test
        // Obtener la dirección de movimiento del personaje en base a los controles WASD
        inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        isMoving = (inputDirection.x != 0 || inputDirection.y != 0) ? true : false;
        // Calcular el ángulo en grados entre la dirección de movimiento del personaje y la dirección de rotación del ratón
        angleBetween = Vector3.Angle(direction, inputDirection);
        //Si el angulo es superior a 100 (dando un margen de 10 grados) se considera que anda de espaldas
        bool isMovingOpposite = (angleBetween > 100) ? true : false;
        // Ajustar la velocidad del personaje en función de si se mueve en dirección contraria o no
        float adjustedSpeed = isMovingOpposite ? speed * 0.5f : speed;
        // Mover el personaje en base a la dirección de los controles WASD, la velocidad ajustada y el tiempo delta
        transform.position += new Vector3(inputDirection.x, inputDirection.y, 0) * adjustedSpeed * Time.deltaTime;

        playAnimation();
    }

    void playAnimation()
    {

        if (isMoving && !isAiming)
        {
            //Si el resultado de esta operación es mayor que 0, se mueve hacia la izquierda
            //y si es mayor se mueve hacia la derecha
            crossProduct = Vector3.Cross(direction, inputDirection).z;
            switch (angleBetween)
            {
                case < 25:
                    anim.Play("Forward");

                    break;
                case < 75:
                    if (crossProduct > 0)
                    {
                        anim.Play("ForwardL");
                    } 
                    if (crossProduct < 0)
                    {
                        anim.Play("ForwardR");
                    }
                    break;
                case < 105:
                    if (crossProduct > 0)
                    {
                        anim.Play("Left");
                    }
                    if (crossProduct < 0)
                    {
                        anim.Play("Right");
                    }
                    break;
                case < 155:
                    if (crossProduct > 0)
                    {
                        anim.Play("BackwardL");
                    }
                    if (crossProduct < 0)
                    {
                        anim.Play("BackwardR");
                    }
                    break;
                case < 180:
                    anim.Play("Backward");
                    break;
                default:
                    anim.Play("Idle");
                    break;
            }
        } else if (!isAiming)
        {
            anim.Play("Idle");
        }

    }

    void aim()
    {
        if (Input.GetButtonDown("Fire2"))
        {
            isAiming = true;
            anim.Play("Aim");
            speed = 2;
        }
        if (Input.GetButtonUp("Fire2"))
        {
            isAimAnimationEnded = false;
            anim.Play("AimReverse");
        }
        if (isReverseAimAnimationEnded)
        {
            speed = 5;
            isAiming = false;
            isReverseAimAnimationEnded = false;
        }
        if (isAiming && isAimAnimationEnded)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                shootingController.pistolShoot();
            }
        }
    }

    void aimAnimationEnded(AnimationEvent animationEvent)
    {
        isAimAnimationEnded = true;
    }

    void reverseAimAnimationEnded(AnimationEvent animationEvent)
    {
        isReverseAimAnimationEnded = true;
    }

}

