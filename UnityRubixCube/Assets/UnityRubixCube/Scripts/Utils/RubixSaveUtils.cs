using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace UnityRubixCube.Utils{
    public static class RubixSaveUtils 
    {
        private const string PREF_CUBIES = "cubies";
        private const string PREF_ELAPSEDTIME = "elapsedtime";
        [System.Serializable]
        public struct CubieSaveInfo
        {
            public Cubie.CubieIndex Index;
            public Vector3 localPosition;
            public Vector3 localScale;
            public Quaternion localRotation;

            public bool faceUp;
            public bool faceDown;
            public bool faceLeft;
            public bool faceRight;
            public bool faceFront;
            public bool faceBack;
            public CubieSaveInfo(Cubie.CubieIndex index, Vector3 localPosition, Vector3 localScale, Quaternion localRotation,
                bool faceUp, bool faceDown,
                bool faceLeft, bool faceRight,
                bool faceFront, bool faceBack
            ){
                this.Index = index;
                this.localPosition = localPosition;
                this.localScale = localScale;
                this.localRotation = localRotation;
                this.faceUp = faceUp ;
                this.faceDown = faceDown ;
                this.faceLeft = faceLeft ;
                this.faceRight = faceRight ;
                this.faceFront =faceFront ;
                this.faceBack = faceBack ;
            }
        }

        //Save Transform
        public static void SaveCubies(ReadOnlyCollection<Cubie> targets)
        {
            // TransformInfo[] trnfrm = new TransformInfo[tranformToSave.Length];
            var cubieSaveInfos = new CubieSaveInfo[targets.Count];
            for (int i = 0; i < targets.Count; i++)
            {
                cubieSaveInfos[i] = new CubieSaveInfo(targets[i].Index,
                                                        targets[i].transform.localPosition,
                                                        targets[i].transform.localScale,
                                                        targets[i].transform.localRotation,
                                                        targets[i].FaceUp.gameObject.activeSelf,
                                                        targets[i].FaceDown.gameObject.activeSelf,
                                                        targets[i].FaceLeft.gameObject.activeSelf,
                                                        targets[i].FaceRight.gameObject.activeSelf,
                                                        targets[i].FaceFront.gameObject.activeSelf,
                                                        targets[i].FaceBack.gameObject.activeSelf
                                                    );
            }

            string cubiesJson = JsonUtils.ToJson(cubieSaveInfos, true);
            PlayerPrefs.SetString(PREF_CUBIES, cubiesJson);
        }

        public static bool HasCubies(){
            return PlayerPrefs.HasKey(PREF_CUBIES);
        }

        public static void SaveElapsedTime(float elapsedtime){
            PlayerPrefs.SetFloat(PREF_ELAPSEDTIME, elapsedtime);
        }

        public static void DeleteAll(){
            PlayerPrefs.DeleteAll();
        }

        //Load Transform
        public static CubieSaveInfo[] LoadCubieSaveInfos()
        {
            string cubiesJson = PlayerPrefs.GetString(PREF_CUBIES);
            if (string.IsNullOrEmpty(cubiesJson))
            {
                return null;
            }

            return JsonUtils.FromJson<CubieSaveInfo>(cubiesJson);
        }

        public static float LoadElapsedTime(){
            return PlayerPrefs.GetFloat(PREF_ELAPSEDTIME, 0f);
        }
    }
}