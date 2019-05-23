namespace Texart.Api
{
    public delegate TResult Factory<out TResult, in T>(T input);
}
