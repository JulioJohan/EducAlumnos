using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Profesor : MonoBehaviour
{
    public GameObject Item;
    public GameObject AtaqueE;
    public GameObject panelReinicio;
    public float Speed;
    public float JumpForce;
    public float JumMax;
    public LayerMask CapaSuelo;

    private Rigidbody2D Rigidbody2D;
    private Animator Animator;
    private BoxCollider2D boxCollider;
    private float Horizontal;
    private bool Grounded;
    private float JumpRest;
    public float damageAmount = 4f;
    private bool isDying;

    public float LastShoot;
    

    [SerializeField] private float vida;
    [SerializeField] private float maximoVida;
    [SerializeField] private BarraVida barraVida;

    public GameManagerNuevo gameManagerNuevo;
    public MongoDBManager mongoDBManager;
    private bool puntosGuardados = false;

    public static Profesor Instancia { get; private set; }

    void Start()
    {
        gameManagerNuevo = GameObject.FindObjectOfType<GameManagerNuevo>();

        mongoDBManager = GameObject.FindObjectOfType<MongoDBManager>();
        if (panelReinicio != null)
        {
            panelReinicio.SetActive(false);
        }
        
        Rigidbody2D = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        Animator = GetComponent<Animator>();
        JumpRest = JumMax;
        vida = maximoVida;
        barraVida.InicializarBarraVida(vida);
    }

    void Awake()
    {
        if (Instancia == null)
        {
            Instancia = this;
        }
        else
        {
            Destroy(gameObject);// Si ya hay una instancia, destruir este objeto.
        }
    }
    // Update is called once per frame
    void Update()
    {
        //Movimiento del personaje
        Horizontal = Input.GetAxisRaw("Horizontal");

        //Establece la dirección en la cual el personaje estara avanzando
        if (Horizontal < 0.0f) transform.localScale = new Vector2 (-10.0f, 10.0f);
        else if (Horizontal > 0.0f) transform.localScale = new Vector2(10.0f, 10.0f);

        //Llamada de las animaciones de caminar y Suelo
        Animator.SetBool("Caminando", Horizontal != 0.0f);
        //Determina base en el escenario
        Animator.SetBool("EnSuelo",Suelo());
        
        Debug.DrawRay(transform.position, Vector2.down * 0.1f, Color.red);

        Jump();

        if(Input.GetKey(KeyCode.Space) && Time.time > LastShoot + 0.75)
        {
            Shoot();
            LastShoot = Time.time;
        }
        
    }

    private void MostrarPanelReinicio()
    {
        if (panelReinicio != null)
        {
            panelReinicio.SetActive(true);
        }
    }

    public void guardarBaseDatos()
    {
        if(vida <= 0 && !puntosGuardados)
        {
            string nombre = PlayerPrefs.GetString("NombreJugador");
            int score = mongoDBManager.scoreInitial;
            StartCoroutine(mongoDBManager.guardarPuntosBaseDatos(score, nombre));
            puntosGuardados = true;
        }

    }

    public void TomarDaño(float daño)
    {
        vida -= daño;
        barraVida.CambiarVidaActual(vida);

        if (vida <= 0)
        {
            guardarBaseDatos();

            print("scorescorescorescorescore");
//            print(score);/
            Animator.SetTrigger("Muerte");
            MostrarPanelReinicio();
            Speed = 0;
            Rigidbody2D.gravityScale = 1000;
            isDying = true;
            AnimatorStateInfo stateInfo = Animator.GetCurrentAnimatorStateInfo(0);
            if (isDying && stateInfo.normalizedTime >= 1f)
            {
                Debug.Log("Entreee");
                Destroy(gameObject);
                // Destruir el objeto después de que la animación haya terminado
            }
            // Rigidbody2D.velocity.x = null;
            // Rigidbody2D.velocity.y = null;
            //Destroy(gameObject);
        }
    }
    

    bool Suelo(){
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, new Vector2(boxCollider.bounds.size.x,boxCollider.bounds.size.y), 0f, Vector2.down, 0.2f, CapaSuelo);
        return raycastHit.collider != null;
    }

    //Metodo para saltar el personaje
    private void Jump()
    {
        //Determina el limite de saltos del personaje
        if(Suelo()){

            JumpRest = JumMax;
        }

        if (Input.GetKeyDown(KeyCode.W) && JumpRest > 0)
        {
            JumpRest--;
            Rigidbody2D.velocity = new Vector2(Rigidbody2D.velocity.x,0f);
            //Debug.Log("Jumping!");
            Rigidbody2D.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        }
    }

    //Metodo para establecer la velocidad y el posicion limite del personaje
    private void FixedUpdate()
    {
        Rigidbody2D.velocity = new Vector2(Horizontal * Speed, Rigidbody2D.velocity.y);
        Rigidbody2D.position = new Vector2(Mathf.Clamp(Rigidbody2D.position.x, -8.28f, 43.96f),
            Rigidbody2D.position.y);
    }

    private void Shoot()
    {
        Vector3 direction;
        if(transform.localScale.x == 10.0f) direction = Vector3.right;
        else direction = Vector3.left;

        GameObject ataque = Instantiate(Item,transform.position + direction * 0.1f ,Quaternion.identity);
        ataque.GetComponent<Item>().SetDirection(direction);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemigo"))
        {
            TomarDaño(damageAmount);
            barraVida.CambiarVidaActual(vida);
            //Debug.Log("Vida actualizada: " + vida);
        }
    }


}