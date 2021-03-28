using UnityEngine;

public class Spring 
{
    public PointMass End1 { get; }
    public PointMass End2 { get; }
    public float Stiffness { get; }
    public float Damping { get;}
    public float TargetLength { get;}

    public Spring(PointMass end1, PointMass end2, float stiffness, float damping)
    {
        End1 = end1;
        End2 = end2;
        Stiffness = stiffness;
        Damping = damping;
        TargetLength = Vector3.Distance(End1.transform.position, End2.transform.position) * 0.95f;
    }

    public void Update()
    {
        var x = End1.transform.position - End2.transform.position;
 
        var length = x.magnitude;
        // these springs can only pull, not push
        if (length <= TargetLength)
            return;
 
        x = (x / length) * (length - TargetLength);
        var dv = End2.Velocity - End1.Velocity;
        var force = Stiffness * x - dv * Damping;
 
        End1.ApplyForce(-force);
        End2.ApplyForce(force);
    }
}
