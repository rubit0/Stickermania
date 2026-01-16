using UnityEngine;

namespace Utils
{
    public class ObjectRotator : MonoBehaviour
    {
        float rot;
        public float rotInc = 1f;

        public Vector3 baseEuler;

        // Start is called before the first frame update
        void Start()
        {
            rot = baseEuler.y;
        }

        // Update is called once per frame
        void Update()
        {
            rot += rotInc;
            if (rot > 360F)
                rot -= 360F;

            transform.rotation = Quaternion.Euler(baseEuler.x, rot, baseEuler.z);
        }
    }
}
