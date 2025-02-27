// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using System.Threading.Tasks;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores.Serialization;
using Microsoft.Extensions.Logging;
using Duende.IdentityServer.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Duende.IdentityServer.Stores;

/// <summary>
/// Default authorization code store.
/// </summary>
public class DefaultBackChannelAuthenticationRequestStore : DefaultGrantStore<BackChannelAuthenticationRequest>, IBackChannelAuthenticationRequestStore
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultAuthorizationCodeStore"/> class.
    /// </summary>
    /// <param name="store">The store.</param>
    /// <param name="serializer">The serializer.</param>
    /// <param name="handleGenerationService">The handle generation service.</param>
    /// <param name="logger">The logger.</param>
    public DefaultBackChannelAuthenticationRequestStore(
        IPersistedGrantStore store,
        IPersistentGrantSerializer serializer,
        IHandleGenerationService handleGenerationService,
        ILogger<DefaultBackChannelAuthenticationRequestStore> logger)
        : base(IdentityServerConstants.PersistedGrantTypes.BackChannelAuthenticationRequest, store, serializer, handleGenerationService, logger)
    {
    }

    /// <inheritdoc/>
    public async Task<string> CreateRequestAsync(BackChannelAuthenticationRequest request)
    {
        var handle = await CreateHandleAsync();
        request.InternalId = GetHashedKey(handle);
        await StoreItemByHashedKeyAsync(request.InternalId, request, request.ClientId, request.Subject.GetSubjectId(), null, null, request.CreationTime, request.CreationTime.AddSeconds(request.Lifetime));
        return handle;
    }

    /// <inheritdoc/>
    public Task<BackChannelAuthenticationRequest> GetByInternalIdAsync(string id)
    {
        return GetItemByHashedKeyAsync(id);
    }

    /// <inheritdoc/>
    public Task<BackChannelAuthenticationRequest> GetByAuthenticationRequestIdAsync(string requestId)
    {
        return GetItemAsync(requestId);
    }

    /// <inheritdoc/>
    public Task RemoveByInternalIdAsync(string requestId)
    {
        return RemoveItemByHashedKeyAsync(requestId);
    }

    /// <inheritdoc/>
    public Task<IEnumerable<BackChannelAuthenticationRequest>> GetLoginsForUserAsync(string subjectId, string clientId = null)
    {
        return GetAllAsync(new PersistedGrantFilter {
            SubjectId = subjectId,
            ClientId = clientId,
        });
    }

    /// <inheritdoc/>
    public Task UpdateByInternalIdAsync(string id, BackChannelAuthenticationRequest request)
    {
        return StoreItemByHashedKeyAsync(id, request, request.ClientId, request.Subject.GetSubjectId(), request.SessionId, request.Description, request.CreationTime, request.CreationTime.AddSeconds(request.Lifetime));
    }
}