using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private Transform camTransform;
    [SerializeField]
    private float _shakeDuration = 1.0f;
    private float _shakeAmount = 0.7f;
    private float _decreaseFactor = 1.0f;

    Vector3 originalPos;

    void Awake()
    {
        if (camTransform == null)
        {
            camTransform = GetComponent(typeof(Transform)) as Transform;
        }
    }

    void OnEnable()
    {
        originalPos = camTransform.localPosition;
    }

    void Update()
    {
        if (_shakeDuration > 0)
        {
            camTransform.localPosition = originalPos + Random.insideUnitSphere * _shakeAmount;

            _shakeDuration -= Time.deltaTime * _decreaseFactor;
        }
        else
        {
            _shakeDuration = 0f;
            camTransform.localPosition = originalPos;
        }
    }

    public void ResetShakeDuration(float shakeDuration)
    {
        _shakeDuration = shakeDuration;
    }
}