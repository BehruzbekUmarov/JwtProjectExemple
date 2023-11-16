using System.Reflection;

namespace Jwt.Domain.Options;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
