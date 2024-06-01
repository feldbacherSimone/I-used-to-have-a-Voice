using UnityEngine;

namespace _IUTHAV.Scripts.ComicPanel.Interaction
{
    public class SelectionMaterialChange : MonoBehaviour, ISelectable
    {
        [Tooltip("Valid Panels, where Object can be selected. If none are selected, Object will be selectable everywhere")]
        [SerializeField] protected Panel[] validPanels;
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

        public void OnSelect(SelectionContext context)
        {
            if (context.IsValidPanelExists(validPanels)) {
                if (_meshRenderer != null) _meshRenderer.material = highlightMat;
            }
        }

        public void OnDeselect()
        {
            _meshRenderer.material = initialMat; 
        }
    }
}