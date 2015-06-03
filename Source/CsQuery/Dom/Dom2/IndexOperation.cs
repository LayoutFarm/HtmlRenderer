using System;
using System.Collections.Generic;

using System.Text;

namespace CsQuery.Engine
{
    public enum IndexOperationType
    {
        /// <summary>
        /// Adds to the index
        /// </summary>
        Add=1,
        /// <summary>
        /// Remove from the index.
        /// </summary>
        Remove=2,
        /// <summary>
        /// Change the value only.
        /// </summary>
        Change =3
    }

    public struct IndexOperation
    {
        public IndexOperationType IndexOperationType;
        public ushort[] Key;
        public IDomObject Value;
    }
}
