using UnityEngine;

namespace FirstPersonMobileTools.Utility
{

    [System.Serializable]
    public class Bobbing {
        
        [System.Serializable]
        public class SpeedMultiplier
        {
            public float InStep = 1.0f;
            public float OutStep = 1.0f;
        }

        public AnimationCurve BobCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
        public Vector3 BobRange = new Vector3(0.1f, 0.1f, 0.1f);
        public SpeedMultiplier Speed_Multiplier = new SpeedMultiplier();

        private Vector3 PreviousLerp = Vector3.zero;
        private Vector3 TargetPosition = Vector3.zero;
        private float BobSpeed;
        private float BobCurveDuration = 0.0f;
        private float CycleTime = 0; 

        [HideInInspector] public float StepCount = 0.0f;
        [HideInInspector] public bool BackToOriginalPosition = true;
        [HideInInspector] public bool OnStep = false;
        
        public void SetUp()
        {
            BobCurveDuration = BobCurve[BobCurve.length - 1].time;
            TargetPosition = BobRange;
            BobSpeed = Speed_Multiplier.InStep;
        }

        public Vector3 UpdateBobValue(float speed, Vector3 _BobRange)
        {
            CycleTime += Time.deltaTime * BobSpeed * speed;
            CycleTime = Mathf.Clamp(CycleTime, 0, BobCurveDuration);

            if (CycleTime == BobCurveDuration) 
            {  
                CycleTime = 0;
                StepCount++;
                BobSpeed = StepCount % 2 == 1? Speed_Multiplier.OutStep : Speed_Multiplier.InStep;
                BackToOriginalPosition = StepCount % 2 == 0;
                OnStep = StepCount % 2 == 1;
                
                TargetPosition = new Vector3(StepCount % 4 == 0 || StepCount % 4 == 1? _BobRange.x : -_BobRange.x, _BobRange.y, _BobRange.z);
            } 
            else if (BackToOriginalPosition || OnStep) 
            {
                BackToOriginalPosition = false;
                OnStep = false;
            }

            float CurveTime = Mathf.Clamp(StepCount % 2 == 0? CycleTime : BobCurveDuration - CycleTime, 0, BobCurveDuration);

            Vector3 Lerp = Vector3.Lerp(Vector3.zero, TargetPosition, BobCurve.Evaluate(CurveTime));
            Vector3 LerpDelta = Lerp - PreviousLerp;
            PreviousLerp = Lerp;

            return LerpDelta;
        }

    }

}