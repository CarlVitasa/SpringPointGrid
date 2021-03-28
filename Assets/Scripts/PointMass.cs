using UnityEngine;

public class PointMass : MonoBehaviour
{
    public float InverseMass { get; set; }
    public Vector3 Velocity { get; private set; }

    private Vector3 _acceleration;
    private float _damping = 0.98f;
    private const float VELOCITY_THRESHOLD = 0.000001f;
    private const float DAMPING_THRESHOLD = 0.98f;

    public void ApplyForce(Vector3 force) => _acceleration += force * InverseMass;

    public void IncreaseDamping(float factor) => _damping *= factor;
    
    private void Update()
    {
        Velocity += _acceleration;
        transform.position += Velocity;
        transform.localScale += new Vector3(Velocity.z, Velocity.z, 0);
        _acceleration = Vector3.zero;
        
        if (Velocity.sqrMagnitude < VELOCITY_THRESHOLD)
        {
            Velocity = Vector3.zero;;
        }
        
        Velocity *= _damping;
        _damping = DAMPING_THRESHOLD;
    }
}
