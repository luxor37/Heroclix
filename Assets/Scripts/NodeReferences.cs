using UnityEngine;

namespace GridMaster
{
    public class NodeReferences : MonoBehaviour
    {
        public MeshRenderer TileRender;
        public Material[] TileMaterials;

        public void ChangeTileMaterial(int index)
        {
            TileRender.material = TileMaterials[index];
        }
    }
}