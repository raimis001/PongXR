using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features.Meta;
using UnityEngine.XR.ARSubsystems;




public class PassthroughActivator : MonoBehaviour
{

    public bool enablePassthrough = true;
    ARCameraManager camManager;

    void Awake() { camManager = GetComponent<ARCameraManager>(); }

    void Start() 
    { 
        if (!enablePassthrough) return;

        Color c = Color.black; 
        c.a = 0; 
        Camera.main.backgroundColor = c; 
        camManager.enabled = true; 
    }

    void OnDisable() { camManager.enabled = false; }
}

