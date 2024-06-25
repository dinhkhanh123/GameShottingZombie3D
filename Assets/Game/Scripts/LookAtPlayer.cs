using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private Transform mainCameraTransform;

    void Start()
    {
        // L?y tham chi?u ??n Transform c?a camera ch�nh
        mainCameraTransform = Camera.main.transform;

        if (mainCameraTransform == null)
        {
            Debug.LogError("Main camera not found in the scene.");
        }
    }
    // Update is called once per frame
    void Update()
    {
        // N?u c� tham chi?u ??n camera ch�nh
        if (mainCameraTransform != null)
        {
            // Xoay thanh m�u c?a zombie ?? h??ng v? camera ch�nh
            transform.LookAt(transform.position + mainCameraTransform.forward);
        }
    }
}
