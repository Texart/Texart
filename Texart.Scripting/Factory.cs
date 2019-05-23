using System;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Texart.Plugins
{
    public delegate T Factory<out T>(Lazy<JToken> json);
}