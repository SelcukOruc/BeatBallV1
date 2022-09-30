using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyMotion : MonoBehaviour
{
    [SerializeField] private ConfigurableJoint cj;
    [SerializeField] Transform targetLimb;
    [SerializeField] private bool mirrorMotion;
    private void Start()
    {
        cj = GetComponent<ConfigurableJoint>();
    }
    private void Update()
    {
        if (!mirrorMotion)
            cj.targetRotation = targetLimb.rotation;
        else
            cj.targetRotation = Quaternion.Inverse(targetLimb.rotation);

    }



}
