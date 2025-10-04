using UnityEngine;

public class FishingRodState : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        FishingController controller = animator.GetComponent<FishingController>();
        if (controller != null)
            controller.fishingRod.SetActive(true);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        FishingController controller = animator.GetComponent<FishingController>();
        if (controller != null)
            controller.fishingRod.SetActive(false);
    }
}