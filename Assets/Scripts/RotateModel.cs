using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateModel : MonoBehaviour
{
    [SerializeField]
    private Transform modeTransform;
    private bool isRotate = false;
    private Vector3 startPoint;
    private Vector3 startAngel;
    private float rotateScale = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isRotate) {
            isRotate = true;
            startPoint = Input.mousePosition;
            startAngel = modeTransform.eulerAngles;
        }
        if (Input.GetMouseButtonUp(0)) {
            isRotate = false;
        }
        if (isRotate) {
            var currentPoint = Input.mousePosition;
            var x = startPoint.x - currentPoint.x;
            modeTransform.eulerAngles = startAngel + new Vector3(0,x * rotateScale, 0);
        }
    }
}
