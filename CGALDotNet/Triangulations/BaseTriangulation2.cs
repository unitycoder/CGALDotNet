﻿using System;
using System.Collections.Generic;
using System.Text;

using CGALDotNet.Geometry;
using CGALDotNet.Polygons;

namespace CGALDotNet.Triangulations
{

    public enum TRIANGULATION_TYPE
    {
        TRIANGULATION,
        DELAUNAY,
        CONSTRAINED
    }

    /// <summary>
    /// Base triangulation class for Triangulation, DelaunayTriangulation 
    /// and ConstrainedTriangulation.
    /// </summary>
    public abstract class BaseTriangulation2 : CGALObject
    {
        /// <summary>
        /// 
        /// </summary>
        private BaseTriangulation2()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kernel"></param>
        internal BaseTriangulation2(BaseTriangulationKernel2 kernel)
        {
            Kernel = kernel;
            Ptr = Kernel.Create();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kernel"></param>
        /// <param name="points"></param>
        internal BaseTriangulation2(BaseTriangulationKernel2 kernel, Point2d[] points)
        {
            Kernel = kernel;
            Ptr = Kernel.Create();
            Insert(points, points.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="kernel"></param>
        /// <param name="ptr"></param>
        internal BaseTriangulation2(BaseTriangulationKernel2 kernel, IntPtr ptr) : base(ptr)
        {
            Kernel = kernel;
        }

        /// <summary>
        /// The triangulations kernel.
        /// </summary>
        protected private BaseTriangulationKernel2 Kernel { get; private set; }

        /// <summary>
        /// The number of verices in the triangulation.
        /// </summary>
        public int VertexCount => Kernel.VertexCount(Ptr);

        /// <summary>
        /// The number of triangles in the triangulation.
        /// </summary>
        public int TriangleCount => Kernel.FaceCount(Ptr);

        /// <summary>
        /// The number of indices need to represent the
        /// triangulation (number of triangles * 3).
        /// </summary>
        public int IndiceCount => TriangleCount * 3;

        /// <summary>
        /// A number that will change if the unmanaged 
        /// triangulation model changes.
        /// </summary>
        public int BuildStamp => Kernel.BuildStamp(Ptr);

        /// <summary>
        /// Clear the triangulation.
        /// </summary>
        public void Clear()
        {
            Kernel.Clear(Ptr);
        }

        /// <summary>
        /// Is this a valid triangulation.
        /// </summary>
        /// <param name="level"></param>
        /// <returns>True if valid.</returns>
        public bool IsValid(int level = 0)
        {
            return Kernel.IsValid(Ptr, level);
        }

        /// <summary>
        /// Force the face and vertex indices to be set.
        /// </summary>
        public void ForceSetIndices()
        {
            Kernel.SetIndices(Ptr);
        }

        /// <summary>
        /// Inserts point p in the triangulation.
        ///If point coincides with an already existing vertex the triangulation remains unchanged.
        ///If point is on an edge, the two incident faces are split in two.
        ///If point is strictly inside a face of the triangulation, the face is split in three.
        ///If point is strictly outside the convex hull, p is linked to all visible points on the 
        ///convex hull to form the new triangulation.
        /// </summary>
        /// <param name="point">The point to insert.</param>
        public void Insert(Point2d point)
        {
            Kernel.InsertPoint(Ptr, point);
        }

        /// <summary>
        /// Inserts points into the triangulation.
        ///If point coincides with an already existing vertex the triangulation remains unchanged.
        ///If point is on an edge, the two incident faces are split in two.
        ///If point is strictly inside a face of the triangulation, the face is split in three.
        ///If point is strictly outside the convex hull, p is linked to all visible points on the 
        ///convex hull to form the new triangulation.
        /// </summary>
        /// <param name="point">The point to insert.</param>
        /// <param name="count">The ararys length.</param>
        public void Insert(Point2d[] points, int count)
        {
            ErrorUtil.CheckArray(points, count);
            Kernel.InsertPoints(Ptr, points, count);
        }

        /// <summary>
        /// Get a array of all the points in the triangulation.
        /// </summary>
        /// <param name="points">The point array.</param>
        /// <param name="count">The ararys length.</param>
        public void GetPoints(Point2d[] points, int count)
        {
            ErrorUtil.CheckArray(points, count);
            Kernel.GetPoints(Ptr, points, count);
        }

        /// <summary>
        /// Get a array of the triangle indices.
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="count">The ararys length.</param>
        public void GetIndices(int[] indices, int count)
        {
            ErrorUtil.CheckArray(indices, count);
            Kernel.GetIndices(Ptr, indices, count);
        }

        /// <summary>
        /// Get the vertices point.
        /// </summary>
        /// <param name="index">The vertex index.</param>
        /// <param name="point">The vertices point.</param>
        /// <returns>True if the vertex was found.</returns>
        public bool GetPoint(int index, out Point2d point)
        {
            TriVertex2 vertex;
            if(Kernel.GetVertex(Ptr, index, out vertex))
            {
                point = vertex.Point;
                return true;
            }
            else
            {
                point = new Point2d();
                return false;
            }
        }

        /// <summary>
        /// Get a vertex.
        /// </summary>
        /// <param name="index">The vertex index.</param>
        /// <param name="vertex">The vertex.</param>
        /// <returns>True if the vertex was found.</returns>
        public bool GetVertex(int index, out TriVertex2 vertex)
        {
            return Kernel.GetVertex(Ptr, index, out vertex);
        }

        /// <summary>
        /// Get a array of all the vertices.
        /// </summary>
        /// <param name="vertices">The vertex array.</param>
        /// <param name="count">The ararys length.</param>
        public void GetVertices(TriVertex2[] vertices, int count)
        {
            ErrorUtil.CheckArray(vertices, count);
            Kernel.GetVertices(Ptr, vertices, count);
        }

        /// <summary>
        /// Get a triangule face.
        /// </summary>
        /// <param name="index">The faces index</param>
        /// <param name="face">The face</param>
        /// <returns>True if the face was found.</returns>
        public bool GetFace(int index, out TriFace2 face)
        {
            return Kernel.GetFace(Ptr, index, out face);
        }

        /// <summary>
        /// Get a array of all the triangle faces.
        /// </summary>
        /// <param name="faces">A array of faces.</param>
        /// <param name="count">The ararys length.</param>
        public void GetFaces(TriFace2[] faces, int count)
        {
            ErrorUtil.CheckArray(faces, count);
            Kernel.GetFaces(Ptr, faces, count);
        }

        /// <summary>
        /// Get the segment between the face and a neighbour.
        /// </summary>
        /// <param name="faceIndex">The faces index</param>
        /// <param name="neighbourIndex">The neighbour (0-2) index in the face.</param>
        /// <param name="segment">The segment.</param>
        /// <returns>True if the face was found.</returns>
        public bool GetSegment(int faceIndex, int neighbourIndex, out Segment2d segment)
        {
            return Kernel.GetSegment(Ptr, faceIndex, neighbourIndex, out segment);
        }

        /// <summary>
        /// Get a faces triangle.
        /// </summary>
        /// <param name="faceIndex">The faces index</param>
        /// <param name="triangle">The triangle</param>
        /// <returns>True if the face was found</returns>
        public bool GetTriangle(int faceIndex, out Triangle2d triangle)
        {
            return Kernel.GetTriangle(Ptr, faceIndex, out triangle);
        }

        /// <summary>
        /// Get a array of all the triangles.
        /// </summary>
        /// <param name="triangules">A array of triangules.</param>
        /// <param name="count">The ararys length.</param>
        public void GetTriangles(Triangle2d[] triangles, int count)
        {
            ErrorUtil.CheckArray(triangles, count);
            Kernel.GetTriangles(Ptr, triangles, count);
        }

        /// <summary>
        /// Get a faces circumcenter.
        /// </summary>
        /// <param name="faceIndex">The faces index</param>
        /// <param name="circumcenter">The circumcenter. A circle
        /// that passes through all three of the triangules vertices.</param>
        /// <returns>True if the face was found.</returns>
        public bool GetCircumcenter(int faceIndex, out Point2d circumcenter)
        {
            return Kernel.GetCircumcenter(Ptr, faceIndex, out circumcenter);
        }

        /// <summary>
        /// Get a array of all the circumcenters.
        /// </summary>
        /// <param name="circumcenters">A array of circumcenters.</param>
        /// <param name="count">The ararys length.</param>
        public void GetCircumcenters(Point2d[] circumcenters, int count)
        {
            ErrorUtil.CheckArray(circumcenters, count);
            Kernel.GetCircumcenters(Ptr, circumcenters, count);
        }

        /// <summary>
        /// Get the index of the faces neighbour.
        /// </summary>
        /// <param name="faceIndex">The faces index.</param>
        /// <param name="neighbourIndex">The neighbour (0-2) index in the face.</param>
        /// <returns>The index of the neighbour face in the triangulation. 
        /// -1 if there is no neighbour face at this index.</returns>
        public int NeighbourIndex(int faceIndex, int neighbourIndex)
        {
            if (neighbourIndex < 0 || neighbourIndex > 2)
                return -1;

            return Kernel.NeighbourIndex(Ptr, faceIndex, neighbourIndex);
        }

        /// <summary>
        /// Locate the face the point hits.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="face">The face the point has hit.</param>
        /// <returns>True if the point hit a face.</returns>
        public bool LocateFace(Point2d point, out TriFace2 face)
        {
            return Kernel.LocateFace(Ptr, point, out face);
        }

        /// <summary>
        /// Locate the closest vertex to point.
        /// </summary>
        /// <param name="point">The point</param>
        /// <param name="vertex">The closest vertex.</param>
        /// <returns>True if point hit a face and found a vertex.</returns>
        public bool LocateVertex(Point2d point, double radius, out TriVertex2 vertex)
        {
            //Locate the face the point hit.
            vertex = new TriVertex2();
            if (Kernel.LocateFace(Ptr, point, out TriFace2 face))
            {
                //Find the closest vertex in the face to the point.
                double min = double.PositiveInfinity;
                TriVertex2 closest = new TriVertex2();

                for (int i = 0; i < 3; i++)
                {
                    int v = face.GetVertexIndex(i);
                    if (v == -1) continue;

                    //If vertex found find its distance to point.
                    if(GetVertex(v, out vertex))
                    {
                        var sqdist = Point2d.SqrDistance(vertex.Point, point);
                        if(sqdist < min)
                        {
                            min = sqdist;
                            closest = vertex;
                        }
                    }
                }

                //Face had no valid vertices.
                //Should not happen but check anyway.
                if (min == double.PositiveInfinity || min > radius * radius)
                    return false;
                else
                {
                    vertex = closest;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Locate the closest  edge and segment to point.
        /// </summary>
        /// <param name="point">The point</param>
        /// <param name="edge">The closest edge.</param>
        /// <returns>True if the point hit a face and found a edge.</returns>
        public bool LocateEdge(Point2d point, double radius, out TriEdge2 edge)
        {
            //Locate the face the point hit.
            edge = new TriEdge2();
 
            if (Kernel.LocateFace(Ptr, point, out TriFace2 face))
            {
                //Find the closest edge to the point in the face.
                double min = double.PositiveInfinity;
                TriEdge2 closest = new TriEdge2();

                for (int i = 0; i < 3; i++)
                {
                    int v1 = face.GetVertexIndex(i+0);
                    int v2 = face.GetVertexIndex(i+1);
                    if (v1 == -1 || v2 == -1) continue;

                    if (GetVertex(v1, out TriVertex2 vertex1) &&
                        GetVertex(v2, out TriVertex2 vertex2))
                    {
                        var p1 = vertex1.Point;
                        var p2 = vertex2.Point;

                        var seg = new Segment2d(p1, p2);
                        var sqdist = seg.SqrDistance(point);

                        if (sqdist < min)
                        {
                            min = sqdist;

                            int neighboutIndex = CGALGlobal.Wrap(i - 1, 3);
                            closest = new TriEdge2(face.Index, neighboutIndex);
                            closest.Segment = new Segment2d(p1, p2);
                        }
                    }
                }

                //Face had no valid vertices.
                //Should not happen but check anyway.
                if (min == double.PositiveInfinity || min > radius * radius)
                    return false;
                else
                {
                    edge = closest;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Move the vertex.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="point"></param>
        /// <param name="ifNoCollision">If there is not already another vertex placed on the point, 
        /// the triangulation is modified such that the new position of vertex same as point.</param>
        /// <param name="vertex">The moved vertex</param>
        /// <returns>True if the vertex was found.</returns>
        public bool MoveVertex(int index, Point2d point, bool ifNoCollision, out TriVertex2 vertex)
        {
            return Kernel.MoveVertex(Ptr, index, point, ifNoCollision, out vertex);
        }

        /// <summary>
        /// Remove the vertex.
        /// </summary>
        /// <param name="index">The vertices index.</param>
        /// <returns>True if removed.</returns>
        public bool RemoveVertex(int index)
        {
            return Kernel.RemoveVertex(Ptr, index);
        }

        /// <summary>
        /// Flip a edge between the face and a neighbour.
        /// </summary>
        /// <param name="faceIndex">The faces index</param>
        /// <param name="neighbourIndex">The neighbour (0-2) index in the face.</param>
        /// <returns>True if the edge was flipped.</returns>
        public bool FlipEdge(int faceIndex, int neighbourIndex)
        {
            if (neighbourIndex < 0 || neighbourIndex > 2)
                return false;

            return Kernel.FlipEdge(Ptr, faceIndex, neighbourIndex);
        }

        /// <summary>
        /// Translate the triangulation.
        /// </summary>
        /// <param name="translation">The amount to translate.</param>
        public void Translate(Point2d translation)
        {
            Kernel.Transform(Ptr, translation, 0, 1);
        }

        /// <summary>
        /// Rotate the triangulation.
        /// </summary>
        /// <param name="rotation">The amount to rotate in radians.</param>
        public void Rotate(Radian rotation)
        {
            Kernel.Transform(Ptr, Point2d.Zero, rotation.angle, 1);
        }

        /// <summary>
        /// Scale the triangulation.
        /// </summary>
        /// <param name="scale">The amount to scale.</param>
        public void Scale(double scale)
        {
            Kernel.Transform(Ptr, Point2d.Zero, 0, scale);
        }

        /// <summary>
        /// Transform the triangulation with a TRS matrix.
        /// </summary>
        /// <param name="translation">The amount to translate.</param>
        /// <param name="rotation">The amount to rotate.</param>
        /// <param name="scale">The amount to scale.</param>
        public void Transform(Point2d translation, Radian rotation, double scale)
        {
            Kernel.Transform(Ptr, translation, rotation.angle, scale);
        }

        public void Print()
        {
            var builder = new StringBuilder();
            Print(builder);
            Console.WriteLine(builder.ToString());
        }

        public virtual void Print(StringBuilder builder)
        {
            builder.AppendLine(ToString());
        }

        /// <summary>
        /// Release any unmanaged resources.
        /// </summary>
        protected override void ReleasePtr()
        {
            Kernel.Release(Ptr);
        }
    }
}
