using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features.Meta;




public class PassthroughActivator : MonoBehaviour
{
    ARCameraFeature feature;

    void StartPassthrought()
    {
        
        feature = OpenXRSettings.Instance.GetFeature<ARCameraFeature>();
        
    }

}

