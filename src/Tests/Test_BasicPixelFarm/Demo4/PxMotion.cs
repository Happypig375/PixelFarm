﻿//MIT, 2014-present,WinterDev

using System;
using PixelFarm;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.VectorMath;

using LayoutFarm.Svg;
using PaintLab.Svg;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm.UI
{
    public abstract class BasicSprite : UIElement
    {
        public double _angle = 0;
        public double _spriteScale = 1.0;
        public double _skewX = 0;
        public double _skewY = 0;
        public override void Walk(UIVisitor visitor)
        {

        }
        public int Width { get; set; }
        public int Height { get; set; }
       
    }


    public class MyTestSprite : BasicSprite
    {
        SpriteShape _spriteShape;
        VgRenderVx _vgRenderVx;

        float _posX, _posY;
        float _mouseDownX, _mouseDownY;
        Affine _currentTx = null;
        byte alpha;
        bool _hitTestOnSubPart;
        public MyTestSprite(VgRenderVx vgRenderVx)
        {
            this.Width = 500;
            this.Height = 500;
            AlphaValue = 255;
            _vgRenderVx = vgRenderVx;
        }
        public float X { get { return _posX; } }
        public float Y { get { return _posY; } }
        public SpriteShape SpriteShape
        {
            get { return _spriteShape; }
            set { _spriteShape = value; }
        }
        public int SharpenRadius
        {
            get;
            set;
        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return _spriteShape; }
        }
        public override void InvalidateGraphics()
        {
            if (this.HasReadyRenderElement)
            {
                //invalidate 'bubble' rect 
                //is (0,0,w,h) start invalidate from current primary render element
                this.CurrentPrimaryRenderElement.InvalidateGraphics();
            }
        }

        protected override bool HasReadyRenderElement
        {
            get { return _spriteShape != null; }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (_spriteShape == null)
            {
                //TODO: review bounds again
                RectD bounds = _vgRenderVx.GetBounds();
                _spriteShape = new SpriteShape(_vgRenderVx, rootgfx, (int)bounds.Width, (int)bounds.Height);
                _spriteShape.SetController(this);//listen event 
                _spriteShape.SetLocation((int)_posX, (int)_posY);
            }
            return _spriteShape;
        }
        public bool HitTestOnSubPart
        {
            get { return _hitTestOnSubPart; }
            set
            {
                _hitTestOnSubPart = value;
                if (_spriteShape != null)
                {
                    _hitTestOnSubPart = value;
                }
            }
        }
        public byte AlphaValue
        {
            get { return this.alpha; }
            set
            {
                this.alpha = value;
                //change alpha value
                //TODO: review here...   
                if (_spriteShape != null)
                {
                    _spriteShape.ApplyNewAlpha(value);
                }

                //int j = lionShape.NumPaths;
                //var colorBuffer = lionShape.Colors;
                //for (int i = lionShape.NumPaths - 1; i >= 0; --i)
                //{
                //    colorBuffer[i] = colorBuffer[i].NewFromChangeAlpha(alpha);
                //}
            }
        }

        public Affine CurrentAffineTx { get { return _currentTx; } }
        public void SetLocation(float left, float top)
        {
            _posX = left;
            _posY = top;
            if (_spriteShape != null)
            {
                _spriteShape.SetLocation((int)_posX, (int)_posY);
            }

        }


        public SvgRenderElement HitTest(float x, float y, bool withSupPart)
        {
            SvgRenderElement result = null;
            VgHitChainPool.GetFreeHitTestChain(out SvgHitChain svgHitChain);
            svgHitChain.WithSubPartTest = withSupPart;
            if (HitTest(x, y, svgHitChain))
            {
                int hitCount = svgHitChain.Count;
                if (hitCount > 0)
                {
                    result = svgHitChain.GetLastHitInfo().svg;
                }
            }
            VgHitChainPool.ReleaseHitTestChain(ref svgHitChain);
            return result;
        }
        public bool HitTest(float x, float y, SvgHitChain svgHitChain)
        {
            RectD bounds = _spriteShape.Bounds;
            if (bounds.Contains(x, y))
            {
                _mouseDownX = x;
                _mouseDownY = y;

                //....
                if (svgHitChain.WithSubPartTest)
                {
                    //fine hit on sup part***

                    svgHitChain.SetHitTestPos(x, y);
                    _spriteShape.HitTestOnSubPart(svgHitChain);
                    //check if we hit on sup part
                    int hitCount = svgHitChain.Count;
                    if (hitCount > 0)
                    {
                        SvgRenderElement svgElem = svgHitChain.GetLastHitInfo().svg;
                        //if yes then change its bg color
                        svgElem.VisualSpec.FillColor = Color.Red;
                        _spriteShape.InvalidateGraphics();
                    }
                    return hitCount > 0;
                }
                return true;
            }
            else
            {
                _mouseDownX = _mouseDownY = 0;
            }
            return false;
        }


        //public override void Render(PixelFarm.Drawing.Painter p)
        //{
        //    if (_currentTx == null)
        //    {
        //        _currentTx = Affine.NewMatix(
        //              AffinePlan.Translate(-_spriteShape.Center.x, -_spriteShape.Center.y),
        //              AffinePlan.Scale(_spriteScale, _spriteScale),
        //              AffinePlan.Rotate(_angle + Math.PI),
        //              AffinePlan.Skew(_skewX / 1000.0, _skewY / 1000.0),
        //              AffinePlan.Translate(Width / 2, Height / 2)
        //      );
        //    }

        //    if (JustMove)
        //    {
        //        float ox = p.OriginX;
        //        float oy = p.OriginY;

        //        p.SetOrigin(ox + _posX, oy + _posY);
        //        _spriteShape.Paint(p);
        //        p.SetOrigin(ox, oy);

        //    }
        //    else
        //    {
        //        _spriteShape.Paint(p, _currentTx);
        //    }

        //}

        public SpriteShape GetSpriteShape()
        {
            return _spriteShape;
        }
    }

    public class SvgRenderVxLoader
    {
        public static VgRenderVx CreateSvgRenderVxFromFile(string filename)
        {
            SvgDocBuilder docBuilder = new SvgDocBuilder();
            SvgParser svg = new SvgParser(docBuilder);
            SvgRenderVxDocBuilder builder = new SvgRenderVxDocBuilder();

            //svg.ReadSvgFile("d:\\WImageTest\\lion.svg");
            //svg.ReadSvgFile("d:\\WImageTest\\tiger001.svg");
            svg.ReadSvgFile(filename);
            return builder.CreateRenderVx(docBuilder.ResultDocument);
        }

    }

    public class SpriteShape : RenderElement
    {
        VgRenderVx _svgRenderVx;
        byte _alpha;
        Vector2 _center;
        RectD _boundingRect;
        Affine _currentTx;
        public SpriteShape(VgRenderVx svgRenderVx, RootGraphic root, int w, int h)
                   : base(root, w, h)
        {
            LoadFromSvg(svgRenderVx);
        }
        public bool EnableHitOnSupParts { get; set; }
        protected override bool _MayHasOverlapChild()
        {
            return EnableHitOnSupParts;
        }

        public RectD Bounds
        {
            get
            {
                return _boundingRect;
            }
        }
        public void ResetTransform()
        {
            _currentTx = null;
        }
        public void ApplyTransform(Affine tx)
        {
            //apply transform to all part
            if (_currentTx == null)
            {
                _currentTx = tx;
            }
            else
            {
                //ORDER is IMPORTANT
                _currentTx = _currentTx * tx;
            }
        }
        public void ApplyTransform(Bilinear tx)
        {
            //int elemCount = _svgRenderVx.SvgVxCount;
            //for (int i = 0; i < elemCount; ++i)
            //{
            //    _svgRenderVx.SetInnerVx(i, SvgCmd.TransformToNew(_svgRenderVx.GetInnerVx(i), tx));
            //}
        }
        public Vector2 Center
        {
            get
            {
                return _center;
            }
        }
        public VgRenderVx GetRenderVx()
        {
            return _svgRenderVx;
        }

        public void ApplyNewAlpha(byte alphaValue0_255)
        {
            _alpha = alphaValue0_255;
        }
        public void Paint(Painter p)
        {
            VgPainterArgsPool.GetFreePainterArgs(p, out VgPaintArgs paintArgs);
            paintArgs._currentTx = _currentTx;
            _svgRenderVx._renderE.Paint(paintArgs);
            VgPainterArgsPool.ReleasePainterArgs(ref paintArgs);
        }
        public void Paint(VgPaintArgs paintArgs)
        {
            _svgRenderVx._renderE.Paint(paintArgs);
        }

        public void Paint(Painter p, PixelFarm.CpuBlit.VertexProcessing.Perspective tx)
        {
            //TODO: implement this...
            //use prefix command for render vx
            //p.Render(_svgRenderVx);
            //_svgRenderVx.Render(p);
        }
        public void Paint(Painter p, PixelFarm.CpuBlit.VertexProcessing.Affine tx)
        {
            //TODO: implement this...
            //use prefix command for render vx 
            //------
            VgPainterArgsPool.GetFreePainterArgs(p, out VgPaintArgs paintArgs);
            paintArgs._currentTx = tx;
            paintArgs.ExternalVxsVisitHandler = (vxs, painterA) =>
            {
                //use external painter handler
                //draw only outline with its fill-color.
                Painter m_painter = paintArgs.P;
                Color prevFillColor = m_painter.FillColor;
                m_painter.FillColor = m_painter.FillColor;
                m_painter.Fill(vxs);
                m_painter.FillColor = prevFillColor;
            };
            _svgRenderVx._renderE.Paint(paintArgs);
            VgPainterArgsPool.ReleasePainterArgs(ref paintArgs);

        }
        public void DrawOutline(Painter p)
        {
            //walk all parts and draw only outline 
            //not fill
            //int renderVxCount = _svgRenderVx.VgCmdCount;
            //for (int i = 0; i < renderVxCount; ++i)
            //{ 
            //} 
            //int j = lionShape.NumPaths;
            //int[] pathList = lionShape.PathIndexList;
            //Drawing.Color[] colors = lionShape.Colors;

            //var vxs = GetFreeVxs();
            //var vxs2 = stroke1.MakeVxs(affTx.TransformToVxs(lionShape.Vxs, vxs), GetFreeVxs());
            //for (int i = 0; i < j; ++i)
            //{
            //    p.StrokeColor = colors[i];
            //    p.Draw(new PixelFarm.Drawing.VertexStoreSnap(vxs2, pathList[i]));

            //}
            ////not agg   
            //Release(ref vxs);
            //Release(ref vxs2);
            //return; //** 
        }

        public void LoadFromSvg(VgRenderVx svgRenderVx)
        {
            _svgRenderVx = svgRenderVx;
            UpdateBounds();
            //find center 
            _center.x = (_boundingRect.Right - _boundingRect.Left) / 2.0;
            _center.y = (_boundingRect.Top - _boundingRect.Bottom) / 2.0;
        }
        public void UpdateBounds()
        {
            _svgRenderVx.InvalidateBounds();
            this._boundingRect = _svgRenderVx.GetBounds();

            _boundingRect.Offset(this.X, this.Y);
            SetSize((int)_boundingRect.Width, (int)_boundingRect.Height);
        }
        public void HitTestOnSubPart(SvgHitChain hitChain)
        {

            _svgRenderVx._renderE.HitTest(hitChain);
        }

        public override void ResetRootGraphics(RootGraphic rootgfx)
        {
            DirectSetRootGraphics(this, rootgfx);
        }
        public override void ChildrenHitTestCore(HitChain hitChain)
        {
            base.ChildrenHitTestCore(hitChain);
        }
        public override void CustomDrawToThisCanvas(DrawBoard canvas, Rectangle updateArea)
        {
            Painter p = canvas.GetPainter();

            if (p != null)
            {
                float ox = p.OriginX;
                float oy = p.OriginY;
                //create agg's painter?
                p.SetOrigin(ox + X, oy + Y);
                Paint(p);
                p.SetOrigin(ox, oy);
            }
        }


    }
    public static class VgHitChainPool
    {
        //
        //
        [System.ThreadStatic]
        static System.Collections.Generic.Stack<SvgHitChain> s_hitChains = new System.Collections.Generic.Stack<SvgHitChain>();

        public static void GetFreeHitTestChain(out SvgHitChain hitTestArgs)
        {
            if (s_hitChains.Count > 0)
            {
                hitTestArgs = s_hitChains.Pop();
            }
            else
            {
                hitTestArgs = new SvgHitChain();
            }
        }
        public static void ReleaseHitTestChain(ref SvgHitChain hitTestArgs)
        {
            hitTestArgs.Clear();
            s_hitChains.Push(hitTestArgs);
            hitTestArgs = null;
        }
    }
}
