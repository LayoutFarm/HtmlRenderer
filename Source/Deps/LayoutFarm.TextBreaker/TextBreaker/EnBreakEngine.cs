//MIT, 2016, WinterDev 

namespace LayoutFarm.TextBreaker
{

    public class EnTextBreaker : TextBreaker
    {

        enum LexState
        {
            Init,
            Whitespace,
            Text,
            Number,
        }
        BreakBounds breakBounds = new BreakBounds();
        public override void DoBreak(char[] input, int start, int len, OnBreak onbreak)
        {
            //----------------------------------------
            //simple break word/ num/ punc / space
            //similar to lexer function            
            //----------------------------------------
            LexState lexState = LexState.Init;
            int endBefore = start + len;
            for (int i = start; i < endBefore; ++i)
            {
                char c = input[i];
                switch (lexState)
                {
                    case LexState.Init:
                        {
                            //check char
                            if (c == '\r')
                            {
                                //check next if '\n'
                                if (i < endBefore - 1)
                                {
                                    if (input[i + 1] == '\n')
                                    {
                                        //this is '\r\n' linebreak
                                        breakBounds.startIndex = i;
                                        breakBounds.length = 2;
                                        breakBounds.kind = WorkKind.NewLine;
                                        onbreak(breakBounds);
                                        breakBounds.length = 0;
                                        lexState = LexState.Init;

                                        i++;
                                        continue;
                                    }
                                }
                                else
                                {
                                    //sinple \r?
                                    //to whitespace?
                                    lexState = LexState.Whitespace;
                                    breakBounds.startIndex = i;
                                }
                            }
                            else if (c == '\n')
                            {
                                breakBounds.startIndex = i;
                                breakBounds.length = 1;
                                breakBounds.kind = WorkKind.NewLine;
                                onbreak(breakBounds);
                                breakBounds.length = 0;
                                lexState = LexState.Init;
                                continue;
                            }
                            else if (char.IsLetter(c))
                            {
                                //just collect
                                breakBounds.startIndex = i;
                                breakBounds.kind = WorkKind.Text;
                                lexState = LexState.Text;
                            }
                            else if (char.IsNumber(c))
                            {
                                breakBounds.startIndex = i;
                                breakBounds.kind = WorkKind.Number;
                                lexState = LexState.Number;

                            }
                            else if (char.IsPunctuation(c))
                            {
                                breakBounds.startIndex = i;
                                breakBounds.length = 1;
                                breakBounds.kind = WorkKind.Punc;

                                //we not collect punc
                                onbreak(breakBounds);
                                breakBounds.length = 0;
                                lexState = LexState.Init;
                                continue;
                            }
                            else if (char.IsWhiteSpace(c))
                            {
                                //we collect whitespace
                                breakBounds.startIndex = i;
                                breakBounds.kind = WorkKind.Whitespace;
                                lexState = LexState.Whitespace;
                            }
                            else
                            {
                                throw new System.NotSupportedException();
                            }
                        }
                        break;
                    case LexState.Number:
                        {
                            //in number state
                            if (!char.IsNumber(c))
                            {
                                //if number then continue collect
                                //if not

                                //flush current state 
                                breakBounds.length = i - breakBounds.startIndex;
                                onbreak(breakBounds);
                                breakBounds.length = 0;
                                lexState = LexState.Init;
                                goto case LexState.Init;
                            }
                        }
                        break;
                    case LexState.Text:
                        {
                            if (!char.IsLetter(c))
                            {
                                //flush
                                breakBounds.length = i - breakBounds.startIndex;
                                onbreak(breakBounds);
                                breakBounds.length = 0;
                                lexState = LexState.Init;
                                goto case LexState.Init;
                            }
                        }
                        break;
                    case LexState.Whitespace:
                        {
                            if (!char.IsWhiteSpace(c))
                            {
                                breakBounds.length = i - breakBounds.startIndex;
                                onbreak(breakBounds);
                                breakBounds.length = 0;
                                lexState = LexState.Init;
                                goto case LexState.Init;
                            }
                        }
                        break;
                }

            }
            if (breakBounds.startIndex < start + len)
            {
                //some remaining data
                breakBounds.length = (start + len) - breakBounds.startIndex;
                onbreak(breakBounds);
            }
        }
    }

}