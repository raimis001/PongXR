using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features.Meta;




public class PassthroughActivator : MonoBehaviour
{
    ARCameraFeature feature;

    void StartPassthrought()
    {
        
        feature = OpenXRSettings.Instance.GetFeature<ARCameraFeature>();
        
    }
    ARCameraManager camManager;

    void Awake() { camManager = GetComponent<ARCameraManager>(); }

    void OnEnable() { camManager.enabled = true; }

    void OnDisable() { camManager.enabled = false; }
}

