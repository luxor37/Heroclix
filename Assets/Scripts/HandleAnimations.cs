using UnityEngine;

namespace UnitControl
{
    public class HandleAnimations : MonoBehaviour
    {
        private Animator _anim;
        private UnitStates _states;

        private void Start()
        {
            _states = GetComponent<UnitStates>();

            SetupAnimator();
        }

        private void Update()
        {
            _anim.SetFloat("Movement", (_states.Move) ? 1 : 0, 0.1f, Time.deltaTime);

            _states.MovingSpeed = _anim.GetFloat("Movement") * _states.MaxSpeed;
        }

        private void SetupAnimator()
        {
            _anim = GetComponent<Animator>();

            var a = GetComponentsInChildren<Animator>();

            foreach (var animator in a)
            {
                if (animator != _anim)
                {
                    _anim.avatar = animator.avatar;
                    Destroy(animator);
                    break;
                }
            }
        }
    }
}