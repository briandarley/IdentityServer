using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace IdentityServer4.EntityFramework.Storage.Interfaces
{
    /// <summary>
    /// Abstraction for the operational data context.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IPersistedGrantDbContext : IDisposable
    {
        /// <summary>
        /// Gets or sets the persisted grants.
        /// </summary>
        /// <value>
        /// The persisted grants.
        /// </value>
        DbSet<Entities.PersistedGrant> PersistedGrants { get; set; }

        /// <summary>
        /// Gets or sets the device flow codes.
        /// </summary>
        /// <value>
        /// The device flow codes.
        /// </value>
        DbSet<Entities.DeviceFlowCodes> DeviceFlowCodes { get; set; }

        /// <summary>
        /// Saves the changes.
        /// </summary>
        /// <returns></returns>
        Task<int> SaveChangesAsync();
    }
}