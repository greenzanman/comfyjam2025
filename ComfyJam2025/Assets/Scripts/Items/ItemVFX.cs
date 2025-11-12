// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class ItemVFX : MonoBehaviour {
//     [SerializeField] private float bounceHeight = 0.05f;
//     [SerializeField] private float bounceDuration = 0.5f;

//     private Vector3 originalPosition;
//     private float elapsedTime = 0.0f;
//     private bool isBouncingUp = true; // Flag to track bounce direction
//     private bool bouncin = true;
//     private void Start() {
//         // Ensure values are within valid ranges
//         bounceHeight = Mathf.Clamp(bounceHeight, 0.1f, 5.0f);
//         bounceDuration = Mathf.Clamp(bounceDuration, 0.1f, 5.0f);

//         originalPosition = transform.position; // Store initial position
//         StartCoroutine(BounceLoop());

//     }

//     private IEnumerator BounceLoop() {
//         var smolRandomDelay = Random.Range(0, 0.2f);
//         yield return new WaitForSeconds(smolRandomDelay);
//         while (bouncin) {
//             elapsedTime += Time.deltaTime;

//             // Calculate bounce height based on time and bounce duration
//             float currentHeight = bounceHeight * Mathf.Sin(Mathf.PI * elapsedTime / bounceDuration);

//             // Update position with bounce height
//             transform.position = originalPosition + new Vector3(0, currentHeight, 0);

//             // Reverse direction if reaching top or bottom
//             if (currentHeight >= bounceHeight) {
//                 isBouncingUp = !isBouncingUp;
//                 elapsedTime = 0.0f; // Reset timer for smooth transition
//             }

//             yield return null;
//         }
//     }
//     public void SetBounce(bool bounce) {
//         bouncin = bounce;
//     }
// }