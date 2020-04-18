using GTA.Math;

namespace AreaLib
{
    public class Rectangle2D : AreaBase
    {
        #region Private properties
        private Vector2 _min;
        private Vector2 _max;
        #endregion

        #region Public properties
        public Vector2 Min
        {
            get
            {
                return _min;
            }
        }

        public Vector2 Max
        {
            get
            {
                return _max;
            }
        }
        #endregion

        #region Constructor
        public Rectangle2D(Vector2 min, Vector2 max)
        {
            _min = Vector2.Minimize(min, max);
            _max = Vector2.Maximize(min, max);
        }
        #endregion

        #region Methods
        public override bool IsPointInside(Vector3 position)
        {
            return position.X >= _min.X && position.X <= _max.X
                && position.Y >= _min.Y && position.Y <= _max.Y;
        }
        #endregion
    }
}
