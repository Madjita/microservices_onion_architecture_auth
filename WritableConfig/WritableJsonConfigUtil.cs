using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using WritableConfig.Services.SystemTextJson;

namespace WritableConfig
{
    public static class WritableJsonConfigUtil
    {
        public static void CopyFieldsFromObject<T>(this T objTo, T objFrom)
        {
            if (objTo == null)
                throw new ArgumentNullException(nameof(objTo));

            if (objFrom == null)
                throw new ArgumentNullException(nameof(objFrom));

            var propInfo = objFrom.GetType().GetProperties();
            foreach (var item in propInfo)
            {
                var tmp = objTo.GetType().GetProperty(item.Name);
                if (tmp == null || !tmp.CanWrite) continue;
                tmp.SetValue(objTo, item.GetValue(objFrom, null), null);
            }
        }
    }
}