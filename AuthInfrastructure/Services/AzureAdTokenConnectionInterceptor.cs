using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AuthInfrastructure.Services
{
    public class AzureAdTokenConnectionInterceptor : DbConnectionInterceptor
    {
        private readonly TokenCredential _credential;
        public AzureAdTokenConnectionInterceptor(TokenCredential credential) => _credential = credential;

        public override async ValueTask<InterceptionResult> ConnectionOpeningAsync(DbConnection connection, ConnectionEventData eventData, InterceptionResult result, CancellationToken cancellationToken = default)
        {
            if (connection is SqlConnection sqlConn)
            {
                var token = await _credential.GetTokenAsync(
                    new TokenRequestContext(new[] { "https://database.windows.net/.default" }),
                    cancellationToken);
                sqlConn.AccessToken = token.Token;
            }

            return await base.ConnectionOpeningAsync(connection, eventData, result, cancellationToken);
        }
    }
}