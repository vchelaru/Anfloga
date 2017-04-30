﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace RenderingLibrary.Graphics
{
    public class Layer
    {
        #region Fields

        List<IRenderableIpso> mRenderables = new List<IRenderableIpso>();

        ReadOnlyCollection<IRenderableIpso> mRenderablesReadOnly;

        #endregion

        #region Properties

        public IRenderableIpso ScissorIpso { get; set; }

        public LayerCameraSettings LayerCameraSettings
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public ReadOnlyCollection<IRenderableIpso> Renderables
        {
            get
            {
                return mRenderablesReadOnly;
            }
        }

        //internal List<IRenderableIpso> RenderablesWriteable
        //{
        //    get
        //    {
        //        return mRenderables;
        //    }
        //}

        public Layer ParentLayer
        {
            get;
            set;
        }

        #endregion

        public Layer()
        {
            mRenderablesReadOnly = new ReadOnlyCollection<IRenderableIpso>(mRenderables);
        }

        public void Add(IRenderableIpso renderable)
        {
            lock (mRenderables)
            {
                mRenderables.Add(renderable);
            }
        }

        public void Remove(IRenderableIpso renderable)
        {
            mRenderables.Remove(renderable);
        }

        /// <summary>
        /// This is a stable sort on Z.  It's incredibly fast on already-sorted lists so we'll do this over something like the built-in 
        /// binary sorts that .NET offers.
        /// </summary>
        internal void SortRenderables()
        {
            if (mRenderables.Count == 1 || mRenderables.Count == 0)
                return;

            int whereObjectBelongs;

            for (int i = 1; i < mRenderables.Count; i++)
            {
                if ((mRenderables[i]).Z < (mRenderables[i - 1]).Z)
                {
                    if (i == 1)
                    {
                        mRenderables.Insert(0, mRenderables[i]);
                        mRenderables.RemoveAt(i + 1);
                        continue;
                    }

                    for (whereObjectBelongs = i - 2; whereObjectBelongs > -1; whereObjectBelongs--)
                    {
                        if ((mRenderables[i]).Z >= (mRenderables[whereObjectBelongs]).Z)
                        {
                            mRenderables.Insert(whereObjectBelongs + 1, mRenderables[i]);
                            mRenderables.RemoveAt(i + 1);
                            break;
                        }
                        else if (whereObjectBelongs == 0 && (mRenderables[i]).Z < (mRenderables[0]).Z)
                        {
                            mRenderables.Insert(0, mRenderables[i]);
                            mRenderables.RemoveAt(i + 1);
                            break;
                        }
                    }
                }
            }
        }

        public override string ToString()
        {
            return Name + " : " + mRenderables.Count + " IRenderables";
        }

        public bool ContainsRenderable(IRenderable whatToTest)
        {
            if (this.Renderables.Contains(whatToTest))
            {
                return true;
            }

            foreach (IRenderable renderable in this.Renderables)
            {
                if (renderable is SortableLayer)
                {
                    if (((SortableLayer)renderable).ContainsRenderable(whatToTest))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

    }
}
