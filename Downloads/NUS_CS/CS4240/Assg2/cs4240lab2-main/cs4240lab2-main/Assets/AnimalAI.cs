using TMPro;
using UnityEngine;
using static UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets.DynamicMoveProvider;

public class AnimalAI : MonoBehaviour
{
    public enum AnimalState { AS_Idle, AS_Moving, AS_Eating }
    [SerializeField] private AnimalState currentState;
    
    public int numTargets = 0;
    public Transform[] targetLocations;
    public float moveSpd = 2;
    public float rotationSpd = 100;
    public int currTargetToMove = 0;

    public bool isEating = false;
    public float eatingTimer;
    public float eatingDuration = 2;

    public float idleTimer;
    public float idleDuration = 2;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentState = AnimalState.AS_Idle;
        ChangeState(AnimalState.AS_Idle);

        if (targetLocations.Length != 0)
        {
            numTargets = targetLocations.Length;
            currTargetToMove = 0;
        }
        else
        {
            numTargets = 0;
            currTargetToMove = -1;
        }
    }

    public void ChangeState(AnimalState _animalState)
    {
        currentState = _animalState;
        eatingTimer = eatingDuration;
        idleTimer = idleDuration;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case AnimalState.AS_Idle:
                // Stay idle, could randomly move or do other idle actions
                Idle();
                break;
            case AnimalState.AS_Moving:
                Move();
                break;
            case AnimalState.AS_Eating:
                Eat();
                break;
        }
    }
    public void Idle()
    {
        // Do idle behaviors (animations, etc)
        
        // If idle time is over
        idleTimer -= Time.deltaTime;
        if (idleTimer <= 0.0f)
        {
            ChangeState(AnimalState.AS_Moving);
        }
    }

    public void Move()
    {
        // Do move behaviors (animations, etc)

        // 0 means there is no targets assigned so will never move
        if (numTargets > 0)
        {
            Vector3 dir = targetLocations[currTargetToMove].position - transform.position;
            
            // Check if arrived at target location
            if (dir.magnitude < 0.1f)
            {
                // Transition to idle
                ChangeState(AnimalState.AS_Idle);
                currTargetToMove++;
                if (currTargetToMove == numTargets)
                {
                    currTargetToMove = 0;
                }
            }
            else
            {
                // Rotate towards the target position
                Quaternion toRotation = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpd * Time.deltaTime);

                // Move towards target location
                transform.position += moveSpd * dir.normalized * Time.deltaTime;
            }
        }
    }

    public void Eat()
    {
        // Do eat behaviors (animations, etc)

        // If eating time is over
        eatingTimer -= Time.deltaTime;
        if (eatingTimer <= 0.0f)
        {
            ChangeState(AnimalState.AS_Moving);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Food"))
        {
            ChangeState(AnimalState.AS_Eating);
            other.gameObject.SetActive(false); // Simulate the animal consuming the object
            ScoreUI.Instance.IncreaseScore(1); // Increase score
        }
    }
}
