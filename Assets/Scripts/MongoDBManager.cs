using UnityEngine;
using MongoDB.Driver;
using TMPro;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PuntuacionJugador
{
    public int Puntuaje;
    public string Nombre;
}
[System.Serializable]
public class Jugador
{
    public int Puntuaje;
    public string Nombre;
}

[System.Serializable]
public class Jugadores
{
    public List<Jugador> jugadores;
}


public class MongoDBManager : MonoBehaviour
{
    private MongoClient client;
    private IMongoDatabase database;
    private IMongoCollection<Jugador> jugadoresCollection;
    public TMP_Text textoUI;
    public TMP_Text score;
    public int scoreInitial = 0;
    ///private int scoreInitial = 0;


    private void Start()
    {
        ///scoreInitial = score.text.;///

        //string connectionString = "mongodb+srv://mariagmenr:laravel8@cluster0.u0z54ln.mongodb.net/?retryWrites=true&w=majority";
        //client = new MongoClient(connectionString);
        //database = client.GetDatabase("Educalumnos");
        //jugadoresCollection = database.GetCollection<Jugador>("jugadores");
        //ConsultarMejoresPuntajes();
        print(score);
    }


    public IEnumerator guardarPuntosBaseDatos(int puntos, string nombreJugador)
    {
        print(puntos);
        print(nombreJugador);
        PuntuacionJugador data = new PuntuacionJugador();
        data.Nombre = nombreJugador;
        data.Puntuaje = puntos;
        string json = JsonUtility.ToJson(data);
        print(data);
        string url = "https://educalumnos-backend.onrender.com/add_score";

        using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            print(bodyRaw);
            print(json);


            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                Debug.Log(www.downloadHandler.text); // Imprime la respuesta del servidor
            }
            else
            {
                print("Se guardo correctamente");
            }
        }
    }

    IEnumerator mejoresJugadores()
    {
        string url = "https://educalumnos-backend.onrender.com/top_scores";
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Procesar los resultados
                string jsonResponse = www.downloadHandler.text;
                Jugadores mejoresJugadores = JsonUtility.FromJson<Jugadores>("{\"jugadores\":" + jsonResponse + "}");

                textoUI.text += "\n";
                textoUI.text += "\n";

                foreach (Jugador jugador in mejoresJugadores.jugadores)
                {
                    textoUI.text += jugador.Puntuaje.ToString() + "\n";
                    textoUI.text += jugador.Nombre.ToString()
                        + "\n";
                }
            }
        }
    }

   // public void RegistrarJugador(string nombre)
   // {
     //   Jugador nuevoJugador = new Jugador { Nombre = nombre, Puntaje = 0 };
     //   jugadoresCollection.InsertOne(nuevoJugador);

    //    Debug.Log("Jugador registrado en MongoDB: " + nombre);
   // }



   // void ConsultarMejoresPuntajes()
    //{
//        var collection = database.GetCollection<BsonDocument>("jugadores");

  //      var filtro = Builders<BsonDocument>.Filter.Empty;

    //    var opcionesOrden = Builders<BsonDocument>.Sort.Descending("Puntaje");

//        var resultados = collection.Find(filtro).Sort(opcionesOrden).Limit(5).ToList();

        // Procesa los resultados.
//        foreach (var documento in resultados)
 //       {
 //          var nombre = documento["Nombre"].AsString;
 //           var puntaje = documento["Puntaje"].AsInt32;
 //
 //           textoUI.text += $"{nombre}     {puntaje}\n";
  //      }
    //}

    // public void SumarPuntos(string nombreJugador, int puntos)
    // {
    //     var filtro = Builders<Jugador>.Filter.Eq(j => j.Nombre, nombreJugador);
    //     var update = Builders<Jugador>.Update.Inc(j => j.Puntaje, puntos);

    //     jugadoresCollection.UpdateOne(filtro, update);

    //     Debug.Log($"Se sumaron {puntos} puntos al jugador {nombreJugador}.");
    // }
  //  public void SumarPuntos(string nombreJugador, int puntos)
   // {
   //     var filtro = Builders<Jugador>.Filter.Eq(j => j.Nombre, nombreJugador);
   //     var update = Builders<Jugador>.Update.Inc(j => j.Puntaje, puntos);

//       jugadoresCollection.UpdateOne(filtro, update);

        // Obtener el puntaje actualizado después de la actualización
//        var jugadorActualizado = jugadoresCollection.Find(filtro).FirstOrDefault();
//        int nuevoPuntaje = jugadorActualizado != null ? jugadorActualizado.Puntaje : 0;

        // Mostrar solo el puntaje en la interfaz de usuario
//        MostrarPuntajeEnUI(nuevoPuntaje);

      //  Debug.Log($"Se sumaron {puntos} puntos al jugador {nombreJugador}. Puntaje actual: {nuevoPuntaje}");
//    }

   public void MostrarPuntajeEnUI(int puntaje)
    {
        scoreInitial += puntaje;
        print(scoreInitial.ToString());
        score.text = scoreInitial.ToString();
    
    }   

}

//public class Jugador
//{
//    [BsonId]
//    [BsonRepresentation(BsonType.ObjectId)]
//    public string Id { get; set; }
//    public string Nombre { get; set; }
//    public int Puntaje { get; set; }
//}
