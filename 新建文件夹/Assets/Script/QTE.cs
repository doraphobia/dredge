using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QTEGame : MonoBehaviour
{
    public RectTransform pointer;         // Pointer that rotates around the circle
    public RectTransform hitZone;         // Visual hit zone indicator (optional)
    public float rotationSpeed = 100f;    // Speed of pointer rotation
    private bool isGameActive = false;    // Tracks if the game is active

    public GameObject objectToFollowMousePrefab; // Prefab for the object to follow the mouse
    private GameObject objectToFollowMouseInstance; // Instance of the prefab

    public Canvas mainCanvas;             // Reference to the Canvas
    public GameObject targetObject;        // The target object in Canvas to change color
    private Image targetImageComponent;    // Image component for UI objects in Canvas

    public Image targetImage;             // Reference to the image in Canvas to replace on win
    public Sprite winSprite;              // The new sprite to set on win

    public event Action OnMiniGameSuccess;

    [Header("Hit Zone Settings")]
    public float hitZoneAngle1 = 90f;     // Position of the first hit zone (degrees)
    public float hitZoneAngle2 = 270f;    // Position of the second hit zone (degrees)
    public float tolerance = 5f;          // Allowed tolerance for a successful hit

    private bool isFollowingMouse = false; // Tracks if the object is currently following the mouse

    private bool hitZone1Achieved = false;
    private bool hitZone2Achieved = false;

    public Slider progressBar;               // Progress bar slider
    public float baseProgressSpeed = 0.1f;   // Base speed of progress increase
    private float progressSpeed;             // Speed of progress, which will increase with each hit
    public float speedMultiplier = 1.5f;     // Multiplier for exponential speed increase


    void Start()
    {
        progressSpeed = baseProgressSpeed;

        if (progressBar != null)
        {
            progressBar.value = 0;
        }

        if (mainCanvas == null)
        {
            mainCanvas = FindObjectOfType<Canvas>();
            if (mainCanvas == null)
            {
                return;
            }
        }

        if (targetObject != null)
        {
            targetImageComponent = targetObject.GetComponent<Image>();
        }

        StartGame();
    }
    void Update()
    {
        if (isGameActive)
        {
            RotatePointer();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                CheckHit();
            }

            // Continuously increase the progress bar while the game is active
            if (progressBar != null && progressBar.value < 1)
            {
                progressBar.value += progressSpeed * Time.deltaTime;

                if (progressBar.value >= 1)
                {
                    progressBar.value = 1;
                    ShowObjectToFollowMouse();
                }
            }
        }

        if (isFollowingMouse && objectToFollowMouseInstance != null)
        {
            StickToMouse();

            if (Input.GetMouseButtonDown(0))
            {
                isFollowingMouse = false;
            }

            if (Input.GetMouseButtonDown(1))
            {
                RotateObjectBy90Degrees();
            }
        }

        transform.SetAsLastSibling();
    }


    private void RotateObjectBy90Degrees()
    {
        if (objectToFollowMouseInstance != null)
        {
            objectToFollowMouseInstance.transform.Rotate(0, 0, 90);
        }
    }


    private void RotatePointer()
    {
        float angle = Mathf.Repeat(Time.time * rotationSpeed, 360f);
        pointer.localRotation = Quaternion.Euler(0, 0, -angle); 
    }

    private void CheckHit()
    {
        float pointerAngle = pointer.localEulerAngles.z;

        if (pointerAngle > 180)
        {
            pointerAngle -= 360;
        }

        bool inHitZone1 = Mathf.Abs(pointerAngle - hitZoneAngle1) <= tolerance;
        bool inHitZone2 = Mathf.Abs(pointerAngle - hitZoneAngle2) <= tolerance;

        if (inHitZone1 && !hitZone1Achieved)
        {
            hitZone1Achieved = true;
            Debug.Log("Hit Zone 1 Achieved!");
            ChangeTargetColor(Color.green);

            progressSpeed *= speedMultiplier;
        }

        if (inHitZone2 && !hitZone2Achieved)
        {
            hitZone2Achieved = true;
            Debug.Log("Hit Zone 2 Achieved!");
            ChangeTargetColor(Color.blue);

            progressSpeed *= speedMultiplier;
        }

        if (hitZone1Achieved && hitZone2Achieved)
        {
            GameSuccess();
        }
    }


    private void GameSuccess()
    {

        // Trigger the victory event
        OnMiniGameSuccess?.Invoke();

        if (targetImage != null && winSprite != null)
        {
            targetImage.sprite = winSprite;
        }

        if (objectToFollowMousePrefab != null && objectToFollowMouseInstance == null)
        {
            objectToFollowMouseInstance = Instantiate(objectToFollowMousePrefab, mainCanvas.transform);
            isFollowingMouse = true; // Start following the mouse
        }

        isGameActive = false;
    }

    private void GameFailure()
    {
        Debug.Log("Fail");
    }

    public void StartGame()
    {
        isGameActive = true;
        hitZone1Achieved = false; 
        hitZone2Achieved = false;

        if (objectToFollowMouseInstance != null)
        {
            Destroy(objectToFollowMouseInstance); 
        }
    }

    private void StickToMouse()
    {
        if (objectToFollowMouseInstance != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                mainCanvas.transform as RectTransform,
                Input.mousePosition,
                mainCanvas.worldCamera,
                out Vector2 localPoint);

            objectToFollowMouseInstance.GetComponent<RectTransform>().localPosition = localPoint;
        }
    }

    private void ChangeTargetColor(Color newColor)
    {
        if (targetImageComponent != null)
        {
            targetImageComponent.color = newColor;
        }
        else
        {
            Debug.LogWarning("Tt");
        }
    }
    private void ShowObjectToFollowMouse()
    {
        if (objectToFollowMousePrefab != null && objectToFollowMouseInstance == null)
        {
            objectToFollowMouseInstance = Instantiate(objectToFollowMousePrefab, mainCanvas.transform);
            isFollowingMouse = true;
        }

        isGameActive = false;
    }
}
