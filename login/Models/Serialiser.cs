using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace login.Models
{
    public class Serialiser
    {
        public static string SerializeObject(Object toSerialize)
        {
            return new JavaScriptSerializer().Serialize(toSerialize);
        }

        public static T DeSerializeObject<T>(string json)
        {
            return new JavaScriptSerializer().Deserialize<T>(json);
        }

        public static T DeSerializeObject2<T>(string json) where T : new()
        {
            if (json == null)
                json = "[]";
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static T DeSerializeObject3<T>(string json) where T : new()
        {
            if (json == null)
                return new T();

            T res = new T();
            try
            {
                res = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
                res = new T();
            }
            return res;
        }

        public static T ReadFromJsonFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                reader = new StreamReader(filePath);
                var fileContents = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(fileContents);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
    }
}