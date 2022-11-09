using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
public class Die : MonoBehaviour
{
    public DieState DieState => _dieState;

    [SerializeField] private Transform _desktopTransform;
    [SerializeField] private Collider _boundsCollider;
    [SerializeField] private float _endRollVelocity;
    [SerializeField] private Transform _respawnPoint;
    [SerializeField] private Transform _forceApplyPoint;
    [SerializeField] private List<DieNumber> _dieNumbers;

    private DieState _dieState;
    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();

        Transform[] children = GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            //Find all scripts which display numbers on die sides (used later to determine die result too)

            DieNumber dieNumber = child.GetComponent<DieNumber>();
            if (dieNumber) _dieNumbers.Add(dieNumber);
        }

        ResetDie();
    }

    private void Update()
    {
        //Determine if roll ended

        if (_rb.velocity.magnitude <= _endRollVelocity && _dieState == DieState.Rolling) EndRoll();
    }

    private void EndRoll()
    {
        //Find out which side is the top one on the die, and the number associated with it, send result to UIManager

        DieNumber result = _dieNumbers.Aggregate((maxHeight, x) => (maxHeight == null || x.transform.position.y > maxHeight.transform.position.y ? x : maxHeight));
        UIManager.Instance.ReportResult(result.Number);
        ResetDie();
    }

    public void ResetDie()
    {
        //Puts die in the middle of the desktop, ready to be grabbed via click or button

        _dieState = DieState.Ready;

        _rb.velocity = Vector3.zero;
        _rb.isKinematic = true;

        transform.position = _respawnPoint.position;
    }

    public void SnapDie(Vector3 position)
    {
        //When die is dragged, update its position to match cursor

        _dieState = DieState.InHand;
        transform.position = position;

        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }

    public void ThrowDie(Vector3 throwForce)
    {
        //Applies a force to a die (at a set position so it feels more realistic), sends information to UIManager

        _dieState = DieState.Thrown;

        _rb.isKinematic = false;
        _rb.AddForceAtPosition(throwForce, _forceApplyPoint.position);

        UIManager.Instance.ReportNewRoll();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //This check is added for futureproofing purpose
        //In case there are other objects die could collide with (e.g. multiple die throw)

        //When state is Rolling, die can't be grabbed with mouse or rolled via button

        if (collision.transform == _desktopTransform) _dieState = DieState.Rolling;
    }

    private void OnTriggerExit(Collider other)
    {
        //This check is added for futureproofing purpose
        //In case there are other trigger colliders that the die could exit from

        //If die exits the collider defined as bounds, it is placed back in the middle of the desktop but it retains its velocity and roll continues

        if (other == _boundsCollider)
        {
            Vector3 newPos = new Vector3(_respawnPoint.position.x, transform.position.y, _respawnPoint.position.z);
            transform.position = _respawnPoint.position;
        }
    }
}