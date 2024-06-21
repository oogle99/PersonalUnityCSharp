using System.Collections.Generic;
using UnityEngine;

public class MouseInteraction : MonoBehaviour
{
    private Camera cam;

    [Header("References")]
    [SerializeField] private LayerMask placeableMask;
    [SerializeField] private LayerMask colliderMask;
    [SerializeField] private GameObject selectedObject;
    [SerializeField] private GameObject selectedChild;
    [SerializeField] private GameObject[] placeableObjects;

    [Header("Placement Params")]
    [SerializeField] private float gridSize;
    [SerializeField] private float objectClearance;
    [SerializeField] private Vector3 collisionScale;

    [Header("Orignal Materials")]
    [SerializeField] private Material originalCubeMat;
    [SerializeField] private Material originalSphereMat;
    [SerializeField] private Material originalCapsuleMat;
    [SerializeField] private Material originalCylinderMat;

    private bool materialCloned = false;
    private Dictionary<GameObject, Material> materialMap;

    void Start()
    {
        Cursor.visible = false;
        cam = Camera.main;
        selectedObject = placeableObjects[0];
        selectedChild = selectedObject.transform.GetChild(0).gameObject;

        InitializeMaterialMap();
    }

    void Update()
    {
        if (Application.isFocused)
        {
            Cursor.visible = false;
        }

        selectedChild = selectedObject.transform.GetChild(0).gameObject;

        CalculateMousePos();
        SelectNewObject();
    }

    private void InitializeMaterialMap()
    {
        materialMap = new Dictionary<GameObject, Material>
        {
            { placeableObjects[1], originalCubeMat },
            { placeableObjects[2], originalSphereMat },
            { placeableObjects[3], originalCapsuleMat },
            { placeableObjects[4], originalCylinderMat }
        };
    }

    private void CalculateMousePos()
    {
        if (Input.GetMouseButton(1))
        {
            Cursor.visible = true;
            return;
        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, placeableMask))
        {
            Vector3 newPos = selectedObject == placeableObjects[0] 
                ? raycastHit.point 
                : new Vector3(
                    Mathf.Floor(raycastHit.point.x / gridSize) * gridSize,
                    raycastHit.point.y,
                    Mathf.Floor(raycastHit.point.z / gridSize) * gridSize
                );

            selectedObject.transform.position = newPos;
            //Debug.DrawLine(ray.origin, raycastHit.point, Color.green);

            Collider[] colliders = Physics.OverlapSphere(newPos, objectClearance, colliderMask);
            int colliderCount = colliders.Length;

            if (selectedObject != placeableObjects[0])
            {
                if (colliderCount <= 1)
                {
                    selectedChild.transform.localScale = Vector3.one;
                    SetChildMaterialColor(selectedChild, Color.green);
                }
                else
                {
                    selectedChild.transform.localScale = collisionScale;
                    SetChildMaterialColor(selectedChild, Color.red);
                }
            }
            
            if (Input.GetMouseButtonDown(0) && selectedObject != placeableObjects[0])
            {
                PlaceObject();
            }
        }
    }

    private void SelectNewObject()
    {
        for (int i = 0; i < placeableObjects.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                ActivateDesiredArrayObject(i);
                break;
            }
        }
    }

    private void ActivateDesiredArrayObject(int placeableIndex)
    {
        selectedObject.transform.Translate(selectedObject.transform.position.x, -5f, selectedObject.transform.position.z);
        selectedObject.SetActive(false);
        selectedObject = placeableObjects[placeableIndex];
        selectedObject.SetActive(true);
    }

    private void PlaceObject()
    {
        Collider[] colliders = Physics.OverlapSphere(selectedObject.transform.position, objectClearance, colliderMask);
        if (colliders.Length > 1)
        {
            return;
        }

        // Call the Dictionary Map
        if (materialMap.ContainsKey(selectedObject))
        {
            selectedChild.GetComponent<Renderer>().material = materialMap[selectedObject];
        }

        Instantiate(selectedChild, selectedChild.transform.position, selectedChild.transform.rotation);
        materialCloned = false;
    }

    private void SetChildMaterialColor(GameObject child, Color color)
    {
        var renderer = child.GetComponent<Renderer>();
        if (!materialCloned)
        {
            renderer.material = new Material(renderer.material);
            materialCloned = true;
        }
        
        renderer.material.color = color;
    }
}
