//BSD 2014, WinterCore

// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using System;
using System.Collections.Generic;
using HtmlRenderer.Entities;
using HtmlRenderer.Parse;
using HtmlRenderer.Utils;

namespace HtmlRenderer
{
    public class CssMediaBlock
    {
        Dictionary<string, CssCodeBlockGroup> cssCodeBlockCollections = new Dictionary<string, CssCodeBlockGroup>(StringComparer.InvariantCultureIgnoreCase);
        public CssMediaBlock(string mediaName)
        {
            this.MediaName = mediaName;
        }
        public string MediaName
        {
            get;
            private set;
        }
        public IEnumerable<CssCodeBlock> GetCodeBlockIter(string className)
        {
            CssCodeBlockGroup blockGroup;
            if (this.cssCodeBlockCollections.TryGetValue(className, out blockGroup))
            {
                int j = blockGroup.Count;
                for (int i = 0; i < j; ++i)
                {
                    yield return blockGroup.GetBlock(i);
                }
            }
        }
        public IEnumerable<CssCodeBlock> GetCodeBlockIter()
        {

            foreach (CssCodeBlockGroup group in cssCodeBlockCollections.Values)
            {
                int j = group.Count;
                for (int i = 0; i < j; ++i)
                {
                    yield return group.GetBlock(i);
                }
            }
        }
        public void AddCodeBlock(CssCodeBlock cssBlock)
        {

            CssCodeBlockGroup foundGroup;
            if (!cssCodeBlockCollections.TryGetValue(cssBlock.CssClassName, out foundGroup))
            {
                //if not found create new one
                foundGroup = new CssCodeBlockGroup(cssBlock.CssClassName);
                cssCodeBlockCollections.Add(cssBlock.CssClassName, foundGroup);
            }
            foundGroup.AddCodeBlock(cssBlock);

        }
        public CssMediaBlock Clone()
        {
            CssMediaBlock newclone = new CssMediaBlock(this.MediaName);
            foreach (var group in this.cssCodeBlockCollections.Values)
            {
                newclone.cssCodeBlockCollections.Add(group.Name, group);
            }
            return newclone;
        }

        public IEnumerable<CssCodeBlockGroup> GetCodeBlockGroupIter()
        {
            foreach (CssCodeBlockGroup group in this.cssCodeBlockCollections.Values)
            {
                yield return group;
            }
        }



    }

    public class CssCodeBlockGroup
    {
        List<CssCodeBlock> cssCodeBlocks = new List<CssCodeBlock>();
        public CssCodeBlockGroup(string name)
        {
            this.Name = name;
        }
        public string Name
        {
            get;
            private set;
        }
        public int Count
        {
            get
            {
                return this.cssCodeBlocks.Count;
            }
        }
        public CssCodeBlock GetBlock(int index)
        {
            return this.cssCodeBlocks[index];
        }
        public void AddCodeBlock(CssCodeBlock cssBlock)
        {
            //block has class name and may be additional selectors
            bool merged = false;
            //var list = mid[cssBlock.CssClassName];

            foreach (CssCodeBlock myblock in cssCodeBlocks)
            {
                if (myblock.EqualsSelector(cssBlock))
                {
                    merged = true;
                    myblock.Merge(cssBlock);
                    break;
                }
            }

            if (!merged)
            {
                // general block must be first
                if (cssBlock.Selectors == null)
                    cssCodeBlocks.Insert(0, cssBlock);
                else
                    cssCodeBlocks.Add(cssBlock);
            }
        }
        public CssCodeBlockGroup Clone()
        {
            CssCodeBlockGroup newGroup = new CssCodeBlockGroup(this.Name);
            foreach (var block in cssCodeBlocks)
            {
                //?
                newGroup.AddCodeBlock(block);
            }
            return newGroup;
        }
    }


    /// <summary>
    /// Holds parsed stylesheet css blocks arranged by media and classes.<br/>
    /// <seealso cref="CssCodeBlock"/>
    /// </summary>
    /// <remarks>
    /// To learn more about CSS blocks visit CSS spec: http://www.w3.org/TR/CSS21/syndata.html#block
    /// </remarks>
    public sealed class CssSheet
    {


        /// <summary>
        /// used to return empty array
        /// </summary>
        private static readonly List<CssCodeBlock> _emptyList = new List<CssCodeBlock>();

        /// <summary>
        /// dictionary of media type to dictionary of css class name to the cssBlocks collection with all the data.
        /// </summary>
        private readonly Dictionary<string, CssMediaBlock> _mediaBlocks = new Dictionary<string, CssMediaBlock>();

        CssMediaBlock defaultAllMediaBlock;

        /// <summary>
        /// Init.
        /// </summary>
        internal CssSheet()
        {
            defaultAllMediaBlock = new CssMediaBlock("all");
            _mediaBlocks.Add("all", defaultAllMediaBlock);
        }

        /// <summary>
        /// Parse the given stylesheet to <see cref="CssSheet"/> object.<br/>
        /// If <paramref name="combineWithDefault"/> is true the parsed css blocks are added to the 
        /// default css data (as defined by W3), merged if class name already exists. If false only the data in the given stylesheet is returned.
        /// </summary>
        /// <seealso cref="http://www.w3.org/TR/CSS21/sample.html"/>
        /// <param name="stylesheet">the stylesheet source to parse</param>
        /// <param name="combineWithDefault">true - combine the parsed css data with default css data, false - return only the parsed css data</param>
        /// <returns>the parsed css data</returns>
        public static CssSheet Parse(string stylesheet, bool combineWithDefault = true)
        {
            return CssParser.ParseStyleSheet(stylesheet, combineWithDefault);
        }

        public CssMediaBlock DefaultMediaBlock
        {
            get
            {
                return this.defaultAllMediaBlock;
            }

        }
        ///// <summary>
        ///// dictionary of media type to dictionary of css class name to the cssBlocks collection with all the data
        ///// </summary>
        //internal IDictionary<string, Dictionary<string, List<CssCodeBlock>>> MediaBlocks
        //{
        //    get { return _mediaBlocks; }
        //}

        internal IEnumerable<CssMediaBlock> GetMediaBlockIter()
        {
            foreach (var media in this._mediaBlocks.Values)
            {
                yield return media;
            }
        }

        ///// <summary>
        ///// Check if there are css blocks for the given class selector.
        ///// </summary>
        ///// <param name="className">the class selector to check for css blocks by</param>
        ///// <param name="media">optinal: the css media type (default - all)</param>
        ///// <returns>true - has css blocks for the class, false - otherwise</returns>
        //public bool ContainsCssBlock(string className, string media = "all")
        //{
        //    Dictionary<string, List<CssCodeBlock>> mid;
        //    return _mediaBlocks.TryGetValue(media, out mid) && mid.ContainsKey(className);
        //}

        /// <summary>
        /// Get collection of css blocks for the requested class selector.<br/>
        /// the <paramref name="className"/> can be: class name, html element name, html element and 
        /// class name (elm.class), hash tag with element id (#id).<br/>
        /// returned all the blocks that word on the requested class selector, it can contain simple
        /// selector or hierarchy selector.
        /// </summary>
        /// <param name="className">the class selector to get css blocks by</param>
        /// <param name="media">optinal: the css media type (default - all)</param>
        /// <returns>collection of css blocks, empty collection if no blocks exists (never null)</returns>
        public IEnumerable<CssCodeBlock> GetCssBlock(string className, string media = "all")
        {
            //iter 
            CssMediaBlock mediaBlock;
            if (_mediaBlocks.TryGetValue(media, out mediaBlock))
            {
                return mediaBlock.GetCodeBlockIter(className);
            }
            return _emptyList;
        }


        /// <summary>
        /// Add the given css block to the css data, merging to existing block if required.
        /// </summary>
        /// <remarks>
        /// If there is no css blocks for the same class it will be added to data collection.<br/>
        /// If there is already css blocks for the same class it will check for each existing block
        /// if the hierarchical selectors match (or not exists). if do the two css blocks will be merged into
        /// one where the new block properties overwrite existing if needed. if the new block doesn't mach any
        /// existing it will be added either to the beggining of the list if it has no  hierarchical selectors or at the end.<br/>
        /// Css block without hierarchical selectors must be added to the beginning of the list so more specific block
        /// can overwrite it when the style is applied.
        /// </remarks>
        /// <param name="media">the media type to add the CSS to</param>
        /// <param name="cssBlock">the css block to add</param>
        public void AddCssBlock(string mediaName, CssCodeBlock cssBlock)
        {
            CssMediaBlock found;
            if (!_mediaBlocks.TryGetValue(mediaName, out found))
            {
                found = new CssMediaBlock(mediaName);
                _mediaBlocks.Add(mediaName, found);
            }
            found.AddCodeBlock(cssBlock);
        }

        /// <summary>
        /// Combine this CSS data blocks with <paramref name="other"/> CSS blocks for each media.<br/>
        /// Merge blocks if exists in both.
        /// </summary>
        /// <param name="other">the CSS data to combine with</param>
        public void Combine(CssSheet other)
        {
            ArgChecker.AssertArgNotNull(other, "other");
            // for each media block
            foreach (CssMediaBlock mediaBlock in other.GetMediaBlockIter())
            {
                // for each css class in the media block
                foreach (var group in mediaBlock.GetCodeBlockGroupIter())
                {
                    // for each css block of the css class
                    int j = group.Count;
                    for (int i = 0; i < j; ++i)
                    {
                        AddCssBlock(mediaBlock.MediaName, group.GetBlock(i));
                    }
                }
            }
        }

        /// <summary>
        /// Create deep copy of the css data with cloned css blocks.
        /// </summary>
        /// <returns>cloned object</returns>
        public CssSheet Clone()
        {
            var newClone = new CssSheet();
            foreach (CssMediaBlock mediaBlock in _mediaBlocks.Values)
            {
                newClone._mediaBlocks[mediaBlock.MediaName] = mediaBlock.Clone();
            }
            return newClone;
        }
    }
}
