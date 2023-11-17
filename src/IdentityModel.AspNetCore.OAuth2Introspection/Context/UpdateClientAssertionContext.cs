using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using UNC.IdentityModel.Client.Messages;

namespace IdentityModel.AspNetCore.OAuth2Introspection
{
    /// <summary>
    /// Context for the UpdateClientAssertion event
    /// </summary>
    public class UpdateClientAssertionContext : ResultContext<OAuth2IntrospectionOptions>
    {
        /// <summary>
        /// ctor
        /// </summary>
        public UpdateClientAssertionContext(
            HttpContext context,
            AuthenticationScheme scheme,
            OAuth2IntrospectionOptions options)
            : base(context, scheme, options) { }

        /// <summary>
        /// The client assertion
        /// </summary>
        public ClientAssertion ClientAssertion { get; set; }

        /// <summary>
        /// The client assertion expiration time
        /// </summary>
        public DateTime ClientAssertionExpirationTime { get; set; }
    }
}