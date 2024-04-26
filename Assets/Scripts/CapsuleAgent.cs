using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class CapsuleAgent : Agent
{
    public GameObject target;
    private readonly Vector3 finishPosition = new Vector3(4f, .1f, 0f);
    private readonly Vector3 agentResetPos = new Vector3(2.5f, 1f, .16f);
    private readonly Vector3 targetResetPos = new Vector3(-4f, .1f, 0f);

    [SerializeField] public float targetXSpeed = 40f;
    [SerializeField] public KeyCode jumpKey = KeyCode.Space;

    private bool isGrounded; // Flag to track if the agent is grounded

    public override void OnEpisodeBegin()
    {
        Debug.Log("OnEpisodeBegin");
        // Reset stuff
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.localPosition = agentResetPos;
        target.transform.position = targetResetPos;
        isGrounded = true;

        // Apply force to target ->> todo: randomize targetXSpeed
        target.GetComponent<Rigidbody>().velocity = new Vector3(targetXSpeed, 0f, 0f);

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(target.transform.localPosition);
        sensor.AddObservation(transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Jump
        var jumpAction = actionBuffers.DiscreteActions[0];
        Debug.Log($"Jump Action: {jumpAction}");
        if (jumpAction == 1 && isGrounded) // Check if jump action is requested and agent is grounded
        {
            Jump(); // Call the Jump method
        }

        // Check if target has collided with the agent
        if (Vector3.Distance(target.transform.localPosition, transform.localPosition) < 1.42f)
        {
            SetReward(-1f);
            EndEpisode();
            return;
        }
        // Check if target has reached finish position
        if (target.transform.localPosition.x >= finishPosition.x)
        {
            SetReward(1f);
            EndEpisode();
            return;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // does not work either
        Debug.Log("Entering Heuristic Mode");
        Debug.Log($"Press {jumpKey} to jump");
        var discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = 0;
        if (Input.GetKey(jumpKey))
        {
            discreteActions[0] = 1;
        }
    }

    private void Jump()
    {
        // Apply vertical force to make the agent jump
        GetComponent<Rigidbody>().AddForce(Vector3.up * 10f, ForceMode.Impulse);
        isGrounded = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if agent lands/collides with the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
