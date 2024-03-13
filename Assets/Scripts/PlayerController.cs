using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private int contador;
    private float velocidad;
    private float aumentoDeVelocidad = 0.33f;
    public TextMeshProUGUI textoContador;
    public TextMeshProUGUI textoVelocidad;
    public GameObject objetoTextoVictoria;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        contador = 0;
        velocidad = 10;
        ActualizarTextoContador();
        objetoTextoVictoria.SetActive(false);
    }

    void OnTriggerEnter(Collider otro)
    {
        if (otro.gameObject.CompareTag("PickUp"))
        {
            otro.gameObject.SetActive(false);
            contador++;
            velocidad += aumentoDeVelocidad;
            ActualizarTextoContador();
            ActualizarTextoVelocidad();
        }
    }

    void ActualizarTextoContador()
    {
        textoContador.text = "Contador: " + contador.ToString();
        if (contador >= 8)
        {
            objetoTextoVictoria.SetActive(true);
        }
    }

    void ActualizarTextoVelocidad()
    {
        textoVelocidad.text = "Velocidad: " + velocidad.ToString("F2");
    }

    void FixedUpdate()
    {
        Vector3 movimiento = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
        rb.AddForce(movimiento * velocidad);
    }

    // Método para obtener el valor del contador
    public int ObtenerContador()
    {
        return contador;
    }
}
