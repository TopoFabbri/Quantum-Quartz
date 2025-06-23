using Code.Scripts.Interfaces;
using Code.Scripts.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Code.Scripts.Tools;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CollectibleTrigger : MonoBehaviour
{
    enum CollectibleState
    {
        Idle = 0,
        Following = 1,
        PickedUp = 2
    }

    [Disable(false, true)]
    [SerializeField] private int id = 0;
    [SerializeField] private float pickupTimer = 2;
    [SerializeField] private float maxDistance = 2;
    [SerializeField] private float maxSpeed = 8;
    [SerializeField] private float yAdjustment = 1;
    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;

    const string STATE_PARAM = "State";
    const string GHOST_PARAM = "Ghost";

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
        if (this.gameObject.scene.isLoaded) //Avoids the prefab calling this function as well
        {
            SetUniqueId();
        }
    }

    private void SetUniqueId()
    {
        HashSet<int> usedIds = new HashSet<int>();
        foreach (CollectibleTrigger obj in FindObjectsOfType<CollectibleTrigger>(true))
        {
            if (!ReferenceEquals(obj, this))
            {
                usedIds.Add(obj.id);
            }
        }

        if (usedIds.Contains(id))
        {
            int minId = 0;
            while (usedIds.Contains(minId))
            {
                minId++;
            }
            id = minId;
            EditorUtility.SetDirty(this);
        }

        if (id >= 32)
        {
            Debug.LogError("Too many collectibles in level, can't add a 33rd!\nIncrease the amount of collectibles that can be saved to disk before adding more to the level!");
            DestroyImmediate(gameObject);
        }
    }
#endif

    private void Start()
    {
        initialPos = transform.position;
        anim.SetBool(GHOST_PARAM, GameManager.Instance.HasCollectible(id));
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
        anim.SetInteger(STATE_PARAM, (int)CollectibleState.Following);
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
            anim.SetInteger(STATE_PARAM, (int)CollectibleState.PickedUp);
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
        GameManager.Instance.PickUpCollectible(id);
        Destroy(gameObject);
    }

    private void OnCancelPickup()
    {
        Unsubscribe();
        timer = 0;
        following = null;
        returning = true;
        anim.SetInteger(STATE_PARAM, (int)CollectibleState.Idle);
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
