using UnityEngine;
using UnityEditor;

public class cameraViewCoords
{
    [MenuItem("Tools/Get Mouse World Coords")]
    static void GetCoords() {
        // Get the ray from the mouse position in the Scene View
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        
        // Create a plane (e.g., Y=0) to intersect with
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        
        if (plane.Raycast(ray, out float enter)) {
            Vector3 worldPoint = ray.GetPoint(enter);
            Debug.Log($"Mouse World Coords: {worldPoint}");
        }
    }
    [MenuItem("Tools/Get Camera World Coords")]
    static void GetCameraCoords()
    {
        SceneView sceneView = SceneView.lastActiveSceneView;
        if (sceneView != null) {
            Vector3 camPos = sceneView.camera.transform.position;
            Quaternion camRot = sceneView.camera.transform.rotation;
            Debug.Log($"Scene Camera Pos: {camPos}, Rot: {camRot}");
        }
    }
}
