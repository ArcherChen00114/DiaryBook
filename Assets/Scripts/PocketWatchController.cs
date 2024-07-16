using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PocketWatchController : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private Transform HourHand;
    [SerializeField]
    private Transform MinuteHand;
    [SerializeField]
    private Transform SecondHand;
    [SerializeField]
    private GameObject watchPanel;
    private bool inRotate;
    public bool Debug;

    private GameTime time;

    // Start is called before the first frame update
    void Start()
    {
        time = EventHandler.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 发射射线检测点击的对象
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == this.transform)
                {
                    OpenWatchPanel();
                }
            }
        }
                time.Second += Time.deltaTime;
        if (time.Second > 60)
        {
            time.Minute += 1;
            time.Second -= 60;
        }
        if (time.Minute > 60)
        {
            time.Hour += 1;
            time.Minute -= 60;
        }
        if (time.Hour > 60)
        {
            time.Day += 1;
            time.Hour -= 24;
        }
        if (!inRotate) { 
            MoveToTime(time);
        }
    }
    private void OpenWatchPanel() {
        if (!watchPanel.activeSelf) {
            watchPanel.SetActive(true);
            UIManager.Instance.DisablePad();
        }
    }

    public void closeWatchPanel()
    {
        if (watchPanel.activeSelf)
        {
            watchPanel.SetActive(false);
            UIManager.Instance.EnablePad();
        }
    }
    private IEnumerator SmoothRotate(Transform hand, float targetRotation, float duration)
    {
        float startRotation = hand.localEulerAngles.z;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float currentRotation = Mathf.LerpAngle(startRotation, -targetRotation, elapsedTime / duration);
            hand.localRotation = Quaternion.Euler(0, 0, currentRotation);
            inRotate = false;
            yield return null;
        }

        hand.localRotation = Quaternion.Euler(0, 0, -targetRotation);
    }
    public void SmoothMoveToTargetTime(GameTime Time, float duration)
    {
        float hourRotation = (time.Hour % 24) * (360f / 12f) + (time.Minute % 60.01f) * (360f / (12f * 60));
        float minuteRotation = (time.Minute % 60) * (360f / 60f) + (time.Second % 60.01f) * (360f / (60f * 60));
        float secondRotation = (time.Second % 60) * (360f / 60f);

        inRotate = true;
        StartCoroutine(SmoothRotate(HourHand, hourRotation, duration));
        StartCoroutine(SmoothRotate(MinuteHand, minuteRotation, duration));
        StartCoroutine(SmoothRotate(SecondHand, secondRotation, duration));
    }

    public void MoveToTime(GameTime time) {

        // 计算每个指针的旋转角度

        float hourRotation = (time.Hour % 24) * (360f / 12f) + (time.Minute % 60.01f) * (360f / (12f*60));
        float minuteRotation = (time.Minute % 60) * (360f / 60f) + (time.Second % 60.01f) * (360f / (60f*60));
        float secondRotation = (time.Second % 60) * (360f / 60f);
        SecondHand.localRotation = Quaternion.Euler(0, 180 + secondRotation,0);
        MinuteHand.localRotation = Quaternion.Euler(0, minuteRotation - 90,0);
        HourHand.localRotation = Quaternion.Euler(0, hourRotation,0);
    }
}
