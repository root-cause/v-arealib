using GTA.Math;

namespace AreaLib
{
    public class Sphere : AreaBase
    {
        #region Private properties
        private float _radius;
        private float _radiusSquared;
        #endregion

        #region Public properties
        public Vector3 Center { get; set; }

        public float Radius
        {
            get
            {
                return _radius;
            }

            set
            {
                _radius = value;
                _radiusSquared = value * value;
            }
        }
        #endregion

        #region Constructor
        public Sphere(Vector3 center, float radius)
        {
            Center = center;
            Radius = radius;
        }
        #endregion

        #region Methods
        public override bool IsPointInside(Vector3 position)
        {
            return Center.DistanceToSquared(position) <= _radiusSquared;
        }
        #endregion
    }
}