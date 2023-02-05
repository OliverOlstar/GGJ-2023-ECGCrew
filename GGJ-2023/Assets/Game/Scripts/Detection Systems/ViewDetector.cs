using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewDetector : MonoBehaviour
{
	[Header("Settings")]
	[SerializeField]
	private float distance = 10f;
	[SerializeField]
	private float angle = 30f;
	[SerializeField]
	private float height = 1.0f;
	[SerializeField]
	private Color meshDetectionColor = Color.red;

	private Mesh detectionMesh = null;

	private Mesh CreateViewMesh()
	{
		Mesh viewMesh = new Mesh();

		int numTriangles = 8;
		int sides = 3;	// Using the amount of sides based on Triangles to calculate the amount of vert's we'll need
		int numVerticies = numTriangles * sides;

		Vector3[] verticies = new Vector3[numVerticies];
		int[] triangles = new int[numVerticies];

		Vector3 bottomCenter = Vector3.zero;
		Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
		Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

		Vector3 topCenter = bottomCenter + (Vector3.up * height);
		Vector3 topLeft = bottomLeft + (Vector3.up * height);
		Vector3 topRight = bottomRight + (Vector3.up * height);

		int currentVert = 0;

		// Left
		verticies[currentVert++] = bottomCenter;
		verticies[currentVert++] = bottomLeft;
		verticies[currentVert++] = topLeft;

		verticies[currentVert++] = topLeft;
		verticies[currentVert++] = topCenter;
		verticies[currentVert++] = bottomCenter;

		// Right
		verticies[currentVert++] = bottomCenter;
		verticies[currentVert++] = topCenter;
		verticies[currentVert++] = topRight;

		verticies[currentVert++] = topRight;
		verticies[currentVert++] = bottomRight;
		verticies[currentVert++] = bottomCenter;

		// Far
		verticies[currentVert++] = bottomLeft;
		verticies[currentVert++] = bottomRight;
		verticies[currentVert++] = topRight;

		verticies[currentVert++] = topRight;
		verticies[currentVert++] = topLeft;
		verticies[currentVert++] = bottomLeft;

		// Top
		verticies[currentVert++] = topCenter;
		verticies[currentVert++] = topLeft;
		verticies[currentVert++] = topRight;

		// Bottom
		verticies[currentVert++] = bottomCenter;
		verticies[currentVert++] = bottomRight;
		verticies[currentVert++] = bottomLeft;

		for (int i = 0; i < numVerticies; i++)
		{
			triangles[i] = i;
		}

		viewMesh.vertices = verticies;
		viewMesh.triangles = triangles;
		viewMesh.RecalculateNormals();

		return viewMesh;
	}

	private void OnValidate()
	{
		detectionMesh = CreateViewMesh();
	}

	private void OnDrawGizmos()
	{
		if (detectionMesh)
		{
			Gizmos.color = meshDetectionColor;
			Gizmos.DrawMesh(detectionMesh, transform.position, transform.rotation);
		}
	}
}
