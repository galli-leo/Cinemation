using System;

namespace Cinemation.Core.Indexers
{
    internal abstract class Indexer
    {

        /// <summary>
        /// Gets the base <see cref="Uri"/>.
        /// </summary>
        /// <returns>Returns the base <see cref="Uri"/>.</returns>
        public abstract Uri GetBaseUri();

    }
}
