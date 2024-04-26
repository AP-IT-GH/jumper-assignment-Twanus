using UnityEngine;

public class CubeJump : MonoBehaviour
{
    public float jumpForce = 10f;
    private bool isGrounded;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            // Apply vertical force to make the cube jump
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            // Set isGrounded to false to prevent continuous jumping
            isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with the ground (tagged as "Ground")
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
