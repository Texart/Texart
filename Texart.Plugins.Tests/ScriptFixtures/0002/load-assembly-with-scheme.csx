#r "Texart.Api.dll"
#r "file://./Texart.ScriptInterface.dll"

using Texart.ScriptInterface;

if (typeof(Tx).Assembly.GetName().Name == "Texart.ScriptInterface")
{
    return 42;
}
else
{
    return -1;
}
