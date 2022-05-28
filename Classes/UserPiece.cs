using System;

namespace DodgeGame.Classes
{
    public class UserPiece : GamePiece
    {
        protected const int USER_DIMENTION = 75;
        protected const double LIFE_USER_MAX = 550;
        protected const int USER_SPEED = 15;

        public UserPiece(double x, double y) : base(x, y, USER_DIMENTION, USER_DIMENTION)
        {
            this.Speed = USER_SPEED;
            this.Life = LIFE_USER_MAX;
        }

        public double HPPercent
        {
            /*returns the life in percentage for the life bar*/

            get { return (this.Life / LIFE_USER_MAX) * 100; }
        }
    }
}
