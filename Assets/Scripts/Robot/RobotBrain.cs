using UnityEngine;

public class RobotBrain : MonoBehaviour
{
    public static RobotBrain instance { get; private set; }
    public MeshRenderer faceMesh;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one Robot Brain in scene");
        }
        else
        {
            instance = this;
        }
    }

    public void SetExpression(Material mat)
    {
        Material[] materials = faceMesh.materials;
        materials[2] = mat;
        faceMesh.materials = materials;
    }
}
