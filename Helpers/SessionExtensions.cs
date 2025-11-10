using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace CrudMVCApp.Helpers
{
    public static class SessionExtensions
    {
        //Clase de extension para guardar o recuperar datos en la session
        public static void SetObject(this ISession session, string key, object value) //Guardar objeto
        {
            session.SetString(key, JsonConvert.SerializeObject(value)); //Guarda el Json como string en la sesion 
        }

        public static T GetObject<T>(this ISession session, string key) //Recuperar objeto
        {
            var value = session.GetString(key); 

            return value == null ? default : JsonConvert.DeserializeObject<T>(value); //Obtiene el string Json almacenado 
        }
    }
}

