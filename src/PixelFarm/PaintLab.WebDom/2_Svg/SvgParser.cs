﻿//MIT, 2018-present, WinterDev
//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.3
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------
//
// SVG parser.
//
//----------------------------------------------------------------------------
using System;
using System.Collections.Generic;

using LayoutFarm.WebDom.Parser;
using LayoutFarm.WebLexer;

namespace LayoutFarm.Svg
{

    public abstract class XmlParserBase
    {
        int parseState = 0;
        protected TextSnapshot _textSnapshot;
        MyXmlLexer myXmlLexer = new MyXmlLexer();
        string waitingAttrName;
        string currentNodeName;
        Stack<string> openEltStack = new Stack<string>();

        TextSpan nodeNamePrefix;
        bool hasNodeNamePrefix;

        TextSpan attrName;
        TextSpan attrPrefix;
        bool hasAttrPrefix;

        protected struct TextSpan
        {
            public readonly int startIndex;
            public readonly int len;
            public TextSpan(int startIndex, int len)
            {
                this.startIndex = startIndex;
                this.len = len;
            }
#if DEBUG
            public override string ToString()
            {
                return startIndex + "," + len;
            }
#endif
            public static readonly TextSpan Empty = new TextSpan();
        }


        public XmlParserBase()
        {
            myXmlLexer.LexStateChanged += MyXmlLexer_LexStateChanged;
        }

        private void MyXmlLexer_LexStateChanged(XmlLexerEvent lexEvent, int startIndex, int len)
        {

            switch (lexEvent)
            {
                default:
                    {
                        throw new NotSupportedException();
                    }
                case XmlLexerEvent.VisitOpenAngle:
                    {
                        //enter new context
                    }
                    break;
                case XmlLexerEvent.CommentContent:
                    {

                    }
                    break;
                case XmlLexerEvent.NamePrefix:
                    {
                        //name prefix of 

#if DEBUG
                        string testStr = _textSnapshot.Substring(startIndex, len);
#endif

                        switch (parseState)
                        {
                            default:
                                throw new NotSupportedException();
                            case 0:
                                nodeNamePrefix = new TextSpan(startIndex, len);
                                hasNodeNamePrefix = true;
                                break;
                            case 1:
                                //attribute part
                                attrPrefix = new TextSpan(startIndex, len);
                                hasAttrPrefix = true;
                                break;
                            case 2: //   </a
                                nodeNamePrefix = new TextSpan(startIndex, len);
                                hasNodeNamePrefix = true;
                                break;
                        }
                    }
                    break;
                case XmlLexerEvent.FromContentPart:
                    {

                        //text content of the element 
                        OnTextNode(new TextSpan(startIndex, len));
                    }
                    break;
                case XmlLexerEvent.AttributeValueAsLiteralString:
                    {
                        //assign value and add to parent
                        //string attrValue = textSnapshot.Substring(startIndex, len);
                        if (parseState == 11)
                        {
                            //doctype node
                            //add to its parameter
                        }
                        else
                        {
                            //add value to current attribute node
                            parseState = 1;
                            OnAttribute(attrName, new TextSpan(startIndex, len));
                        }
                    }
                    break;
                case XmlLexerEvent.Attribute:
                    {
                        //create attribute node and wait for its value
                        attrName = new TextSpan(startIndex, len);
                        //string attrName = textSnapshot.Substring(startIndex, len);
                    }
                    break;
                case XmlLexerEvent.NodeNameOrAttribute:
                    {
                        //the lexer dose not store state of element name or attribute name
                        //so we use parseState to decide here

                        string name = _textSnapshot.Substring(startIndex, len);
                        switch (parseState)
                        {
                            case 0:
                                {
                                    //element name=> create element 
                                    if (currentNodeName != null)
                                    {
                                        OnEnteringElementBody();
                                        openEltStack.Push(currentNodeName);
                                    }

                                    currentNodeName = name;
                                    //enter new node                                   
                                    OnVisitNewElement(new TextSpan(startIndex, len));

                                    parseState = 1; //enter attribute 
                                    waitingAttrName = null;
                                }
                                break;
                            case 1:
                                {
                                    //wait for attr value 
                                    if (waitingAttrName != null)
                                    {
                                        //push waiting attr
                                        //create new attribute

                                        //eg. in html
                                        //but this is not valid in Xml

                                        throw new NotSupportedException();
                                    }
                                    waitingAttrName = name;
                                }
                                break;
                            case 2:
                                {
                                    //****
                                    //node name after open slash  </
                                    //TODO: review here,avoid direct string comparison
                                    if (currentNodeName == name)
                                    {
                                        OnExitingElementBody();

                                        if (openEltStack.Count > 0)
                                        {
                                            waitingAttrName = null;
                                            currentNodeName = openEltStack.Pop();
                                        }
                                        parseState = 3;
                                    }
                                    else
                                    {
                                        //eg. in html
                                        //but this is not valid in Xml
                                        //not match open-close tag
                                        throw new NotSupportedException();
                                    }
                                }
                                break;
                            case 4:
                                {
                                    //attribute value as id ***
                                    //eg. in Html, but not for general Xml
                                    throw new NotSupportedException();
                                }

                            case 10:
                                {
                                    //eg <! 
                                    parseState = 11;
                                }
                                break;
                            case 11:
                                {
                                    //comment node

                                }
                                break;
                            default:
                                {
                                }
                                break;
                        }
                    }
                    break;
                case XmlLexerEvent.VisitCloseAngle:
                    {
                        //close angle of current new node
                        //enter into its content 
                        if (parseState == 11)
                        {
                            //add doctype to html 
                        }
                        else
                        {

                        }
                        waitingAttrName = null;
                        parseState = 0;
                    }
                    break;
                case XmlLexerEvent.VisitAttrAssign:
                    {

                        parseState = 4;
                    }
                    break;
                case XmlLexerEvent.VisitOpenSlashAngle:
                    {
                        parseState = 2;
                    }
                    break;
                case XmlLexerEvent.VisitCloseSlashAngle:
                    {
                        //   />
                        if (openEltStack.Count > 0)
                        {
                            OnExitingElementBody();
                            //curTextNode = null;
                            //curAttr = null;
                            waitingAttrName = null;
                            currentNodeName = openEltStack.Pop();
                        }
                        parseState = 0;
                    }
                    break;
                case XmlLexerEvent.VisitOpenAngleExclimation:
                    {
                        parseState = 10;
                    }
                    break;

            }
        }

        public virtual void ParseDocument(TextSnapshot textSnapshot)
        {
            this._textSnapshot = textSnapshot;


            OnBegin();
            //reset
            openEltStack.Clear();
            waitingAttrName = null;
            currentNodeName = null;
            parseState = 0;

            //

            myXmlLexer.BeginLex();
            myXmlLexer.Analyze(textSnapshot);
            myXmlLexer.EndLex();

            OnFinish();
        }

        protected virtual void OnBegin()
        {

        }
        public virtual void OnFinish()
        {

        }


        //-------------------
        protected virtual void OnTextNode(TextSpan text) { }
        protected virtual void OnAttribute(TextSpan localAttr, TextSpan value) { }
        protected virtual void OnAttribute(TextSpan ns, TextSpan localAttr, TextSpan value) { }

        protected virtual void OnVisitNewElement(TextSpan ns, TextSpan localName) { }
        protected virtual void OnVisitNewElement(TextSpan localName) { }

        protected virtual void OnEnteringElementBody() { }
        protected virtual void OnExitingElementBody() { }
    }




    public class SvgParser : XmlParserBase
    {

        ISvgDocBuilder _svgDocBuilder;

        public SvgParser(ISvgDocBuilder svgDocBuilder)
        {
            _svgDocBuilder = svgDocBuilder;
        }

        protected override void OnBegin()
        {
            _svgDocBuilder.OnBegin();
            base.OnBegin();
        }
        public void ReadSvgString(string svgString)
        {
            ParseDocument(new TextSnapshot(svgString));
        }
        public void ReadSvgCharBuffer(char[] svgBuffer)
        {
            ParseDocument(new TextSnapshot(svgBuffer));
        }
        public void ReadSvgFile(string svgFileName)
        {
            ReadSvgString(System.IO.File.ReadAllText(svgFileName));
        }
        protected override void OnVisitNewElement(TextSpan ns, TextSpan localName)
        {
            throw new NotSupportedException();
        }
        protected override void OnVisitNewElement(TextSpan localName)
        {
            string elemName = _textSnapshot.Substring(localName.startIndex, localName.len);
            _svgDocBuilder.OnVisitNewElement(elemName);
        }

        protected override void OnAttribute(TextSpan localAttr, TextSpan value)
        {
            string attrLocalName = _textSnapshot.Substring(localAttr.startIndex, localAttr.len);
            string attrValue = _textSnapshot.Substring(value.startIndex, value.len);
            _svgDocBuilder.OnAttribute(attrLocalName, attrValue);
        }
        protected override void OnAttribute(TextSpan ns, TextSpan localAttr, TextSpan value)
        {
            string attrLocalName = _textSnapshot.Substring(localAttr.startIndex, localAttr.len);
            string attrValue = _textSnapshot.Substring(value.startIndex, value.len);
            _svgDocBuilder.OnAttribute(attrLocalName, attrValue);

        }
        protected override void OnEnteringElementBody()
        {
            _svgDocBuilder.OnEnteringElementBody();
        }
        protected override void OnExitingElementBody()
        {
            _svgDocBuilder.OnExitingElementBody();
        }

        public static void ParseTransform(string value, SvgVisualSpec spec)
        {
            //TODO: ....

            int openParPos = value.IndexOf('(');
            if (openParPos > -1)
            {
                string right = value.Substring(openParPos + 1, value.Length - (openParPos + 1)).Trim();
                string left = value.Substring(0, openParPos);
                switch (left)
                {
                    default:
                        break;
                    case "matrix":
                        {
                            //read matrix args  
                            spec.Transform = new SvgTransformMatrix(ParseMatrixArgs(right));
                        }
                        break;
                    case "translate":
                        {
                            //translate matrix
                            float[] matrixArgs = ParseMatrixArgs(right);
                            spec.Transform = new SvgTranslate(matrixArgs[0], matrixArgs[1]);
                        }
                        break;
                    case "rotate":
                        {
                            float[] matrixArgs = ParseMatrixArgs(right);
                            if (matrixArgs.Length == 1)
                            {
                                spec.Transform = new SvgRotate(matrixArgs[0]);
                            }
                            else if (matrixArgs.Length == 3)
                            {
                                //rotate around the axis
                                spec.Transform = new SvgRotate(matrixArgs[0], matrixArgs[1], matrixArgs[2]);
                            }

                        }
                        break;
                    case "scale":
                        {
                            float[] matrixArgs = ParseMatrixArgs(right);
                            spec.Transform = new SvgScale(matrixArgs[0], matrixArgs[1]);
                        }
                        break;
                    case "skewX":
                        {
                            float[] matrixArgs = ParseMatrixArgs(right);
                            spec.Transform = new SvgSkew(matrixArgs[0], 0);
                        }
                        break;
                    case "skewY":
                        {
                            float[] matrixArgs = ParseMatrixArgs(right);
                            spec.Transform = new SvgSkew(0, matrixArgs[1]);
                        }
                        break;
                }
            }
            else
            {
                //?
            }
        }

        static readonly char[] s_matrixStrSplitters = new char[] { ',', ' ' };
        static float[] ParseMatrixArgs(string matrixTransformArgs)
        {
            int close_paren = matrixTransformArgs.IndexOf(')');
            matrixTransformArgs = matrixTransformArgs.Substring(0, close_paren);
            string[] elem_string_args = matrixTransformArgs.Split(s_matrixStrSplitters);
            int j = elem_string_args.Length;
            float[] elem_values = new float[j];
            for (int i = 0; i < j; ++i)
            {
                elem_values[i] = float.Parse(elem_string_args[i],System.Globalization.CultureInfo.InvariantCulture);
            }
            return elem_values;
        }

    }

}