using UnityEngine;
using UnityEngine.Events;

public class HandTouch : MonoBehaviour
{

    public UnityEvent<XRHand> OnHand;
    HandControll hand;

    private void Start()
    {
        hand = GetComponentInParent<HandControll>();
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        XRHand otherHand = other.GetComponentInParent<XRHand>();
        if (otherHand != null)
            OnHand.Invoke(otherHand);

        hand.ShowText(other.name);
    }
}
