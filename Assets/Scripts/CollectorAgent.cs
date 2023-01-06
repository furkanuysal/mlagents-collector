using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine;

public class CollectorAgent : Agent
{
    [Tooltip("Hareket edecek olan platform")]
    public GameObject platform;

    private Vector3 startPosition;
    private CharController charController;
    new private Rigidbody rigidbody;

    public override void Initialize()
    {
        startPosition = transform.position;
        charController = GetComponent<CharController>();
        rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        transform.position = startPosition;
        transform.rotation = Quaternion.Euler(Vector3.up * Random.Range(0f, 360f));
        rigidbody.velocity = Vector3.zero;

        
        platform.transform.position = startPosition + Quaternion.Euler(Vector3.up * Random.Range(1f, 360f)) * Vector3.forward * 20f;

        //Debug.Log(transform.position.ToString());
        //Debug.Log(platform.transform.position.ToString());
        
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        int vertical = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
        int horizontal = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
        bool jump = Input.GetKey(KeyCode.Space);

        ActionSegment<int> actions = actionsOut.DiscreteActions;
        actions[0] = vertical >= 0 ? vertical : 2;
        actions[1] = horizontal >= 0 ? horizontal : 2;
        actions[2] = jump ? 1 : 0;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        if (Vector3.Distance(startPosition, transform.position) > 40f)
        {
            AddReward(-1f);
            EndEpisode();
        }

        float vertical = actions.DiscreteActions[0] <= 1 ? actions.DiscreteActions[0] : -1;
        float horizontal = actions.DiscreteActions[1] <= 1 ? actions.DiscreteActions[1] : -1;
        bool jump = actions.DiscreteActions[2] > 0;

        charController.ForwardInput = vertical;
        charController.TurnInput = horizontal;
        charController.JumpInput = jump;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "collectible")
        {
            AddReward(1f);
            EndEpisode();
        }
    }
}