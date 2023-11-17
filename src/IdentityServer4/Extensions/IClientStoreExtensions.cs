// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using IdentityServer4.Stores;
using System.Threading.Tasks;
using IdentityServer4.Storage.Models;
using IdentityServer4.Storage.Stores;

namespace IdentityServer4.Extensions
{
    /// <summary>
    /// Extension for IClientStore
    /// </summary>
    public static class IClientStoreExtensions
    {
        /// <summary>
        /// Finds the enabled client by identifier.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <returns></returns>
        public static async Task<Client> FindEnabledClientByIdAsync(this IClientStore store, string clientId)
        {
            var client = await store.FindClientByIdAsync(clientId);
            if (client != null && client.Enabled) return client;

            return null;
        }
    }
}