using UnityEngine;

namespace _IUTHAV.Scripts.ComicPanel {
    public class VectorCalculation : MonoBehaviour
    {
        [SerializeField] private Vector3 input;
        [SerializeField] private Vector3 output;
        [SerializeField] private GameObject normalPlane;

 

        void Update()
        {
            var resultZ = normalPlane.transform.rotation * Vector3.forward;
            var resultY = normalPlane.transform.rotation * Vector3.up;
            var resultX = normalPlane.transform.rotation * Vector3.right;
            Debug.Log(resultZ);
        
            Vector3 z = new Vector3(0, 0, input.z);
            Vector3 y = new Vector3(0, input.y, 0);
            Vector3 x = new Vector3(input.x, 0,0);
        
            output = Vector3.Project(z, resultZ);
            output += Vector3.Project(x, resultX);
            output += Vector3.Project(y, resultY);
            transform.position = output;
        }
    }
}
