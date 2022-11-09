using UnityEngine;

public class DragRoller : MonoBehaviour
{
    [SerializeField] private Die _die;

    [SerializeField] private float _dieHeight;
    [SerializeField] private float _throwForceMultiplier;
    [SerializeField] private float _rollMinForce;

    [SerializeField] private float _simulatedRollMinForce;
    [SerializeField] private float _simulatedRollMaxForce;
    [SerializeField] private float _simulatedRollMinDirectionValue;
    [SerializeField] private float _simulatedRollMaxDirectionValue;

    private bool _dieInHand;
    private float _mouseVelocity;
    private Vector3 _mouseDirection;
    private Vector3 _lastMousePos;

    private static string _desktopLayerMask = "Desktop";

    private void Update()
    {
        if (_dieInHand) TrackMouseDrag();
        DragDie();
    }

    private void TrackMouseDrag()
    {
        //Track current mouse direction and velocity

        if (CursorRaycast(out RaycastHit hit))
        {
            _mouseDirection = (hit.point - _lastMousePos).normalized;
            _mouseVelocity = Vector3.Magnitude(hit.point - _lastMousePos);
            _lastMousePos = hit.point;
        }
    }

    private void DragDie()
    {
        //Detect left mouse click, decide what happens to the die

        if (Input.GetMouseButton(0))
        {
            if (CursorRaycast(out RaycastHit hit) && !(_die.DieState == DieState.Thrown || _die.DieState == DieState.Rolling))
            {
                _dieInHand = true;
                _die.SnapDie(new Vector3(hit.point.x, _dieHeight, hit.point.z)); //Make the die follow the cursor
            }
            else if (_dieInHand)
            {
                //If the cursor leaves the desktop, reset die position
                _dieInHand = false;
                _die.ResetDie();
            }
        }
        else if (_dieInHand) ReleaseDie(); //Throw die
    }

    private void ReleaseDie()
    {
        //Throw die with cursor-based direction/velocity, or reset if velocity is too low

        _dieInHand = false;

        Vector3 throwForce = _mouseDirection * _mouseVelocity * _throwForceMultiplier;

        if (throwForce.magnitude >= _rollMinForce) _die.ThrowDie(throwForce);
        else _die.ResetDie();
    }

    private bool CursorRaycast(out RaycastHit hit)
    {
        //Shoots a raycast to where the cursor is, finds out if it hits the desktop (layermask check)

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        return (Physics.Raycast(ray, out hit, float.MaxValue, LayerMask.GetMask(_desktopLayerMask)));
    }

    public void RandomDieRoll()
    {
        //A simulated die roll activated via UI button

        if (_die.DieState != DieState.Ready) return;

        float x, z;
        x = Random.Range(_simulatedRollMinDirectionValue, _simulatedRollMaxDirectionValue);
        z = Random.Range(_simulatedRollMinDirectionValue, _simulatedRollMaxDirectionValue);

        //50 % chance for the value x or z to be converted to a negative value
        if (Random.Range(0, 2) == 1) x = -x;
        if (Random.Range(0, 2) == 1) z = -z;

        //Prevent a situation, where, if mindirectionvalue is set to zero, both x and z components of the directional vector are zero (hence the die roll doesn't work)
        if (x == 0 && z == 0) x = _simulatedRollMaxDirectionValue;

        Vector3 randomDirection = new Vector3(x, 0, z);

        float randomForce = Random.Range(_simulatedRollMinForce, _simulatedRollMaxForce);

        _die.transform.position = new Vector3(_die.transform.position.x, _dieHeight, _die.transform.position.z);
        _die.ThrowDie(randomDirection * randomForce);
    }
}