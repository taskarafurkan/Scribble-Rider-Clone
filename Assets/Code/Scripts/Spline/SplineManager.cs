using DrawMechanic;
using SplineMesh;
using System.Collections.Generic;
using UnityEngine;

namespace SplineMechanic
{
    public class SplineManager : MonoBehaviour
    {
        #region Fields
        [Header("Script References")]
        [SerializeField] private DrawManager _drawManager;

        [Header("Spline Properties")]
        [SerializeField] private Mesh _splineMesh;
        [SerializeField] private Material _splineMaterial;
        [SerializeField] private Vector3 _spiralScale;
        [SerializeField] private float _splineScaleMultiplier;
        [SerializeField] private PhysicMaterial _physicsMaterial;

        private Spline _spline;
        private SplineSmoother _splineSmoother;
        private SplineMeshTiling _splineMeshTiling;

        private Vector3 _newNodePos;
        protected GameObject _splineGO;
        private Vector3 _centerPoint;

        protected List<Spline> _createdSplineList = new List<Spline>();

        #endregion

        #region Methods

        private void OnEnable()
        {
            _drawManager.OnDrawFinish += OnDrawFinish;
        }

        protected virtual void OnDrawFinish(Vector3[] linePointsArray)
        {
            if (_splineGO == null)
                CreateSpline(null);

            CalculateCenterPoint(linePointsArray);
            CreateSplineNodes(linePointsArray);
        }

        protected virtual void CreateSpline(Transform parent)
        {
            // Setting up spline
            _splineGO = new GameObject("Spline");
            _splineGO.transform.Rotate(180, 90, 0, Space.Self);
            _splineGO.transform.parent = parent.transform;
            _splineGO.transform.localPosition = new Vector3(0, 0, 0);
            _splineGO.transform.localScale *= _splineScaleMultiplier;

            _spline = _splineGO.AddComponent<Spline>();

            _splineSmoother = _splineGO.AddComponent<SplineSmoother>();
            _splineSmoother.enabled = false;

            _splineMeshTiling = _splineGO.AddComponent<SplineMeshTiling>();
            _splineMeshTiling.mesh = _splineMesh;
            _splineMeshTiling.material = _splineMaterial;
            _splineMeshTiling.physicMaterial = _physicsMaterial;
            _splineMeshTiling.rotation = new Vector3(0, 90, 0);
            _splineMeshTiling.scale = _spiralScale;
            _splineMeshTiling.generateCollider = true;
            _splineMeshTiling.updateInPlayMode = true;
            _splineMeshTiling.curveSpace = true;
            _splineMeshTiling.mode = MeshBender.FillingMode.StretchToInterval;
            //_splineMeshTiling.curveSpace = false;
            //_splineMeshTiling.mode = MeshBender.FillingMode.Repeat;
        }

        protected virtual void CreateSplineNodes(Vector3[] linePointsArray)
        {
            if (linePointsArray.Length < 2)
                return;

            _spline.Reset();
            //_spline.RefreshCurves();

            for (int i = 0; i < linePointsArray.Length; i++)
            {
                _newNodePos = _centerPoint - linePointsArray[i];

                if (i <= 1)
                {
                    // First two node is default so we don't create             
                    _spline.nodes[i].Position = _newNodePos;
                    _spline.nodes[i].Direction = _newNodePos;
                }
                else
                {
                    // If distance between desired node and last node is less than given number don't add node
                    if (Vector2.Distance(_newNodePos, _spline.nodes[_spline.nodes.Count - 1].Position) > 0/*0.2f*/)
                    {
                        // Create node for given position and direction
                        _spline.AddNode(new SplineNode(_newNodePos, _newNodePos));
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

            foreach (Spline spline in _createdSplineList)
            {
                spline.nodes = _spline.nodes;
                spline.RefreshCurves();

            }
        }

        protected virtual void CalculateCenterPoint(Vector3[] linePointsArray)
        {
            _centerPoint = Vector3.zero;

            for (int i = 0; i < linePointsArray.Length; i++)
            {
                _centerPoint += linePointsArray[i];
            }

            _centerPoint /= linePointsArray.Length;
        }
    }
    #endregion
}