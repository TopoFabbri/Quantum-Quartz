using Code.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleTrigger : MonoBehaviour
{
    enum CollectibleState
    {
        Idle = 0,
        Following = 1,
        PickedUp = 2
    }

    [SerializeField] private int id = 0;
    [SerializeField] private float pickupTimer = 2;
    [SerializeField] private float maxDistance = 2;
    [SerializeField] private float maxSpeed = 8;
    [SerializeField] private float yAdjustment = 1;
    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;

    const string ANIMATOR_PARAM = "State";

    ICollector collector;
    Rigidbody2D following;
    float timer = 0;
    Vector2 initialPos;
    bool returning = false;

#if UNITY_EDITOR
    private void Reset()
    {
        SetUniqueId();
    }

    private void OnValidate()
    {
        SetUniqueId();
    }

    private void SetUniqueId()
    {
        HashSet<int> usedIds = new HashSet<int>();
        foreach (CollectibleTrigger obj in FindObjectsOfType<CollectibleTrigger>(true))
        {
            if (obj.GetInstanceID() != this.GetInstanceID())
            {
                usedIds.Add(obj.id);
            }
        }

        int minId = 0;
        if (usedIds.Contains(id))
        {
            while (usedIds.Contains(minId))
            {
                minId++;
            }
            id = minId;
        }
    }
#endif

    private void Start()
    {
        initialPos = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (following || !other.TryGetComponent(out ICollector collector))
        {
            return;
        }

        this.following = collector.GetFollowObject(rb);
        if (!following)
        {
            return;
        }

        this.collector = collector;
        this.collector.OnAdvancePickup += OnAdvancePickup;
        this.collector.OnPausePickup += OnStopPickup;
        this.collector.OnCancelPickup += OnCancelPickup;
        this.returning = false;
        anim.SetInteger(ANIMATOR_PARAM, (int)CollectibleState.Following);
    }

    private void FixedUpdate()
    {
        rb.velocity = Vector2.zero;

        if (following || returning)
        {
            Vector2 destination = returning ? initialPos : following.position;
            Vector2 offset = destination - (Vector2)transform.position;
            float offsetDist = offset.magnitude / maxDistance;
            float speed = maxSpeed * (offsetDist <= 1 ? (returning ? 1 : speedCurve.Evaluate(offsetDist)) : offsetDist);
            rb.velocity = offset.normalized * speed + yAdjustment * Vector2.up * offset.normalized.y;

            if (returning && rb.velocity.magnitude * Time.fixedDeltaTime > offset.magnitude)
            {
                // If the destination would be reached/passed this frame, adjust speed to reach exactly and then stop returning
                rb.velocity = offset / Time.fixedDeltaTime;
                returning = false;
            }
        }
    }

    private void OnAdvancePickup(float deltaTime)
    {
        timer += deltaTime;

        if (timer >= pickupTimer)
        {
            anim.SetInteger(ANIMATOR_PARAM, (int)CollectibleState.PickedUp);
            Unsubscribe();
        }
    }

    private void OnStopPickup()
    {
        timer = 0;
    }
    
    //Called from animation event
    private void OnEndPickup()
    {
        Destroy(gameObject);
    }

    private void OnCancelPickup()
    {
        Unsubscribe();
        timer = 0;
        following = null;
        returning = true;
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void Unsubscribe()
    {
        if (collector != null)
        {
            collector.OnAdvancePickup -= OnAdvancePickup;
            collector.OnPausePickup -= OnStopPickup;
            collector.OnCancelPickup -= OnCancelPickup;
        }
    }
}
