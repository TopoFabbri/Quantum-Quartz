﻿using System.Collections;
using Code.Scripts.Camera;
using Code.Scripts.FSM;
using Code.Scripts.Game;
using Code.Scripts.StateSettings;
using UnityEngine;

namespace Code.Scripts.States
{
    /// <summary>
    /// Die state
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DeathState<T> : BaseState<T>
    {
        protected DeathSettings DeathSettings => (DeathSettings)settings;

        private readonly Transform transform;
        private readonly Rigidbody2D rb;
        private readonly MonoBehaviour mb;
        private readonly CameraController camController;

        public Vector2 Direction { get; set; }
        public bool Ended { get; private set; }

        private bool moving;
        private float speed;

        public DeathState(T id, StateSettings.StateSettings settings, Transform transform, Rigidbody2D rb,
            MonoBehaviour mb) : base(id, settings)
        {
            this.transform = transform;
            this.rb = rb;
            this.mb = mb;

            if (UnityEngine.Camera.main != null)
                UnityEngine.Camera.main.TryGetComponent(out camController);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            Ended = false;
            moving = true;
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;

            if (camController)
                camController.Shake(DeathSettings.shakeDur, DeathSettings.shakeMag);
            
            mb.StartCoroutine(WaitAndEnd());
        }

        public override void OnExit()
        {
            base.OnExit();
            
            Stats.SetDeaths(Stats.GetDeaths() + 1);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            speed = moving
                ? speed + DeathSettings.accel * Time.deltaTime
                : speed - DeathSettings.accel * Time.deltaTime;

            speed = Mathf.Clamp(speed, 0f, DeathSettings.maxSpeed);

            Vector2 target = (Vector2)transform.position + Direction;
            transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }

        /// <summary>
        /// Wait for duration and set to ended
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitAndEnd()
        {
            yield return new WaitForSeconds(DeathSettings.movingDuration);
            moving = false;

            yield return new WaitForSeconds(DeathSettings.duration - DeathSettings.movingDuration);
            Ended = true;
        }
    }
}