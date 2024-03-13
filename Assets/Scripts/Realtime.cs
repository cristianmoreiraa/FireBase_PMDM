using UnityEngine;

using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Unity.VisualScripting;

public class Realtime : MonoBehaviour
{
    // conexion con Firebase
    private FirebaseApp _app;
    // Singleton de la Base de Datos
    private FirebaseDatabase _db;
    // referencia a la 'coleccion' Clientesd
    private DatabaseReference _refClientes;
    // referencia a un cliente en concreto
    private DatabaseReference _refAA002;
    // GameObject a modificar
    public GameObject ondavital;
    // contador para update
    private float _i;

    /*
    * Mover monedas 
    */
    public GameObject player;
    public GameObject PickUp1;
    public GameObject PickUp;

    
    /*
     * Base de datos usada en formato JSON
     *      {
              "Jugadores": {
                    "AA01": {
                      "nombre": "Vegeta",
                      "puntos": 0
                    },
                    "AA02": {
                      "nombre": "Son Goku",
                      "puntos": 1
                    }
               }
            }
     */
    
    // Start is called before the first frame update
    void Start()
    {
        // inicializamos contador
        _i = 0;
        
        // realizamos la conexion a Firebase
        _app = Conexion();
        
        // obtenemos el Singleton de la base de datos
        _db = FirebaseDatabase.DefaultInstance;
        
        // Obtenemos la referencia a TODA la base de datos
        // DatabaseReference reference = db.RootReference;
        
        // Definimos la referencia a Clientes
        _refClientes = _db.GetReference("Jugadores");
        
        // Definimos la referencia a AA02
        _refAA002 = _db.GetReference("Jugadores/AA02");
        
        // Recogemos todos los valores de Clientes
        _refClientes.GetValueAsync().ContinueWithOnMainThread(task => {
                if (task.IsFaulted) {
                    // Handle the error...
                }
                else if (task.IsCompleted) {
                    DataSnapshot snapshot = task.Result;
                    // mostramos los datos
                    RecorreResultado(snapshot);
                    //Debug.Log(snapshot.value);
                }
            });
        
        // Añadimos el evento cambia un valor
        _refAA002.ValueChanged += HandleValueChanged;

        // Añadimos un nodo
        AltaDevice();

        DatabaseReference pickUpRef1 = _db.GetReference("PickUps/PickUP1");
        DatabaseReference pickUpRef = _db.GetReference("PickUps/PickUp");

        pickUpRef.ValueChanged += (sender, args) => HandleValueChanged(args.Snapshot, PickUp);
        pickUpRef1.ValueChanged += (sender, args) => HandleValueChanged(args.Snapshot, PickUp1);
    }

    void HandleValueChanged(DataSnapshot snapshot, GameObject pickUpObject){
        
        float pickUpX = float.Parse(snapshot.Child("x").Value.ToString());
        float pickUpY = float.Parse(snapshot.Child("y").Value.ToString());
        float pickUpZ = float.Parse(snapshot.Child("z").Value.ToString());

        Vector3 newPosition = new Vector3(pickUpX, pickUpY, pickUpZ);

        pickUpObject.transform.position = newPosition;
    }

    
    // realizamos la conexion a Firebase
    // devolvemos una instancia de esta aplicacion
    FirebaseApp Conexion()
    {
        FirebaseApp firebaseApp = null;
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                firebaseApp = FirebaseApp.DefaultInstance;
                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                Debug.LogError(System.String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
                firebaseApp = null;
            }
        });
            
        return firebaseApp;
    }
    
    // evento cambia valor en AA02
    // escalo objeto en la escena
    void HandleValueChanged(object sender, ValueChangedEventArgs args) {
        if (args.DatabaseError != null) {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // Mostramos lo resultados
        MuestroJugador(args.Snapshot);
        // escalo objeto
        float escala = float.Parse(args.Snapshot.Child("puntos").Value.ToString());
        Vector3 cambioEscala = new Vector3(escala, escala, escala);
        ondavital.transform.localScale = cambioEscala;
    }

    // recorro un snapshot de un nivel
    void RecorreResultado(DataSnapshot snapshot)
    {
        foreach(var resultado in snapshot.Children) // Clientes
        {
            Debug.LogFormat("Key = {0}", resultado.Key);  // "Key = AAxx"
            foreach(var levels in resultado.Children)
            {
                Debug.LogFormat("(key){0}:(value){1}", levels.Key, levels.Value);
            }
        }
    }
    
    // muestro un jugador
    void MuestroJugador(DataSnapshot jugador)
    {
        foreach (var resultado in jugador.Children) // jugador
        {
            Debug.LogFormat("{0}:{1}", resultado.Key, resultado.Value);
        }
    }


    // doy de alta un nodo con un identificador unico
    void AltaDevice()
    {
        _refClientes.Child(SystemInfo.deviceUniqueIdentifier).Child("nombre").SetValueAsync("Mi dispositivo");
    }
    
    // Update is called once per frame
    void Update()
    {
        float playerX = player.transform.position.x;
        float playerY = player.transform.position.y;

        // Actualizo la base de datos en cada frame, CUIDADO!!!!! 
        _refClientes.Child("AA01").Child("puntos").SetValueAsync(_i);
        _i = _i + 0.01f;
        _refClientes.Child("AA01").Child("x").SetValueAsync(playerX);
        _refClientes.Child("AA01").Child("y").SetValueAsync(playerY);
       
        


    }
}