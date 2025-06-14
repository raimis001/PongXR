using TMPro;
using UnityEngine;

public class HandControll : MonoBehaviour
{
    public TMP_Text debugText;
    public ball bumba;
    XRHand hand;

    private void Start()
    {
        hand = GetComponent<XRHand>();
    }

    public void ShowText(string text)
    {
        if (debugText == null)
        {
            return;
        }
        debugText.text = text;
    }

    public void onPermissionGranted(string grantTxt)
    {
        ShowText(grantTxt);
    }

    private void Update()
    {
        if (hand.kind != UnityEngine.XR.XRNode.RightHand)
            return;

        if (hand.triggerDown)
        {
            bumba.DropBall();
        }
        if (hand.gripDown)
        {
            bumba.ResetBall();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Rigidbody body = GetComponent<Rigidbody>();
            ShowText(body.linearVelocity.ToString());
        }
    }
}
