using SplineMechanic;
using SplineMesh;
using UnityEngine;

public class SplineCar : SplineManager
{
    [Header("Wheels Transform")]
    [SerializeField] private Transform[] _wheelsParent;

    private GameObject _firstSpline;
    //// Duplicate main spline for other wheels
    //GameObject wheelFrontLeft = Instantiate(_splineGO, _wheelFrontLeft);
    //GameObject wheelBackRight = Instantiate(_splineGO, _wheelBackRight);
    //GameObject wheelBackLeft = Instantiate(_splineGO, _wheelBackLeft);

    //_createdSplineList.Add(wheelFrontLeft.GetComponent<Spline>());
    //        _createdSplineList.Add(wheelBackRight.GetComponent<Spline>());
    //        _createdSplineList.Add(wheelBackLeft.GetComponent<Spline>());

    protected override void OnDrawFinish(Vector3[] linePointsArray)
    {
        if (_splineGO == null)
        {
            for (int i = 0; i < _wheelsParent.Length; i++)
            {
                if (i == 0)
                {
                    CreateSpline(_wheelsParent[0]);
                    _firstSpline = _wheelsParent[0].GetChild(0).gameObject;
                }
                else
                {
                    Instantiate(_firstSpline, _wheelsParent[i]);
                    _createdSplineList.Add(_wheelsParent[i].GetChild(0).GetComponent<Spline>());
                }
            }
        }

        CalculateCenterPoint(linePointsArray);
        CreateSplineNodes(linePointsArray);
    }
}