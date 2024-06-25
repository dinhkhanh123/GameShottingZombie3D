using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
   public Animator animator;

    int horizontalValue;
    int verticalValue;

     void Awake()
    {
        // Get a reference to the Animator component and set up hash values for parameters
        animator = GetComponent<Animator>();
        horizontalValue = Animator.StringToHash("Horizontal");
        verticalValue = Animator.StringToHash("Vertical");
    }
    /// <summary>
    /// Update the horizontal and vertical movement parameters in the Animator.
    /// </summary>
    /// <param name="horizontalMovement">The value of horizontal movement.</param>
    /// <param name="verticalMovement">The value of vertical movement.</param>
    public void ChangeAnimatorValues(float horizontalMovement, float verticalMovement, bool isSprinting)
    {
        float snappedHorizontalMovement;
        float snappedVerticalMovement;

        #region Snapped Horizontal
        if (horizontalMovement > 0 && horizontalMovement < 0.55f)
        {
            snappedHorizontalMovement = 0.5f;
        }
        else if (horizontalMovement > 0.55f)
        {
            snappedHorizontalMovement = 1f;
        }
        else if (horizontalMovement < 0 && horizontalMovement > -0.55f)
        {
            snappedHorizontalMovement = -0.5f;
        }
        else if (horizontalMovement < -0.55f)
        {
            snappedHorizontalMovement = -1f;
        }
        else
        {
            snappedHorizontalMovement = 0;
        }
        #endregion

        #region Snapped Vertical
        if (verticalMovement > 0 && verticalMovement < 0.55f)
        {
            snappedVerticalMovement = 0.5f;
        }
        else if (verticalMovement > 0.55f)
        {
            snappedVerticalMovement = 1f;
        }
        else if (verticalMovement < 0 && verticalMovement > -0.55f)
        {
            snappedVerticalMovement = -0.5f;
        }
        else if (verticalMovement < -0.55f)
        {
            snappedVerticalMovement = -1f;
        }
        else
        {
            snappedVerticalMovement = 0;
        }
        #endregion

        if (isSprinting)
        {
            snappedHorizontalMovement = horizontalMovement;
            snappedVerticalMovement = 2;
        }


        // Set the Animator parameters for horizontal and vertical movement
        // Thi?t l?p giá tr? c?a Animator cho tham s? ngang (horizontal) v?i giá tr? horizontalMovement.
        // Tham s? th? hai là giá tr? ??u vào c?n ???c ??t, th? ba là th?i gian truy?n vào ?? th?c hi?n d?ch chuy?n (smoothing),
        // và th? t? là th?i gian gi?a các frame ?? xác ??nh t?c ?? c?a d?ch chuy?n.
        animator.SetFloat(horizontalValue, snappedHorizontalMovement, 0.1f, Time.deltaTime);
        animator.SetFloat(verticalValue, snappedVerticalMovement, 0.1f, Time.deltaTime);
    }


    public void PlayTargetAnim(string targetAnim, bool isInteracting)
    {
        animator.SetBool("isInteracting", isInteracting);
        animator.CrossFade(targetAnim, 0.2f);
    }
}
