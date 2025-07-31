using System.Collections.Generic;
using UnityEngine;

public class JumpThroughPlatform3D : MonoBehaviour
{
    [SerializeField] private BoxCollider localCollider;
    [SerializeField] private LayerMask onlyAffectsMask = 2147483647;

    private LayerMask ignoringMask = 0;
    private HashSet<Collider> ignoringColliders = new();

    private void FixedUpdate()
    {
        if (ignoringColliders.Count == 0)
        {
            enabled = false;
            ignoringMask = 0;
            return;
        }
        List<Collider> toRemove = null;
        foreach (var item in ignoringColliders)
        {
            // TODO: this is supposed to check if it's outside collision, but we'll keep those non-slanty for the jam
            if (item.bounds.min.y > localCollider.bounds.max.y)
            {
                toRemove ??= new();
                toRemove.Add(item);
                Physics.IgnoreCollision(localCollider, item, false);
            }
        }
        if (toRemove != null)
        {
            foreach (var x in toRemove)
            {
                ignoringColliders.Remove(x);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((onlyAffectsMask.value & (1 << collision.collider.gameObject.layer)) == 0)
        {
            return;
        }
        foreach (var x in collision.contacts)
        {
            if (x.normal.y > -0.2f)
            {
                if (!ignoringColliders.Contains(collision.collider))
                {
                    ignoringColliders.Add(collision.collider);
                    Physics.IgnoreCollision(localCollider, collision.collider, true);
                    collision.collider.attachedRigidbody.linearVelocity = collision.relativeVelocity;
                    ignoringMask = ignoringMask | (1 << collision.collider.gameObject.layer);
                    enabled = true;
                    return;
                }
            }
        }
    }
}
