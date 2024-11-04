using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowObjectOnTrigger : MonoBehaviour
{
    public GameObject objectToShow;        
    public GameObject gamePrefab;          
    public GameObject player;               
    public Canvas canvas;                  

    private bool isPlayerInRange = false; 
    private Move playerMovementScript;    
    private GameObject instantiatedGame;  

    void Start()
    {
        if (objectToShow != null)
        {
            objectToShow.SetActive(false);
        }

        playerMovementScript = player.GetComponent<Move>();
        if (playerMovementScript == null)
        {
            Debug.LogWarning("r.");
        }

        if (canvas == null)
        {
            canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError(".");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            isPlayerInRange = true;
            if (objectToShow != null)
            {
                objectToShow.SetActive(true); 
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            isPlayerInRange = false;
            if (objectToShow != null)
            {
                objectToShow.SetActive(false); 
            }
        }
    }

    void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.F))
        {
            if (gamePrefab != null && canvas != null && instantiatedGame == null)
            {
                canvas.gameObject.SetActive(true); // Show the Canvas if it's hidden
                instantiatedGame = Instantiate(gamePrefab, canvas.transform);

                RectTransform rectTransform = instantiatedGame.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = Vector2.zero; // Center it on the Canvas
                }
                else
                {
                    Debug.LogWarning("t.");
                }

                if (playerMovementScript != null)
                {
                    playerMovementScript.enabled = false;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.X) && instantiatedGame != null)
        {

            canvas.gameObject.SetActive(false);

            if (playerMovementScript != null)
            {
                playerMovementScript.enabled = true;
            }

            instantiatedGame = null;
        }
    }
}
