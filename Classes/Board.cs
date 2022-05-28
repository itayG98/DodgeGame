using System;

namespace DodgeGame.Classes
{
    public class Board
    {
        private Random r = new Random();
        public EnemyPiece[] enemis;
        public UserPiece user;
        public double width;
        public double height;
        public const int ENEMYS_COUNT = 15;
        public const double START_SPEED = 1;
        public double enemySpeed;

        public Board(double width, double height)
        {
            this.width = width;
            this.height = height;
            this.enemySpeed = START_SPEED;
            enemis = new EnemyPiece[ENEMYS_COUNT];
            user = new UserPiece(width / 2, 2 * height / 3);
            for (int i = 0; i < enemis.Length; i++)
            {
                enemis[i] = new EnemyPiece(r.Next(0, (int)width), r.Next(-(int)height / 4, (int)height / 4), r.Next(35, 75), r.Next(35, 75));
                enemis[i].SerialNum = i;
                enemis[i].Speed = enemySpeed;
            }
        }
        public Board(double width, double height, double _XUser, double _YUser, double currentEnemySpeed)
        {
            /*Constructor with current position for restart game method*/

            this.width = width;
            this.height = height;
            this.enemySpeed = currentEnemySpeed;
            enemis = new EnemyPiece[ENEMYS_COUNT];
            user = new UserPiece(_XUser, _YUser);
            for (int i = 0; i < enemis.Length; i++)
            {
                /*Two enemis will not be in the same X position {'i/Width X'} */

                enemis[i] = new EnemyPiece(r.Next((int)(i * width / ENEMYS_COUNT), (int)((i + 1) * width / ENEMYS_COUNT)), r.Next(-(int)height / 4, (int)height / 4), r.Next(35, 75), r.Next(35, 75));
                enemis[i].SerialNum = i;
                enemis[i].Speed = enemySpeed;
            }
        }

        public bool IsWinner()
        {
            /*Check if won*/

            int aliveEnemis = 0;
            if (user.Life <= 0)
            {
                return false;
            }
            for (int i = 0; i < ENEMYS_COUNT; i++)
            {
                if (enemis[i].Life > 0)
                {
                    aliveEnemis++;
                }
            }
            return aliveEnemis < 2;
        }

        public void userMove(string direction)
        {
            if (user.Life > 0)
            {
                direction = direction.ToLower();
                switch (direction)
                {
                    case "up":
                        {
                            if (user.Y < 0)
                                user.Y = height - user.RecHeight;
                            else
                                user.Y -= user.Speed;
                            break;
                        }
                    case "down":
                        {
                            if (user.Y >= height - user.RecHeight)
                                user.Y = 0;
                            else
                                user.Y += user.Speed;
                            break;
                        }
                    case "right":
                        {
                            if (user.X + user.RecWidth > width)
                                user.X = 0;
                            else
                                user.X += user.Speed;
                            break;
                        }
                    case "left":
                        {
                            if (user.X < 0)
                                user.X = width - user.RecWidth;
                            else
                                user.X -= user.Speed;
                            break;
                        }
                }
            }
        }

        public void enemyMove(EnemyPiece enem)
        {
            if (enem.Life >= 0)
            {
                if (enem.GetCenterX() > user.GetCenterX() - enem.RecWidth / 2)
                {
                    enem.X -= enem.Speed;
                }
                else if (enem.GetCenterX() < user.GetCenterX() + enem.RecWidth / 2)
                {
                    enem.X += enem.Speed;
                }
                if (enem.GetCenterY() > user.GetCenterY() - enem.RecHeight / 2)
                {
                    enem.Y -= enem.Speed;
                }
                else if (enem.GetCenterY() < user.GetCenterY() + enem.RecHeight / 2)
                {
                    enem.Y += enem.Speed;
                }
            }
        }
        public void userClashed()
        {
            for (int i = 0; i < enemis.Length; i++)
            {
                if (enemis[i].Life > 0 && enemis[i].overlapRectangles(user))
                {
                    user.Life -= enemis[i].Power;
                }
            }
        }
        public bool enemisClash(EnemyPiece enem)
        {
            if (enem.Life < 0)
            {
                return false;
            }
            for (int i = 0; i < enemis.Length; i++)
            {
                if (i != enem.SerialNum && enemis[i].Life > 0 && enem.overlapRectangles(enemis[i]) && !(enemis[i].overlapRectangles(user) || (enem.overlapRectangles(user))))
                {
                    enem.Life -= enemis[i].Power;
                    return true;
                }
            }
            return false;
        }
    }
}





