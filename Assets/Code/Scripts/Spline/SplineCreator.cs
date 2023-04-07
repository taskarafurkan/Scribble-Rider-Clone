using DrawMechanic;
using SplineMesh;
using UnityEngine;

public class SplineCreator : MonoBehaviour
{
    [SerializeField] private Spline _spline;
    [SerializeField] private SplineSmoother _splineSmoother;

    private Vector3 _centerPoint;

    [SerializeField] private DrawManager _drawManager;

    private void OnEnable()
    {
        _drawManager.OnDrawFinish += OnDrawFinish;
    }

    private void OnDrawFinish(Vector3[] linePointsArray)
    {
        CalculateCenterPoint(linePointsArray);
        CreateSpline(linePointsArray);
    }

    private void CalculateCenterPoint(Vector3[] linePointsArray)
    {
        _centerPoint = Vector3.zero;

        for (int i = 0; i < linePointsArray.Length; i++)
        {
            _centerPoint += linePointsArray[i];
        }

        _centerPoint /= linePointsArray.Length;
    }

    public void CreateSpline(Vector3[] linePointsArray)
    {
        if (linePointsArray.Length < 2)
            return;

        _spline.Reset();
        _spline.RefreshCurves();



        for (int i = 0; i < linePointsArray.Length; i++)
        {
            Vector3 newNodePos = _centerPoint - linePointsArray[i];

            if (i <= 1)
            {
                // First two node is default so we don't create
                //_spline.nodes[i].Position = linePoints[i];
                //_spline.nodes[i].Direction = linePoints[i];
                _spline.nodes[i].Position = newNodePos;
                _spline.nodes[i].Direction = newNodePos;
            }
            else
            {
                // If distance between desired node and last node is less than given number don't add node
                if (Vector2.Distance(newNodePos, _spline.nodes[_spline.nodes.Count - 1].Position) > .1f)
                {
                    // Create node for given position and direction
                    _spline.AddNode(new SplineNode(newNodePos, newNodePos));

                    //_spline.AddNode(new SplineNode(linePoints[i], linePoints[i]));
                }
            }

        }

        // Smooths the all nodes directions
        _splineSmoother.SmoothAll();

        // Calculate up direction for spline so weird scratches doesn't occur
        foreach (SplineNode node in _spline.nodes)
        {
            if (node.Position.x > node.Direction.x)
            {
                node.Up = new Vector3(0, -1);
            }
            else if (node.Position.x < node.Direction.x)
            {
                node.Up = new Vector3(0, 1);
            }

            if (node.Position.y > node.Direction.y)
            {
                node.Up += new Vector3(1, 0);
            }
            else if (node.Position.y < node.Direction.y)
            {
                node.Up += new Vector3(-1, 0);
            }
        }

    }
}