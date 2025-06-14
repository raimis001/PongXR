using System;
using UnityEngine;

public class BallGrab : MonoBehaviour
{
    XRHand hand;
    bool isBallGrabbed = true;
    public ball bumba;
    private void Start()
    {
        hand = GetComponentInParent<XRHand>();
        if (hand == null)
        {
            Debug.LogError("BallGrab script requires an XRHand component on the same GameObject.");
        }
    }
    private void Update()
    {
        if (isBallGrabbed && hand.triggerDown)
        {
            ReleaseBall();
        }
        if (!isBallGrabbed && hand.gripDown)
        {
            GrabBall();
        }
        
    }

    private void GrabBall()
    {
        bumba.ResetBall(); // Reset the ball's position and state
        bumba.transform.SetParent(transform); // Attach the ball to the hand
        bumba.transform.localPosition = Vector3.zero; // Reset position relative to the hand
        bumba.transform.localRotation = Quaternion.identity; // Reset rotation relative to the hand
        isBallGrabbed = true; // Set the flag to indicate the ball is grabbed
    }

    private void ReleaseBall()
    {
        bumba.transform.SetParent(null); // Detach the ball from the hand
        bumba.DropBall();

        isBallGrabbed = false; 

    }
}

