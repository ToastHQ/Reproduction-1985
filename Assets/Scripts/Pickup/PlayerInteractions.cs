using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    [Header("InteractableInfo")] public float sphereCastRadius = 0.5f;

    public LayerMask layers;
    public GameObject lookObject;

    [Header("Pickup")] [SerializeField] private Transform pickupParent;

    public GameObject pickupParentLook;
    public GameObject currentlyPickedUpObject;
    public bool freeze;

    [Header("ObjectFollow")] [SerializeField]
    private float minSpeed;

    [SerializeField] private float maxSpeed = 300f;
    [SerializeField] private float maxDistance = 10f;

    [Header("Rotation")] public float rotationSpeed = 100f;

    public Vector2 holdingRotation;
    public LayerMask pickupLayerMask;
    private float currentDist;
    private float currentSpeed;
    private Vector3 fixedRot;
    private RaycastHit hit;

    private Quaternion lookRot;
    private Camera mainCamera;
    private PhysicsObject physicsObject;
    private Rigidbody pickupRB;
    private Vector3 raycastPos;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    //Interactable Object detections and distance check
    private void Update()
    {
        //Here we check if we're currently looking at an interactable object
        raycastPos = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.SphereCast(raycastPos, sphereCastRadius, mainCamera.transform.forward, out hit, maxDistance,
                layers))
        {
            lookObject = hit.collider.transform.root.gameObject;
            if (lookObject.layer != 9 && lookObject.transform.childCount > 0)
                lookObject = lookObject.transform.GetChild(0).gameObject;
        }
        else
        {
            lookObject = null;
        }


        //if we press the button of choice
        if (Input.GetButtonDown("Fire2"))
        {
            //and we're not holding anything
            if (currentlyPickedUpObject == null)
            {
                //and we are looking an interactable object
                if (lookObject != null) PickUpObject();
            }
            //if we press the pickup button and have something, we drop it
            else
            {
                BreakConnection();
            }
        }
    }

    //Velocity movement toward pickup parent and rotation
    private void FixedUpdate()
    {
        if (currentlyPickedUpObject != null)
        {
            currentDist = Vector3.Distance(pickupParent.position, pickupRB.position);
            currentSpeed = Mathf.SmoothStep(minSpeed, maxSpeed, currentDist / maxDistance);
            currentSpeed *= Time.fixedDeltaTime;
            Vector3 direction = pickupParent.position - pickupRB.position;
            pickupRB.linearVelocity = direction.normalized * currentSpeed;
            //Rotation
            //lookRot = Quaternion.LookRotation(pickupParentLook.transform.position - pickupRB.position);
            //lookRot = Quaternion.Slerp(mainCamera.transform.rotation, lookRot, rotationSpeed * Time.deltaTime);
            lookRot = Quaternion.Euler(fixedRot);
            pickupRB.MoveRotation(lookRot);
        }
    }

    private void OnDisable()
    {
        BreakConnection();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(hit.point, sphereCastRadius);
    }

    //Release the object
    public void BreakConnection()
    {
        if (pickupRB != null) pickupRB.constraints = RigidbodyConstraints.None;
        currentlyPickedUpObject = null;
        if (physicsObject != null) physicsObject.pickedUp = false;
        currentDist = 0;
        //Freeze
        if (pickupRB != null)
        {
            var rigids = pickupRB.GetComponentsInChildren(typeof(Rigidbody), true);
            foreach (Rigidbody rig in rigids)
                if (freeze || rig.GetComponentInChildren<SnapObject>() != null)
                    rig.isKinematic = true;
                else
                    rig.isKinematic = false;

            Rigidbody rigid = pickupRB.GetComponent<Rigidbody>();
            if (rigid != null)
            {
                if (freeze)
                    rigid.isKinematic = true;
                else
                    rigid.isKinematic = false;
            }

            if (freeze)
            {
                if (lookObject.layer == 9)
                {
                    //Layer
                    pickupRB.gameObject.layer = 16;
                    var transforms = pickupRB.GetComponentsInChildren(typeof(Transform), true);
                    foreach (Transform transrights in transforms) transrights.gameObject.layer = 16;
                }

                if (lookObject.layer == 17)
                {
                    //Layer
                    pickupRB.gameObject.layer = 18;
                    var transforms = pickupRB.GetComponentsInChildren(typeof(Transform), true);
                    foreach (Transform transrights in transforms) transrights.gameObject.layer = 18;
                }
            }
        }
    }

    public void PickUpObject()
    {
        if (lookObject.layer == 9 || lookObject.layer == 16 || lookObject.layer == 17 || lookObject.layer == 18)
        {
            lookObject = lookObject.transform.root.gameObject;
            physicsObject = lookObject.GetComponentInChildren<PhysicsObject>();
            currentlyPickedUpObject = lookObject;
            pickupRB = currentlyPickedUpObject.GetComponent<Rigidbody>();
            if (pickupRB == null) pickupRB = currentlyPickedUpObject.GetComponentInChildren<Rigidbody>();
            pickupRB.constraints = RigidbodyConstraints.FreezeRotation;
            if (physicsObject != null)
            {
                physicsObject.playerInteractions = this;
                StartCoroutine(physicsObject.PickUp());
            }

            pickupParent.transform.localEulerAngles = Vector3.zero;
            mainCamera.transform.parent.GetComponent<Player>().holdingRotation = new Vector2(0, 0);
            //Freeze
            var rigids = pickupRB.GetComponentsInChildren(typeof(Rigidbody), true);
            foreach (Rigidbody rig in rigids) rig.isKinematic = false;
            Rigidbody rigid = pickupRB.GetComponent<Rigidbody>();
            if (rigid != null) rigid.isKinematic = false;
            if (lookObject.layer == 16)
            {
                //Layer
                pickupRB.gameObject.layer = 9;
                var transforms = pickupRB.GetComponentsInChildren(typeof(Transform), true);
                foreach (Transform transrights in transforms) transrights.gameObject.layer = 9;
            }

            if (lookObject.layer == 18)
            {
                //Layer
                pickupRB.gameObject.layer = 17;
                var transforms = pickupRB.GetComponentsInChildren(typeof(Transform), true);
                foreach (Transform transrights in transforms) transrights.gameObject.layer = 17;
            }
        }
    }

    public bool PickupCheck(bool freezed)
    {
        freeze = freezed;
        //Pickups
        if (currentlyPickedUpObject != null)
        {
            fixedRot = new Vector3(holdingRotation.y, holdingRotation.x, 0);
            if (Input.GetMouseButton(2))
            {
                holdingRotation.x += Input.GetAxis("Mouse X") * 1.5f;
                holdingRotation.y += Input.GetAxis("Mouse Y") * -1.5f;
                return false;
            }

            return true;
        }

        return true;
    }
}