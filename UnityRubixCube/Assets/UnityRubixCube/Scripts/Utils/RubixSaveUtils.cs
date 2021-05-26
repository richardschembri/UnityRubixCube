using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityRubixCube;

namespace UnityRubixCube.Utils{
    public static class RubixSaveUtils 
    {
        private const string PREF_CUBIES = "cubies";
        private const string PREF_ELAPSEDTIME = "elapsedtime";
        private const string PREF_MOVES = "moves";
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

        [System.Serializable]
        public struct MoveSaveInfo{
            public int LayerIndex;
            public int MoveAxis;
            public bool Clockwise;
            public bool IsShuffle;

            public MoveSaveInfo(int layerIndex, int moveAxis, bool clockwise, bool isShuffle){
                LayerIndex = layerIndex;
                MoveAxis = moveAxis;
                Clockwise = clockwise;
                IsShuffle = isShuffle;
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

        public static void SaveMoves(LinkedList<RubixCube.Move> targets){                
           if(targets.Count <= 0)
               return;
           var moveSaveInfos = new MoveSaveInfo[targets.Count];
           var target = targets.First;
           int i = 0;
           do{
               moveSaveInfos[i] = new MoveSaveInfo(
                                        target.Value.LayerIndex,
                                        (int)target.Value.MoveAxis,
                                        target.Value.Clockwise,
                                        target.Value.IsShuffle
               );                
               target = target.Next; 
               i++;
           }while(i < targets.Count);
            string movesJson = JsonUtils.ToJson(moveSaveInfos, true);
            PlayerPrefs.SetString(PREF_MOVES, movesJson);
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
            if(!PlayerPrefs.HasKey(PREF_CUBIES)){
                return null;
            }
            string cubiesJson = PlayerPrefs.GetString(PREF_CUBIES);

            return JsonUtils.FromJson<CubieSaveInfo>(cubiesJson);
        }

        public static LinkedList<RubixCube.Move> LoadMoves(){
            var result = new LinkedList<RubixCube.Move>();
            if(!PlayerPrefs.HasKey(PREF_MOVES)){
                return result;
            }
            string movesJson = PlayerPrefs.GetString(PREF_MOVES);

            var moveSaveInfos = JsonUtils.FromJson<MoveSaveInfo>(movesJson);
            for(int i = 0; i < moveSaveInfos.Length; i++){
                result.AddLast(new RubixCube.Move(moveSaveInfos[i]));
            }
            return result;
        }

        public static float LoadElapsedTime(){
            return PlayerPrefs.GetFloat(PREF_ELAPSEDTIME, 0f);
        }
    }
}