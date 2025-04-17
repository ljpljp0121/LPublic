using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

/*
 * This script moves a sphere along a Catmull-Rom spline using 4 waypoints. The waypoints are randomly generated, and
 * the speed is modified by the TimeScale of the sphere.
 */

namespace MagicPigGames.MagicTime.Demo
{
    public class SphereMovement : MonoBehaviour
    {
        [Header("Options")]
        public float baseSpeed = 5f;
        public int curveResolution = 20; // How smooth the curve is
        
        [Header("Plumbing")]
        public BoxCollider boundingBox;
        public Sphere sphere;

        private readonly List<Vector3> _waypoints = new List<Vector3>(); // Waypoints for curved motion
        private float _t = 0; // Keeps track of the interpolation value along the spline

        private void Awake() => EnsureMinimumWaypoints();

        private void Update()
        {
            EnsureMinimumWaypoints();
    
            // Interpolate smoothly along the curve between waypoints
            _t += sphere.DeltaTime * baseSpeed / curveResolution;
            
            // If we reached the end of the segment, move to the next one by removing the current and adding
            // a new one at the end.
            if (_t >= 1f)
            {
                _t = 0f;
                _waypoints.RemoveAt(0);
                _waypoints.Add(GetRandomPointInBoundingBox());
            }

            // Calculate the next position using a Catmull-Rom spline
            var newPosition = CatmullRomSpline(
                _waypoints[0], // Always move toward the first waypoint
                _waypoints[1], // Use the next waypoints for smooth interpolation
                _waypoints[2],
                _waypoints[3],
                _t
            );

            transform.position = newPosition;
        }

        // Get a random point within the bounding box
        private Vector3 GetRandomPointInBoundingBox()
        {
            Bounds bounds = boundingBox.bounds;
            return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }

        // Ensure we have exactly 4 waypoints to generate curves
        private void EnsureMinimumWaypoints()
        {
            while (_waypoints.Count < 4) 
                _waypoints.Add(GetRandomPointInBoundingBox());
        }

        // Calculate the position along the curve using Catmull-Rom formula
        private Vector3 CatmullRomSpline(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) =>
            0.5f * (
                2f * p1 +
                (-p0 + p2) * t +
                (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t +
                (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t
            );

        // Automatically populate sphere with Sphere component
        private void OnValidate()
        {
            if (sphere == null) sphere = GetComponent<Sphere>();
        }
    }
}