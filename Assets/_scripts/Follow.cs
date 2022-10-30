using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{

    [SerializeField] private UnityEngine.Transform _target;
    private void LateUpdate() {
        if (_target) {
            //FollowTarget(_target);
            LookAtTarget(_target);
            RotateAroundTarget(_target);
        }
    }
    
    void FollowTarget(UnityEngine.Transform target) {
        transform.position = target.position - transform.position;
    }

    void LookAtTarget (UnityEngine.Transform target) {
        transform.LookAt(target);
    }

    void RotateAroundTarget (UnityEngine.Transform target) {

        float x = Mathf.Cos(Time.deltaTime);
        float z = Mathf.Sin(Time.deltaTime);

        transform.RotateAround(target.position, Vector3.up, 20 * Time.deltaTime);
    }
}
