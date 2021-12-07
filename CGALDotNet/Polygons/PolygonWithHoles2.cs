﻿using System;
using System.Collections.Generic;
using System.Text;

using CGALDotNet.Geometry;
using CGALDotNet.Triangulations;

namespace CGALDotNet.Polygons
{
    /// <summary>
    /// Polygon with holes consists of a boundary and holes.
    /// </summary>
    public enum POLYGON_ELEMENT 
    { 
        BOUNDARY, 
        HOLE
    }

    /// <summary>
    /// Generic polygon definition.
    /// </summary>
    /// <typeparam name="K">The kernel type.</typeparam>
    public sealed class PolygonWithHoles2<K> : PolygonWithHoles2 where K : CGALKernel, new()
    {
        /// <summary>
        /// Default constuctor.
        /// </summary>
        public PolygonWithHoles2() : base(new K())
        {

        }

        /// <summary>
        /// Construct polygon with the boundary.
        /// </summary>
        /// <param name="boundary">A CCW polygon.</param>
        public PolygonWithHoles2(Polygon2<K> boundary) : base(new K(), boundary)
        {

        }

        /// <summary>
        /// Construct polygon with the boundary points
        /// </summary>
        /// <param name="boundary">A CCW set of points.</param>
        public PolygonWithHoles2(Point2d[] boundary) : base(new K(), boundary)
        {

        }

        /// <summary>
        /// Create from a pointer.
        /// </summary>
        /// <param name="ptr">The polygons pointer.</param>
        internal PolygonWithHoles2(IntPtr ptr) : base(new K(), ptr)
        {

        }

        /// <summary>
        /// The polygon as a string.
        /// </summary>
        /// <returns>The polygon as a string.</returns>
        public override string ToString()
        {
            return string.Format("[PolygonWithHoles2<{0}>: IsUnbounded={1}, HoleCount={2}]", 
                Kernel.KernelName, IsUnbounded, HoleCount);
        }

        /// <summary>
        /// Create a deep copy of the polygon.
        /// </summary>
        /// <returns>The copy.</returns>
        public PolygonWithHoles2<K> Copy()
        {
            return new PolygonWithHoles2<K>(Kernel.Copy(Ptr));
        }

        /// <summary>
        /// Create a deep copy of the polygon element.
        /// </summary>
        /// <param name="element">The element type to copy.</param>
        /// <param name="index">If element os a hole thiss is the holes index.</param>
        /// <returns>The copy.</returns>
        public Polygon2<K> Copy(POLYGON_ELEMENT element, int index = 0)
        {
            if (element == POLYGON_ELEMENT.BOUNDARY)
            {
                var ptr = Kernel.CopyPolygon(Ptr, BOUNDARY_INDEX);

                if (ptr != IntPtr.Zero)
                    return new Polygon2<K>(ptr);
                else
                    throw new InvalidOperationException("Failed to find boundary.");
            }
            else
            {
                return new Polygon2<K>(Kernel.CopyPolygon(Ptr, index));
            }
        }

        /// <summary>
        /// Create a copy of boundary and hole polygons.
        /// </summary>
        /// <returns>The list of polygons.</returns>
        public List<Polygon2<K>> ToList()
        {
            int count = HoleCount;
            if (!IsUnbounded)
                count++;

            var polygons = new List<Polygon2<K>>(count);

            if (!IsUnbounded)
                polygons.Add(Copy(POLYGON_ELEMENT.BOUNDARY));

            for (int i = 0; i < HoleCount; i++)
                polygons.Add(Copy(POLYGON_ELEMENT.HOLE, i));

            return polygons;
        }

        /// <summary>
        /// Triangulate the polygon.
        /// </summary>
        /// <param name="indices">The triangle indices.</param>
        public void Triangulate(List<int> indices)
        {
            var ct = new ConstrainedTriangulation2<K>();
            ct.InsertConstraint(this);
            ct.GetConstrainedDomainIndices(indices);
        }

        /// <summary>
        /// Do the polygons intersect.
        /// </summary>
        /// <param name="polygon">The other polygon.</param>
        /// <returns>Do the polygons intersect.</returns>
        public bool Intersects(Polygon2<K> polygon)
        {
            return PolygonBoolean2<K>.Instance.DoIntersect(polygon, this);
        }

        /// <summary>
        /// Do the polygons intersect.
        /// </summary>
        /// <param name="polygon">The other polygon.</param>
        /// <returns>Do the polygons intersect.</returns>
        public bool Intersects(PolygonWithHoles2<K> polygon)
        {
            return PolygonBoolean2<K>.Instance.DoIntersect(polygon, this);
        }

        /// <summary>
        /// Connect all the holes of the polygon 
        /// and return as a polygon.
        /// </summary>
        /// <returns>The connected polygon.</returns>
        public Polygon2<K> ConnectHoles()
        {
            var ptr = Kernel.ConnectHoles(Ptr);
            return new Polygon2<K>(ptr);
        }
    }

    /// <summary>
    /// The abstract polygon definition.
    /// </summary>
    public abstract class PolygonWithHoles2 : CGALObject
    {
        protected const int BOUNDARY_INDEX = -1;

        /// <summary>
        /// Default constructor.
        /// </summary>
        private PolygonWithHoles2()
        {
            IsUnbounded = true;
        }

        /// <summary>
        /// Construct polygon with the kernel.
        /// </summary>
        /// <param name="kernel"></param>
        internal PolygonWithHoles2(CGALKernel kernel)
        {
            Kernel = kernel.PolygonWithHolesKernel2;
            Ptr = Kernel.Create();
            IsUnbounded = true;
        }

        /// <summary>
        /// Construct the polygon with the kernel and boundary.
        /// </summary>
        /// <param name="kernel"></param>
        /// <param name="boundary">A CCW polygon.</param>
        internal PolygonWithHoles2(CGALKernel kernel, Polygon2 boundary)
        {
            Kernel = kernel.PolygonWithHolesKernel2;
            Ptr = Kernel.CreateFromPolygon(boundary.Ptr);
            IsUnbounded = false;
        }

        /// <summary>
        /// Construct the polygon with the kernel and boundary.
        /// </summary>
        /// <param name="kernel"></param>
        /// <param name="boundary">A CCW set of points.</param>
        internal PolygonWithHoles2(CGALKernel kernel, Point2d[] boundary)
        {
            Kernel = kernel.PolygonWithHolesKernel2;
            Ptr = Kernel.CreateFromPoints(boundary, boundary.Length);
            IsUnbounded = false;
        }

        /// <summary>
        /// Construct the polygon with the kernel and pointer.
        /// </summary>
        /// <param name="kernel"></param>
        /// <param name="ptr"></param>
        internal PolygonWithHoles2(CGALKernel kernel, IntPtr ptr) : base(ptr)
        {
            Kernel = kernel.PolygonWithHolesKernel2;
            HoleCount = Kernel.HoleCount(Ptr);
            IsUnbounded = FindIfUnbounded();
        }

        /// <summary>
        /// Is the polygon unbounded. 
        /// ie no boundary polygon has been set.
        /// </summary>
        public bool IsUnbounded { get; protected set; }

        /// <summary>
        /// Number of points in the boindary polygon.
        /// </summary>
        public int Count => PointCount(POLYGON_ELEMENT.BOUNDARY);

        /// <summary>
        /// The number of holes in polygon.
        /// </summary>
        public int HoleCount { get; protected set; }

        /// <summary>
        /// Is this a simple polygon.
        /// Certains actions can only be carried out on simple polygons.
        /// </summary>
        public bool IsSimple => FindIfSimple(POLYGON_ELEMENT.BOUNDARY);

        /// <summary>
        /// The polygons orientation.
        /// Certain actions depend on the polygons orientation.
        /// </summary>
        public ORIENTATION Orientation => FindOrientation(POLYGON_ELEMENT.BOUNDARY);

        /// <summary>
        /// The orientation expressed as the clock direction.
        /// </summary>
        public CLOCK_DIR ClockDir => (CLOCK_DIR)Orientation;

        /// <summary>
        /// Is the polygon degenerate.
        /// Polygons with < 3 points are degenerate.
        /// </summary>
        public bool IsDegenerate => Count < 3 || Orientation == ORIENTATION.ZERO;

        /// <summary>
        /// Is the polygon cw orientated.
        /// </summary>
        public bool IsClockWise => ClockDir == CLOCK_DIR.CLOCKWISE;

        /// <summary>
        /// Is the polygon ccw orientated.
        /// </summary>
        public bool IsCounterClockWise => ClockDir == CLOCK_DIR.COUNTER_CLOCKWISE;

        /// <summary>
        /// The polygon kernel.
        /// </summary>
        protected private PolygonWithHolesKernel2 Kernel { get; private set; }

        /// <summary>
        /// Valid polygon with holes must have a simple and ccw boundary
        /// and all holes must be simple and cw.
        /// </summary>
        /// <returns></returns>
        public bool IsValid()
        {
            if(!IsUnbounded)
            {
                if (!FindIfSimple(POLYGON_ELEMENT.BOUNDARY))
                    return false;

                if (FindOrientation(POLYGON_ELEMENT.BOUNDARY) != ORIENTATION.POSITIVE)
                    return false;
            }

            for(int i = 0; i < HoleCount; i++)
            {
                if (!FindIfSimple(POLYGON_ELEMENT.HOLE, i))
                    return false;

                if (FindOrientation(POLYGON_ELEMENT.HOLE, i) != ORIENTATION.NEGATIVE)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Clear the polygon.
        /// </summary>
        public void Clear()
        {
            Kernel.Clear(Ptr);
            IsUnbounded = true;
            HoleCount = 0;
        }

        /// <summary>
        /// Get the number of points of a polygon element.
        /// </summary>
        /// <param name="element">The element type.</param>
        /// <param name="index">If element type is a hole this is the holes index.</param>
        /// <returns></returns>
        public int PointCount(POLYGON_ELEMENT element, int index = 0)
        {
            if (element == POLYGON_ELEMENT.BOUNDARY)
                return Kernel.PointCount(Ptr, BOUNDARY_INDEX);
            else
                return Kernel.PointCount(Ptr, index);
        }

        /// <summary>
        /// Remove a polygon.
        /// Can remove the boundary or a hole.
        /// </summary>
        /// <param name="element">The element type.</param>
        /// <param name="index">If element type is a hole this is the holes index.</param>
        public void Remove(POLYGON_ELEMENT element, int index = 0)
        {
            if (element == POLYGON_ELEMENT.BOUNDARY)
            {
                Kernel.ClearBoundary(Ptr);
                IsUnbounded = true;
            }
            else
            {
                Kernel.RemoveHole(Ptr, index);
                HoleCount--;
            }
        }

        /// <summary>
        /// Reverse the polygon.
        /// </summary>
        /// <param name="element">The element type.</param>
        /// <param name="index">If element type is a hole this is the holes index.</param>
        public void Reverse(POLYGON_ELEMENT element, int index = 0)
        {
            if (element == POLYGON_ELEMENT.BOUNDARY)
                Kernel.ReversePolygon(Ptr, BOUNDARY_INDEX);
            else
                Kernel.ReversePolygon(Ptr, index);
        }

        /// <summary>
        /// Get a polygons point.
        /// </summary>
        /// <param name="element">The element type.</param>
        /// <param name="pointIndex">The index of the point in the polygon.</param>
        /// <param name="holeIndex">If element type is a hole this is the holes index.</param>
        /// <returns></returns>
        public Point2d GetPoint(POLYGON_ELEMENT element, int pointIndex, int holeIndex = 0)
        {
            if (element == POLYGON_ELEMENT.BOUNDARY)
                return Kernel.GetPoint(Ptr, BOUNDARY_INDEX, pointIndex);
            else
                return Kernel.GetPoint(Ptr, holeIndex, pointIndex);
        }

        /// <summary>
        /// Get the points in the polygon element.
        /// </summary>
        /// <param name="element">The element type.</param>
        /// <param name="points">The point array to copy points into.</param>
        /// <param name="count">The ararys length.</param>
        /// <param name="holeIndex">If element type is a hole this is the holes index.</param>
        public void GetPoints(POLYGON_ELEMENT element, Point2d[] points, int count, int holeIndex = 0)
        {
            ErrorUtil.CheckArray(points, count);

            if (element == POLYGON_ELEMENT.BOUNDARY)
                Kernel.GetPoints(Ptr, points, BOUNDARY_INDEX, points.Length);
            else
                Kernel.GetPoints(Ptr, points, holeIndex, points.Length);
            
        }

        /// <summary>
        /// Get all the points in the polygon boundary and holes.
        /// </summary>
        /// <param name="points">The point array to copy into.</param>
        public void GetAllPoints(List<Point2d> points)
        {
            int count = PointCount(POLYGON_ELEMENT.BOUNDARY);
            var arr = new Point2d[count];
            GetPoints(POLYGON_ELEMENT.BOUNDARY, arr, arr.Length);
            points.AddRange(arr);

            for(int i = 0; i < HoleCount; i++)
            {
                count = PointCount(POLYGON_ELEMENT.HOLE, i);
                arr = new Point2d[count];
                GetPoints(POLYGON_ELEMENT.HOLE, arr, arr.Length, i);
                points.AddRange(arr);
            }
        }

        /// <summary>
        /// Set a polygons point.
        /// </summary>
        /// <param name="element">The element type.</param>
        /// <param name="pointIndex">The index of the point in the polygon.</param>
        /// <param name="point">The point to set.</param>
        /// <param name="holeIndex">If element type is a hole this is the holes index.</param>
        public void SetPoint(POLYGON_ELEMENT element, int pointIndex, Point2d point, int holeIndex = 0)
        {
            if (element == POLYGON_ELEMENT.BOUNDARY)  
                Kernel.SetPoint(Ptr, BOUNDARY_INDEX, pointIndex, point);
            else
                Kernel.SetPoint(Ptr, holeIndex, pointIndex, point);
        }

        /// <summary>
        /// Set all the points in the polygon. If the point array is longer
        /// than the polygon is current the extra points are appended to the end.
        /// </summary>
        /// <param name="element">The element type.</param>
        /// <param name="points">The points to set.</param>
        /// <param name="count">The ararys length.</param>
        /// <param name="holeIndex">If element type is a hole this is the holes index.</param>
        public void SetPoints(POLYGON_ELEMENT element, Point2d[] points, int count, int holeIndex = 0)
        {
            ErrorUtil.CheckArray(points, count);

            if (element == POLYGON_ELEMENT.BOUNDARY)
                Kernel.SetPoints(Ptr, points, BOUNDARY_INDEX, points.Length);
            else
                Kernel.SetPoints(Ptr, points, holeIndex, points.Length);
        }

        /// <summary>
        /// Add a polygon as a holes.
        /// Holes must simple and CW.
        /// </summary>
        /// <param name="polygon">The hole polygon.</param>
        public void AddHole(Polygon2 polygon)
        {
            Kernel.AddHoleFromPolygon(Ptr, polygon.Ptr);
            HoleCount++;
        }

        /// <summary>
        /// Add a hole from a set of points.
        /// </summary>
        /// <param name="points">A CW set of points.</param>
        /// <param name="count">The ararys length.</param>
        public void AddHole(Point2d[] points, int count)
        {
            ErrorUtil.CheckArray(points, count);
            Kernel.AddHoleFromPoints(Ptr, points, count);
            HoleCount++;
        }

        /// <summary>
        /// Find if the polygon has a boundary.
        /// </summary>
        /// <returns>True if the polygon has a boundary.</returns>
        public bool FindIfUnbounded()
        {
            return Kernel.IsUnbounded(Ptr);
        }

        /// <summary>
        /// Find the polygons bounding box.
        /// </summary>
        /// <param name="element">The element type.</param>
        /// <param name="index">If element type is a hole this is the holes index.</param>
        /// <returns>The polygons bounding box.</returns>
        public Box2d FindBoundingBox(POLYGON_ELEMENT element, int index = 0)
        {
            if (element == POLYGON_ELEMENT.BOUNDARY)
                return Kernel.GetBoundingBox(Ptr, BOUNDARY_INDEX);
            else
                return Kernel.GetBoundingBox(Ptr, index);
        }

        /// <summary>
        /// Find if the polygon is simple.
        /// </summary>
        /// <param name="element">The element type.</param>
        /// <param name="index">If element type is a hole this is the holes index.</param>
        /// <returns>True if the polygon is simple.</returns>
        public bool FindIfSimple(POLYGON_ELEMENT element, int index = 0)
        {
            if(element == POLYGON_ELEMENT.BOUNDARY)
                return Kernel.IsSimple(Ptr, BOUNDARY_INDEX);
            else
                return Kernel.IsSimple(Ptr, index);
        }

        /// <summary>
        /// Find if the polygon is convex.
        /// </summary>
        /// <param name="element">The element type.</param>
        /// <param name="index">If element type is a hole this is the holes index.</param>
        /// <returns>True if polygon is convex.</returns>
        public bool FindIfConvex(POLYGON_ELEMENT element, int index = 0)
        {
            if (element == POLYGON_ELEMENT.BOUNDARY)
                return Kernel.IsConvex(Ptr, BOUNDARY_INDEX);
            else
                return Kernel.IsConvex(Ptr, index);
        }

        /// <summary>
        /// Find the orientation of polygon.
        /// </summary>
        /// <param name="element">The element type.</param>
        /// <param name="index">If element type is a hole this is the holes index.</param>
        /// <returns>The orientation of the polygon.</returns>
        public ORIENTATION FindOrientation(POLYGON_ELEMENT element, int index = 0)
        {
            if (element == POLYGON_ELEMENT.BOUNDARY)
                return Kernel.Orientation(Ptr, BOUNDARY_INDEX);
            else
                return Kernel.Orientation(Ptr, index);
        }

        /// <summary>
        /// Find the orientated side the point is on.
        /// </summary>
        /// <param name="element">The element type.</param>
        /// <param name="point"></param>
        /// <param name="index">If element type is a hole this is the holes index.</param>
        /// <returns>The orientated side of point compared to the polygon.</returns>
        public ORIENTED_SIDE OrientedSide(POLYGON_ELEMENT element, Point2d point, int index = 0)
        {
            if (element == POLYGON_ELEMENT.BOUNDARY)
                return Kernel.OrientedSide(Ptr, BOUNDARY_INDEX, point);
            else
                return Kernel.OrientedSide(Ptr, index, point);
        }

        /// <summary>
        /// The signed area of the polygon.
        /// </summary>
        /// <param name="element">The element type.</param>
        /// <param name="index">If element type is a hole this is the holes index.</param>
        /// </summary>
        /// <returns>The signed area is positive if polygon is ccw 
        /// and negation if cw.</returns>
        public double FindSignedArea(POLYGON_ELEMENT element, int index = 0)
        {
            if (element == POLYGON_ELEMENT.BOUNDARY)
                return Kernel.SignedArea(Ptr, BOUNDARY_INDEX);
            else
                return Kernel.SignedArea(Ptr, index);
        }

        /// <summary>
        /// The area of the polygon.
        /// </summary>
        /// <param name="element">The element type.</param>
        /// <param name="index">If element type is a hole this is the holes index.</param>
        /// <returns>The polygons area.</returns>
        public double FindArea(POLYGON_ELEMENT element, int index = 0)
        {
            return Math.Abs(FindSignedArea(element, index));
        }

        /// <summary>
        /// Release the unmanaged resoures.
        /// </summary>
        protected override void ReleasePtr()
        {
            Kernel.Release(Ptr);
        }

        /// <summary>
        /// Does the polygon fully contain the other polygon.
        /// </summary>
        /// <param name="polygon">The other polygon.</param>
        /// <param name="inculdeBoundary">Should the boundary be included.</param>
        /// <returns>True if the polygon is contained within this polygon.</returns>
        private bool ContainsPolygon(Polygon2 polygon, bool inculdeBoundary = true)
        {
            for (int i = 0; i < polygon.Count; i++)
            {
                if (!ContainsPoint(polygon.GetPoint(i), inculdeBoundary))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Does this polygon contain the point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="inculdeBoundary">Should points on the boundary be 
        /// counted as being inside the polygon.</param>
        /// <returns>True if the polygon contain the point.</returns>
        public bool ContainsPoint(Point2d point, bool inculdeBoundary = true)
        {
            var orientation = FindOrientation(POLYGON_ELEMENT.BOUNDARY);
            return Kernel.ContainsPoint(Ptr, point, orientation, inculdeBoundary);
        }

        /// <summary>
        /// Translate the polygon.
        /// </summary>
        /// <param name="translation">The amount to translate.</param>
        public void Translate(Point2d translation)
        {
            Kernel.Translate(Ptr, BOUNDARY_INDEX, translation);

            int count = HoleCount;
            for (int i = 0; i < count; i++)
                Kernel.Translate(Ptr, i, translation);
        }

        /// <summary>
        /// Translate the polygon.
        /// </summary>
        /// <param name="element">The element type.</param>
        /// <param name="translation">The amount to translate.</param>
        /// <param name="index">If element type is a hole this is the holes index.</param>
        public void Translate(POLYGON_ELEMENT element, Point2d translation, int index = 0)
        {
            if(element == POLYGON_ELEMENT.BOUNDARY)
                Kernel.Translate(Ptr, BOUNDARY_INDEX, translation);
            else
                Kernel.Translate(Ptr, index, translation);
        }

        /// <summary>
        /// Rotate the polygon.
        /// </summary>
        /// <param name="rotation">The amount to rotate in radians.</param>
        public void Rotate(Radian rotation)
        {
            Kernel.Rotate(Ptr, BOUNDARY_INDEX, rotation.angle);

            int count = HoleCount;
            for (int i = 0; i < count; i++)
                Kernel.Rotate(Ptr, i, rotation.angle);
        }

        /// <summary>
        /// Rotate the polygon.
        /// </summary>
        /// <param name="element">The element type.</param>
        /// <param name="rotation">The amount to rotate in radians.</param>
        /// <param name="index">If element type is a hole this is the holes index.</param>
        public void Rotate(POLYGON_ELEMENT element, Radian rotation, int index = 0)
        {
            if (element == POLYGON_ELEMENT.BOUNDARY)
                Kernel.Rotate(Ptr, BOUNDARY_INDEX, rotation.angle);
            else
                Kernel.Rotate(Ptr, index, rotation.angle);
        }

        /// <summary>
        /// Rotate the polygon.
        /// </summary>
        /// <param name="scale">The amount to scale.</param>
        public void Scale(double scale)
        {
            Kernel.Scale(Ptr, BOUNDARY_INDEX, scale);

            int count = HoleCount;
            for (int i = 0; i < count; i++)
                Kernel.Scale(Ptr, i, scale);
        }

        /// <summary>
        /// Scale the polygon.
        /// </summary>
        /// <param name="element">The element type.</param>
        /// <param name="scale">The amount to scale.</param>
        /// <param name="index">If element type is a hole this is the holes index.</param>
        public void Scale(POLYGON_ELEMENT element, double scale, int index = 0)
        {
            if (element == POLYGON_ELEMENT.BOUNDARY)
                Kernel.Scale(Ptr, BOUNDARY_INDEX, scale);
            else
                Kernel.Scale(Ptr, index, scale);
        }

        /// <summary>
        /// Transform the polygon.
        /// </summary>
        /// <param name="translation">The amount to translate.</param>
        /// <param name="rotation">The amount to rotate in radians.</param>
        /// <param name="scale">The amount to scale.</param>
        public void Transform(Point2d translation, Radian rotation, double scale)
        {
            Kernel.Transform(Ptr, BOUNDARY_INDEX, translation, rotation.angle, scale);

            int count = HoleCount;
            for (int i = 0; i < count; i++)
                Kernel.Transform(Ptr, i, translation, rotation.angle, scale);
        }

        /// <summary>
        /// Transform the polygon.
        /// </summary>
        /// <param name="element">The element type.</param>
        /// <param name="translation">The amount to translate.</param>
        /// <param name="rotation">The amount to rotate in radians.</param>
        /// <param name="scale">The amount to scale.</param>
        /// <param name="index">If element type is a hole this is the holes index.</param>
        public void Transform(POLYGON_ELEMENT element, Point2d translation, Radian rotation, double scale, int index = 0)
        {
            if (element == POLYGON_ELEMENT.BOUNDARY)
                Kernel.Transform(Ptr, BOUNDARY_INDEX, translation, rotation.angle, scale);
            else
                Kernel.Transform(Ptr, index, translation, rotation.angle, scale);
        }

        /// <summary>
        /// Print debug infomation.
        /// </summary>
        public void Print()
        {
            var builder = new StringBuilder();
            Print(builder);
            Console.WriteLine(builder.ToString());
        }

        /// <summary>
        /// Print debug infomation.
        /// </summary>
        /// <param name="builder"></param>
        public void Print(StringBuilder builder)
        {
            builder.AppendLine(ToString());
            builder.AppendLine("Is unbounded = " + IsUnbounded);

            if (!IsUnbounded)
            {
                var element = POLYGON_ELEMENT.BOUNDARY;
                builder.AppendLine("Boundary point count = " + PointCount(element));
                builder.AppendLine("Boundary is simple = " + FindIfSimple(element));
                builder.AppendLine("Boundary is convex = " + FindIfConvex(element));
                builder.AppendLine("Boundary orientation = " + FindOrientation(element));
                builder.AppendLine("Boundary signed Area = " + FindSignedArea(element));
            }

            for (int i = 0; i < HoleCount; i++)
            {
                builder.AppendLine("");
                var element = POLYGON_ELEMENT.HOLE;
                builder.AppendLine("Hole " + i + " point count = " + PointCount(element, i));
                builder.AppendLine("Hole " + i + " is simple = " + FindIfSimple(element, i));
                builder.AppendLine("Hole " + i + " is convex = " + FindIfConvex(element, i));
                builder.AppendLine("Hole " + i + " is orientation = " + FindOrientation(element, i));
                builder.AppendLine("Hole " + i + " is signed area = " + FindSignedArea(element, i));
                builder.AppendLine();
            }
        }

    }
}
