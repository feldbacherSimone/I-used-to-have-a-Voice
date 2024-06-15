using System;
using System.Collections.Generic;
using UnityEngine;

namespace _IUTHAV.Scripts.ComicPanel.Interaction
{
    public class OutlineSelectionResponse : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;

        [SerializeField] private Material m_Outline;
        [SerializeField] private bool overrideMatSettings; 
        [SerializeField] private float lineWidth = 0.1f; 
        [SerializeField] private Color lineColor = Color.white;
        

        private void Awake()
        {
            if (!_meshRenderer && !TryGetComponent(out _meshRenderer))
            {
                Debug.LogError("No Mesh Renderer for Outline Found", this);
            }

            if (!overrideMatSettings)
            {
                lineWidth = m_Outline.GetFloat("_OutlineThickness");
                lineColor = m_Outline.GetColor("_OutlineColor");
            }
            
        }
        public void AddOutline()
        {
            if (_meshRenderer.materials.Length < 2 || _meshRenderer.materials[1] != m_Outline)
            {
                _meshRenderer.SetMaterials(new List<Material>{_meshRenderer.materials[0], m_Outline}); 
                m_Outline.SetColor("_OutlineColor", lineColor);
            }
            m_Outline.SetFloat("_OutlineThickness", lineWidth);
        }
        public void RemoveOutline()
        {
            m_Outline.SetFloat("_OutlineThickness", 0.0f);
        }

        private void OnDisable()
        {
            m_Outline.SetFloat("_OutlineThickness", lineWidth);
        }
    }
}