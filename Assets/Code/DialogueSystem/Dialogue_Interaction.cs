using System.Collections;
using UnityEngine;

public class Dialogue_Interaction : MonoBehaviour
{
    [SerializeField] private Camera MainCamera;
    [SerializeField] private Dialogue dialogue;

    [Tooltip("Khoảng cách để tương tác với NPC")]
    [SerializeField] private float interactionDistance = 2f;

    [Tooltip("Layer nào dùng để tương tác được")]
    [SerializeField] private LayerMask interactableLayer;

    private bool isInteracting = false;

    void Update()
    {
        if (!isInteracting && Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            if (Physics.Raycast(MainCamera.transform.position, MainCamera.transform.forward, out hit, interactionDistance, interactableLayer))
            {
                if (dialogue != null)
                {
                    isInteracting = true;
                    dialogue.gameObject.SetActive(true);
                    StartCoroutine(DisablePlayerInteraction());
                }
            }
        }
    }

    IEnumerator DisablePlayerInteraction()
    {
        yield return new WaitUntil(() => dialogue == null || !dialogue.gameObject.activeSelf);
        isInteracting = false;
    }
}
