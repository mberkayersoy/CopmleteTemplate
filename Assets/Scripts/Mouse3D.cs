using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Mouse3D : MonoBehaviour
{
    public static Mouse3D Instance { get; private set; }

    [SerializeField] private LayerMask mouseColliderLayerMask = new LayerMask();

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, mouseColliderLayerMask))
        {
            transform.position = raycastHit.point;
        }
    }

    public static Vector3 GetMouseWorldPosition()
    {
        if (Instance == null)
        {
            Debug.LogError("Mouse3D object does not exist!");
        }

        return Instance.GetMouseWorldPosition_Instance();
    }

    public Vector3 GetMouseWorldPosition_Instance()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, mouseColliderLayerMask))
        {
            return raycastHit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }
}
