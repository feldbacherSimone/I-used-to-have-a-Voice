using System;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;
using UnityEngine.Serialization;

namespace _IUTHAV.Scripts.Tilemap {
    public class PathController : MonoBehaviour {
        
        [SerializeField] private PathCreator pathCreator;
        [SerializeField] private List<Transform> pathWaypoints = new List<Transform>();
        //public EndOfPathInstruction endOfPathInstruction;
        public float speed = 5;
        [SerializeField] float distanceTravelled;
        [SerializeField] Transform rotationTarget;
        
        private float m_deletedWaypointDistanceAdjustment;

        private List<Transform> m_waypointQueue = new List<Transform>();
        
        [SerializeField] private int m_currentWaypointIndex;
        
#if UNITY_EDITOR
        private void OnValidate() {
            UpdatePosition();
        }
#endif
        [ContextMenu("ConfigureWaypoints")]
        private void ConfigureWaypoints() {
            if (m_waypointQueue.Count == 0) {
                m_waypointQueue.AddRange(pathWaypoints);
            }
            
            // Create a new bezier path from the waypoints.
            BezierPath bezierPath = new BezierPath (m_waypointQueue, false, PathSpace.xyz, true);
            
            pathCreator.bezierPath = bezierPath;

            m_currentWaypointIndex = pathWaypoints.Count;
        }

        public void AddWaypoints(IEnumerable<Transform> transforms) => pathWaypoints.AddRange(transforms);
        
        public void UpdatePosition() {
            
            if (pathWaypoints != null) {
                Vector3 position = pathCreator.path.GetPointAtDistance(distanceTravelled - m_deletedWaypointDistanceAdjustment, EndOfPathInstruction.Stop);
                pathCreator.transform.Translate(transform.position - position);
                Quaternion rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled - m_deletedWaypointDistanceAdjustment, EndOfPathInstruction.Stop);
                rotationTarget.rotation = rotation;
                
                
            }
            
        }
        
        private void UpdateWaypointPosition(bool updateLastWaypoint = false) {
            Transform firstWaypoint = m_waypointQueue[0];
            Transform lastWaypoint = m_waypointQueue[^1];
            
            var target = pathWaypoints[m_currentWaypointIndex];

            m_deletedWaypointDistanceAdjustment = distanceTravelled;

            if (updateLastWaypoint) {
                firstWaypoint.SetPositionAndRotation(lastWaypoint.position, lastWaypoint.rotation);
                lastWaypoint.SetPositionAndRotation(target.position, target.rotation);
            }
            else {
                lastWaypoint.SetPositionAndRotation(firstWaypoint.position, firstWaypoint.rotation);
                firstWaypoint.SetPositionAndRotation(target.position, target.rotation);
            }
            
            ConfigureWaypoints();

        }


    }
}