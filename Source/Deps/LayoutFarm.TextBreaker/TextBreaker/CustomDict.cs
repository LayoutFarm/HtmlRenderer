//MIT, 2016, WinterDev
// some code from icu-project
// © 2016 and later: Unicode, Inc. and others.
// License & terms of use: http://www.unicode.org/copyright.html#License

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LayoutFarm.TextBreaker
{
    /// <summary>
    /// my custom dic
    /// </summary>
    public class CustomDic
    {
        TextBuffer textBuffer;
        Dictionary<char, WordGroup> wordGroups = new Dictionary<char, WordGroup>();
        internal TextBuffer TextBuffer { get { return textBuffer; } }
        char firstChar, lastChar;
        public void SetCharRange(char firstChar, char lastChar)
        {
            this.firstChar = firstChar;
            this.lastChar = lastChar;
        }
        public char FirstChar { get { return firstChar; } }
        public char LastChar { get { return lastChar; } }
        public void LoadFromTextfile(string filename)
        {
            //once only
            if (textBuffer != null)
            {
                return;
            }
            //---------------
            using (FileStream fs = new FileStream(filename, FileMode.Open))
            using (StreamReader reader = new StreamReader(fs))
            {
                //init with filesize
                textBuffer = new TextBuffer((int)fs.Length);
                string line = reader.ReadLine();
                while (line != null)
                {
                    line = line.Trim();
                    char[] lineBuffer = line.ToCharArray();
                    int lineLen = lineBuffer.Length;
                    char c0;
                    if (lineLen > 0 && (c0 = lineBuffer[0]) != '#')
                    {
                        int startAt = textBuffer.CurrentPosition;
                        textBuffer.AddWord(lineBuffer);

#if DEBUG
                        if (lineLen > byte.MaxValue)
                        {
                            throw new NotSupportedException();
                        }
#endif

                        WordSpan wordspan = new WordSpan(startAt, (byte)lineLen);
                        //each wordgroup contains text span

                        WordGroup found;
                        if (!wordGroups.TryGetValue(c0, out found))
                        {
                            found = new WordGroup(new WordSpan(startAt, 1));
                            wordGroups.Add(c0, found);
                        }
                        found.AddWordSpan(wordspan);

                    }
                    //- next line
                    line = reader.ReadLine();
                }

                reader.Close();
                fs.Close();
            }
            //------------------------------------------------------------------
            textBuffer.Freeze();
            //do index
            DoIndex();
        }

        void DoIndex()
        {
            foreach (WordGroup wordGroup in wordGroups.Values)
            {
                wordGroup.DoIndex(this.textBuffer, this);
            } 
        }


        public void GetWordList(char startWithChar, List<string> output)
        {
            WordGroup found;
            if (wordGroups.TryGetValue(startWithChar, out found))
            {
                //iterate and collect into 
                found.CollectAllWords(this.textBuffer, output);
            }
        }
        internal WordGroup GetWordGroupForFirstChar(char c)
        {
            WordGroup wordGroup;
            if (wordGroups.TryGetValue(c, out wordGroup))
            {
                return wordGroup;
            }
            return null;
        }
    }


    struct WordSpan
    {
        public readonly int startAt;
        public readonly byte len;

        public WordSpan(int startAt, byte len)
        {
            this.startAt = startAt;
            this.len = len;
        }
        public char GetChar(int index, TextBuffer textBuffer)
        {
            return textBuffer.GetChar(startAt + index);
        }
        public string GetString(TextBuffer textBuffer)
        {
            return textBuffer.GetString(startAt, len);
        }
        public bool SameTextContent(WordSpan another, TextBuffer textBuffer)
        {
            if (another.len == this.len)
            {
                for (int i = another.len - 1; i >= 0; --i)
                {
                    if (this.GetChar(i, textBuffer) != another.GetChar(i, textBuffer))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
            //return this.startAt == another.startAt && this.len == another.len;
        }
    }

    class TextBuffer
    {
        List<char> _tmpCharList;
        int position;
        bool isFrozen;
        char[] charBuffer;
        public TextBuffer(int initCapacity)
        {
            _tmpCharList = new List<char>(initCapacity);
        }
        public void AddWord(char[] wordBuffer)
        {
            _tmpCharList.AddRange(wordBuffer);
            //append with  ' ' 
            _tmpCharList.Add(' ');
            position += wordBuffer.Length + 1;
        }
        public void Freeze()
        {
            charBuffer = _tmpCharList.ToArray();
            _tmpCharList = null;
        }
        public int CurrentPosition
        {
            get { return position; }
        }
        public char GetChar(int index)
        {
            return charBuffer[index];
        }
        public string GetString(int index, int len)
        {
            return new string(this.charBuffer, index, len);
        }
    }


    struct CandidateWord
    {
        public int w_index;
        public int w_len;
        public int max_match;
        public bool IsFullMatch()
        {
            return w_len == max_match;
        }
    }

    public struct BreakSpan
    {
        public int startAt;
        public int len;
    }


    public enum DataState
    {
        UnIndex,
        Indexed,
        TooLongPrefix,
        SmallAmountOfMembers
    }



    public class WordGroup
    {
        List<WordSpan> unIndexWordSpans = new List<WordSpan>();
        //  Dictionary<char, WordGroup> wordGroups;
        WordGroup[] wordGroups2;
        WordSpan prefixSpan;
#if DEBUG
        static int debugTotalId;
        int debugId = debugTotalId++;
#endif
        internal WordGroup(WordSpan prefixSpan)
        {
            this.prefixSpan = prefixSpan;
            this.PrefixLen = prefixSpan.len;
        }
        internal WordSpan PrefixSpan { get { return this.prefixSpan; } }
        public DataState DataState { get; private set; }

        internal string GetPrefix(TextBuffer buffer)
        {
            return prefixSpan.GetString(buffer);
        }
        internal bool PrefixIsWord
        {
            get;
            private set;
        }
        internal void CollectAllWords(TextBuffer textBuffer, List<string> output)
        {
            if (this.PrefixIsWord)
            {
                output.Add(GetPrefix(textBuffer));
            }
            if (wordGroups2 != null)
            {
                foreach (WordGroup wordGroup in wordGroups2)
                {
                    if (wordGroup != null)
                    {
                        wordGroup.CollectAllWords(textBuffer, output);
                    }
                }
            }
            if (unIndexWordSpans != null)
            {
                foreach (var span in unIndexWordSpans)
                {
                    output.Add(span.GetString(textBuffer));
                }
            }
        }
        public int PrefixLen { get; private set; }

        internal void AddWordSpan(WordSpan span)
        {
            unIndexWordSpans.Add(span);
            this.DataState = DataState.UnIndex;
        }
        public int UnIndexMemberCount
        {
            get
            {

                if (unIndexWordSpans == null) return 0;
                return unIndexWordSpans.Count;
            }
        }


        bool _hasEvalPrefix;
        internal void DoIndex(TextBuffer textBuffer, CustomDic owner)
        {
            //recursive
            if (this.PrefixLen > 7)
            {
                this.DataState = DataState.TooLongPrefix;
                return;
            }

            if (wordGroups2 == null)
            {
                //wordGroups = new Dictionary<char, WordGroup>();
                wordGroups2 = new WordGroup[owner.LastChar - owner.FirstChar + 1];
            }
            //--------------------------------
            int j = unIndexWordSpans.Count;
            int thisPrefixLen = this.PrefixLen;
            int doSepAt = thisPrefixLen;
            for (int i = 0; i < j; ++i)
            {
                WordSpan sp = unIndexWordSpans[i];

                if (sp.len > doSepAt)
                {
                    char c = sp.GetChar(doSepAt, textBuffer);

                    int c_index = c - owner.FirstChar;
                    WordGroup found = wordGroups2[c_index];
                    if (found == null)
                    {
                        //not found
                        found = new WordGroup(new WordSpan(sp.startAt, (byte)(doSepAt + 1)));
                        wordGroups2[c_index] = found;
                    }

                    //WordGroup found;
                    //if (!wordGroups.TryGetValue(c, out found))
                    //{
                    //    found = new WordGroup(new WordSpan(sp.startAt, (byte)(doSepAt + 1)));
                    //    wordGroups.Add(c, found);
                    //}

                    found.AddWordSpan(sp);
                }
                else
                {
                    if (!_hasEvalPrefix)
                    {
                        if (sp.SameTextContent(this.prefixSpan, textBuffer))
                        {

                            _hasEvalPrefix = true;
                            this.PrefixIsWord = true;
                        }
                    }
                }

            }
            this.DataState = DataState.Indexed;
            unIndexWordSpans.Clear();
            unIndexWordSpans = null;
            //--------------------------------
            //do sup index
            //foreach (WordGroup subgroup in this.wordGroups.Values)
            bool hasSomeSubGroup = false;
            foreach (WordGroup subgroup in this.wordGroups2)
            {
                if (subgroup != null)
                {
                    hasSomeSubGroup = true;
                    if (subgroup.UnIndexMemberCount > 10)
                    {
                        subgroup.DoIndex(textBuffer, owner);
                    }
                    else
                    {
                        subgroup.DataState = DataState.SmallAmountOfMembers;
                        subgroup.DoIndexOfSmallAmount(textBuffer);
                    }
                }
            }
            //--------------------------------
            this.DataState = DataState.Indexed;

            if (!hasSomeSubGroup)
            {
                //clear
                wordGroups2 = null;
            }
        }
        void DoIndexOfSmallAmount(TextBuffer textBuffer)
        {
            //check ext
            int j = unIndexWordSpans.Count;
            int thisPrefixLen = this.PrefixLen;
            int doSepAt = thisPrefixLen;
            for (int i = 0; i < j; ++i)
            {
                WordSpan sp = unIndexWordSpans[i];
                if (sp.SameTextContent(this.PrefixSpan, textBuffer))
                {
                    this.PrefixIsWord = true;
                    break;
                }
            }
        }

        internal void FindBreak(WordVisitor visitor)
        {
            //recursive
            char c = visitor.Char;
            visitor.FoundWord = false;
            if (wordGroups2 != null)
            {
                int c_index = c - visitor.CurrentCustomDic.FirstChar;
                WordGroup foundSubGroup = wordGroups2[c_index];
                // WordGroup foundSubGroup;
                //if (wordGroups.TryGetValue(c, out foundSubGroup))
                if (foundSubGroup != null)
                {
                    //found next group
                    if (!visitor.IsEnd)
                    {
                        int index = visitor.CurrentIndex;
                        visitor.SetCurrentIndex(index + 1);
                        foundSubGroup.FindBreak(visitor);
                        if (!visitor.FoundWord)
                        {
                            //not found in deeper level
                            visitor.SetCurrentIndex(index);
                        }
                    }
                    else
                    {

                        if (foundSubGroup.PrefixIsWord)
                        {

                            int savedIndex = visitor.CurrentIndex;
                            int newBreakAt = visitor.LatestBreakAt + this.PrefixLen;
                            visitor.SetCurrentIndex(newBreakAt);
                            //check next char can be the char of new word or not
                            //this depends on each lang 
                            char canBeStartChar = visitor.Char;
                            if (visitor.CanbeStartChar(canBeStartChar))
                            {
                                visitor.AddWordBreakAt(newBreakAt + 1);
                                visitor.SetCurrentIndex(visitor.LatestBreakAt);
                            }
                            else
                            {
                                visitor.SetCurrentIndex(savedIndex);
                            }

                        }

                    }
                }
            }
            //-------
            if (!visitor.FoundWord)
            {


                if (unIndexWordSpans != null)
                {
                    //at this wordgroup
                    //no subground anymore
                    //so we should find the word one by one
                    //start at prefix
                    //and select the one that 
                    int pos = visitor.CurrentIndex;
                    int latestBreak = visitor.LatestBreakAt;
                    int len = (pos - latestBreak);
                    int n = unIndexWordSpans.Count;

                    List<CandidateWord> candidateWords = visitor.GetTempCandidateWords();
                    for (int i = 0; i < n; ++i)
                    {
                        //begin new word
                        WordSpan w = unIndexWordSpans[i];
                        int savedIndex = visitor.CurrentIndex;
                        c = visitor.Char;
                        int wordLen = w.len;
                        int matchCharCount = 0;
                        if (wordLen > len)
                        {
                            //char[] wbuff = w.ToCharArray();
                            ////check if this word match or not
                            TextBuffer currentTextBuffer = visitor.CurrentCustomDic.TextBuffer;
                            for (int p = len; p < wordLen; ++p)
                            {
                                char c2 = w.GetChar(p, currentTextBuffer);
                                if (c2 == c)
                                {
                                    matchCharCount++;
                                    //match 
                                    //read next
                                    if (!visitor.IsEnd)
                                    {
                                        visitor.SetCurrentIndex(visitor.CurrentIndex + 1);
                                        c = visitor.Char;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        //reset
                        if (matchCharCount > 0)
                        {
                            CandidateWord candidate = new CandidateWord();
                            candidate.w_index = i;
                            candidate.w_len = wordLen;
                            candidate.max_match = len + matchCharCount;
                            if (candidate.IsFullMatch())
                            {
                                candidateWords.Add(candidate);
                            }
                        }
                        visitor.SetCurrentIndex(savedIndex);
                    }

                    if (candidateWords.Count == 1)
                    {
                        CandidateWord candidate = candidateWords[0];

                        int savedIndex = visitor.CurrentIndex;
                        int newBreakAt = visitor.LatestBreakAt + candidate.max_match;
                        visitor.SetCurrentIndex(newBreakAt);
                        //check next char can be the char of new word or not
                        //this depends on each lang 
                        char canBeStartChar = visitor.Char;
                        if (visitor.CanbeStartChar(canBeStartChar))
                        {
                            visitor.AddWordBreakAt(newBreakAt);
                            visitor.SetCurrentIndex(newBreakAt);
                        }
                        else
                        {
                            visitor.SetCurrentIndex(savedIndex);
                        }


                    }
                    else if (candidateWords.Count > 0)
                    {
                        CandidateWord candidate = candidateWords[candidateWords.Count - 1];

                        int savedIndex = visitor.CurrentIndex;
                        int newBreakAt = visitor.LatestBreakAt + candidate.max_match;
                        visitor.SetCurrentIndex(newBreakAt);
                        //check next char can be the char of new word or not
                        //this depends on each lang 
                        char canBeStartChar = visitor.Char;
                        if (visitor.CanbeStartChar(canBeStartChar))
                        {
                            visitor.AddWordBreakAt(newBreakAt);
                            visitor.SetCurrentIndex(newBreakAt);
                        }
                        else
                        {
                            visitor.SetCurrentIndex(savedIndex);
                        }

                        //visitor.AddWordBreakAt(
                        //       visitor.LatestBreakAt + candidate.max_match);
                        //visitor.SetCurrentIndex(visitor.LatestBreakAt);


                    }
                    else
                    {

                    }
                }
                if (!visitor.FoundWord)
                {
                    if (this.PrefixIsWord)
                    {
                        int savedIndex = visitor.CurrentIndex;
                        int newBreakAt = visitor.LatestBreakAt + this.PrefixLen;
                        visitor.SetCurrentIndex(newBreakAt);
                        //check next char can be the char of new word or not
                        //this depends on each lang 
                        char canBeStartChar = visitor.Char;
                        if (visitor.CanbeStartChar(canBeStartChar))
                        {
                            visitor.AddWordBreakAt(newBreakAt);
                            visitor.SetCurrentIndex(newBreakAt);
                        }
                        else
                        {
                            visitor.SetCurrentIndex(savedIndex);
                        }
                    }
                }
            }
            else
            {


                //if (unIndexMemberWords != null)
                //{
                //    //at this wordgroup
                //    //no subground anymore
                //    //so we should find the word one by one
                //    //start at prefix
                //    //and select the one that 

                //}
            }

        }

#if DEBUG
        public override string ToString()
        {
            StringBuilder stbuilder = new StringBuilder();
            stbuilder.Append(this.prefixSpan.startAt + " " + this.prefixSpan.len);
            stbuilder.Append(" " + this.DataState);
            //---------  

            if (unIndexWordSpans != null)
            {
                stbuilder.Append(",u_index=" + unIndexWordSpans.Count + " ");
            }
            return stbuilder.ToString();
        }
#endif

    }


}