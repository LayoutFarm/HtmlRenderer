//BSD  2014 ,WinterDev

using System;
using System.Collections.Generic;
using System.Text;

namespace LayoutFarm.WebDom.Parser
{

    public class CssParser
    {
        CssLexer lexer;
        char[] textBuffer;
        CssParseState parseState;
        CssDocument cssDocument;

        Stack<CssAtMedia> _mediaStack = new Stack<CssAtMedia>();
        CssAtMedia _currentAtMedia;
        CssRuleSet _currentRuleSet;

        CssAttributeSelectorExpression _currentSelectorAttr;
        CssSimpleElementSelector _currentSelectorExpr;
        CssPropertyDeclaration _currentProperty;
        CssCodeValueExpression _latestPropertyValue;

        public CssParser()
        {
            lexer = new CssLexer(LexerEmitHandler);
        }


        public void ParseCssStyleSheet(char[] textBuffer)
        {
            this.textBuffer = textBuffer;
            Reset();

            this.parseState = CssParseState.Init;
            lexer.Lex(textBuffer);
            //-----------------------------
            //expand some compound property             
            foreach (CssDocMember mb in cssDocument.GetCssDocMemberIter())
            {
                switch (mb.MemberKind)
                {
                    case WebDom.CssDocMemberKind.RuleSet:
                        EvaluateRuleSet((WebDom.CssRuleSet)mb);
                        break;
                    case WebDom.CssDocMemberKind.Media:
                        EvaluateMedia((WebDom.CssAtMedia)mb);
                        break;
                    default:
                    case WebDom.CssDocMemberKind.Page:
                        throw new NotSupportedException();
                }
            }
            //-----------------------------
        }
        public CssRuleSet ParseCssPropertyDeclarationList(char[] textBuffer)
        {
            this.textBuffer = textBuffer;
            Reset();

            //------------- 
            _currentRuleSet = new CssRuleSet();
            //-------------
            this.parseState = CssParseState.BlockBody;
            lexer.Lex(textBuffer);

            EvaluateRuleSet(this._currentRuleSet);
            return this._currentRuleSet;
        }

        void Reset()
        {
            cssDocument = new CssDocument();
            _currentAtMedia = new CssAtMedia();
            cssDocument.Add(_currentAtMedia);
            _mediaStack.Clear();

            this._currentSelectorAttr = null;
            this._currentSelectorExpr = null;
            this._currentProperty = null;
            this._latestPropertyValue = null;

        }

        void LexerEmitHandler(CssTokenName tkname, int start, int len)
        {

            switch (parseState)
            {
                default:
                    {
                        throw new NotSupportedException();
                    } break;
                case CssParseState.Init:
                    {

                        switch (tkname)
                        {

                            case CssTokenName.Comment:
                                {
                                    //comment token  
                                } break;
                            case CssTokenName.RBrace:
                                {
                                    //exit from current ruleset block
                                    if (this._mediaStack.Count > 0)
                                    {
                                        this._currentAtMedia = this._mediaStack.Pop();
                                    }
                                } break;
                            case CssTokenName.Star:
                                {

                                    //start new code block 
                                    CssRuleSet newblock;
                                    this._currentAtMedia.AddRuleSet(this._currentRuleSet = newblock = new CssRuleSet());
                                    newblock.AddSelector(this._currentSelectorExpr = new CssSimpleElementSelector(SimpleElementSelectorKind.All));

                                    parseState = CssParseState.MoreBlockName;
                                } break;
                            case CssTokenName.At:
                                {
                                    //at rule                                    
                                    parseState = CssParseState.ExpectAtRuleName;
                                } break;
                            //--------------------------------------------------
                            //1.
                            case CssTokenName.Colon:
                                {
                                    CssRuleSet newblock;
                                    _currentAtMedia.AddRuleSet(this._currentRuleSet = newblock = new CssRuleSet());
                                    newblock.AddSelector(this._currentSelectorExpr = new CssSimpleElementSelector(SimpleElementSelectorKind.PseudoClass));

                                    parseState = CssParseState.ExpectIdenAfterSpecialBlockNameSymbol;
                                } break;
                            //2.
                            case CssTokenName.Dot:
                                {
                                    CssRuleSet newblock;
                                    _currentAtMedia.AddRuleSet(this._currentRuleSet = newblock = new CssRuleSet());
                                    newblock.AddSelector(this._currentSelectorExpr = new CssSimpleElementSelector(SimpleElementSelectorKind.ClassName));

                                    parseState = CssParseState.ExpectIdenAfterSpecialBlockNameSymbol;
                                } break;
                            //3. 
                            case CssTokenName.DoubleColon:
                                {

                                    CssRuleSet newblock;
                                    _currentAtMedia.AddRuleSet(this._currentRuleSet = newblock = new CssRuleSet());
                                    newblock.AddSelector(this._currentSelectorExpr = new CssSimpleElementSelector(SimpleElementSelectorKind.Extend));

                                    parseState = CssParseState.ExpectIdenAfterSpecialBlockNameSymbol;
                                } break;
                            //4.
                            case CssTokenName.Iden:
                                {
                                    //block name
                                    CssRuleSet newblock;
                                    _currentAtMedia.AddRuleSet(this._currentRuleSet = newblock = new CssRuleSet());
                                    newblock.AddSelector(this._currentSelectorExpr = new CssSimpleElementSelector());
                                    this._currentSelectorExpr.Name = new string(this.textBuffer, start, len);
                                    parseState = CssParseState.MoreBlockName;
                                } break;
                            //5. 
                            case CssTokenName.Sharp:
                                {
                                    CssRuleSet newblock;
                                    _currentAtMedia.AddRuleSet(this._currentRuleSet = newblock = new CssRuleSet());
                                    newblock.AddSelector(this._currentSelectorExpr = new CssSimpleElementSelector(SimpleElementSelectorKind.Id));

                                    parseState = CssParseState.ExpectIdenAfterSpecialBlockNameSymbol;
                                } break;
                        }
                    } break;
                case CssParseState.MoreBlockName:
                    {
                        //more 
                        switch (tkname)
                        {

                            case CssTokenName.LBrace:
                                {
                                    //block body
                                    parseState = CssParseState.BlockBody;
                                } break;
                            case CssTokenName.LBracket:
                                {
                                    //element attr
                                    parseState = CssParseState.ExpectBlockAttrIden;
                                } break;
                            //1. 
                            case CssTokenName.Colon:
                                {
                                    //wait iden after colon
                                    var cssSelector = new CssSimpleElementSelector();
                                    cssSelector.selectorType = SimpleElementSelectorKind.PseudoClass;
                                    _currentRuleSet.AddSelector(cssSelector);

                                    this._currentSelectorExpr = cssSelector;
                                    parseState = CssParseState.ExpectIdenAfterSpecialBlockNameSymbol;
                                } break;
                            //2. 
                            case CssTokenName.Dot:
                                {
                                    var cssSelector = new CssSimpleElementSelector();
                                    cssSelector.selectorType = SimpleElementSelectorKind.ClassName;
                                    _currentRuleSet.AddSelector(cssSelector);
                                    this._currentSelectorExpr = cssSelector;
                                    parseState = CssParseState.ExpectIdenAfterSpecialBlockNameSymbol;
                                } break;
                            //3. 
                            case CssTokenName.DoubleColon:
                                {
                                    var cssSelector = new CssSimpleElementSelector();
                                    cssSelector.selectorType = SimpleElementSelectorKind.Extend;
                                    _currentRuleSet.AddSelector(cssSelector);

                                    this._currentSelectorExpr = cssSelector;
                                    parseState = CssParseState.ExpectIdenAfterSpecialBlockNameSymbol;

                                } break;
                            //4. 
                            case CssTokenName.Iden:
                                {

                                    //add more block name                                     
                                    var cssSelector = new CssSimpleElementSelector();
                                    cssSelector.selectorType = SimpleElementSelectorKind.TagName;
                                    cssSelector.Name = new string(this.textBuffer, start, len);
                                    _currentRuleSet.AddSelector(cssSelector);

                                    this._currentSelectorExpr = cssSelector;
                                } break;
                            //5. 
                            case CssTokenName.Sharp:
                                {
                                    //id
                                    var cssSelector = new CssSimpleElementSelector();
                                    cssSelector.selectorType = SimpleElementSelectorKind.Id;
                                    _currentRuleSet.AddSelector(cssSelector);

                                    this._currentSelectorExpr = cssSelector;
                                    parseState = CssParseState.ExpectIdenAfterSpecialBlockNameSymbol;

                                } break;
                            //----------------------------------------------------
                            //element combinator operators
                            case CssTokenName.Comma:
                                {
                                    this._currentRuleSet.PrepareExpression(CssCombinatorOperator.List);
                                } break;
                            case CssTokenName.RAngle:
                                {

                                } break;
                            case CssTokenName.Plus:
                                {

                                } break;
                            case CssTokenName.Tile:
                                {

                                } break;
                            //----------------------------------------------------
                            default:
                                {
                                    throw new NotSupportedException();

                                } break;
                        }
                    } break;
                case CssParseState.ExpectIdenAfterSpecialBlockNameSymbol:
                    {

                        switch (tkname)
                        {
                            case CssTokenName.Iden:
                                {
                                    this._currentSelectorExpr.Name = new string(this.textBuffer, start, len);

                                    parseState = CssParseState.MoreBlockName;
                                } break;
                            default:
                                {
                                    throw new NotSupportedException();
                                }
                        }
                    } break;
                case CssParseState.ExpectBlockAttrIden:
                    {
                        switch (tkname)
                        {
                            case CssTokenName.Iden:
                                {
                                    //attribute  
                                    parseState = CssParseState.AfterAttrName;
                                    this._currentSelectorExpr.AddAttribute(this._currentSelectorAttr = new CssAttributeSelectorExpression());
                                    this._currentSelectorAttr.AttributeName = new string(this.textBuffer, start, len);
                                } break;
                            default:
                                {
                                    throw new NotSupportedException();
                                } break;
                        }
                    } break;
                case CssParseState.AfterAttrName:
                    {
                        switch (tkname)
                        {
                            case CssTokenName.OpEq:
                                {
                                    parseState = CssParseState.ExpectedBlockAttrValue;
                                    //expected  attr value
                                } break;
                            case CssTokenName.RBracket:
                                {
                                    //no attr value
                                    parseState = CssParseState.MoreBlockName;

                                } break;
                            default:
                                {
                                    throw new NotSupportedException();
                                } break;
                        }
                    } break;
                case CssParseState.ExpectedBlockAttrValue:
                    {
                        switch (tkname)
                        {
                            case CssTokenName.LiteralString:
                                {
                                    this._currentSelectorAttr.valueExpression = this._latestPropertyValue =
                                        new CssCodePrimitiveExpression(new string(this.textBuffer, start, len), CssValueHint.LiteralString);

                                    this._currentSelectorAttr = null;
                                } break;
                            default:
                                {

                                } break;
                        }
                        parseState = CssParseState.AfterBlockNameAttr;

                    } break;
                case CssParseState.AfterBlockNameAttr:
                    {
                        switch (tkname)
                        {
                            default:
                                {

                                } break;
                            case CssTokenName.RBracket:
                                {
                                    parseState = CssParseState.MoreBlockName;
                                    this._currentSelectorAttr = null;
                                } break;
                        }
                    } break;
                case CssParseState.BlockBody:
                    {
                        switch (tkname)
                        {
                            case CssTokenName.Iden:
                                {
                                    //block name

                                    //create css property 
                                    var wellknownName = UserMapUtil.GetWellKnownPropName(
                                         new string(this.textBuffer, start, len));

                                    _currentRuleSet.AddCssCodeProperty(this._currentProperty =
                                        new CssPropertyDeclaration(wellknownName));

                                    this._latestPropertyValue = null;

                                    parseState = CssParseState.AfterPropertyName;

                                } break;
                            case CssTokenName.RBrace:
                                {
                                    //close current block
                                    this._currentProperty = null;
                                    this._currentSelectorAttr = null;
                                    this._currentSelectorExpr = null;

                                    parseState = CssParseState.Init;
                                } break;
                            default:
                                {
                                    throw new NotSupportedException();
                                } break;
                        }

                    } break;
                case CssParseState.AfterPropertyName:
                    {
                        if (tkname == CssTokenName.Colon)
                        {
                            parseState = CssParseState.ExpectPropertyValue;
                        }
                        else
                        {
                            throw new NotSupportedException();
                        }
                    } break;
                case CssParseState.ExpectPropertyValue:
                    {
                        switch (tkname)
                        {
                            default:
                                {
                                    throw new NotSupportedException();
                                }
                            case CssTokenName.Sharp:
                                {
                                    //follow by hex color value
                                    parseState = CssParseState.ExpectValueOfHexColor;
                                } break;
                            case CssTokenName.LiteralString:
                                {

                                    this._currentProperty.AddValue(this._latestPropertyValue =
                                         new CssCodePrimitiveExpression(new string(this.textBuffer, start, len), CssValueHint.LiteralString));

                                    parseState = CssParseState.AfterPropertyValue;

                                } break;
                            case CssTokenName.Number:
                                {
                                    float number = float.Parse(new string(this.textBuffer, start, len));
                                    this._currentProperty.AddValue(this._latestPropertyValue =
                                          new CssCodePrimitiveExpression(number));

                                    parseState = CssParseState.AfterPropertyValue;

                                } break;
                            case CssTokenName.NumberUnit:
                                {

                                } break;
                            case CssTokenName.Iden:
                                {
                                    //property value
                                    this._currentProperty.AddValue(this._latestPropertyValue =
                                        new CssCodePrimitiveExpression(new string(this.textBuffer, start, len), CssValueHint.Iden));

                                    parseState = CssParseState.AfterPropertyValue;

                                } break;
                        }
                    } break;
                case CssParseState.ExpectValueOfHexColor:
                    {
                        switch (tkname)
                        {
                            case CssTokenName.Iden:
                                {
                                    this._currentProperty.AddValue(this._latestPropertyValue =
                                       new CssCodePrimitiveExpression("#" + new string(this.textBuffer, start, len), CssValueHint.HexColor));

                                } break;
                            case CssTokenName.Number:
                                {

                                    this._currentProperty.AddValue(this._latestPropertyValue =
                                         new CssCodePrimitiveExpression("#" + new string(this.textBuffer, start, len), CssValueHint.HexColor));

                                } break;
                            default:
                                {
                                    throw new NotSupportedException();
                                }
                        }
                        parseState = CssParseState.AfterPropertyValue;
                    } break;
                case CssParseState.AfterPropertyValue:
                    {
                        switch (tkname)
                        {
                            default:
                                {
                                    throw new NotSupportedException();
                                } break;
                            case CssTokenName.LiteralString:
                                {
                                    var literalValue = new string(this.textBuffer, start, len);

                                } break;
                            case CssTokenName.LParen:
                                {
                                    //function 
                                    parseState = CssParseState.ExpectedFuncParameter;
                                    //make current prop value as func
                                    CssCodeFunctionCallExpression funcCallExpr = new CssCodeFunctionCallExpression(
                                        this._latestPropertyValue.ToString());
                                    int valueCount = this._currentProperty.ValueCount;
                                    this._currentProperty.ReplaceValue(valueCount - 1, funcCallExpr);
                                    this._latestPropertyValue = funcCallExpr;


                                } break;
                            case CssTokenName.RBrace:
                                {
                                    //close block
                                    parseState = CssParseState.Init;
                                } break;
                            case CssTokenName.SemiColon:
                                {
                                    //start new proeprty
                                    parseState = CssParseState.BlockBody;
                                    this._currentProperty = null;
                                } break;
                            case CssTokenName.Iden:
                                {
                                    //another property value                                     
                                    this._currentProperty.AddValue(this._latestPropertyValue =
                                        new CssCodePrimitiveExpression(new string(this.textBuffer, start, len), CssValueHint.Iden));
                                } break;
                            case CssTokenName.Number:
                                {
                                    //another property value
                                    float number = float.Parse(new string(this.textBuffer, start, len));
                                    this._currentProperty.AddValue(this._latestPropertyValue =
                                        new CssCodePrimitiveExpression(number));
                                } break;
                            case CssTokenName.NumberUnit:
                                {
                                    //number unit 
                                    this._currentProperty.AddUnitToLatestValue(new string(this.textBuffer, start, len));
                                } break;
                            case CssTokenName.Sharp:
                                {
                                    parseState = CssParseState.ExpectValueOfHexColor;
                                } break;
                        }
                    } break;
                case CssParseState.ExpectAtRuleName:
                    {
                        //iden
                        switch (tkname)
                        {
                            default:
                                {
                                    throw new NotSupportedException();
                                }
                            case CssTokenName.Iden:
                                {

                                    string iden = new string(this.textBuffer, start, len);
                                    //create new rule
                                    _currentRuleSet = null;
                                    _currentProperty = null;
                                    _currentSelectorAttr = null;
                                    _currentSelectorExpr = null;

                                    switch (iden)
                                    {
                                        case "media":
                                            {
                                                parseState = CssParseState.MediaList;
                                                //store previous media 
                                                if (this._currentAtMedia != null)
                                                {
                                                    this._mediaStack.Push(this._currentAtMedia);
                                                }

                                                this.cssDocument.Add(this._currentAtMedia = new CssAtMedia());

                                            } break;
                                        case "import":
                                            {
                                                parseState = CssParseState.ExpectImportURL;
                                            } break;
                                        case "page":
                                            {
                                                throw new NotSupportedException();
                                            } break;
                                        default:
                                            {
                                                throw new NotSupportedException();
                                            } break;
                                    }
                                } break;
                        }
                    } break;
                case CssParseState.MediaList:
                    {
                        //medialist sep by comma
                        switch (tkname)
                        {
                            default:
                                {
                                    throw new NotSupportedException();
                                }
                            case CssTokenName.Iden:
                                {
                                    //media name                                     
                                    this._currentAtMedia.AddMedia(new string(this.textBuffer, start, len));
                                } break;
                            case CssTokenName.Comma:
                                {
                                    //wait for another media
                                } break;
                            case CssTokenName.LBrace:
                                {
                                    //begin rule set part
                                    parseState = CssParseState.Init;
                                } break;
                        }

                    } break;
                case CssParseState.ExpectedFuncParameter:
                    {
                        string funcArg = new string(this.textBuffer, start, len);
                        switch (tkname)
                        {
                            default:
                                {
                                    throw new NotSupportedException();
                                }
                            case CssTokenName.RParen:
                                {
                                    this.parseState = CssParseState.AfterPropertyValue;

                                } break;
                            case CssTokenName.LiteralString:
                                {
                                    ((CssCodeFunctionCallExpression)this._latestPropertyValue).AddFuncArg(
                                        new CssCodePrimitiveExpression(funcArg, CssValueHint.LiteralString));
                                    this.parseState = CssParseState.AfterFuncParameter;
                                } break;
                            case CssTokenName.Number:
                                {
                                    float number = float.Parse(funcArg);
                                    ((CssCodeFunctionCallExpression)this._latestPropertyValue).AddFuncArg(
                                          new CssCodePrimitiveExpression(number));
                                    this.parseState = CssParseState.AfterFuncParameter;
                                } break;
                            case CssTokenName.Iden:
                                {
                                    ((CssCodeFunctionCallExpression)this._latestPropertyValue).AddFuncArg(
                                        new CssCodePrimitiveExpression(funcArg, CssValueHint.Iden));
                                    this.parseState = CssParseState.AfterFuncParameter;
                                } break;
                        }
                    } break;
                case CssParseState.AfterFuncParameter:
                    {
                        switch (tkname)
                        {

                            default:
                                {
                                    throw new NotSupportedException();
                                }
                            case CssTokenName.RParen:
                                {
                                    this.parseState = CssParseState.AfterPropertyValue;

                                } break;
                            case CssTokenName.Comma:
                                {
                                    this.parseState = CssParseState.ExpectedFuncParameter;
                                } break;
                        }
                    } break;
            }
        }
        void ExpandFontProperty(CssPropertyDeclaration decl, List<CssPropertyDeclaration> newProps)
        {

            //may has more than one prop value
            int valCount = decl.ValueCount;
            for (int i = 0; i < valCount; ++i)
            {
                CssCodePrimitiveExpression value = decl.GetPropertyValue(i) as CssCodePrimitiveExpression;
                //what this prop mean 
                if (value == null)
                {
                    continue;
                }
                //------------------------------
                switch (value.Hint)
                {
                    case CssValueHint.Iden:
                        {
                            //font style
                            //font vairant
                            //font weight
                            //font named size         
                            //font family
                            if (UserMapUtil.IsFontStyle(value.Value))
                            {
                                newProps.Add(new CssPropertyDeclaration(WellknownCssPropertyName.FontStyle, value));
                                continue;
                            }

                            if (UserMapUtil.IsFontVariant(value.Value))
                            {
                                newProps.Add(new CssPropertyDeclaration(WellknownCssPropertyName.FontVariant, value));
                                continue;
                            }
                            //----------
                            if (UserMapUtil.IsFontWeight(value.Value))
                            {
                                newProps.Add(new CssPropertyDeclaration(WellknownCssPropertyName.FontWeight, value));
                                continue;
                            }
                            newProps.Add(new CssPropertyDeclaration(WellknownCssPropertyName.FontFamily, value));
                        } break;
                    case CssValueHint.Number:
                        {
                            //font size ?
                            newProps.Add(new CssPropertyDeclaration(WellknownCssPropertyName.FontSize, value));

                        } break;
                    default:
                        {
                        } break;
                }
            }
        }

        void EvaluateRuleSet(WebDom.CssRuleSet ruleset)
        {
            //only some prop need to be alter
            List<CssPropertyDeclaration> newProps = null;
            foreach (CssPropertyDeclaration decl in ruleset.GetAssignmentIter())
            {

                switch (decl.WellknownPropertyName)
                {
                    case WellknownCssPropertyName.Font:
                        {
                            if (newProps == null) newProps = new List<CssPropertyDeclaration>();
                            ExpandFontProperty(decl, newProps);
                            decl.IsExpand = true;
                        } break;
                    case WellknownCssPropertyName.Border:
                        {
                            if (newProps == null) newProps = new List<CssPropertyDeclaration>();
                            ExpandBorderProperty(decl, BorderDirection.All, newProps);
                            decl.IsExpand = true;
                        } break;
                    case WellknownCssPropertyName.BorderLeft:
                        {
                            if (newProps == null) newProps = new List<CssPropertyDeclaration>();
                            ExpandBorderProperty(decl, BorderDirection.Left, newProps);
                            decl.IsExpand = true;
                        } break;
                    case WellknownCssPropertyName.BorderRight:
                        {
                            if (newProps == null) newProps = new List<CssPropertyDeclaration>();
                            ExpandBorderProperty(decl, BorderDirection.Right, newProps);
                            decl.IsExpand = true;
                        } break;
                    case WellknownCssPropertyName.BorderTop:
                        {
                            if (newProps == null) newProps = new List<CssPropertyDeclaration>();
                            ExpandBorderProperty(decl, BorderDirection.Top, newProps);
                            decl.IsExpand = true;
                        } break;
                    case WellknownCssPropertyName.BorderBottom:
                        {
                            if (newProps == null) newProps = new List<CssPropertyDeclaration>();
                            ExpandBorderProperty(decl, BorderDirection.Bottom, newProps);
                            decl.IsExpand = true;
                        } break;
                    //---------------------------
                    case WellknownCssPropertyName.BorderStyle:
                        {
                            if (newProps == null) newProps = new List<CssPropertyDeclaration>();
                            ExpandCssEdgeProperty(decl,
                                WellknownCssPropertyName.BorderLeftStyle,
                                WellknownCssPropertyName.BorderTopStyle,
                                WellknownCssPropertyName.BorderRightStyle,
                                WellknownCssPropertyName.BorderBottomStyle,
                                newProps);
                            decl.IsExpand = true;
                        } break;
                    case WellknownCssPropertyName.BorderWidth:
                        {
                            if (newProps == null) newProps = new List<CssPropertyDeclaration>();
                            ExpandCssEdgeProperty(decl,
                                 WellknownCssPropertyName.BorderLeftWidth,
                                 WellknownCssPropertyName.BorderTopWidth,
                                 WellknownCssPropertyName.BorderRightWidth,
                                 WellknownCssPropertyName.BorderBottomWidth,
                                 newProps);

                            decl.IsExpand = true;
                        } break;
                    case WellknownCssPropertyName.BorderColor:
                        {
                            if (newProps == null) newProps = new List<CssPropertyDeclaration>();
                            ExpandCssEdgeProperty(decl,
                                WellknownCssPropertyName.BorderLeftColor,
                                WellknownCssPropertyName.BorderTopColor,
                                WellknownCssPropertyName.BorderRightColor,
                                WellknownCssPropertyName.BorderBottomColor,
                                newProps);
                            decl.IsExpand = true;
                        } break;
                    //---------------------------
                    case WellknownCssPropertyName.Margin:
                        {
                            if (newProps == null) newProps = new List<CssPropertyDeclaration>();
                            ExpandCssEdgeProperty(decl,
                                WellknownCssPropertyName.MarginLeft,
                                WellknownCssPropertyName.MarginTop,
                                WellknownCssPropertyName.MarginRight,
                                WellknownCssPropertyName.MarginBottom,
                                newProps);
                            decl.IsExpand = true;
                        } break;
                    case WellknownCssPropertyName.Padding:
                        {
                            if (newProps == null) newProps = new List<CssPropertyDeclaration>();
                            ExpandCssEdgeProperty(decl,
                                WellknownCssPropertyName.PaddingLeft,
                                WellknownCssPropertyName.PaddingTop,
                                WellknownCssPropertyName.PaddingRight,
                                WellknownCssPropertyName.PaddingBottom,
                                newProps);
                            decl.IsExpand = true;
                        } break; 
                }
            }
            //--------------------
            //add new prop to ruleset
            if (newProps == null)
            {
                return;
            }
            //------------
            int newPropCount = newProps.Count;
            for (int i = 0; i < newPropCount; ++i)
            {
                //add new prop to ruleset
                ruleset.AddCssCodeProperty(newProps[i]);
            }
        }

        enum BorderDirection
        {
            All, Left, Right, Top, Bottom
        }

        void ExpandBorderProperty(CssPropertyDeclaration decl, BorderDirection borderDirection, List<CssPropertyDeclaration> newProps)
        {

            int j = decl.ValueCount;

            for (int i = 0; i < j; ++i)
            {
                CssCodePrimitiveExpression cssCodePropertyValue = decl.GetPropertyValue(i) as CssCodePrimitiveExpression;
                if (cssCodePropertyValue == null)
                {
                    continue;
                }
                //what value means ?
                //border width/ style / color
                if (cssCodePropertyValue.Hint == CssValueHint.Number ||
                     UserMapUtil.IsNamedBorderWidth(cssCodePropertyValue.Value))
                {
                    //border width
                    switch (borderDirection)
                    {
                        default:
                        case BorderDirection.All:
                            {
                                newProps.Add(CloneProp(WellknownCssPropertyName.BorderLeftWidth, cssCodePropertyValue));
                                newProps.Add(CloneProp(WellknownCssPropertyName.BorderTopWidth, cssCodePropertyValue));
                                newProps.Add(CloneProp(WellknownCssPropertyName.BorderRightWidth, cssCodePropertyValue));
                                newProps.Add(CloneProp(WellknownCssPropertyName.BorderBottomWidth, cssCodePropertyValue));

                            } break;
                        case BorderDirection.Left:
                            {
                                newProps.Add(CloneProp(WellknownCssPropertyName.BorderLeftWidth, cssCodePropertyValue));
                            } break;
                        case BorderDirection.Right:
                            {
                                newProps.Add(CloneProp(WellknownCssPropertyName.BorderRightWidth, cssCodePropertyValue));
                            } break;
                        case BorderDirection.Top:
                            {
                                newProps.Add(CloneProp(WellknownCssPropertyName.BorderTopWidth, cssCodePropertyValue));
                            } break;
                        case BorderDirection.Bottom:
                            {
                                newProps.Add(CloneProp(WellknownCssPropertyName.BorderBottomWidth, cssCodePropertyValue));
                            } break;
                    }
                    continue;
                }

                //------
                if (UserMapUtil.IsBorderStyle(cssCodePropertyValue.Value))
                {

                    //border style
                    switch (borderDirection)
                    {
                        default:
                        case BorderDirection.All:
                            {
                                newProps.Add(CloneProp(WellknownCssPropertyName.BorderLeftStyle, cssCodePropertyValue));
                                newProps.Add(CloneProp(WellknownCssPropertyName.BorderRightStyle, cssCodePropertyValue));
                                newProps.Add(CloneProp(WellknownCssPropertyName.BorderTopStyle, cssCodePropertyValue));
                                newProps.Add(CloneProp(WellknownCssPropertyName.BorderBottomStyle, cssCodePropertyValue));

                            } break;
                        case BorderDirection.Left:
                            {
                                newProps.Add(CloneProp(WellknownCssPropertyName.BorderLeftStyle, cssCodePropertyValue));
                            } break;
                        case BorderDirection.Right:
                            {
                                newProps.Add(CloneProp(WellknownCssPropertyName.BorderRightStyle, cssCodePropertyValue));
                            } break;
                        case BorderDirection.Top:
                            {
                                newProps.Add(CloneProp(WellknownCssPropertyName.BorderTopStyle, cssCodePropertyValue));
                            } break;
                        case BorderDirection.Bottom:
                            {
                                newProps.Add(CloneProp(WellknownCssPropertyName.BorderBottomStyle, cssCodePropertyValue));
                            } break;
                    }

                    continue;
                }

                string value = cssCodePropertyValue.ToString();
                if (value.StartsWith("#"))
                {
                    //expand border color
                    switch (borderDirection)
                    {
                        default:
                        case BorderDirection.All:
                            {
                                newProps.Add(CloneProp(WellknownCssPropertyName.BorderLeftColor, cssCodePropertyValue));
                                newProps.Add(CloneProp(WellknownCssPropertyName.BorderTopColor, cssCodePropertyValue));
                                newProps.Add(CloneProp(WellknownCssPropertyName.BorderRightColor, cssCodePropertyValue));
                                newProps.Add(CloneProp(WellknownCssPropertyName.BorderBottomColor, cssCodePropertyValue));
                            } break;
                        case BorderDirection.Left:
                            {
                                newProps.Add(CloneProp(WellknownCssPropertyName.BorderLeftColor, cssCodePropertyValue));
                            } break;
                        case BorderDirection.Right:
                            {
                                newProps.Add(CloneProp(WellknownCssPropertyName.BorderRightColor, cssCodePropertyValue));

                            } break;
                        case BorderDirection.Top:
                            {
                                newProps.Add(CloneProp(WellknownCssPropertyName.BorderTopColor, cssCodePropertyValue));
                            } break;
                        case BorderDirection.Bottom:
                            {
                                newProps.Add(CloneProp(WellknownCssPropertyName.BorderBottomColor, cssCodePropertyValue));
                            } break;
                    }

                    continue;
                }
            }

        }

        static CssPropertyDeclaration CloneProp(WellknownCssPropertyName newName, CssCodeValueExpression prop)
        {
            return new CssPropertyDeclaration(newName, prop);
        }

        void ExpandCssEdgeProperty(CssPropertyDeclaration decl,
            WellknownCssPropertyName left,
            WellknownCssPropertyName top,
            WellknownCssPropertyName right,
            WellknownCssPropertyName bottom,
            List<CssPropertyDeclaration> newProps)
        {

            switch (decl.ValueCount)
            {
                case 0:
                    {
                    } break;
                case 1:
                    {

                        CssCodeValueExpression value = decl.GetPropertyValue(0);
                        newProps.Add(CloneProp(left, value));
                        newProps.Add(CloneProp(top, value));
                        newProps.Add(CloneProp(right, value));
                        newProps.Add(CloneProp(bottom, value));

                    } break;
                case 2:
                    {

                        newProps.Add(CloneProp(top, decl.GetPropertyValue(0)));
                        newProps.Add(CloneProp(bottom, decl.GetPropertyValue(0)));
                        newProps.Add(CloneProp(left, decl.GetPropertyValue(1)));
                        newProps.Add(CloneProp(right, decl.GetPropertyValue(1)));

                    } break;
                case 3:
                    {

                        newProps.Add(CloneProp(top, decl.GetPropertyValue(0)));
                        newProps.Add(CloneProp(left, decl.GetPropertyValue(1)));
                        newProps.Add(CloneProp(right, decl.GetPropertyValue(1)));
                        newProps.Add(CloneProp(bottom, decl.GetPropertyValue(2)));
                    } break;
                default://4 or more
                    {
                        newProps.Add(CloneProp(top, decl.GetPropertyValue(0)));
                        newProps.Add(CloneProp(right, decl.GetPropertyValue(1)));
                        newProps.Add(CloneProp(bottom, decl.GetPropertyValue(2)));
                        newProps.Add(CloneProp(left, decl.GetPropertyValue(3)));
                    } break;

            }

        }
        void EvaluateMedia(WebDom.CssAtMedia atMedia)
        {
            foreach (var ruleset in atMedia.GetRuleSetIter())
            {
                EvaluateRuleSet(ruleset);
            }
        }
        public CssDocument OutputCssDocument
        {
            get
            {
                return this.cssDocument;
            }
        }
    }
    public enum CssParseState
    {
        Init,
        MoreBlockName,
        ExpectIdenAfterSpecialBlockNameSymbol,
        BlockBody,
        AfterPropertyName,
        ExpectPropertyValue,
        ExpectValueOfHexColor,

        AfterPropertyValue,
        Comment,
        ExpectBlockAttrIden,

        AfterBlockAttrIden,
        AfterAttrName,
        ExpectedBlockAttrValue,
        AfterBlockNameAttr,
        ExpectAtRuleName,

        //@media
        MediaList,

        //@import
        ExpectImportURL,

        ExpectedFuncParameter,
        AfterFuncParameter,
    }
    public enum CssLexState
    {
        Init,
        Whitespace,
        Comment,
        Iden,
        CollectString,
        Number,
        UnitAfterNumber,
    }

    public enum CssTokenName
    {
        Unknown,
        Newline,
        Whitespace,
        At,
        Comma,

        Plus, //+
        Minus,//-
        Star,//*
        Divide,// /
        Percent,// %
        Dot, // .
        Colon, // :
        Cap, //^
        OpEq,//=
        Dollar,//$
        Tile, //~

        SemiColon,
        Sharp, //#

        OrPipe, //|

        LParen,
        RParen,
        LBracket,
        RBracket,
        LBrace,
        RBrace,

        LAngle, //<
        RAngle,  //>


        Iden,
        Number,
        NumberUnit,
        LiteralString,

        Comment,

        Quote, //  '
        DoubleQuote,  // "

        //------------------
        DoubleColon, //::
        TileAssign, //~=
        StarAssign,//*=
        CapAssign,//^=
        DollarAssign,//$=  
        OrPipeAssign,//|= 
        //------------------


    }


    delegate void CssLexerEmitHandler(CssTokenName tkname, int startIndex, int len);

    class CssLexer
    {

        int _appendLength = 0;
        int _startIndex = 0;
        CssLexerEmitHandler _emitHandler;
        char latestEscapeChar;

        public CssLexer(CssLexerEmitHandler emitHandler)
        {
            this._emitHandler = emitHandler;
        }
        public void Lex(char[] cssSourceBuffer)
        {
            //----------------------
            //clear previous result
            this._appendLength = 0;
            this._startIndex = 0;
            this.latestEscapeChar = '\0';
            //----------------------

            CssLexState lexState = CssLexState.Init;
            int j = cssSourceBuffer.Length;

            for (int i = 0; i < j; ++i)
            {
                char c = cssSourceBuffer[i];
                // Console.Write(c);

                //-------------------------------------- 
                switch (lexState)
                {
                    default:
                        {
                            throw new NotSupportedException();
                        } break;

                    case CssLexState.Init:
                        {
                            //-------------------------------------- 
                            //1. first name
                            CssTokenName terminalTokenName = GetTerminalTokenName(c);
                            //--------------------------------------  
                            switch (terminalTokenName)
                            {
                                default:
                                    {
                                        Emit(terminalTokenName, i);
                                    } break;
                                case CssTokenName.Colon:
                                    {
                                        if (i < j - 1)
                                        {
                                            char c1 = cssSourceBuffer[i + 1];
                                            if (c1 == ':')
                                            {
                                                i++;
                                                Emit(CssTokenName.DoubleColon, i);
                                                continue;
                                            }
                                        }
                                        Emit(terminalTokenName, i);
                                    } break;
                                case CssTokenName.DoubleQuote:
                                    {
                                        latestEscapeChar = '"';
                                        lexState = CssLexState.CollectString;
                                    } break;
                                case CssTokenName.Quote:
                                    {
                                        latestEscapeChar = '\'';
                                        lexState = CssLexState.CollectString;
                                    } break;
                                case CssTokenName.Whitespace:
                                case CssTokenName.Newline:
                                    continue;
                                case CssTokenName.Divide:
                                    {
                                        //is open comment or not
                                        if (i < j - 1)
                                        {
                                            if (cssSourceBuffer[i + 1] == '*')
                                            {
                                                i++;
                                                //Emit(CssTokenName.LComment, i);
                                                lexState = CssLexState.Comment;
                                                continue;
                                            }
                                        }
                                        Emit(CssTokenName.Divide, i);
                                    } break;
                                case CssTokenName.Sharp:
                                    {
                                        AppendBuffer(i, c);
                                        lexState = CssLexState.Iden;

                                    } break;
                                case CssTokenName.Dot:
                                    {
                                        if (i < j - 1)
                                        {
                                            char c1 = cssSourceBuffer[i + 1];
                                            if (char.IsNumber(c1))
                                            {
                                                AppendBuffer(i, c);
                                                i++;
                                                AppendBuffer(i, c1);
                                                lexState = CssLexState.Number;
                                                continue;
                                            }
                                        }

                                        Emit(terminalTokenName, i);

                                    } break;
                                case CssTokenName.Minus:
                                    {
                                    } break;
                                case CssTokenName.Unknown:
                                    {
                                        //this is not terminal  
                                        AppendBuffer(i, c);
                                        if (char.IsNumber(c))
                                        {
                                            lexState = CssLexState.Number;
                                        }
                                        else
                                        {
                                            lexState = CssLexState.Iden;
                                        }
                                    } break;
                            }
                        } break;
                    case CssLexState.Whitespace:
                        {

                        } break;
                    case CssLexState.CollectString:
                        {
                            if (c == latestEscapeChar)
                            {
                                //exit collect string 
                                lexState = CssLexState.Init;
                                EmitBuffer(i, CssTokenName.LiteralString);

                            }
                            else
                            {
                                AppendBuffer(i, c);
                            }
                        } break;
                    case CssLexState.Comment:
                        {
                            if (c == '*')
                            {
                                if (i < j - 1)
                                {
                                    char c1 = cssSourceBuffer[i + 1];
                                    if (c1 == '/')
                                    {
                                        i++;
                                        //Emit(CssTokenName.RComment, i);
                                        lexState = CssLexState.Init;
                                        continue;
                                    }
                                }
                            }
                            //skip comment?
                        } break;
                    case CssLexState.Iden:
                        {

                            CssTokenName terminalTokenName = GetTerminalTokenName(c);
                            switch (terminalTokenName)
                            {
                                case CssTokenName.Whitespace:
                                case CssTokenName.Newline:
                                    {
                                        EmitBuffer(i, CssTokenName.Iden);
                                    } break;
                                case CssTokenName.Divide:
                                    {
                                        //is open comment or not
                                        throw new NotSupportedException();
                                    }
                                case CssTokenName.Star:
                                    {
                                        //is close comment or not 
                                        throw new NotSupportedException();
                                    }
                                case CssTokenName.Minus:
                                    {
                                        //iden can contains minus 
                                        AppendBuffer(i, c);

                                    } break;

                                default:
                                    {
                                        //flush exising buffer
                                        EmitBuffer(i, CssTokenName.Iden);
                                        Emit(terminalTokenName, i);

                                        lexState = CssLexState.Init;

                                    } break;
                                case CssTokenName.Unknown:
                                    {
                                        //this is not terminal 
                                        AppendBuffer(i, c);
                                        lexState = CssLexState.Iden;
                                    } break;
                            }
                        } break;
                    case CssLexState.Number:
                        {
                            if (char.IsNumber(c))
                            {
                                AppendBuffer(i, c);
                                continue;
                            }
                            //---------------------------------------------------------- 
                            CssTokenName terminalTokenName = GetTerminalTokenName(c);
                            switch (terminalTokenName)
                            {
                                case CssTokenName.Whitespace:
                                case CssTokenName.Newline:
                                    {
                                        if (this._appendLength > 0)
                                        {
                                            EmitBuffer(i, CssTokenName.Number);
                                        }

                                        lexState = CssLexState.Init;
                                    } break;
                                case CssTokenName.Divide:
                                    {
                                        //is open comment or not
                                        throw new NotSupportedException();
                                    }
                                case CssTokenName.Star:
                                    {   //is close comment or not 
                                        throw new NotSupportedException();
                                    }
                                case CssTokenName.Dot:
                                    {
                                        //after number
                                        if (i < j - 1)
                                        {
                                            char c1 = cssSourceBuffer[i + 1];
                                            if (char.IsNumber(c1))
                                            {
                                                AppendBuffer(i, c);
                                                i++;
                                                AppendBuffer(i, c1);
                                                lexState = CssLexState.Number;
                                                continue;
                                            }
                                        }
                                        EmitBuffer(i, CssTokenName.Number);
                                        Emit(terminalTokenName, i);
                                    } break;
                                default:
                                    {
                                        //flush exising buffer
                                        EmitBuffer(i, CssTokenName.Number);
                                        Emit(terminalTokenName, i);
                                        lexState = CssLexState.Init;
                                    } break;

                                case CssTokenName.Unknown:
                                    {
                                        EmitBuffer(i, CssTokenName.Number);
                                        //iden after number may be unit of number*** 
                                        AppendBuffer(i, c);
                                        lexState = CssLexState.UnitAfterNumber;
                                    } break;
                            }
                        } break;
                    case CssLexState.UnitAfterNumber:
                        {


                            if (char.IsLetter(c))
                            {
                                AppendBuffer(i, c);
                            }
                            else
                            {
                                //terminate

                                EmitBuffer(i, CssTokenName.NumberUnit);
                                //-------------------------------------------
                                CssTokenName terminalTokenName = GetTerminalTokenName(c);
                                switch (terminalTokenName)
                                {
                                    case CssTokenName.Whitespace:
                                    case CssTokenName.Newline:
                                        {
                                        } break;
                                    default:
                                        {
                                            Emit(terminalTokenName, i);
                                        } break;
                                }
                                lexState = CssLexState.Init;
                            }

                        } break;
                }
            }
            if (this._appendLength > 0)
            {
                if (lexState == CssLexState.UnitAfterNumber)
                {
                    EmitBuffer(cssSourceBuffer.Length - 1, CssTokenName.NumberUnit);
                }
                else
                {
                    EmitBuffer(cssSourceBuffer.Length - 1, CssTokenName.Iden);
                }
            }
        }



        void AppendBuffer(int i, char c)
        {
            if (_appendLength == 0)
            {
                this._startIndex = i;
            }
            this._appendLength++;
        }
        void EmitBuffer(int i, CssTokenName tokenName)
        {
            //flush existing buffer
            if (this._appendLength > 0)
            {
                _emitHandler(tokenName, this._startIndex, this._appendLength);
            }
            this._appendLength = 0;
        }
        void Emit(CssTokenName tkname, int i)
        {
            _emitHandler(tkname, i, 1);
        }

        static CssTokenName GetTerminalTokenName(char c)
        {
            CssTokenName tokenName;
            if (terminals.TryGetValue(c, out tokenName))
            {
                return tokenName;
            }
            else
            {
                return CssTokenName.Unknown;
            }
        }

        //===============================================================================================
        static readonly Dictionary<char, CssTokenName> terminals = new Dictionary<char, CssTokenName>();
        static readonly Dictionary<string, CssTokenName> multiCharTokens = new Dictionary<string, CssTokenName>();

        static CssLexer()
        {
            //" @+-*/%.:;[](){}"
            terminals.Add(' ', CssTokenName.Whitespace);
            terminals.Add('\r', CssTokenName.Whitespace);
            terminals.Add('\t', CssTokenName.Whitespace);
            terminals.Add('\f', CssTokenName.Whitespace);
            terminals.Add('\n', CssTokenName.Newline);
            terminals.Add('\'', CssTokenName.Quote);
            terminals.Add('"', CssTokenName.DoubleQuote);
            terminals.Add(',', CssTokenName.Comma);

            terminals.Add('@', CssTokenName.At);

            terminals.Add('+', CssTokenName.Plus);
            terminals.Add('-', CssTokenName.Minus);
            terminals.Add('*', CssTokenName.Star);
            terminals.Add('/', CssTokenName.Divide);
            terminals.Add('%', CssTokenName.Percent);
            terminals.Add('#', CssTokenName.Sharp);
            terminals.Add('~', CssTokenName.Tile);


            terminals.Add('.', CssTokenName.Dot);
            terminals.Add(':', CssTokenName.Colon);
            terminals.Add(';', CssTokenName.SemiColon);

            terminals.Add('[', CssTokenName.LBracket);
            terminals.Add(']', CssTokenName.RBracket);

            terminals.Add('(', CssTokenName.LParen);
            terminals.Add(')', CssTokenName.RParen);

            terminals.Add('{', CssTokenName.LBrace);
            terminals.Add('}', CssTokenName.RBrace);
            terminals.Add('<', CssTokenName.LAngle);
            terminals.Add('>', CssTokenName.RAngle);

            terminals.Add('=', CssTokenName.OpEq);
            terminals.Add('|', CssTokenName.OrPipe);
            terminals.Add('$', CssTokenName.Dollar);
            terminals.Add('^', CssTokenName.Cap);
            //----------------------------------- 
            multiCharTokens.Add("|=", CssTokenName.OrPipeAssign);
            multiCharTokens.Add("~=", CssTokenName.TileAssign);
            multiCharTokens.Add("^=", CssTokenName.CapAssign);
            multiCharTokens.Add("$=", CssTokenName.DollarAssign);
            multiCharTokens.Add("*=", CssTokenName.StarAssign);
            //----------------------------------- 
        }


    }
}