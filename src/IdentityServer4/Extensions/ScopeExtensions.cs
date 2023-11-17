// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using IdentityServer4.Storage.Models;

namespace IdentityServer4.Extensions
{
    internal static class ScopeExtensions
    {
        [DebuggerStepThrough]
        public static string ToSpaceSeparatedString(this IEnumerable<ApiScope> apiScopes)
        {
            var scopeNames = from s in apiScopes
                             select s.Name;

            return string.Join(" ", scopeNames.ToArray());
        }

        [DebuggerStepThrough]
        public static IEnumerable<string> ToStringList(this IEnumerable<ApiScope> apiScopes)
        {
            var scopeNames = from s in apiScopes
                             select s.Name;

            return scopeNames;
        }
    }
}