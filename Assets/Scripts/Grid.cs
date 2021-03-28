using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private Transform _root;
    [SerializeField] private PointMass _pointMass;
    [SerializeField] private Camera _camera;
    [SerializeField] private float _force = 1f;
    [SerializeField] private float _radius = 2f;
    
    private const int COLUMNS = 46;
    private const int ROWS = 30;
    private const float SPACING = 0.375f;
    private const float STIFFNESS = 0.28f;
    private const float DAMPING = 0.06f;
    private const float BORDER_STIFFNESS = 0.1f;
    private const float BORDER_DAMPING = 0.1f;
    private const float OTHER_STIFFNESS = 0.002f;
    private const float OTHER_DAMPING = 0.002f;

    private List<Spring> _springs = new List<Spring>();
    private PointMass[,] _points = new PointMass[COLUMNS, ROWS];
    private PointMass[,] _fixedPoints = new PointMass[COLUMNS, ROWS];
    private void Start()
    {
        SpawnPointMasses();
        SetupPointMasses();
    }

    private void SpawnPointMasses()
    {
        var startPosition = _root.transform.position;

        for (var y = 0; y < ROWS; y++)
        {
            for (var x = 0; x < COLUMNS; x++)
            {
                var position = new Vector2(
                    startPosition.x + x*SPACING, 
                    startPosition.y + y*SPACING);
                var pointMass = Instantiate(_pointMass, position, Quaternion.identity);

                if (x == 0 || y == 0 || x == COLUMNS - 1 || y == ROWS - 1)
                {
                    pointMass.InverseMass = 0;
                }
                else
                {
                    pointMass.InverseMass = 1;
                }

                pointMass.transform.parent = _root;
                _points[x, y] = pointMass;
                _fixedPoints[x, y] = pointMass;
            }
        }
    }

    private void SetupPointMasses()
    {
        for (var y = 0; y < ROWS; y++)
        {
            for (var x = 0; x < COLUMNS; x++)
            {
                // anchor the border of the grid 
                if (x == 0 || y == 0 || x == COLUMNS - 1 || y == ROWS - 1)
                { 
                    _springs.Add(new Spring(
                        _fixedPoints[x, y], 
                        _points[x, y], 
                        BORDER_STIFFNESS, 
                        BORDER_DAMPING)); 
                }
                else if (x % 3 == 0 && y % 3 == 0)
                {
                    _springs.Add(new Spring(
                        _fixedPoints[x, y], 
                        _points[x, y], 
                        OTHER_STIFFNESS, 
                        OTHER_DAMPING));
                }

                if (x > 0)
                {
                    _springs.Add(new Spring(
                        _points[x-1, y], 
                        _points[x, y], 
                        STIFFNESS, 
                        DAMPING));
                }
                
                if (y > 0)
                {
                    _springs.Add(new Spring(
                        _points[x, y-1], 
                        _points[x, y], 
                        STIFFNESS, 
                        DAMPING));
                }
            }
        }
    }

    public void ApplyDirectedForce(Vector3 force, Vector3 position, float radius)
    {
        if (Input.GetMouseButtonDown(0))
        {
            foreach (var point in _points)
            {
                if (Vector3.SqrMagnitude(position - point.gameObject.transform.position) < radius * radius)
                    point.ApplyForce(
                        force / (10 + Vector3.Distance(position, point.gameObject.transform.position)));
            }
        }
    }

    private void ApplyImplosiveForce(float force, Vector3 position, float radius)
    {
        if (Input.GetMouseButton(0))
        {
            foreach (var point in _points)
            {
                var distance = position - point.gameObject.transform.position;
                var dist2 = Vector3.SqrMagnitude(distance);
                if (dist2 < radius * radius)
                {
                    point.ApplyForce(force * (position - point.gameObject.transform.position) / (100 + dist2));
                    point.IncreaseDamping(0.6f);
                }
            }
        }
    }
    
    public void ApplyExplosiveForce(float force, Vector3 position, float radius)
    {
        if (Input.GetMouseButton(0))
        {
            foreach (var point in _points)
            {
                var distance = position - point.gameObject.transform.position;
                var dist2 = Vector3.SqrMagnitude(distance);
                if (dist2 < radius * radius)
                {
                    point.ApplyForce(100 * force * (point.transform.position - position) / (10000 + dist2));
                    point.IncreaseDamping(0.6f);
                }
            }
        }
    }
    
    private void Update()
    {
        var mouseWorldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;
        // ApplyDirectedForce(new Vector3(0,0,_force), mouseWorldPosition, _radius);
        ApplyImplosiveForce(_force, mouseWorldPosition, _radius);
        // ApplyExplosiveForce(_force, mouseWorldPosition, _radius);
        
        foreach (var spring in _springs)
        {
            spring.Update();
        }
    }
}
