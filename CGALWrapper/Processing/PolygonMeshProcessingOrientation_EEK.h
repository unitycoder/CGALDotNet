#pragma once

#include "../CGALWrapper.h"

extern "C"
{
	CGALWRAPPER_API void* PolygonMeshProcessingOrientation_EEK_Create();

	CGALWRAPPER_API void PolygonMeshProcessingOrientation_EEK_Release(void* ptr);

	//Polyhedron

	CGALWRAPPER_API BOOL PolygonMeshProcessingOrientation_EEK_DoesBoundAVolume_PH(void* meshPtr);

	CGALWRAPPER_API BOOL PolygonMeshProcessingOrientation_EEK_IsOutwardOriented_PH(void* meshPtr);

	CGALWRAPPER_API void PolygonMeshProcessingOrientation_EEK_Orient_PH(void* meshPtr);

	CGALWRAPPER_API void PolygonMeshProcessingOrientation_EEK_OrientToBoundAVolume_PH(void* meshPtr);

	CGALWRAPPER_API void PolygonMeshProcessingOrientation_EEK_ReverseFaceOrientations_PH(void* meshPtr);

	//Surface Mesh

	CGALWRAPPER_API BOOL PolygonMeshProcessingOrientation_EEK_DoesBoundAVolume_SM(void* meshPtr);

	CGALWRAPPER_API BOOL PolygonMeshProcessingOrientation_EEK_IsOutwardOriented_SM(void* meshPtr);

	CGALWRAPPER_API void PolygonMeshProcessingOrientation_EEK_Orient_SM(void* meshPtr);

	CGALWRAPPER_API void PolygonMeshProcessingOrientation_EEK_OrientToBoundAVolume_SM(void* meshPtr);

	CGALWRAPPER_API void PolygonMeshProcessingOrientation_EEK_ReverseFaceOrientations_SM(void* meshPtr);

}

