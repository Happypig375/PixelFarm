﻿//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

namespace LayoutFarm.Text
{
    class TextLineWriter : TextLineReader
    {
        public TextLineWriter(EditableTextFlowLayer textLayer)
            : base(textLayer)
        {
        }
        public TextSpanStyle CurrentSpanStyle
        {
            get
            {
                return this.TextLayer.CurrentTextSpanStyle;
            }
        }
        public void Reload(IEnumerable<EditableRun> runs)
        {
            this.TextLayer.Reload(runs);
        }
        public void Clear()
        {
            this.MoveToLine(0);
            CurrentLine.Clear();
            EnsureCurrentTextRun();
        }
        public void EnsureCurrentTextRun(int index)
        {
            var run = CurrentTextRun;
            if (run == null || !run.HasParent)
            {

                SetCurrentCharIndexToBegin();
                if (index != -1)
                {
                    int limit = CurrentLine.CharCount;
                    if (index > limit)
                    {
                        index = limit;
                    }
                    SetCurrentCharIndex(index);
                }
            }
        }
        public void EnsureCurrentTextRun()
        {
            EnsureCurrentTextRun(CharIndex);
        }
        public void RemoveSelectedTextRuns(VisualSelectionRange selectionRange)
        {
            int precutIndex = selectionRange.StartPoint.LineCharIndex;
            CurrentLine.Remove(selectionRange);
            CurrentLine.TextLineReCalculateActualLineSize();
            CurrentLine.RefreshInlineArrange();
            EnsureCurrentTextRun(precutIndex);
        }

        public void ClearCurrentLine()
        {
            int currentCharIndex = CharIndex;
            CurrentLine.ReplaceAll(null);
            CurrentLine.TextLineReCalculateActualLineSize();
            CurrentLine.RefreshInlineArrange();
            EnsureCurrentTextRun(currentCharIndex);
        }
        public void ReplaceCurrentLine(IEnumerable<EditableRun> textRuns)
        {
            int currentCharIndex = CharIndex;
            CurrentLine.ReplaceAll(textRuns);
            CurrentLine.TextLineReCalculateActualLineSize();
            CurrentLine.RefreshInlineArrange();
            EnsureCurrentTextRun(currentCharIndex);
        }
        public void JoinWithNextLine()
        {
            EditableTextLine.InnerDoJoinWithNextLine(this.CurrentLine);
            EnsureCurrentTextRun();
        }
        char BackSpaceOneChar()
        {
            if (CurrentTextRun == null)
            {
                return '\0';
            }
            else
            {
                if (CharIndex == 0)
                {
                    return '\0';
                }

                //move back 1 char and do delete 
                EditableRun removingTextRun = CurrentTextRun;
                int removeIndex = CurrentTextRunCharIndex;
                SetCurrentCharStepLeft();
                char toBeRemovedChar = CurrentChar;

                EditableRun.InnerRemove(removingTextRun, removeIndex, 1, false);
                if (removingTextRun.CharacterCount == 0)
                {
                    CurrentLine.Remove(removingTextRun);
                    EnsureCurrentTextRun();
                }
                CurrentLine.TextLineReCalculateActualLineSize();
                CurrentLine.RefreshInlineArrange();
                return toBeRemovedChar;
            }
        }
        public EditableRun GetCurrentTextRun()
        {
            if (CurrentLine.IsBlankLine)
            {
                return null;
            }
            else
            {
                return CurrentTextRun;
            }
        }

        public bool CanAcceptThisChar(char c)
        {
            //TODO: review here, enable this feature or not
            //some char can't be a start char on blank line
            if (CurrentLine.IsBlankLine &&
                !InternalTextLayerController.CanCaretStopOnThisChar(c))
            {
                return false;
            }
            return true;
        }
        public void AddCharacter(char c)
        {
            if (CurrentLine.IsBlankLine)
            {
                //TODO: review here, enable this feature or not
                //some char can't be a start char on blank line

                if (!InternalTextLayerController.CanCaretStopOnThisChar(c))
                {
                    return;
                }
                //

                //1. new 
                EditableRun t = new EditableTextRun(this.Root,
                    c,
                    this.CurrentSpanStyle);
                var owner = this.FlowLayer.OwnerRenderElement;
                CurrentLine.AddLast(t);
                SetCurrentTextRun(t);
            }
            else
            {
                EditableRun cRun = CurrentTextRun;
                if (cRun != null)
                {
                    if (cRun.IsInsertable)
                    {
                        cRun.InsertAfter(CurrentTextRunCharIndex, c);
                    }
                    else
                    {
                        AddTextSpan(new EditableTextRun(this.Root, c, this.CurrentSpanStyle));
                        return;
                    }
                }
                else
                {
                    throw new NotSupportedException();
                }
            }

            CurrentLine.TextLineReCalculateActualLineSize();
            CurrentLine.RefreshInlineArrange();
            SetCurrentCharStepRight();
        }
        public void AddTextSpan(EditableRun textRun)
        {
            if (CurrentLine.IsBlankLine)
            {
                CurrentLine.AddLast(textRun);
                SetCurrentTextRun(textRun);
                CurrentLine.TextLineReCalculateActualLineSize();
                CurrentLine.RefreshInlineArrange();

                SetCurrentCharIndex(CharIndex + textRun.CharacterCount);
            }
            else
            {
                if (CurrentTextRun != null)
                {
                    VisualPointInfo newPointInfo = CurrentLine.Split(GetCurrentPointInfo());
                    if (newPointInfo.IsOnTheBeginOfLine)
                    {
                        CurrentLine.AddBefore((EditableRun)newPointInfo.TextRun, textRun);
                    }
                    else
                    {
                        CurrentLine.AddAfter((EditableRun)newPointInfo.TextRun, textRun);
                    }
                    CurrentLine.TextLineReCalculateActualLineSize();
                    CurrentLine.RefreshInlineArrange();
                    EnsureCurrentTextRun(CharIndex + textRun.CharacterCount);
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
        }
        public void ReplaceAllLineContent(EditableRun[] runs)
        {
            int charIndex = CharIndex;
            CurrentLine.Clear();
            int j = runs.Length;
            for (int i = 0; i < j; ++i)
            {
                CurrentLine.AddLast(runs[i]);
            }

            EnsureCurrentTextRun(charIndex);
        }

        public char DoBackspaceOneChar()
        {
            //simulate backspace keystroke
            return BackSpaceOneChar();
        }
        public char DoDeleteOneChar()
        {
            if (CharIndex < CurrentLine.CharCount)
            {
                //simulate backspace keystroke

                SetCurrentCharStepRight();
                return BackSpaceOneChar();
            }
            else
            {
                return '\0';
            }
        }

        public void SplitToNewLine()
        {

            EditableRun lineBreakRun = new EditableTextRun(this.Root, '\n', this.CurrentSpanStyle);
            EditableRun currentRun = CurrentTextRun;
            if (CurrentLine.IsBlankLine)
            {
                CurrentLine.AddLast(lineBreakRun);
            }
            else
            {
                if (CharIndex == -1)
                {
                    CurrentLine.AddFirst(lineBreakRun);
                    SetCurrentTextRun(null);
                }
                else
                {
                    EditableRun rightSplitedPart = EditableRun.InnerRemove(currentRun,
                        CurrentTextRunCharIndex + 1, true);
                    if (rightSplitedPart != null)
                    {
                        CurrentLine.AddAfter(currentRun, rightSplitedPart);
                    }
                    CurrentLine.AddAfter(currentRun, lineBreakRun);
                    if (currentRun.CharacterCount == 0)
                    {
                        CurrentLine.Remove(currentRun);
                    }
                }
            }


            this.TextLayer.TopDownReCalculateContentSize();
            EnsureCurrentTextRun();
        }
        public EditableVisualPointInfo[] SplitSelectedText(VisualSelectionRange selectionRange)
        {
            EditableVisualPointInfo[] newPoints = CurrentLine.Split(selectionRange);
            EnsureCurrentTextRun();
            return newPoints;
        }
    }

    abstract class TextLineReader
    {
#if DEBUG
        static int dbugTotalId;
        int dbug_MyId;
        public debugActivityRecorder dbugTextManRecorder;
#endif

        EditableTextFlowLayer _textFlowLayer;
        EditableTextLine currentLine;
        int currentLineY = 0;
        EditableRun currentTextRun;

        int caretXPos = 0;
        /// <summary>
        /// character offset of this run, start from start line, this value is reset for every current run
        /// </summary>
        int rCharOffset = 0;
        /// <summary>
        /// pixel offset of this run, start from the begin of this line, this value is reset for every current run
        /// </summary>
        int rPixelOffset = 0;
        public TextLineReader(EditableTextFlowLayer flowlayer)
        {

#if DEBUG
            this.dbug_MyId = dbugTotalId;
            dbugTotalId++;
#endif

            this._textFlowLayer = flowlayer;
            flowlayer.Reflow += new EventHandler(flowlayer_Reflow);
            currentLine = flowlayer.GetTextLine(0);
            if (currentLine.FirstRun != null)
            {
                currentTextRun = currentLine.FirstRun;
            }
        }

#if DEBUG
        int _i_charIndex;
        int caret_char_index
        {
            get { return _i_charIndex; }
            set
            {
                _i_charIndex = value;
            }
        }
#else
         int caret_char_index;
#endif

        protected RootGraphic Root
        {
            get { return this._textFlowLayer.Root; }
        }
        public EditableTextFlowLayer FlowLayer
        {
            get
            {
                return this._textFlowLayer;
            }
        }
        void flowlayer_Reflow(object sender, EventArgs e)
        {
            int prevCharIndex = caret_char_index;
            this.SetCurrentCharIndexToBegin();
            this.SetCurrentCharIndex(prevCharIndex);
        }
        protected EditableTextLine CurrentLine
        {
            get
            {
                return currentLine;
            }
        }

        protected EditableRun CurrentTextRun
        {
            get
            {
                return currentTextRun;
            }
        }
        protected void SetCurrentTextRun(EditableRun r)
        {
            currentTextRun = r;
        }
        public void FindCurrentHitWord(out int startAt, out int len)
        {
            if (currentTextRun == null)
            {
                startAt = 0;
                len = 0;
                return;
            }

            //
            //we read entire line 
            //and send to line parser to parse a word
            StringBuilder stbuilder = new StringBuilder();
            currentLine.CopyLineContent(stbuilder);
            string lineContent = stbuilder.ToString();
            //find char at

            TextBufferSpan textBufferSpan = new TextBufferSpan(lineContent.ToCharArray());
            ILineSegmentList segmentList = this.Root.TextServices.BreakToLineSegments(ref textBufferSpan);
            if (segmentList != null)
            {
                int segcount = segmentList.Count;
                for (int i = 0; i < segcount; ++i)
                {
                    ILineSegment seg = segmentList[i];
                    if (seg.StartAt + seg.Length >= caret_char_index)
                    {
                        //stop at this segment
                        startAt = seg.StartAt;
                        len = seg.Length;
                        return;
                    }
                }
            }
            else
            {
                //TODO: review here
                //this is a bug!!!
            }
            //?
            startAt = 0;
            len = 0;

        }
        bool MoveToPreviousTextRun()
        {
#if DEBUG
            if (currentTextRun.IsLineBreak)
            {
                throw new NotSupportedException();
            }
#endif
            if (currentTextRun.PrevTextRun != null)
            {
                currentTextRun = currentTextRun.PrevTextRun;
                rCharOffset -= currentTextRun.CharacterCount;
                rPixelOffset -= currentTextRun.Width;
                caret_char_index = rCharOffset + currentTextRun.CharacterCount;
                caretXPos = rPixelOffset + currentTextRun.Width;
                return true;
            }
            return false;
        }

        bool MoveToNextTextRun()
        {
#if DEBUG
            if (currentTextRun.IsLineBreak)
            {
                throw new NotSupportedException();
            }
#endif


            EditableRun nextTextRun = currentTextRun.NextTextRun;
            if (nextTextRun != null && !nextTextRun.IsLineBreak)
            {
                rCharOffset += currentTextRun.CharacterCount;
                rPixelOffset += currentTextRun.Width;
                currentTextRun = nextTextRun;
                caret_char_index = rCharOffset;
                caretXPos = rPixelOffset + currentTextRun.GetRunWidth(0);
                return true;
            }
            return false;
        }

        public void MoveToLine(int lineNumber)
        {
            currentLine = _textFlowLayer.GetTextLine(lineNumber);
            currentLineY = currentLine.Top;

            //if current line is a blank line
            //not first run => currentTextRun= null 
            currentTextRun = (EditableRun)currentLine.FirstRun;

            rCharOffset = 0;
            rPixelOffset = 0;

            caret_char_index = 0;
            caretXPos = 0;
        }
        public void CopyContentToStrignBuilder(StringBuilder stBuilder)
        {
            _textFlowLayer.CopyContentToStringBuilder(stBuilder);
        }
        public char PrevChar
        {
            get
            {
                if (currentTextRun != null)
                {

                    if (caret_char_index == 0 && CharCount == 0)
                    {
                        return '\0';
                    }
                    if (caret_char_index == rCharOffset)
                    {
                        if (currentTextRun.PrevTextRun != null)
                        {
                            return (currentTextRun.PrevTextRun).GetChar(currentTextRun.PrevTextRun.CharacterCount - 1);
                        }
                        else
                        {
                            return '\0';
                        }
                    }
                    else
                    {
                        return currentTextRun.GetChar(caret_char_index - rCharOffset);
                    }
                }
                else
                {
                    return '\0';
                }
            }
        }
        public char NextChar
        {
            get
            {
                if (currentTextRun != null)
                {

                    if (caret_char_index == 0 && CharCount == 0)
                    {
                        return '\0';
                    }
                    if (caret_char_index == rCharOffset + currentTextRun.CharacterCount)
                    {
                        if (currentTextRun.NextTextRun != null)
                        {
                            return (currentTextRun.NextTextRun).GetChar(0);
                        }
                        else
                        {
                            return '\0';
                        }
                    }
                    else
                    {
                        return currentTextRun.GetChar(caret_char_index - rCharOffset);
                    }
                }
                else
                {
                    return '\0';
                }
            }
        }
        /// <summary>
        /// next char width in this line
        /// </summary>
        public int NextCharWidth
        {
            get
            {
                if (currentTextRun != null)
                {
                    if (CharCount == 0)
                    {
                        //no text in this line
                        return 0;
                    }
                    if (caret_char_index == rCharOffset + currentTextRun.CharacterCount)
                    {
                        //-----
                        //this is on the last of current run
                        //if we have next run, just get run of next width
                        //-----
                        EditableRun nextRun = currentTextRun.NextTextRun;
                        if (nextRun != null)
                        {
                            return nextRun.GetRunWidth(0);
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        //in some line

                        return currentTextRun.GetRunWidth(caret_char_index - rCharOffset + 1) -
                                    currentTextRun.GetRunWidth(caret_char_index - rCharOffset);
                    }
                }
                else
                {
                    return 0;
                }
            }
        }

        public EditableVisualPointInfo GetCurrentPointInfo()
        {
#if DEBUG
            if (currentTextRun != null && !currentTextRun.HasParent)
            {
                throw new NotSupportedException();
            }
            if (currentTextRun == null)
            {
            }
#endif      
            EditableVisualPointInfo textPointInfo =
                new EditableVisualPointInfo(currentLine, caret_char_index);
            textPointInfo.SetAdditionVisualInfo(currentTextRun,
                rCharOffset, caretXPos, rPixelOffset);
            return textPointInfo;
        }
        public Point CaretPosition
        {
            get
            {
                return new Point(caretXPos, currentLineY);
            }
        }

        public char CurrentChar
        {
            get
            {
                //1. blank line
                if (currentTextRun == null)
                {
                    return '\0';
                }
                else
                {
                    //2.  
                    if (currentTextRun.CharacterCount == caret_char_index - rCharOffset)
                    {
                        //end of this run
                        return '\0';
                    }

                    return currentTextRun.GetChar(caret_char_index - rCharOffset);
                }
            }
        }
        public int CurrentTextRunCharOffset
        {
            get
            {
                return rCharOffset;
            }
        }
        public int CurrentTextRunPixelOffset
        {
            get
            {
                return rPixelOffset;
            }
        }
        public int CurrentTextRunCharIndex
        {
            get
            {
                return caret_char_index - rCharOffset - 1;
            }
        }
        /// <summary>
        /// try set caret x pos to nearest request value
        /// </summary>
        /// <param name="xpos"></param>
        public void TrySetCaretPos(int xpos, int ypos)
        {

            //--------
            _textFlowLayer.NotifyHitOnSolidTextRun(null);
            //--------
            if (currentTextRun == null)
            {
                caret_char_index = 0;
                caretXPos = 0;
                rCharOffset = 0;
                rPixelOffset = 0;
                return;
            }
            int pixDiff = xpos - caretXPos;
            if (pixDiff > 0)
            {
                do
                {
                    int thisTextRunPixelLength = currentTextRun.Width;
                    if (rPixelOffset + thisTextRunPixelLength > xpos)
                    {
                        EditableRunCharLocation foundLocation = EditableRun.InnerGetCharacterFromPixelOffset(currentTextRun, xpos - rPixelOffset);
                        caretXPos = rPixelOffset + foundLocation.pixelOffset;
                        caret_char_index = rCharOffset + foundLocation.RunCharIndex;

                        //for solid text run
                        //we can send some event to it
                        SolidTextRun solidTextRun = currentTextRun as SolidTextRun;
                        if (solidTextRun != null)
                        {
                            _textFlowLayer.NotifyHitOnSolidTextRun(solidTextRun);
                        }

                        //if (foundLocation.charIndex == -1)
                        //{
                        //    if (!(MoveToPreviousTextRun()))
                        //    {
                        //        caretXPos = 0;
                        //        caret_char_index = 0;
                        //    }
                        //}
                        //else
                        //{
                        //    caretXPos = rPixelOffset + foundLocation.pixelOffset;
                        //    caret_char_index = rCharOffset + foundLocation.charIndex;
                        //}
                        return;
                    }
                } while (MoveToNextTextRun());
                //to the last
                caretXPos = rPixelOffset + currentTextRun.Width;
                caret_char_index = rCharOffset + currentTextRun.CharacterCount;
                return;
            }
            else if (pixDiff < 0)
            {
                do
                {
                    if (xpos >= rPixelOffset)
                    {
                        EditableRunCharLocation foundLocation = EditableRun.InnerGetCharacterFromPixelOffset(currentTextRun, xpos - rPixelOffset);
                        caretXPos = rPixelOffset + foundLocation.pixelOffset;
                        caret_char_index = rCharOffset + foundLocation.RunCharIndex;


                        //for solid text run
                        //we can send some event to it
                        SolidTextRun solidTextRun = currentTextRun as SolidTextRun;
                        if (solidTextRun != null)
                        {
                            _textFlowLayer.NotifyHitOnSolidTextRun(solidTextRun);
                        }


                        //if (foundLocation.charIndex == -1)
                        //{
                        //    if (!MoveToPreviousTextRun())
                        //    {
                        //        caret_char_index = 0;
                        //        caretXPos = 0;
                        //    }
                        //}
                        //else
                        //{
                        //    caretXPos = rPixelOffset + foundLocation.pixelOffset;
                        //    caret_char_index = rCharOffset + foundLocation.RunCharIndex;
                        //}
                        return;
                    }
                } while (MoveToPreviousTextRun());//
                caretXPos = 0;
                caret_char_index = 0;
                return;
            }
        }
        public int CaretXPos
        {
            get
            {
                return caretXPos;
            }

        }

        public int CharIndex
        {
            get { return caret_char_index; }
        }
        int InternalCharIndex
        {
            get
            {
                return caret_char_index;
            }
        }
        public void SetCurrentCharStepRight()
        {
            SetCurrentCharIndex(InternalCharIndex + 1);
        }
        public void SetCurrentCharStepLeft()
        {
            SetCurrentCharIndex(InternalCharIndex - 1);
        }
        public void SetCurrentCharIndexToEnd()
        {
            SetCurrentCharIndex(this.CharCount);
        }
        public void SetCurrentCharIndexToBegin()
        {
            SetCurrentCharIndex(0);
        }
        public void SetCurrentCharIndex(int newCharIndexPointTo)
        {

#if DEBUG
            if (dbugTextManRecorder != null)
            {
                dbugTextManRecorder.WriteInfo("TextLineReader::CharIndex_set=" + newCharIndexPointTo);
                dbugTextManRecorder.BeginContext();
            }
#endif
            if (newCharIndexPointTo < 0 || newCharIndexPointTo > currentLine.CharCount)
            {
                throw new NotSupportedException("index out of range");
            }


            if (newCharIndexPointTo == 0)
            {
                caret_char_index = 0;
                caretXPos = 0;
                rCharOffset = 0;
                rPixelOffset = 0;
                currentTextRun = currentLine.FirstRun;
            }
            else
            {
                int diff = newCharIndexPointTo - caret_char_index;
                switch (diff)
                {
                    case 0:
                        {
                            return;
                        }

                    default:
                        {
                            if (diff > 0)
                            {
                                do
                                {
                                    if (rCharOffset + currentTextRun.CharacterCount >= newCharIndexPointTo)
                                    {
                                        caret_char_index = newCharIndexPointTo;
                                        caretXPos = rPixelOffset + currentTextRun.GetRunWidth(caret_char_index - rCharOffset);
#if DEBUG
                                        if (dbugTextManRecorder != null)
                                        {
                                            dbugTextManRecorder.EndContext();
                                        }
#endif

                                        return;
                                    }
                                } while (MoveToNextTextRun());
                                caret_char_index = rCharOffset + currentTextRun.CharacterCount;
                                caretXPos = rPixelOffset + currentTextRun.Width;
                                return;
                            }
                            else
                            {
                                do
                                {
                                    if (rCharOffset - 1 < newCharIndexPointTo)
                                    {
                                        caret_char_index = newCharIndexPointTo;
                                        caretXPos = rPixelOffset + currentTextRun.GetRunWidth(caret_char_index - rCharOffset);
#if DEBUG
                                        if (dbugTextManRecorder != null)
                                        {
                                            dbugTextManRecorder.EndContext();
                                        }
#endif
                                        return;
                                    }
                                } while (MoveToPreviousTextRun());
                                caret_char_index = 0;
                                caretXPos = 0;
                            }
                        }
                        break;
                }
            }
#if DEBUG
            if (dbugTextManRecorder != null)
            {
                dbugTextManRecorder.EndContext();
            }
#endif

        }


        public bool IsOnEndOfLine
        {
            get
            {
                return caret_char_index == currentLine.CharCount;
            }
        }

        internal EditableTextLine GetTextLine(int lineId)
        {
            return TextLayer.GetTextLine(lineId);
        }

        internal EditableTextLine GetTextLineAtPos(int y)
        {
            return this.TextLayer.GetTextLineAtPos(y);
        }
        public int LineCount
        {
            get
            {
                return TextLayer.LineCount;
            }
        }

        public bool HasNextLine
        {
            get
            {
                return currentLine.Next != null;
            }
        }
        public bool HasPrevLine
        {
            get
            {
                return currentLine.Prev != null;
            }
        }
        public bool IsOnStartOfLine
        {
            get
            {
                return InternalCharIndex == 0;
            }
        }
        public int CharCount
        {
            get
            {
                return currentLine.CharCount;
            }
        }
        public void CopyLineContent(StringBuilder stBuilder)
        {
            currentLine.CopyLineContent(stBuilder);
        }
        public void CopySelectedTextRuns(VisualSelectionRange selectionRange, List<EditableRun> output)
        {
            currentLine.Copy(selectionRange, output);
        }

        public int LineNumber
        {
            get
            {
                return currentLine.LineNumber;
            }
        }
        public void MoveToNextLine()
        {
            MoveToLine(currentLine.LineNumber + 1);
        }
        public void MoveToPrevLine()
        {
            MoveToLine(currentLine.LineNumber - 1);
        }
        public Rectangle LineArea
        {
            get
            {
                return currentLine.ActualLineArea;
            }
        }
        public Rectangle ParentLineArea
        {
            get
            {
                return currentLine.ParentLineArea;
            }
        }


        internal EditableTextFlowLayer TextLayer
        {
            get
            {
                return currentLine.OwnerFlowLayer;
            }
        }
    }
    //class BackGroundTextLineWriter : TextLineWriter
    //{
    //    public BackGroundTextLineWriter(EditableTextFlowLayer visualElementLayer)
    //        : base(visualElementLayer)
    //    {
    //    }
    //}
}