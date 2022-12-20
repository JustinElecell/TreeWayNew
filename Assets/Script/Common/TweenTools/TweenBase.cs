using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace TweenTools
{
    public abstract class TweenBase : MonoBehaviour
    {
        public TweenMode mode = TweenMode.Once;
        public AnimationCurve curve;
        public float duration = 1f;
        public float startDelay = 0f;
        public bool ignoreTimerScale = false;
        public bool resetOnEnable = false;

        public UnityEvent onFinished;

        private bool isInited = false;
        private float timer = 0f;
        private float procress;

        public bool playing;

        private void Init()
        {
            timer = 0f;
            OnInit();
            if (startDelay > 0) StartCoroutine(delay(startDelay));
            else playing = true;
        }

        private void OnEnable()
        {
            if (!isInited || resetOnEnable) Init();
        }

        private void Update()
        {
            if (!playing) return;
            timer += ignoreTimerScale ? Time.unscaledDeltaTime : Time.deltaTime;
            if (timer > duration)
            {
                switch (mode)
                {
                    case TweenMode.Once: timer = duration; playing = false; break;
                    case TweenMode.Loop: timer %= duration; break;
                    case TweenMode.PingPong: timer -= (duration * 2f); break;
                }
            }
            procress = Mathf.Abs(timer / duration);
            OnUpdate(curve.Evaluate(procress));
            if (!playing) onFinished?.Invoke();
        }

        private IEnumerator delay(float dlay)
        {
            yield return new WaitForSeconds(dlay);
            playing = true;
        }

        public abstract void OnInit();
        public abstract void OnUpdate(float value);
    }

    public enum TweenMode
    {
        Once = 0,
        Loop = 1,
        PingPong = 2
    }
}