﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using RenderingLibrary.Graphics;

namespace RenderingLibrary.Math.Geometry
{
    public class ShapeManager
    {
        #region Fields

        static ShapeManager mSelf;

        List<LineRectangle> mRectangles = new List<LineRectangle>();
        List<SolidRectangle> mSolidRectangles = new List<SolidRectangle>();
        List<LineCircle> mCircles = new List<LineCircle>();
        List<LineGrid> mGrids = new List<LineGrid>();
        List<Line> mLines = new List<Line>();

        #endregion

        #region Properties
        public SystemManagers Managers
        {
            get;
            set;
        }

        public static ShapeManager Self
        {
            get
            {
                if (mSelf == null)
                {
                    mSelf = new ShapeManager();
                }
                return mSelf;
            }
        }
        Renderer Renderer
        {
            get
            {
                if (Managers == null)
                {
                    return Renderer.Self;
                }
                else
                {
                    return Managers.Renderer;
                }
            }
        }


        public IEnumerable<LineRectangle> Rectangles { get { return mRectangles; } }
        public IEnumerable<SolidRectangle> SolidRectangles { get { return mSolidRectangles; } }
        public IEnumerable<LineCircle> Circles { get { return mCircles; } }
        public IEnumerable<LineGrid> Grids { get { return mGrids; } }
        public IEnumerable<Line> Lines { get { return mLines; } }
        #endregion

        public void Add(LineRectangle lineRectangle)
        {
            Add(lineRectangle, Renderer.LayersWritable[0]);
        }

        public void Add(LineRectangle lineRectangle, Layer layer)
        {
            if (layer == null)
            {
                layer = Renderer.LayersWritable[0];
            }

            mRectangles.Add(lineRectangle);
            
            layer.Add(lineRectangle);
        }

        public void Add(SolidRectangle solidRectangle, Layer layer = null)
        {
            if (layer == null)
            {
                layer = Renderer.LayersWritable[0];
            }

            mSolidRectangles.Add(solidRectangle);
            layer.Add(solidRectangle);
        }

        public void Add(LineGrid lineGrid, Layer layer = null)
        {
            if (layer == null)
            {
                layer = Renderer.LayersWritable[0];
            }

            mGrids.Add(lineGrid);
            layer.Add(lineGrid);

        }

        public void Add(Line line, Layer layer = null)
        {
            if(layer == null)
            {
                layer = Renderer.LayersWritable[0];
            }

            mLines.Add(line);
            layer.Add(line);
        }

        public void Add(LineCircle lineCircle)
        {
            Add(lineCircle, Renderer.LayersWritable[0]);
        }
            
        public void Add(LineCircle lineCircle, Layer layer)
        {
            mCircles.Add(lineCircle);

            if (layer == null)
            {
                layer = Renderer.LayersWritable[0];
            }

            layer.Add(lineCircle);
        }




        public void Remove(LineRectangle linePrimitive)
        {
            // todo:  Need to make this remove from whatever Layer the line rectangle is on
            mRectangles.Remove(linePrimitive);
            Renderer.RemoveRenderable(linePrimitive);
        }

        public void Remove(SolidRectangle solidRectangle)
        {
            mSolidRectangles.Remove(solidRectangle);
            Renderer.RemoveRenderable(solidRectangle);
        }

        //public void Remove(LineGrid lineGrid)
        //{           
        //    // todo:  Need to make this remove from whatever Layer the line rectangle is on
        //    mGrids.Remove(lineGrid);
        //    Renderer.RemoveRenderable(lineGrid);
        //}

        public void Remove(LineCircle lineCircle)
        {
            mCircles.Remove(lineCircle);
            Renderer.RemoveRenderable(lineCircle);
        }

        public void Remove(Line line)
        {
            mLines.Remove(line);
            Renderer.RemoveRenderable(line);
        }
    }
}
