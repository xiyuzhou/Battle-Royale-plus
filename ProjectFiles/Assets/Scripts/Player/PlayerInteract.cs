using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerInteract : MonoBehaviour
{
    private Camera cam;
    [SerializeField]
    private float distance = 3f;
    [SerializeField]
    private LayerMask mask;
    private InputManager inputManager;
    [SerializeField]
    private GunSystem gunSystem;
    [SerializeField]
    private PlayerStatus status;
    private string CurString;
    private string PastString;
    void Start()
    {
        cam = GetComponent<PlayerLook>().cam;
        inputManager = GetComponent<InputManager>();
        CurString = string.Empty;
        PastString = string.Empty;
        GameUIManager.instance.UpdateInteractText(CurString);
    }
    void Update()
    {
        PastString = CurString;
        CurString = string.Empty;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * distance);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, distance, mask))
        {
            if (hitInfo.collider.GetComponent<Interactable>() != null)
            {
                Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
                interactable.Player = status;
                interactable.gunSystem = gunSystem;
                CurString = interactable.promptMessage;
                if (inputManager.onFoot.Interact.triggered)
                {
                    interactable.BaseInteract();
                }
            }
        }
        if (PastString != CurString)
            GameUIManager.instance.UpdateInteractText(CurString);
    }
}
