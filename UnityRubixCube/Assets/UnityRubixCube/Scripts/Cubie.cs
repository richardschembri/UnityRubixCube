using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityRubixCube {
    public class Cubie : MonoBehaviour
    {
        public Vector3 Index3D {get; private set;}

        public void SetValues(Vector3 index3D, Vector3 localPosition, Vector3 localScale){
            Index3D = index3D;
            transform.localPosition = localPosition;
            transform.localScale = localScale;
            name = $"Cubie({index3D.x},{index3D.y},{index3D.z})";
        }

        public void ToggleFaces(bool active){
            for(int i = 0; i < transform.childCount - 1; i++){
                transform.GetChild(i).gameObject.SetActive(active);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }
    }
}