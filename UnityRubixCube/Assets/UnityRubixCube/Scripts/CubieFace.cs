using UnityEngine;
using RSToolkit;

namespace UnityRubixCube {
    public class CubieFace : RSMonoBehaviour 
    {
        public Cubie ParentCubie {get; private set;}

        private float _mouseZ;

        protected override void InitComponents()
        {
            base.InitComponents();
            ParentCubie = transform.parent.GetComponent<Cubie>();
        }

        private Vector3 GetMouseAsWorldPoint()
        {

            Vector3 mousePosition3D = Input.mousePosition;

            mousePosition3D.z = _mouseZ;

            return Camera.main.ScreenToWorldPoint(mousePosition3D);

        }


        void OnMouseDown()
        {
            _mouseZ = Camera.main.WorldToScreenPoint(
                gameObject.transform.position).z;
            //ParentCubie.ParentCube.Sele
            Debug.Log($"Mouse down boss {transform.parent.name}");
        }

        void OnMouseDrag()
        {

            Debug.Log($"Mouse drag boss {transform.parent.name}");

        }

        void OnMouseUp(){

            Debug.Log($"Mouse UP boss {transform.parent.name}");
        }
    }
}