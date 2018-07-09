using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MicroBluer.Common
{
    public static partial class Extensions
    {
        public static string ToJson<T>(this T model)
        {
            return JsonConvert.SerializeObject(model);
        }

        public static T FromJson<T>(this object model)
        {
            if (model == null) return default(T);
            return JsonConvert.DeserializeObject<T>(model.ToString());
        }
    }
}
