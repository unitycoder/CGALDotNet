﻿using System;
using System.Collections.Generic;
using System.Text;

using CGALDotNet.Geometry;
using CGALDotNet.Polygons;

namespace CGALDotNet.Triangulations
{
    /// <summary>
    /// Generic triangulation class.
    /// </summary>
    /// <typeparam name="K">The kerel.</typeparam>
    public sealed class Triangulation3<K> : Triangulation3 where K : CGALKernel, new()
    {
        /// <summary>
        /// Static instance of a triangulation.
        /// </summary>
        public static readonly Triangulation3<K> Instance = new Triangulation3<K>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Triangulation3() : base(new K())
        {

        }

        /// <summary>
        /// Construct a triangulation from the points.
        /// </summary>
        /// <param name="points">The triangulation points.</param>
        public Triangulation3(Point3d[] points) : base(new K(), points)
        {

        }

        /// <summary>
        /// Construct from a existing triangulation.
        /// </summary>
        /// <param name="ptr">A pointer to the unmanaged object.</param>
        internal Triangulation3(IntPtr ptr) : base(new K(), ptr)
        {

        }

        /// <summary>
        /// The triangulation as a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[Triangulation3<{0}>: VertexCount={1}, CellCount={2}, FacetCount={3}]",
                Kernel.KernelName, VertexCount, CellCount, FacetCount);
        }

        /// <summary>
        /// Create a deep copy of the triangulation.
        /// </summary>
        /// <returns>The deep copy.</returns>
        public Triangulation3<K> Copy()
        {
            return new Triangulation3<K>(Kernel.Copy(Ptr));
        }

    }

    /// <summary>
    /// Abstract base class for the triagulation.
    /// </summary>
    public abstract class Triangulation3 : BaseTriangulation3
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="kernel"></param>
        internal Triangulation3(CGALKernel kernel)
            : base(kernel.TriangulationKernel3)
        {
            TriangulationKernel = Kernel as TriangulationKernel3;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kernel"></param>
        /// <param name="points"></param>
        internal Triangulation3(CGALKernel kernel, Point3d[] points)
            : base(kernel.TriangulationKernel3, points)
        {
            TriangulationKernel = Kernel as TriangulationKernel3;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kernel"></param>
        /// <param name="ptr"></param>
        internal Triangulation3(CGALKernel kernel, IntPtr ptr)
            : base(kernel.TriangulationKernel3, ptr)
        {
            TriangulationKernel = Kernel as TriangulationKernel3;
        }

        /// <summary>
        /// The kernel with the functions unique to the triangulation.
        /// </summary>
        protected private TriangulationKernel3 TriangulationKernel { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        public override void Print(StringBuilder builder)
        {
            builder.AppendLine(ToString());
            builder.AppendLine("Is valid = " + IsValid());
            builder.AppendLine("BuildStamp = " + BuildStamp);
            builder.AppendLine("VertexCount = " + VertexCount);
            builder.AppendLine("CellCount = " + CellCount);
            builder.AppendLine("FiniteCellCount = " + FiniteCellCount);
            builder.AppendLine("EdgeCount = " + EdgeCount);
            builder.AppendLine("FiniteEdgeCount = " + FiniteEdgeCount);
            builder.AppendLine("FacetCount = " + FacetCount);
            builder.AppendLine("FiniteFacetCount = " + FiniteFacetCount);
        }

    }
}
