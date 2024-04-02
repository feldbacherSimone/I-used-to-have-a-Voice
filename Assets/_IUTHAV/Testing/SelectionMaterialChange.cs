using System;
using UnityEngine;

namespace _IUTHAV.Testing
{
    public class SelectionMaterialChange : MonoBehaviour, ISelectable
    {
        [SerializeField] private Material customMaterial;
        private const string DEFAULT_MAT_PATH = "Materials/DefaultSelection";
        private Material highlightMat;
        private Material initialMat;

        private MeshRenderer _meshRenderer;
        private void Start()
        {
            if (customMaterial == null)
            {
                highlightMat = Resources.Load<Material>(DEFAULT_MAT_PATH);
            }
            else
            {
                highlightMat = customMaterial; 
            }

            _meshRenderer = GetComponent<MeshRenderer>();
            if(!_meshRenderer) Debug.LogError("No Mesh Renderer found", this);
            initialMat = _meshRenderer.material;

        }

        public void OnSelect()
        {
            _meshRenderer.material = highlightMat;
        }

        public void OnDeselect()
        {
            _meshRenderer.material = initialMat; 
        }
    }
}