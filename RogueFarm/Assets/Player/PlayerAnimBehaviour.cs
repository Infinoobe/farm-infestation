using UnityEngine;

public class PlayerAnimBehaviour : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsName("Sword Attack 2") || stateInfo.IsName("Sword Attack 3"))
        {
            Debug.Log("Player combo");
            animator.transform.parent.GetComponent<Player>().OnAttackStarted();
        }
    }
}
