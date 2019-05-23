using System.IO;
namespace Texart.Plugins
{
    public delegate T Factory<out T>(Stream jsonStream);
}