using System;

namespace DodgeGame.Classes
{
    public class GamePiece
    {
        protected double life;
        protected double x, y;
        protected double recWidth, recHeight;
        protected double speed;
        public double Speed
        {
            get { return speed; }
            set
            {
                if (value > 0)
                { speed = value; }
                else
                { speed = 2; }
            }
        }
        public double X
        {
            get { return x; }
            set { x = value; }
        }
        public double Y
        {
            get { return y; }
            set { y = value; }
        }
        public double Life
        {
            get { return life; }
            set { life = value; }
        }
        public double RecWidth
        {
            get { return recWidth; }
            set
            {
                if (value > 0)
                    recWidth = value;
            }
        }
        public double RecHeight
        {
            get { return recHeight; }
            set
            {
                if (value > 0)
                    recHeight = value;
            }
        }
        public GamePiece(double x, double y, int _Recwidth, int _Recheight)
        {
            this.X = x;
            this.Y = y;
            this.RecWidth = _Recwidth;
            this.RecHeight = _Recheight;
        }
        public double GetCenterX()
        {
            return this.x + this.recHeight / 2;
        }
        public double GetCenterY()
        {
            return this.y + this.RecWidth / 2;
        }
        public bool overlapRectangles(GamePiece gp1)
        /*check wether to rectangles overlap eachother*/

        /*I am representing 2 rectangles using two points for the rectangle is (a,b) (c,b) (a,d) (c,d))*/
        {
            if (this.life <= 0 || gp1.life <= 0)
            {
                return false;
            }
            /*                     a      b              c                        d              */
            double[] thisRec = { this.X, this.Y, this.X + this.RecWidth, this.Y + this.recHeight };
            /*                     a      b              c                        d              */
            double[] otherRec = { gp1.X, gp1.Y, gp1.X + gp1.RecWidth, gp1.Y + gp1.recHeight };

            /*Top left*/
            if (otherRec[2] >= thisRec[0] && otherRec[2] <= thisRec[2] && otherRec[3] >= thisRec[1] && otherRec[3] <= thisRec[3])
            {
                return true;
            }
            /*Top right*/
            if (otherRec[0] >= thisRec[0] && otherRec[0] <= thisRec[2] && otherRec[3] >= thisRec[1] && otherRec[3] <= thisRec[3])
            {
                return true;
            }
            /*bottom right*/
            if (otherRec[2] >= thisRec[0] && otherRec[2] <= thisRec[2] && otherRec[1] >= thisRec[1] && otherRec[1] <= thisRec[3])
            {
                return true;
            }
            /*bottom left*/
            if (otherRec[0] >= thisRec[0] && otherRec[0] <= thisRec[2] && otherRec[1] >= thisRec[1] && otherRec[1] <= thisRec[3])
            {
                return true;
            }
            return false;
        }

    }
}
