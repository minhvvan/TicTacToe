using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float widthUnit = 6f;
    private Camera _camera;
    
    private void Start()
    {
        _camera = GetComponent<Camera>();
        _camera.orthographicSize = widthUnit / _camera.aspect / 2;
    }
}
