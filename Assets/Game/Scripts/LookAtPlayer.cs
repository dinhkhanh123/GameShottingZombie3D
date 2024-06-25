using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private Transform mainCameraTransform;

    void Start()
    {
        // L?y tham chi?u ??n Transform c?a camera chính
        mainCameraTransform = Camera.main.transform;

        if (mainCameraTransform == null)
        {
            Debug.LogError("Main camera not found in the scene.");
        }
    }
    // Update is called once per frame
    void Update()
    {
        // N?u có tham chi?u ??n camera chính
        if (mainCameraTransform != null)
        {
            // Xoay thanh máu c?a zombie ?? h??ng v? camera chính
            transform.LookAt(transform.position + mainCameraTransform.forward);
        }
    }
}
