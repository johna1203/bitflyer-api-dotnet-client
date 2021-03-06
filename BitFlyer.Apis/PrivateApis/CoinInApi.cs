﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BitFlyer.Apis
{
    public partial class PrivateApi
    {
        private const string CoinInApiPath = "/v1/me/getcoinins";

        public async Task<CoinIn[]> GetCoinIns(AddresseType type = AddresseType.Normal,
            int? count = null, int? before = null, int? after = null,
            CancellationToken cancellationToken = default)
        {
            var query = new Dictionary<string, object>();

            if (count != null)
            {
                query["count"] = count.Value;
            }
            if (before != null)
            {
                query["before"] = before.Value;
            }
            if (after != null)
            {
                query["after"] = after.Value;
            }

            return await Get<CoinIn[]>(CoinInApiPath, query, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
