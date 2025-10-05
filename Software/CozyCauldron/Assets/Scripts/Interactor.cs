
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private float interactionPointRadius = 0.5f;
    [SerializeField] private LayerMask interactableMask;

    private readonly Collider[] colliders = new Collider[3];
    [SerializeField] private int numfound;

    private void Update()
    {
        numfound = Physics.OverlapSphereNonAlloc(interactionPoint.position, interactionPointRadius, colliders, interactableMask);

        if(numfound>0)
        {
            for (int i = 0; i < numfound; i++)
            {
                var interactable = colliders[i].GetComponent<IInteractable>();
                if (interactable != null && Input.GetButtonDown("Interact"))
                {
                    Debug.Log("Calling Interact on " + colliders[i].name);
                    interactable.Interact(this);
                    break; // stop after first successful interact
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(interactionPoint.position, interactionPointRadius);
    }
}