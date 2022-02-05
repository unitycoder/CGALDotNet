﻿using System;
using System.Collections.Generic;
using System.Text;

using CGALDotNetGeometry.Numerics;

namespace CGALDotNet.Arrangements
{
    public struct ArrHalfEdge2
    {
        public bool IsFictitious;

        public int Index;

        public int SourceIndex;

        public int TargetIndex;

        public int FaceIndex;

        public int NextIndex;

        public int PreviousIndex;

        public int TwinIndex;

        public override string ToString()
        {
            return string.Format("[ArrHalfEdge2: Index={0}, FaceIndex={1}, IsFictitious={2}]",
                Index, FaceIndex, IsFictitious);
        }

        public IEnumerable<ArrHalfEdge2> EnumerateEdges(Arrangement2 arr)
        {
            var start = this;
            var e = start;

            int count = arr.HalfEdgeCount;

            do
            {
                yield return e;

                if (e.NextIndex >= 0 && e.NextIndex < count)
                    arr.GetHalfEdge(e.NextIndex, out e);
                else
                    yield break;

            }
            while (e.Index != start.Index);
        }

        public IEnumerable<ArrVertex2> EnumerateVertices(Arrangement2 arr)
        {
            var start = this;
            var e = start;

            int vertCount = arr.VertexCount;
            int edgeCount = arr.HalfEdgeCount;

            do
            {
                if (e.SourceIndex >= 0 && e.SourceIndex < vertCount)
                {
                    ArrVertex2 vert;
                    arr.GetVertex(e.SourceIndex, out vert);
                    yield return vert;
                }

                if (e.NextIndex >= 0 && e.NextIndex < edgeCount)
                    arr.GetHalfEdge(e.NextIndex, out e);
                else
                    yield break;
            }
            while (e.Index != start.Index);
        }
    }
}
