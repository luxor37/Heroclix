using UnityEngine;

namespace UnitControl
{
    [RequireComponent(typeof(HandleAnimations))]
    public class UnitStates : MonoBehaviour
    {
        public int Team;
        public int Health;
        public bool Selected;
        public bool HasPath;
        public bool Move;

        public float MaxSpeed = 6;
        public float MovingSpeed;
    }
}