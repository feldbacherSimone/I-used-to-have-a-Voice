using UnityEngine;

namespace _IUTHAV.Scripts.Panel
{
    public static class CameraMovement 
    {
        private static Vector3 defaultPos;
        [SerializeField] private static float  returnSpeed = 2;

        private static Vector3 resultZ;
        private static Vector3 resultY;
        private static Vector3 resultX; 

        public static void InitProjection(Transform camera, Vector3 defaultPos)
        {
            resultZ = camera.transform.rotation * Vector3.forward;
            resultY = camera.transform.rotation * Vector3.up;
            resultX = camera.transform.rotation * Vector3.right;

            CameraMovement.defaultPos = defaultPos; 
        }
        public static Vector3 GetMovementAmount(Vector3 position)
        {
            Vector3 z = new Vector3(0, 0, position.z);
            Vector3 y = new Vector3(0, position.y, 0);
            Vector3 x = new Vector3(position.x, 0,0);

            Vector3 output; 
            
            output = Vector3.Project(z, resultZ);
            output += Vector3.Project(x, resultX);
            output += Vector3.Project(y, resultY);

            
            return output + defaultPos; 
        }

        public static Vector3? ResetCamera(Transform cameraTarget, Vector3 restPosition)
        {
            if (Vector3.Distance(cameraTarget.position, restPosition) > 0.1)
            {
                return Vector3.MoveTowards(cameraTarget.position, restPosition, returnSpeed * Time.deltaTime);
            }
            return null; 
        }
    }
}