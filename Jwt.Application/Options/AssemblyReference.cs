﻿using System.Reflection;

namespace Jwt.Application.Options;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
