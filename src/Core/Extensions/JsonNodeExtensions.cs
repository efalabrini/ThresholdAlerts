using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Core.Extensions;

public static class JsonNodeExtensions
{
    public static string GetValue(this JsonNode jsonNode, string path)
    {

        string result = string.Empty;
        var arr = path.Split('.');

        if (arr.Length == 1)
        {
            result = jsonNode![arr[0]]!.ToString();
        }

        if (arr.Length == 2)
        {
            result = jsonNode![arr[0]]![arr[1]]!.ToString();
        }

        if (arr.Length == 3)
        {
            result = jsonNode![arr[0]]![arr[1]]![arr[2]]!.ToString();
        }


        return result;

    }
}
