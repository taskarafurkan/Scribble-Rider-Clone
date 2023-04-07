using DrawMechanic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private DrawManager _drawManager;
    [SerializeField] private Rigidbody _rigidbody;
    //[SerializeField] private ConstantForce _constantForce;

    private void OnEnable()
    {
        _drawManager.OnDrawFinish += OnDrawFinish;
    }

    private void OnDrawFinish(Vector3[] obj)
    {
        _drawManager.OnDrawFinish -= OnDrawFinish;

        _rigidbody.isKinematic = false;
        //_constantForce.enabled = true;
    }
}