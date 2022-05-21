using UnityEngine;

namespace LevelControl
{
    public class LevelObject : MonoBehaviour
    {
        public LvlObjectType ObjType;

        public enum LvlObjectType
        {
            Floor,
            Obstacle,
            Hindering
        }
    }
}