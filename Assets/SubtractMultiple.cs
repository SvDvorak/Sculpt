using System.Collections.Generic;
using Parabox.CSG;
using UnityEngine;

public class SubtractMultiple : MonoBehaviour
{
    public GameObject SubtractorsParent;

    public void Start()
    {
        // Initialize two new meshes in the scene
        //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //sphere.transform.localScale = Vector3.one * 1.3;
        var meshfilter = GetComponent<MeshFilter>();

        foreach (Transform subtractor in SubtractorsParent.transform)
        {
            if (!subtractor.gameObject.activeSelf)
                continue;
            var m = CSG.Subtract(gameObject, subtractor.gameObject);
            meshfilter.sharedMesh = m;
        }
        // Perform boolean operation

        // Create a gameObject to render the result
        //composite = new GameObject();
        //composite.AddComponent<MeshFilter>().sharedMesh = m;
        //composite.AddComponent<MeshRenderer>().sharedMaterial = myMaterial;
    }

    public void Update()
    {
    }
}
