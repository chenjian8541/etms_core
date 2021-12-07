using Newtonsoft.Json;

namespace Com.Fubei.OpenApi.Sdk.Extensions
{
    public static class ObjectInvokerExtension
    {
        private static readonly JsonSerializerSettings DefaultJsonSerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };


        public static string SerializeAsJson<T>(this T a) => JsonConvert.SerializeObject(a, DefaultJsonSerializerSettings);
    }
}
