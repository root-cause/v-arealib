using GTA.Math;

namespace AreaLib
{
    public class Rectangle3D : AreaBase
    {
        #region Private properties
        private Vector3 _min;
        private Vector3 _max;
        #endregion

        #region Public properties
        public Vector3 Min
        {
            get
            {
                return _min;
            }
        }

        public Vector3 Max
        {
            get
            {
                return _max;
            }
        }
        #endregion

        #region Constructor
        public Rectangle3D(Vector3 min, Vector3 max)
        {
            _min = Vector3.Minimize(min, max);
            _max = Vector3.Maximize(min, max);
        }
        #endregion

        #region Methods
        public override bool IsPointInside(Vector3 position)
        {
            return position.X >= _min.X && position.X <= _max.X
                && position.Y >= _min.Y && position.Y <= _max.Y
                && position.Z >= _min.Z && position.Z <= _max.Z;
        }
        #endregion
    }
}
