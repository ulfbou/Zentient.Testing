// <copyright file="Behavior.cs" authors="Zentient Framework Team">
// Copyright © 2025 Zentient Framework Team. All rights reserved.
// </copyright>

using System;
using System.Reflection;

namespace Zentient.Testing.Internal
{
    /// <summary>
    /// Represents a configured behavior for a mocked method, including matching predicate and outcome.
    /// </summary>
    /// <param name="Method">Method information this behavior applies to.</param>
    /// <param name="Predicate">Predicate that evaluates call argument arrays.</param>
    /// <param name="Action">Optional action to execute for matched calls.</param>
    /// <param name="ReturnValue">Optional return value for matched calls.</param>
    /// <param name="ExceptionToThrow">Optional exception to throw when the behavior matches.</param>
    public sealed record Behavior(
        MethodInfo Method,
        Func<object?[], bool> Predicate,
        Func<object?[], object?>? Action,
        object? ReturnValue,
        Exception? ExceptionToThrow);
}
