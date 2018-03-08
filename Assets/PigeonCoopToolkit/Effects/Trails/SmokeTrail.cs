using UnityEngine;

namespace PigeonCoopToolkit.Effects.Trails
{
    [AddComponentMenu("Pigeon Coop Toolkit/Effects/Smoke Trail")]
    public class SmokeTrail : TrailRenderer_Base
    {
        public float MinVertexDistance = 0.1f;
        public int MaxNumberOfPoints = 50;
        private Vector3 _lastPosition;
        private float _distanceMoved;
        public float RandomForceScale = 1;

        private Transform _clampPosition;

        /// <summary>
        /// Clamp the position of the first point in the trail to the position of a transform. (ie. the gun barrel)
        /// </summary>
        /// <param name="clampTo">Transform to clamp to</param>
        public void ClampFirstPointTo(Transform clampTo)
        {
            _clampPosition = clampTo;
        }

        /// <summary>
        /// Clears the set clamp to transform.
        /// </summary>
        public void ClearClamp()
        {
            _clampPosition = null;
        }

        protected override void Start()
        {
            base.Start();
            _lastPosition = _t.position;
        }

        protected override void Update()
        {
            if (_emit)
            {
                _distanceMoved += Vector3.Distance(_t.position, _lastPosition);

                if (_distanceMoved != 0 && _distanceMoved >= MinVertexDistance)
                {
                    AddPoint(new SmokeTrailPoint(),_lastPosition);
                    _distanceMoved = 0;
                }
                _lastPosition = _t.position;

            }

            base.Update();
        }

        protected override void OnStartEmit()
        {
            _lastPosition = _t.position;
            _distanceMoved = 0;
        }

        protected override void UpdateTrail(PCTrail trail, float deltaTime)
        {
            if (_clampPosition && trail.Points.Count > 0)
            {
                trail.Points[0].Position = _clampPosition.position;
            }
        }

        protected override void Reset()
        {
            base.Reset();
            MinVertexDistance = 0.1f;
            RandomForceScale = 1;
        }

        protected override void InitialiseNewPoint(PCTrailPoint newPoint)
        {
            ((SmokeTrailPoint)newPoint).RandomVec = Random.onUnitSphere * RandomForceScale;
        }
        protected override void OnTranslate(Vector3 t)
        {
            _lastPosition += t;
        }

        protected override int GetMaxNumberOfPoints()
        {
            return MaxNumberOfPoints;
        }
    }

    public class SmokeTrailPoint : PCTrailPoint
    {
        public Vector3 RandomVec;

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
            Position += RandomVec * deltaTime;
        }
    }
}
