using System.Reflection;

namespace BarberFlow.Api;

public static class ApiAssembly
{
    public static readonly Assembly Instance = typeof(ApiAssembly).Assembly;
}
