using UnityEngine;

namespace _IUTHAV.Scripts.Tilemap {
	public class Wire : MonoBehaviour {
		
		public GameObject start;
		[SerializeField] private GameObject end;
		[SerializeField] private Material segmentMaterial;
		[Space(10)]

		// Diameter of the wire segments
		[SerializeField] private float width = 0.1f;

		// Private fields
		private LineRenderer _mLineRenderer;
		[HideInInspector] public bool _isReady;
		
		public void GenerateWire(GameObject _end) {
			end = _end;
			Start();
		}

		void Start() {

			if (end == null) return;

			if (gameObject.GetComponent<LineRenderer>() == null) gameObject.AddComponent<LineRenderer>();
			_mLineRenderer = gameObject.GetComponent<LineRenderer>();

			_mLineRenderer.useWorldSpace = true;
			_mLineRenderer.startWidth = width;
			_mLineRenderer.endWidth = width;
			_mLineRenderer.material = segmentMaterial;
			_mLineRenderer.positionCount = 2;

			_isReady = true;

		}

		public void Disable() {
			_mLineRenderer.enabled = false;
			_isReady = false;
		}

		void Update() {

			if (_isReady) {
				
				_mLineRenderer.SetPosition(0, start.transform.position);
				_mLineRenderer.SetPosition(1, end.transform.position);
			}
			
			
		}
	}
}
