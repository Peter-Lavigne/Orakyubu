using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// http://wiki.unity3d.com/index.php?title=ReverseNormals

// flips the normals of a mesh filter, which displays
// it from the inside out only

// to get both ways I can modify this script or just have a copy object
// that isn't flipped

public class FlipNormals : MonoBehaviour
{
	void Start () {
		MeshFilter filter = GetComponent(typeof (MeshFilter)) as MeshFilter;
		if (filter != null)
		{
			Mesh mesh = filter.mesh;
 
			Vector3[] normals = mesh.normals;
			for (int i=0;i<normals.Length;i++)
				normals[i] = -normals[i];
			mesh.normals = normals;
 
			for (int m=0;m<mesh.subMeshCount;m++)
			{
				int[] triangles = mesh.GetTriangles(m);
				for (int i=0;i<triangles.Length;i+=3)
				{
					int temp = triangles[i + 0];
					triangles[i + 0] = triangles[i + 1];
					triangles[i + 1] = temp;
				}
				mesh.SetTriangles(triangles, m);
			}
		}		
	}
}
