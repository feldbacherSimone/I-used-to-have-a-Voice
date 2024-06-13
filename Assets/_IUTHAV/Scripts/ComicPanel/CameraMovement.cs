using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace _IUTHAV.Scripts.ComicPanel
{
    public static class CameraMovement 
    {
        private static Vector3 defaultPos;
        private static float  returnSpeed = 2;

        private static Vector3 resultZ;
        private static Vector3 resultY;
        private static Vector3 resultX; 

        public static void InitProjection(Transform camera, Vector3 defaultPos)
        {
            
            resultZ = camera.transform.rotation * Vector3.forward;
            resultY = camera.transform.rotation * Vector3.up;
            resultX = camera.transform.rotation * Vector3.right;
            
            Debug.Log($"camera.transform.rotation: {camera.transform.rotation} * {Vector3.forward} = {resultZ}");

            CameraMovement.defaultPos = defaultPos; 
        }
        public static Vector3 GetMovementAmount(Vector3 position)
        {
            Vector3 x = new Vector3(position.x, 0,0);
            Vector3 y = new Vector3(0, position.y, 0);
            Vector3 z = new Vector3(0, 0, position.z);
            
            Vector3 output;
            output = z.z < 0 ? z.magnitude * -resultZ : z.magnitude * resultZ;
            output += x.x < 0 ? x.magnitude * -resultX : x.magnitude * resultX;
            output += y.y < 0 ? y.magnitude * -resultY : y.magnitude * resultY;

            //Debug.Log($"Delta pos = {output}");
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