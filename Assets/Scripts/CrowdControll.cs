using System.Collections;
using UnityEngine;

public class CrowdControll : MonoBehaviour
{
    public AudioSource[] audioSource;
    IEnumerator Start()
    {
        for (int i = 0; i < audioSource.Length; i++)
        {
            audioSource[i].Play();
            yield return new WaitForSeconds(2);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
