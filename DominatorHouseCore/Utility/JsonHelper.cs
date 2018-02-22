using System;

namespace DominatorHouseCore.Utility
{
    public class JsonHelper
    {
        public static T GetModel<T>(string json) where T : class
        {
            return string.IsNullOrEmpty(json) ? null : Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }

        public static string GetJson<T>(T obj) where T : class
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

    }
}
