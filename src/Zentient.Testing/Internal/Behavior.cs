using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using Zentient.Abstractions.Testing;

namespace Zentient.Testing.Internal
{
    /// <summary>
    /// Represents a behavior configured for a mocked method.
    /// </summary>
    /// <param name="Method">Method info for the method this behavior applies to.</param>
    /// <param name="Predicate">Predicate to match arguments.</param>
    /// <param name="Action">Action to execute when matched.</param>
    /// <param name="ReturnValue">Return value to return when matched.</param>
    /// <param name="ExceptionToThrow">Exception to throw when matched.</param>
    public sealed record Behavior(
        MethodInfo Method,
        Func<object?[], bool> Predicate,
        Func<object?[], object?>? Action,
        object? ReturnValue,
        Exception? ExceptionToThrow);
}
