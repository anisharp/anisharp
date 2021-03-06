﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AniSharp.API.Model.Answer;
using AniSharp.API.Model.Request;

namespace AniSharp.API.Application
{
    /// <summary>
    /// Defines something, that can be queried
    /// </summary>
    interface Queryable
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
