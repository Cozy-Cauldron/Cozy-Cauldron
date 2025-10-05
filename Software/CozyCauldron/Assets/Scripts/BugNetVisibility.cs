using UnityEngine;

public class BugNetState : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        BugNetController controller = animator.GetComponent<BugNetController>();
        if (controller != null)
            controller.bugNet.SetActive(true);
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        BugNetController controller = animator.GetComponent<BugNetController>();
        if (controller != null)
            controller.bugNet.SetActive(false);
    }
}