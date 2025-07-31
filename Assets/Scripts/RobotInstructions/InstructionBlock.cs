using UnityEngine;

[SelectionBase]
public class InstructionBlock : MonoBehaviour
{
    [SerializeField] private Rigidbody body;
    [SerializeField, Min(0f)] private float addedGravity;

    public Rigidbody Body => body;

    private void FixedUpdate()
    {
        body.AddForce(new Vector3(0f, -addedGravity * body.mass, 0f));
    }
}
