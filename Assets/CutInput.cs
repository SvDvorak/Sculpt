using System;
using System.Collections.Generic;
using System.Linq;
using Parabox.CSG;
using UnityEngine;

public class CutInput : MonoBehaviour
{
    private bool _cutting;
    private Vector3 _pos;
    public List<Vector3> _points = new List<Vector3>();
    public GameObject BaseBlock;
    private MeshFilter _baseBlockMesh;

    public void Start()
    {
        _baseBlockMesh = BaseBlock.GetComponent<MeshFilter>();
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _cutting = true;
            SetNewPoint();
        }

        if (_cutting && Input.GetMouseButtonUp(0))
        {
            _cutting = false;
            if(_points.Count > 3)
                PerformCut();
            _points.Clear();
        }

        if (_cutting)
        {
            var posDelta = Input.mousePosition - _pos;
            if (posDelta.magnitude > 100)
            {
                SetNewPoint();
            }
        }
    }

    private void SetNewPoint()
    {
        _pos = Input.mousePosition;
        _points.Add(_pos);
    }

    private void PerformCut()
    {
        var newMesh = new GameObject("Cut");
        var mesh = new Mesh();
        var nearPoints = _points.Select(ToWorldPointNear).ToArray();
        var farPoints = _points.Select(ToWorldPointFar).ToArray();

        mesh.vertices = nearPoints.Concat(farPoints).ToArray();



        var triangulator = new Triangulator();
        var nearTriangles = triangulator.Triangulate(nearPoints);
        var farTriangles = triangulator.Triangulate(farPoints);
        // Move triangle indices since far points are added after near points
        for (int i = 0; i < farTriangles.Length; i++)
        {
            farTriangles[i] = farTriangles[i] + _points.Count;
        }

        // Should be opposite of near triangles
        FlipTriangles(farTriangles);

        var sideTriangles = GenerateSides(nearPoints);

        mesh.triangles = nearTriangles.Concat(sideTriangles).Concat(farTriangles).ToArray();



        mesh.uv = mesh.vertices.Select(x => new Vector2()).ToArray();
        mesh.colors = mesh.vertices.Select(x => new Color()).ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();

        var filter = newMesh.AddComponent<MeshFilter>();
        filter.mesh = mesh;
        newMesh.AddComponent<MeshRenderer>();
        newMesh.AddComponent<MeshCollider>().sharedMesh = mesh;


        var newBaseBlock = CSG.Subtract(BaseBlock, newMesh);
        _baseBlockMesh.sharedMesh = newBaseBlock;

        Destroy(newMesh);
    }

    private int[] GenerateSides(Vector3[] nearPoints)
    {
        var numberOfPoints = _points.Count;
        var sideIndices = numberOfPoints * 6;
        var sideTriangles = new int[sideIndices];
        for (var i = 0; i < sideIndices; i += 6)
        {
            // Take into account last looping square
            var pIndex = i / 6;
            // Triangle 1
            sideTriangles[i] = pIndex;
            sideTriangles[i + 1] = pIndex + _points.Count;
            sideTriangles[i + 2] = (pIndex + 1) % numberOfPoints;

            // Triangle 2
            sideTriangles[i + 3] = (pIndex + 1) % numberOfPoints;
            sideTriangles[i + 4] = pIndex + _points.Count;
            sideTriangles[i + 5] = (pIndex + 1) % numberOfPoints + _points.Count;
        }

        var cutPlaneDirection = Vector3.Cross(nearPoints[1] - nearPoints[0], nearPoints[2] - nearPoints[0]).normalized;
        var cutDir = cutPlaneDirection == transform.forward ? 1 : -1;

        if (cutDir == 1)
        {
            FlipTriangles(sideTriangles);
        }

        return sideTriangles;
    }

    private static void FlipTriangles(int[] triangles)
    {
        for (var i = 0; i < triangles.Length; i += 3)
        {
            var tmp = triangles[i];
            triangles[i] = triangles[i + 1];
            triangles[i + 1] = tmp;
        }
    }

    private static Vector3 ToWorldPointNear(Vector3 pos)
    {
        pos.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(pos);
    }

    private static Vector3 ToWorldPointFar(Vector3 pos)
    {
        pos.z = Camera.main.farClipPlane;
        return Camera.main.ScreenToWorldPoint(pos);
    }

    private static void PrintPositions(Vector3[] positions)
    {
        var totalString = "";
        for (int i = 0; i < positions.Length; i++)
        {
            var pos = positions[i];
            totalString += string.Format("Point {0}: x: {1}   y: {2}   z: {3}\n", i, pos.x, pos.y, pos.z);
        }

        Debug.Log(totalString);
    }

    private static void PrintTriangles(int[] triangleIndices)
    {
        var totalString = "";
        for (int i = 0; i < triangleIndices.Length/3; i++)
        {
            totalString += string.Format("Triangle {0}: {1} {2} {3}\n", i, triangleIndices[i*3], triangleIndices[i*3+1], triangleIndices[i*3+2]);
        }

        Debug.Log(totalString);
    }
}
