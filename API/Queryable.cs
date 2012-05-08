using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AniSharp.API.Model.Answer;

namespace AniSharp.API
{
    /// <summary>
    /// Defines something, that can be queried
    /// </summary>
    public interface Queryable
    {
        /// <summary>
        /// Sends a request, and receives the answer. Blocks!
        /// </summary>
        /// <param name="req">The request</param>
        /// <returns>The answer</returns>
        ApiAnswer query(ApiRequest req);

        /// <summary>
        /// Asks the Queryable to close sockets etc.
        /// </summary>
        void shutdown();
    }
}
