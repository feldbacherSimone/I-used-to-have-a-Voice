using System;
using UnityEngine;

namespace _IUTHAV.Scripts.ComicPanel.Interaction
{
    public class ApplyForceInteraction : MonoBehaviour
    {
        [SerializeField] private float force;
        [SerializeField] private Vector3 forceDirection; 
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void ApplyForce()
        {
            _rigidbody.AddForce(force*forceDirection, ForceMode.Impulse);
        }
    }
}