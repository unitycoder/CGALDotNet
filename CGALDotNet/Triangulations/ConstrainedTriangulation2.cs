﻿using System;
using System.Collections.Generic;
using System.Text;

using CGALDotNet.Geometry;
using CGALDotNet.Polygons;

namespace CGALDotNet.Triangulations
{
    /// <summary>
    /// The generic constrained triangulation class.
    /// </summary>
    /// <typeparam name="K">The kernel</typeparam>
    public sealed class ConstrainedTriangulation2<K> : ConstrainedTriangulation2 where K : CGALKernel, new()
    {
        /// <summary>
        /// A static instance of the triangulation.
        /// </summary>
        public static readonly ConstrainedTriangulation2<K> Instance = new ConstrainedTriangulation2<K>();

        /// <summary>
        /// 
        /// </summary>
        public ConstrainedTriangulation2() : base(new K())
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        public ConstrainedTriangulation2(Point2d[] points) : base(new K(), points)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ptr"></param>
        internal ConstrainedTriangulation2(IntPtr ptr) : base(new K(), ptr)
        {

        }

        /// <summary>
        /// The triangulation as a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("[ConstrainedTriangulation2<{0}>: VertexCount={1}, FaceCount={2}]",
                Kernel.Name, VertexCount, TriangleCount);
        }

        /// <summary>
        /// A deep copy of the triangulation.
        /// </summary>
        /// <returns>The deep copy.</returns>
        public ConstrainedTriangulation2<K> Copy()
        {
            return new ConstrainedTriangulation2<K>(Kernel.Copy(Ptr));
        }

        /// <summary>
        /// Insert the polygons points into the triangulation.
        /// May not retatin the poylgons edges.
        /// </summary>
        /// <param name="polygon"></param>
        public void InsertPolygon(Polygon2<K> polygon)
        {
            Kernel.InsertPolygon(Ptr, polygon.Ptr);
        }

        /// <summary>
        /// Insert the polygons points into the triangulation.
        /// May not retatin the poylgons edges.
        /// </summary>
        /// <param name="pwh"></param>
        public void InsertPolygon(PolygonWithHoles2<K> pwh)
        {
            Kernel.InsertPolygonWithHoles(Ptr, pwh.Ptr);
        }

        /// <summary>
        /// Insert the polygons points and the edges as constraints into the triangulation.
        /// Will retatin the poylgons edges.
        /// </summary>
        /// <param name="polygon">The polygon to insert.</param>
        public void InsertPolygonConstraint(Polygon2<K> polygon)
        {
            TriangulationKernel.InsertPolygonConstraint(Ptr, polygon.Ptr);
        }

        /// <summary>
        /// Insert the polygons points and the edges as constraints into the triangulation.
        /// Will retatin the poylgons edges.
        /// </summary>
        /// <param name="pwh">The polygon to insert.</param>
        public void InsertPolygonConstraint(PolygonWithHoles2<K> pwh)
        {
            TriangulationKernel.InsertPolygonWithHolesConstraint(Ptr, pwh.Ptr);
        }

        /*

        /// <summary>
        /// 
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="indices"></param>
        public void GetPolygonIndices(Polygon2<K> polygon, List<int> indices)
        {
            InsertPolygonConstraint(polygon);

            int count = IndiceCount;
            if (count == 0) return;

            var orientation = polygon.Orientation;

            int[] tmp = new int[count];
            count = TriangulationKernel.GetPolygonIndices(Ptr, polygon.Ptr, tmp, 0, tmp.Length, orientation);

            for (int i = 0; i < count; i++)
                indices.Add(tmp[i]);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pwh"></param>
        /// <param name="indices"></param>
        public void GetPolygonIndices(PolygonWithHoles2<K> pwh, List<int> indices)
        {
            InsertPolygonConstraint(pwh);

            int count = IndiceCount;
            if (count == 0) return;

            var orientation = pwh.FindOrientation(POLYGON_ELEMENT.BOUNDARY);

            int[] tmp = new int[count];
            count = TriangulationKernel.GetPolygonWithHolesIndices(Ptr, pwh.Ptr, tmp, 0, tmp.Length, orientation);

            for (int i = 0; i < count; i++)
                indices.Add(tmp[i]);
        }

        */

    }

    /// <summary>
    /// The abstract triangulation class.
    /// </summary>
    public abstract class ConstrainedTriangulation2 : BaseTriangulation2
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="kernel"></param>
        internal ConstrainedTriangulation2(CGALKernel kernel) 
            : base(kernel.ConstrainedTriangulationKernel2)
        {
            TriangulationKernel = Kernel as ConstrainedTriangulationKernel2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kernel"></param>
        /// <param name="points"></param>
        internal ConstrainedTriangulation2(CGALKernel kernel, Point2d[] points)
            : base(kernel.ConstrainedTriangulationKernel2, points)
        {
            TriangulationKernel = Kernel as ConstrainedTriangulationKernel2;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kernel"></param>
        /// <param name="ptr"></param>
        internal ConstrainedTriangulation2(CGALKernel kernel, IntPtr ptr) 
            : base(kernel.ConstrainedTriangulationKernel2, ptr)
        {
            TriangulationKernel = Kernel as ConstrainedTriangulationKernel2;
        }

        /// <summary>
        /// The kernel with the functions unique to the constrained triangulation.
        /// </summary>
        protected private ConstrainedTriangulationKernel2 TriangulationKernel { get; private set; }

        /// <summary>
        /// The number of constrainted edges in the triangulation.
        /// </summary>
        public int ConstrainedEdgeCount => TriangulationKernel.ConstrainedEdgesCount(Ptr);

        /// <summary>
        /// Get the number of constrainted edges incident to this vertex.
        /// </summary>
        /// <param name="vertIndex">The vertex index in the triagulation.</param>
        /// <returns>The number of constrainted edges to the vertex.</returns>
        public int IncidentConstraintCount(int vertIndex)
        {
            return TriangulationKernel.IncidentConstraintCount(Ptr, vertIndex);
        }

        /// <summary>
        /// Does this vertex have a constrainted edge.
        /// </summary>
        /// <param name="vertIndex">The vertex index in the triagulation.</param>
        /// <returns>Does this vertex have a constrainted edge.</returns>
        public bool HasIncidentConstraint(int vertIndex)
        {
            return TriangulationKernel.HasIncidentConstraints(Ptr, vertIndex);
        }

        /// <summary>
        /// Add a segment as a constraint.
        /// </summary>
        /// <param name="segment">The segment to add.</param>
        public void InsertSegmentConstraint(Segment2d segment)
        {
            InsertSegmentConstraint(segment.A, segment.B);
        }

        /// <summary>
        /// Add the two points as a segment constraint.
        /// </summary>
        /// <param name="a">The segments point a.</param>
        /// <param name="b">The segments point b.</param>
        public void InsertSegmentConstraint(Point2d a, Point2d b)
        {
            TriangulationKernel.InsertSegmentConstraintFromPoints(Ptr, a, b);
        }

        /// <summary>
        /// Add a list of segments as constraint to the triangulation.
        /// </summary>
        /// <param name="segments">The segment array.</param>
        public void InsertSegmentConstraints(Segment2d[] segments)
        {
            TriangulationKernel.InsertSegmentConstraints(Ptr, segments, 0, segments.Length);
        }

        /// <summary>
        /// Get a array of all the constraint edges in the triangulation.
        /// </summary>
        /// <param name="constraints">The edge array.</param>
        public void GetConstraints(TriEdge2[] constraints)
        {
            TriangulationKernel.GetConstraints(Ptr, constraints, 0, constraints.Length);
        }

        /// <summary>
        /// Get a array of all the constraint segments in the triangulation.
        /// </summary>
        /// <param name="constraints">The segment array.</param>
        public void GetConstraints(Segment2d[] constraints)
        {
            TriangulationKernel.GetConstraints(Ptr, constraints, 0, constraints.Length);
        }

        /// <summary>
        /// Get the constraints incident to the vertex.
        /// </summary>
        /// <param name="vertexIndex">The vertex index in the triangulation.</param>
        /// <param name="constraints">The array of edges.</param>
        public void GetIncidentConstraints(int vertexIndex, TriEdge2[] constraints)
        {
            TriangulationKernel.GetIncidentConstraints(Ptr, vertexIndex, constraints, 0, constraints.Length);
        }

        /// <summary>
        /// Remove a constraint between a face and its neighbour.
        /// </summary>
        /// <param name="faceIndex">The faces index in the triangultion.</param>
        /// <param name="neighbourIndex">The neighbours index in the faces neighbour array between 0-2.</param>
        public void RemoveConstraint(int faceIndex, int neighbourIndex)
        {
            if (neighbourIndex < 0 || neighbourIndex > 2)
                return;

            TriangulationKernel.RemoveConstraint(Ptr, faceIndex, neighbourIndex);
        }

        /// <summary>
        /// Remove all constraints incident to a vertex.
        /// </summary>
        /// <param name="vertexIndex">The vertex index in the triangulation.</param>
        public void RemoveIncidentConstraints(int vertexIndex)
        {
            TriangulationKernel.RemoveIncidentConstraints(Ptr, vertexIndex);
        }

        /// <summary>
        /// Get the triangle indices for domain in the triangultion.
        /// Used to triangulate polygons.
        /// </summary>
        /// <param name="indices">The indices.</param>
        internal void GetConstrainedDomainIndices(List<int> indices)
        {
            int count = IndiceCount;
            if (count == 0) return;

            int[] tmp = new int[count];
            count = TriangulationKernel.MarkDomains(Ptr, tmp, 0, tmp.Length);

            for (int i = 0; i < count; i++)
                indices.Add(tmp[i]);;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        public override void Print(StringBuilder builder)
        {
            builder.AppendLine(ToString());
            builder.AppendLine("Is valid = " + IsValid());
            builder.AppendLine("Constrained edges = " + ConstrainedEdgeCount);
        }

    }
}
