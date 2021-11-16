using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;

public class MoveAgent : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform pillar1;
    [SerializeField] private Transform pillar2;
    [SerializeField] private Transform pillar3;
    [SerializeField] private MeshRenderer status;
    [SerializeField] private Material win;
    [SerializeField] private Material loss;
    private Vector3 targetDistance;
    private CharacterController characterController;
    new private Rigidbody rigidbody;

    private int distance;
    private int distanceBuffer;

    public override void Initialize()
    {
        characterController = GetComponent<CharacterController>();
        rigidbody = GetComponent<Rigidbody>();
    }
    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(0,0,-8f);
        transform.rotation = Quaternion.identity;
        rigidbody.velocity = Vector3.zero;

        pillar1.localScale = new Vector3(0.2f, Random.Range(1f, 4f), 0.2f);
        pillar1.localPosition = new Vector3(Random.Range(-9f, -3f), (pillar1.transform.localScale.y / 2), Random.Range(0f, 9f));

        pillar2.localScale = new Vector3(0.2f, Random.Range(1f, 4f), 0.2f);
        pillar2.localPosition = new Vector3(Random.Range(-2.75f, 2.75f), (pillar2.transform.localScale.y / 2), Random.Range(0f, 9f));

        pillar3.localScale = new Vector3(0.2f, Random.Range(1f, 4f), 0.2f);
        pillar3.localPosition = new Vector3(Random.Range(3f, 9f), (pillar3.transform.localScale.y / 2), Random.Range(0f, 9f));

        int pillarRNG = Random.Range(0, 3);

        if(pillarRNG == 0)
        {
            targetTransform.localPosition = new Vector3(pillar1.localPosition.x, pillar1.localPosition.y * 2f + 0.5f, pillar1.localPosition.z);
        } if(pillarRNG == 1)
        {
            targetTransform.localPosition = new Vector3(pillar2.localPosition.x, pillar2.localPosition.y * 2f + 0.5f, pillar2.localPosition.z);
        } if(pillarRNG == 2)
        {
            targetTransform.localPosition = new Vector3(pillar3.localPosition.x, pillar3.localPosition.y * 2f + 0.5f, pillar3.localPosition.z);
        }
        targetDistance = new Vector3(targetTransform.localPosition.x, 0, targetTransform.localPosition.z);
        distance = Mathf.FloorToInt(Vector3.Distance(targetDistance, transform.localPosition));
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float vertical = actions.DiscreteActions[0] <= 1 ? actions.DiscreteActions[0] : -1;
        float horizontal = actions.DiscreteActions[1] <= 1 ? actions.DiscreteActions[1] : -1;

        characterController.ForwardInput = vertical;
        characterController.TurnInput = horizontal;

        distanceBuffer = Mathf.FloorToInt(Vector3.Distance(targetDistance, transform.localPosition));
        if(distanceBuffer == distance - 1)
        {
            AddReward(0.1f);
            distance = distanceBuffer;
        }
        Debug.Log(distance.ToString());
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> actions = actionsOut.DiscreteActions;
        int vertical = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));
        int horizontal = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));

        actions[0] = vertical >= 0 ? vertical : 2;
        actions[1] = horizontal >= 0 ? horizontal : 2;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Wall>(out Wall wall))
        {
            AddReward(-1f);
            status.material = loss;
            EndEpisode();
        }
        if (other.TryGetComponent<Target>(out Target target))
        {
            AddReward(+1f);
            status.material = win;
            EndEpisode();
        }
    }
}
