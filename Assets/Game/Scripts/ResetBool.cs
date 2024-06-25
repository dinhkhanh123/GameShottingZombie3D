using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetBool : StateMachineBehaviour
{
    public string isInteractingBool;
    public bool isInteractingStatus;
    // Ph??ng th?c này ???c g?i khi m?t tr?ng thái (state) m?i ???c nh?p vào trong Animator.
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(isInteractingBool, isInteractingStatus);
    }

   
}
