using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyVFX : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMesh;
    public VisualEffect VFXGraph;
    public float refreshRate;

    private void Start()
    {
        StartCoroutine(UpdateVFX());
    }

    IEnumerator UpdateVFX()
    {
        while (gameObject.activeSelf)
        {
            
            Mesh m = new Mesh();
            skinnedMesh.BakeMesh(m);

            Vector3[] vectores = m.vertices;
            Mesh m2 = new Mesh();
            m2.vertices = vectores;
            
            VFXGraph.SetMesh("Mesh", m2);

            yield return new WaitForSeconds(refreshRate);
        }
    }
}
