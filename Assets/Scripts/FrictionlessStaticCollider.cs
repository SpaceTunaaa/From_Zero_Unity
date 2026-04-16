using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FrictionlessStaticCollider : MonoBehaviour
{
    [SerializeField] private bool forceBodyStatic = true;
    [SerializeField, Min(0f)] private float bounciness;

    private PhysicsMaterial2D frictionlessMaterial;
    private Collider2D staticCollider;
    private Rigidbody2D body;

    private void Awake()
    {
        ApplyFrictionlessMaterial();
    }

    private void OnValidate()
    {
        ApplyFrictionlessMaterial();
    }

    private void ApplyFrictionlessMaterial()
    {
        if (staticCollider == null)
        {
            staticCollider = GetComponent<Collider2D>();
        }

        if (staticCollider == null)
        {
            return;
        }

        if (body == null)
        {
            body = GetComponent<Rigidbody2D>();
        }

        if (forceBodyStatic && body != null)
        {
            body.bodyType = RigidbodyType2D.Static;
        }

        PhysicsMaterial2D material = GetFrictionlessMaterial();
        material.bounciness = bounciness;
        staticCollider.sharedMaterial = material;
    }

    private PhysicsMaterial2D GetFrictionlessMaterial()
    {
        if (frictionlessMaterial == null)
        {
            frictionlessMaterial = new PhysicsMaterial2D("Frictionless Static Material")
            {
                friction = 0f,
                bounciness = 0f
            };
        }

        return frictionlessMaterial;
    }
}
