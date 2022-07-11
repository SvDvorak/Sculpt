using System.Collections.Generic;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    public Transform RotationPoint;

    private Cutter _cutter;
    private bool _cutting;
    private Vector3 _lastPoint;
    private readonly List<Vector3> _points = new List<Vector3>();

    private float _rotationVelocity;
    private Vector3 _lastInputPosition;

    public void Start()
    {
        _cutter = GetComponent<Cutter>();
    }

    public void Update()
    {
        if (Input.touchSupported)
        {
            UpdateTouchInput();
        }
        else
        {
            UpdateMouseInput();
        }

        RotationPoint.Rotate(0, _rotationVelocity, 0);
        _rotationVelocity *= 0.98f;
    }

    private void UpdateTouchInput()
    {
        if (Input.touchCount == 0)
            return;

        if (Input.touchCount == 1)
        {
            var cutInput = Input.GetTouch(0);

            if (cutInput.phase == TouchPhase.Began)
            {
                _cutting = true;
                AddNewPoint(cutInput.position);
            }

            if (_cutting && cutInput.phase == TouchPhase.Ended)
            {
                Cut();
            }

            if (_cutting)
            {
                var posDelta = cutInput.position.ToV3() - _lastPoint;
                UpdatePoints(posDelta);
            }
        }
    }

    private void UpdateMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _cutting = true;
            AddNewPoint(Input.mousePosition);
        }

        if (_cutting && Input.GetMouseButtonUp(0))
        {
            Cut();
        }


        if (_cutting)
        {
            var pointDelta = Input.mousePosition - _lastPoint;
            UpdatePoints(pointDelta);
        }
        else
        {
            if (Input.GetMouseButton(1))
            {
                var positionDelta = (Input.mousePosition - _lastInputPosition);
                _rotationVelocity += positionDelta.x*0.01f;
            }
        }

        _lastInputPosition = Input.mousePosition;
    }

    private void Cut()
    {
        _cutting = false;
        if (_points.Count > 3)
            _cutter.PerformCut(_points);
        _points.Clear();
    }

    private void AddNewPoint(Vector3 point)
    {
        _lastPoint = point;
        _points.Add(_lastPoint);
    }

    private void UpdatePoints(Vector3 posDelta)
    {
        if (posDelta.magnitude > 100)
        {
            AddNewPoint(Input.mousePosition);
        }
    }
}

public static class VectorExtensions
{
    public static Vector3 ToV3(this Vector2 vector)
    {
        return new Vector3(vector.x, vector.y, 0);
    }
}