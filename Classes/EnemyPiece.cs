using System;

namespace DodgeGame.Classes
{
    public class EnemyPiece : GamePiece
    {
        protected int serialNum;
        protected int power;
        public int SerialNum
        {
            get { return serialNum; }
            set
            {
                if (value >= 0)
                    serialNum = value;
            }
        }
        public int Power
        {
            get { return power; }
            set
            {
                if (value >= 0)
                    power = value;
            }
        }
        private Random r = new Random();
        public EnemyPiece(double x, double y, int _Recwidth, int _Recheight) : base(x, y, _Recwidth, _Recheight)
        {
            this.Life = 1;
            this.power = r.Next(5, 15);
        }
    }
}
