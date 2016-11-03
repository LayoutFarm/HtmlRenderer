using System;
using System.Collections.Generic;

using System.Text;
using System.IO;

using CsQuery.HtmlParser;
using CsQuery.Engine;

namespace CsQuery.Implementation
{
    /// <summary>
    /// An incomplete document fragment
    /// </summary>
    public class DomFragment : DomDocument, IDomFragment
    {
        

        /// <summary>
        /// Default constructor.
        /// </summary>

        public DomFragment()
            : base()
        {
        }

        /// <summary>
        /// Create a new DomFragment using the provided DomIndex instance.
        /// </summary>
        ///
        /// <param name="domIndex">
        /// A DomIndex provider
        /// </param>

        public DomFragment(IDomIndex domIndex)
            : base(domIndex)
        {
            
        }

        /// <summary>
        /// Gets the type of the node. For DomFragment objects, this is always NodeType.DOCUMENT_FRAGMENT_NODE.
        /// </summary>

        public override NodeType NodeType
        {
            get { return  NodeType.DOCUMENT_FRAGMENT_NODE; }
        }

        /// <summary>
        /// Gets a value indicating whether this object is indexed. 
        /// </summary>

        public override bool IsIndexed
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this object is fragment. For DomFragment objects, this is
        /// true.
        /// </summary>

        public override bool IsFragment
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Creates a new instance of a DomFragment.
        /// </summary>
        ///
        /// <returns>
        /// The new new.
        /// </returns>

        public override IDomDocument CreateNew()
        {
            return CreateNew<IDomFragment>();
        }
    }
    
}
