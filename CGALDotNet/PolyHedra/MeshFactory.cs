﻿using System;
using System.Collections.Generic;
using System.Text;

using CGALDotNet.Geometry;

namespace CGALDotNet.Polyhedra
{

	public struct UVSphereParams
    {
		public int meridians;
		public int parallels;
		public double scale;

		public static UVSphereParams Default
        {
			get
            {
				var param = new UVSphereParams();
				param.parallels = 32;
				param.meridians = 32;
				param.scale = 1;
				return param;
            }
        }
	}

	public struct NormalizedCubeParams
	{
		public int divisions;
		public double scale;

		public static NormalizedCubeParams Default
		{
			get
			{
				var param = new NormalizedCubeParams();
				param.divisions = 32;
				param.scale = 1;
				return param;
			}
		}
	}

	public struct PlaneParams
	{
		public double width;
		public double height;
		public int divisionsX;
		public int divisionsZ;

		public static PlaneParams Default
		{
			get
			{
				var param = new PlaneParams();
				param.width = 1;
				param.height = 1;
				param.divisionsX = 4;
				param.divisionsZ = 4;
				return param;
			}
		}
	}

	public struct TorusParams
	{
		public int radialDivisions;
		public int tubularDivisions;
		public double radius;
		public double tube;
		public double arc;

		public static TorusParams Default
		{
			get
			{
				var param = new TorusParams();
				param.radialDivisions = 16;
				param.tubularDivisions = 16;
				param.radius = 0.5;
				param.tube = 0.2;
				param.arc = Math.PI * 2;
				return param;
			}
		}
	}

	/// <summary>
	/// https://github.com/caosdoar/spheres/blob/master/src/spheres.cpp
	/// https://github.com/mrdoob/three.js/tree/dev/src/geometries
	/// </summary>
	public static class MeshFactory
    {

		private static Point3d[] Origins =
		{
		new Point3d(-1.0, -1.0, -1.0),
		new Point3d(1.0, -1.0, -1.0),
		new Point3d(1.0, -1.0, 1.0),
		new Point3d(-1.0, -1.0, 1.0),
		new Point3d(-1.0, 1.0, -1.0),
		new Point3d(-1.0, -1.0, 1.0)
		};

		private static Point3d[] Rights =
		{
		new Point3d(2.0, 0.0, 0.0),
		new Point3d(0.0, 0.0, 2.0),
		new Point3d(-2.0, 0.0, 0.0),
		new Point3d(0.0, 0.0, -2.0),
		new Point3d(2.0, 0.0, 0.0),
		new Point3d(2.0, 0.0, 0.0)
		};

		private static Point3d[] Ups =
		{
		new Point3d(0.0, 2.0, 0.0),
		new Point3d(0.0, 2.0, 0.0),
		new Point3d(0.0, 2.0, 0.0),
		new Point3d(0.0, 2.0, 0.0),
		new Point3d(0.0, 0.0, 2.0),
		new Point3d(0.0, 0.0, -2.0)
		};

		private static void AddTriangle(this List<int> list, int item1, int item2, int item3)
        {
            list.Add(item1);
            list.Add(item2);
            list.Add(item3);
        }

		private static void AddQuad(this List<int> list, int item1, int item2, int item3, int item4)
		{
			list.Add(item1);
			list.Add(item2);
			list.Add(item3);
			list.Add(item1);
			list.Add(item3);
			list.Add(item4);
		}

		private static void AddQuadAlt(this List<int> list, int item1, int item2, int item3, int item4)
		{
			list.Add(item1);
			list.Add(item2);
			list.Add(item4);
			list.Add(item2);
			list.Add(item3);
			list.Add(item4);
		}

		public static void CreateCube(List<Point3d> points, List<int> indices, double scale = 1)
        {
            points.Add(new Point3d(-0.5, -0.5, -0.5) * scale);
            points.Add(new Point3d(0.5, -0.5, -0.5) * scale);
            points.Add(new Point3d(0.5, 0.5, -0.5) * scale);
            points.Add(new Point3d(-0.5, 0.5, -0.5) * scale);
            points.Add(new Point3d(-0.5, 0.5, 0.5) * scale);
            points.Add(new Point3d(0.5, 0.5, 0.5) * scale);
            points.Add(new Point3d(0.5, -0.5, 0.5) * scale);
            points.Add(new Point3d(-0.5, -0.5, 0.5) * scale);

            indices.AddTriangle(0, 2, 1); //face front
            indices.AddTriangle(0, 3, 2);
            indices.AddTriangle(2, 3, 4); //face top
            indices.AddTriangle(2, 4, 5);
            indices.AddTriangle(1, 2, 5); //face right
            indices.AddTriangle(1, 5, 6);
            indices.AddTriangle(0, 7, 4); //face left
            indices.AddTriangle(0, 4, 3);
            indices.AddTriangle(5, 4, 7); //face back
            indices.AddTriangle(5, 7, 6);
            indices.AddTriangle(0, 6, 7); //face bottom
            indices.AddTriangle(0, 1, 6);
        }

		public static void CreatePlane(List<Point3d> points, List<int> indices, PlaneParams param)
		{
			double width_half = param.width / 2;
			double height_half = param.height / 2;

			int gridX = param.divisionsX;
			int gridY = param.divisionsZ;

			int gridX1 = gridX + 1;
			int gridY1 = gridY + 1;

			double segment_width = param.width / gridX;
			double segment_height = param.height / gridY;

			for (int iy = 0; iy < gridY1; iy++)
			{
				double y = iy * segment_height - height_half;

				for (int ix = 0; ix < gridX1; ix++)
				{
					double x = ix * segment_width - width_half;
					points.Add(new Point3d(x, 0, -y));
				}
			}

			for (int iy = 0; iy < gridY; iy++)
			{
				for (int ix = 0; ix < gridX; ix++)
				{

					int a = ix + gridX1 * iy;
					int b = ix + gridX1 * (iy + 1);
					int c = (ix + 1) + gridX1 * (iy + 1);
					int d = (ix + 1) + gridX1 * iy;

					indices.AddTriangle(d, b, a);
					indices.AddTriangle(d, c, b);

				}
			}
		}

		public static void CreateUVSphere(List<Point3d> points, List<int> indices, UVSphereParams param)
		{
			double scale = param.scale * 0.5;

			points.Add(new Point3d(0.0, 1.0, 0.0) * scale);

			for (int j = 0; j < param.parallels - 1; ++j)
			{
				double polar = Math.PI * (j + 1) / (double)param.parallels;
				double sp = Math.Sin(polar);
				double cp = Math.Cos(polar);

				for (int i = 0; i < param.meridians; ++i)
				{
					double azimuth = 2.0 * Math.PI * i / (double)param.meridians;
					double sa = Math.Sin(azimuth);
					double ca = Math.Cos(azimuth);
					double x = sp * ca;
					double y = cp;
					double z = sp * sa;

					points.Add(new Point3d(x, y, z) * scale);
				}
			}

			points.Add(new Point3d(0.0, -1.0, 0.0) * scale);

			for (int i = 0; i < param.meridians; ++i)
			{
				int a = i + 1;
				int b = (i + 1) % param.meridians + 1;

				indices.AddTriangle(0, b, a);
			}

			for (int j = 0; j < param.parallels - 2; ++j)
			{
				int aStart = j * param.meridians + 1;
				int bStart = (j + 1) * param.meridians + 1;

				for (int i = 0; i < param.meridians; ++i)
				{
					int a = aStart + i;
					int a1 = aStart + (i + 1) % param.meridians;
					int b = bStart + i;
					int b1 = bStart + (i + 1) % param.meridians;

					indices.AddQuad(a, a1, b1, b);
				}
			}

			for (int i = 0; i < param.meridians; ++i)
			{
				int a = i + param.meridians * (param.parallels - 2) + 1;
				int b = (i + 1) % param.meridians + param.meridians * (param.parallels - 2) + 1;

				indices.AddTriangle(points.Count - 1, a, b);
			}
		}

		public static void CreateNormalizedCube(List<Point3d> points, List<int> indices, NormalizedCubeParams param)
		{
			double scale = param.scale * 0.5;

			double step = 1.0 / param.divisions;
			Point3d step3 = new Point3d(step, step, step);

			for (int face = 0; face < 6; ++face)
			{
				Point3d origin = Origins[face];
				Point3d right = Rights[face];
				Point3d up = Ups[face];

				for (int j = 0; j < param.divisions + 1; ++j)
				{
					Point3d j3= new Point3d(j, j, j);

					for (int i = 0; i < param.divisions + 1; ++i)
					{
						Point3d i3 = new Point3d(i, i, i);
						Point3d p = origin + step3 * (i3 * right + j3 * up);

						points.Add(p.Vector3d.Normalized * scale);
					}
				}
			}

			int k = param.divisions + 1;

			for (int face = 0; face < 6; ++face)
			{
				for (int j = 0; j < param.divisions; ++j)
				{
					bool bottom = j < (param.divisions / 2);

					for (int i = 0; i < param.divisions; ++i)
					{
						bool left = i < (param.divisions / 2);

						int a = (face * k + j) * k + i;
						int b = (face * k + j) * k + i + 1;
						int c = (face * k + j + 1) * k + i;
						int d = (face * k + j + 1) * k + i + 1;

						if (bottom ^ left) 
							indices.AddQuadAlt(a, c, d, b);
						else 
							indices.AddQuad(a, c, d, b);
					}
				}
			}
		}

		public static void CreateTetrahedron(List<Point3d> points, List<int> indices, double scale = 1)
        {
			scale *= 0.5;

			points.Add(new Point3d(1, 1, 1) * scale);
			points.Add(new Point3d(-1, -1, 1) * scale);
			points.Add(new Point3d(-1, 1, -1) * scale);
			points.Add(new Point3d(1, -1, -1) * scale);

			indices.AddTriangle(2, 1, 0);
			indices.AddTriangle(0, 3, 2);
			indices.AddTriangle(1, 3, 0);
			indices.AddTriangle(2, 3, 1);
		}

		public static void CreateOctahedron(List<Point3d> points, List<int> indices, double scale = 1)
        {
			scale *= 0.5;

			points.Add(new Point3d(1, 0, 0) * scale);
			points.Add(new Point3d(-1, 0, 0) * scale);
			points.Add(new Point3d(0, 1, 0) * scale);
			points.Add(new Point3d(0, -1, 0) * scale);
			points.Add(new Point3d(0, 0, 1) * scale);
			points.Add(new Point3d(0, 0, -1) * scale);

			indices.AddTriangle(0, 2, 4);
			indices.AddTriangle(0, 4, 3);
			indices.AddTriangle(0, 3, 5);
			indices.AddTriangle(0, 5, 2);
			indices.AddTriangle(1, 2, 5);
			indices.AddTriangle(1, 5, 3);
			indices.AddTriangle(1, 3, 4);
			indices.AddTriangle(1, 4, 2);
		}

		public static void CreateIcosahedron(List<Point3d> points, List<int> indices, double scale = 1)
		{
			scale *= 0.5;
			double t = (1.0 + Math.Sqrt(5.0)) / 2.0;

			// Vertices
			points.Add(new Vector3d(-1.0, t, 0.0).Normalized * scale);
			points.Add(new Vector3d(1.0, t, 0.0).Normalized * scale);
			points.Add(new Vector3d(-1.0, -t, 0.0).Normalized * scale);
			points.Add(new Vector3d(1.0, -t, 0.0).Normalized * scale);
			points.Add(new Vector3d(0.0, -1.0, t).Normalized * scale);
			points.Add(new Vector3d(0.0, 1.0, t).Normalized * scale);
			points.Add(new Vector3d(0.0, -1.0, -t).Normalized * scale);
			points.Add(new Vector3d(0.0, 1.0, -t).Normalized * scale);
			points.Add(new Vector3d(t, 0.0, -1.0).Normalized * scale);
			points.Add(new Vector3d(t, 0.0, 1.0).Normalized * scale);
			points.Add(new Vector3d(-t, 0.0, -1.0).Normalized * scale);
			points.Add(new Vector3d(-t, 0.0, 1.0).Normalized * scale);

			// Faces
			indices.AddTriangle(0, 11, 5);
			indices.AddTriangle(0, 5, 1);
			indices.AddTriangle(0, 1, 7);
			indices.AddTriangle(0, 7, 10);
			indices.AddTriangle(0, 10, 11);
			indices.AddTriangle(1, 5, 9);
			indices.AddTriangle(5, 11, 4);
			indices.AddTriangle(11, 10, 2);
			indices.AddTriangle(10, 7, 6);
			indices.AddTriangle(7, 1, 8);
			indices.AddTriangle(3, 9, 4);
			indices.AddTriangle(3, 4, 2);
			indices.AddTriangle(3, 2, 6);
			indices.AddTriangle(3, 6, 8);
			indices.AddTriangle(3, 8, 9);
			indices.AddTriangle(4, 9, 5);
			indices.AddTriangle(2, 4, 11);
			indices.AddTriangle(6, 2, 10);
			indices.AddTriangle(8, 6, 7);
			indices.AddTriangle(9, 8, 1);
		}

		public static void CreateDodecahedron(List<Point3d> points, List<int> indices, double scale = 1)
        {
			scale *= 0.5;
			double t = (1 + Math.Sqrt(5)) / 2;
			double r = 1 / t;

			points.Add(new Vector3d(-1, -1, -1).Normalized * scale);
			points.Add(new Vector3d(-1, -1, 1).Normalized * scale);
			points.Add(new Vector3d(-1, 1, -1).Normalized * scale);
			points.Add(new Vector3d(-1, 1, 1).Normalized * scale);
			points.Add(new Vector3d(1, -1, -1).Normalized * scale);
			points.Add(new Vector3d(1, -1, 1).Normalized * scale);
			points.Add(new Vector3d(1, 1, -1).Normalized * scale);
			points.Add(new Vector3d(1, 1, 1).Normalized * scale);
			points.Add(new Vector3d(0, -r, -t).Normalized * scale);
			points.Add(new Vector3d(0, -r, t).Normalized * scale);
			points.Add(new Vector3d(0, r, -t).Normalized * scale);
			points.Add(new Vector3d(0, r, t).Normalized * scale);
			points.Add(new Vector3d(-r, -t, 0).Normalized * scale);
			points.Add(new Vector3d(-r, t, 0).Normalized * scale);
			points.Add(new Vector3d(r, -t, 0).Normalized * scale);
			points.Add(new Vector3d(r, t, 0).Normalized * scale);
			points.Add(new Vector3d(-t, 0, -r).Normalized * scale);
			points.Add(new Vector3d(t, 0, -r).Normalized * scale);
			points.Add(new Vector3d(-t, 0, r).Normalized * scale);
			points.Add(new Vector3d(t, 0, r).Normalized * scale);

			indices.AddTriangle(3, 11, 7);
			indices.AddTriangle(3, 7, 15);
			indices.AddTriangle(3, 15, 13);
			indices.AddTriangle(7, 19, 17);
			indices.AddTriangle(7, 17, 6);
			indices.AddTriangle(7, 6, 15);
			indices.AddTriangle(17, 4, 8);
			indices.AddTriangle(17, 8, 10);
			indices.AddTriangle(17, 10, 6);
			indices.AddTriangle(8, 0, 16);
			indices.AddTriangle(8, 16, 2);
			indices.AddTriangle(8, 2, 10);
			indices.AddTriangle(0, 12, 1);
			indices.AddTriangle(0, 1, 18);
			indices.AddTriangle(0, 18, 16);
			indices.AddTriangle(6, 10, 2);
			indices.AddTriangle(6, 2, 13);
			indices.AddTriangle(6, 13, 15);
			indices.AddTriangle(2, 16, 18);
			indices.AddTriangle(2, 18, 3);
			indices.AddTriangle(2, 3, 13);
			indices.AddTriangle(18, 1, 9);
			indices.AddTriangle(18, 9, 11);
			indices.AddTriangle(18, 11, 3);
			indices.AddTriangle(4, 14, 12);
			indices.AddTriangle(4, 12, 0);
			indices.AddTriangle(4, 0, 8);
			indices.AddTriangle(11, 9, 5);
			indices.AddTriangle(11, 5, 19);
			indices.AddTriangle(11, 19, 7);
			indices.AddTriangle(19, 5, 14);
			indices.AddTriangle(19, 14, 4);
			indices.AddTriangle(19, 4, 17);
			indices.AddTriangle(1, 12, 14);
			indices.AddTriangle(1, 14, 5);
			indices.AddTriangle(1, 5, 9);

		}

		public static void CreateTorus(List<Point3d> points, List<int> indices, TorusParams param)
        {

			for (int j = 0; j <= param.radialDivisions; j++)
			{
				for (int i = 0; i <= param.tubularDivisions; i++)
				{

					double u = i / (double)param.tubularDivisions * param.arc;
					double v = j / (double)param.radialDivisions * Math.PI * 2;

					var vertex = new Point3d();
					vertex.x = (param.radius + param.tube * Math.Cos(v)) * Math.Cos(u);
					vertex.z = (param.radius + param.tube * Math.Cos(v)) * Math.Sin(u);
					vertex.y = param.tube * Math.Sin(v);

					points.Add(vertex);
				}
			}

			for (int j = 1; j <= param.radialDivisions; j++)
			{

				for (int i = 1; i <= param.tubularDivisions; i++)
				{
					int a = (param.tubularDivisions + 1) * j + i - 1;
					int b = (param.tubularDivisions + 1) * (j - 1) + i - 1;
					int c = (param.tubularDivisions + 1) * (j - 1) + i;
					int d = (param.tubularDivisions + 1) * j + i;

					indices.AddTriangle(d, b, a);
					indices.AddTriangle(d, c, b);

				}

			}

		}

	}
}
