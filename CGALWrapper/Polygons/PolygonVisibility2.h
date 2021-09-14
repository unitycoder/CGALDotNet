#pragma once

#include "../CGALWrapper.h"
#include "../Geometry/Geometry2.h"
#include "Polygon2.h"
#include "PolygonWithHoles2.h"

#include <CGAL/enum.h> 
#include <CGAL/Polygon_2.h>
#include <CGAL/Polygon_with_holes_2.h>
#include <CGAL/Simple_polygon_visibility_2.h>
#include <CGAL/Arrangement_2.h>
#include <CGAL/Arr_segment_traits_2.h>
#include <CGAL/Arr_naive_point_location.h>
#include <istream>
#include <vector>


template<class K>
class PolygonVisibility2
{
	typedef typename K::Point_2                                                 Point_2;
	typedef typename K::Segment_2                                               Segment_2;
	typedef typename CGAL::Arr_segment_traits_2<K>                              Traits_2;
	typedef typename CGAL::Arrangement_2<Traits_2>                              Arrangement_2;
	typedef typename Arrangement_2::Face_handle                                 Face_handle;
	typedef typename Arrangement_2::Edge_const_iterator							Edge_const_iterator;
	typedef typename Arrangement_2::Ccb_halfedge_circulator                     Ccb_halfedge_circulator;

	typedef typename Arrangement_2::Face_const_handle 							Face;
	typedef typename CGAL::Arr_naive_point_location<Arrangement_2>				Location;
	typedef typename CGAL::Arr_point_location_result<Arrangement_2>::Type		Type;

	typedef typename CGAL::Polygon_2<K>											Polygon_2;
	typedef typename CGAL::Polygon_with_holes_2<K>								Pwh_2;

	typedef typename CGAL::Simple_polygon_visibility_2<Arrangement_2, CGAL::Tag_false> NSPV;
	typedef typename CGAL::Simple_polygon_visibility_2<Arrangement_2, CGAL::Tag_true> RSPV;

public:

	PolygonVisibility2() {}

	inline static PolygonVisibility2* CastToPolygonVisibility2(void* ptr)
	{
		return static_cast<PolygonVisibility2*>(ptr);
	}

	inline static Polygon_2* CastToPolygon2(void* ptr)
	{
		return static_cast<Polygon_2*>(ptr);
	}

	inline static Pwh_2* CastToPolygonWithHoles2(void* ptr)
	{
		return static_cast<Pwh_2*>(ptr);
	}

	static void* ComputeVisibility(Point2d point, Segment2d* segments, int startIndex, int count)
	{
		return nullptr;
	}

};
