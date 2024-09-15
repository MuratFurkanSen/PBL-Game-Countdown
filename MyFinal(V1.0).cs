using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCountDown
{
    internal class Program
    {
        static void Main(string[] args)
        {
            char[,] gameBoard = new char[23, 53];
            for (int i = 1; i < gameBoard.GetLength(1); i++)
            {
                for (int j = 0; j < gameBoard.GetLength(0); j++)
                    gameBoard[j, i] = ' ';
            }

            for (int i = 0; i < gameBoard.GetLength(1); i++)
            {
                gameBoard[0, i] = '#';
                gameBoard[22, i] = '#';
            }

            for (int i = 1; i < gameBoard.GetLength(0); i++)
            {
                gameBoard[i, 0] = '#';
                gameBoard[i, 52] = '#';
            }

            AddWalls(ref gameBoard, 11, 3);
            AddWalls(ref gameBoard, 7, 5);
            AddWalls(ref gameBoard, 3, 20);

            // Adding Numbers and Player to the Board
            int randX, randY;
            int pX = 0, pY = 0;
            Random random = new Random();

            for (int i = 0; i < 71; i++)
            {
                randX = random.Next(1, 52);
                randY = random.Next(1, 22);

                if (gameBoard[randY, randX] == ' ')
                {
                    if (i < 70)
                    {
                        gameBoard[randY, randX] = Convert.ToChar(random.Next(0, 10) + 48);
                    }
                    else
                    {
                        gameBoard[randY, randX] = 'P';
                        pX = randX;
                        pY = randY;
                    }
                }
                else
                {
                    i--;
                }
            }



            int[,] posZeros = UpdateZerosPosition(gameBoard);


            int score = 0;
            int life = 5;
            int timeCount = 0;
            bool protection = false;
            Console.CursorVisible = false;
            GameBoardUpdater(gameBoard);


            DateTime playerLastTime = DateTime.Now;


            DateTime zeroLastTime = DateTime.Now;
            DateTime countDownLastTime = DateTime.Now;
            DateTime protectionLastTime = DateTime.Now;

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    if ((DateTime.Now - playerLastTime).Milliseconds >= 5)
                    {
                        playerLastTime = DateTime.Now;
                        PlayerMove(ref gameBoard, ref posZeros, ref pX, ref pY, ref score, ref life, ref protection, Console.ReadKey(true)); ;
                    }
                    else
                    {
                        Console.ReadKey(true);
                    }
                }
                if ((DateTime.Now - zeroLastTime).TotalSeconds >= 1)
                {
                    timeCount++;
                    zeroLastTime = DateTime.Now;

                    MoveZeros(ref gameBoard, ref posZeros, ref life, ref protection);
                    InfoUpdater(timeCount, life, score);
                }
                if ((DateTime.Now - countDownLastTime).TotalSeconds >= 15)
                {
                    countDownLastTime = DateTime.Now;
                    CountDown(ref gameBoard, ref posZeros);
                }
                if (!protection)
                {
                    protectionLastTime = DateTime.Now;
                }
                if ((DateTime.Now - protectionLastTime).TotalSeconds >= 2)
                {
                    protection = false;
                    protectionLastTime = DateTime.Now;
                }
            }
        }
        static void PosUpdater(ref char[,] gameBoard, char value, int x, int y)
        {
            gameBoard[y, x] = value;
            Console.SetCursorPosition(x, y);
            Console.Write(value);
        }
        static void InfoUpdater(int time = -1, int life = -1, int score = -1)
        {
            Console.Write("\x1b[32m");
            if (time != -1)
            {
                Console.SetCursorPosition(54, 0); Console.WriteLine("                    ");
                Console.SetCursorPosition(54, 0); Console.WriteLine("Time:" + time);
            }
            if (life != -1)
            {
                Console.SetCursorPosition(54, 1); Console.WriteLine("                    ");
                Console.SetCursorPosition(54, 1); Console.WriteLine("Life:" + life);
            }
            if (score != -1)
            {
                Console.SetCursorPosition(54, 2); Console.WriteLine("                    ");
                Console.SetCursorPosition(54, 2); Console.WriteLine("Score:" + score);
            }
            Console.Write("\x1b[39m");

        }
        static void GameBoardUpdater(char[,] gameBoard)
        {
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < gameBoard.GetLength(0); i++)
            {
                for (int j = 0; j < gameBoard.GetLength(1); j++)
                {
                    if (gameBoard[i, j] == '#') Console.Write("\x1b[95m" + gameBoard[i, j]);
                    else if (gameBoard[i, j] == 'P') Console.Write("\x1b[32m" + gameBoard[i, j]);
                    else if (gameBoard[i, j] == '0') Console.Write("\x1b[31m" + gameBoard[i, j]);
                    else if (gameBoard[i, j] != ' ') Console.Write("\x1b[94m" + gameBoard[i, j]);
                    else Console.Write(gameBoard[i, j]);

                }
                Console.WriteLine();
            }
        }
        static bool EmptySpaceChecker(char[,] gameBoard, int X, int Y)
        {
            if (X - 1 < 0 || Y - 1 < 0 || X + 1 > 52 || Y + 1 > 22)
                return false;
            return (gameBoard[Y - 1, X - 1] == ' ' && gameBoard[Y - 1, X] == ' ' && gameBoard[Y - 1, X + 1] == ' ' &&
                    gameBoard[Y, X - 1] == ' ' && gameBoard[Y, X] == ' ' && gameBoard[Y, X + 1] == ' ' &&
                    gameBoard[Y + 1, X - 1] == ' ' && gameBoard[Y - 1, X] == ' ' && gameBoard[Y + 1, X + 1] == ' ');
        }
        static void AddWalls(ref char[,] gameBoard, int wallLength, int wallCount)
        {
            Random random = new Random();
            for (int Count = 0; Count < wallCount; Count++)
            {
                int randX = random.Next(1, 52);
                int randY = random.Next(1, 22);
                bool flag = true;
                int down = 0, rigth = 0;
                if (random.Next(0, 2) == 0) down = 1;
                else rigth = 1;

                for (int i = 0; i < wallLength; i++)
                {
                    if (!EmptySpaceChecker(gameBoard, randX + (i * rigth), randY + (i * down)))
                    {
                        flag = false;
                        Count--;
                        break;
                    }
                }
                if (flag)
                {
                    for (int i = 0; i < wallLength; i++)
                    {
                        gameBoard[randY + (i * down), randX + (i * rigth)] = '#';
                    }
                }
            }
        }

        static int[,] UpdateZerosPosition(char[,] gameBoard)
        {
            int Count = 0;
            for (int i = 0; i < gameBoard.GetLength(0); i++)
            {
                for (int j = 0; j < gameBoard.GetLength(1); j++)
                {
                    if (gameBoard[i, j] == '0')
                        Count++;
                }
            }

            int[,] zerosPos = new int[Count, 2];
            Count = 0;
            for (int i = 0; i < gameBoard.GetLength(0); i++)
            {
                for (int j = 0; j < gameBoard.GetLength(1); j++)
                {
                    if (gameBoard[i, j] == '0')
                    {
                        zerosPos[Count, 0] = j;
                        zerosPos[Count, 1] = i;
                        Count++;
                    }
                }
            }
            return zerosPos;
        }

        static void PlayerMove(ref char[,] gameBoard, ref int[,] zeroPos, ref int pX, ref int pY, ref int score, ref int life, ref bool protection, ConsoleKeyInfo key)
        {
            int dirX = 0, dirY = 0;
            if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.W) dirY = -1;
            else if (key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.S) dirY = 1;
            else if (key.Key == ConsoleKey.LeftArrow || key.Key == ConsoleKey.A) dirX = -1;
            else if (key.Key == ConsoleKey.RightArrow || key.Key == ConsoleKey.D) dirX = 1;

            PosUpdater(ref gameBoard, ' ', pX, pY);

            if (gameBoard[pY + dirY, pX + dirX] == '0' && !protection)
            {
                Console.Beep();
                life--;
                protection = true;
                InfoUpdater(life: life);
            }

            if (gameBoard[pY + dirY, pX + dirX] == ' ')
            {
                pX += dirX;
                pY += dirY;
            }
            else if (gameBoard[pY + dirY, pX + dirX] != '#')
            {
                int min = 10;
                int borderX = 0;
                int borderY = 0;
                bool pushFlag = false;
                bool smashFlag = false;
                int tempScore;
                for (int tempX = pX + dirX, tempY = pY + dirY; gameBoard[tempY, tempX] - 48 <= min; tempX += dirX, tempY += dirY)
                {
                    char currSqr = gameBoard[tempY, tempX];
                    min = currSqr - 48;

                    if (currSqr == ' ')
                    {
                        borderX = tempX;
                        borderY = tempY;
                        pushFlag = true;
                        break;
                    }
                    if (currSqr == '#')
                    {
                        if (!(Math.Abs(pX - tempX) > 2 || Math.Abs(pY - tempY) > 2))
                        {
                            break;
                        }
                        borderX = tempX - dirX;
                        borderY = tempY - dirY;
                        smashFlag = true;
                        break;
                    }
                }

                if (pushFlag || smashFlag)
                {
                    if (smashFlag)
                    {
                        tempScore = gameBoard[borderY, borderX] - 48;
                        if (tempScore == 0) score += 20;
                        else if (tempScore < 5) score += 2;
                        else if (tempScore < 10) score += 1;
                        InfoUpdater(score: score);

                        Random random = new Random();
                        int newX, newY;
                        do
                        {
                            newY = random.Next(1, gameBoard.GetLength(0) - 1);
                            newX = random.Next(1, gameBoard.GetLength(1) - 1);
                        } while (gameBoard[newY, newX] != ' ');
                        PosUpdater(ref gameBoard, Convert.ToChar(random.Next(5, 10) + 48), newX, newY);
                    }

                    for (int tempX = borderX, tempY = borderY; (pX + dirX != tempX) || (pY + dirY != tempY); tempX -= dirX, tempY -= dirY)
                    {
                        PosUpdater(ref gameBoard, gameBoard[tempY - dirY, tempX - dirX], tempX, tempY);
                    }
                    pX += dirX;
                    pY += dirY;
                    PosUpdater(ref gameBoard, ' ', pX, pY);
                    zeroPos = UpdateZerosPosition(gameBoard);
                }
            }
            Console.Write("\x1b[32m");
            PosUpdater(ref gameBoard, 'P', pX, pY);
            Console.Write("\x1b[39m");
        }

        static void MoveZeros(ref char[,] gameBoard, ref int[,] posZeros, ref int life, ref bool protection)
        {
            for (int i = 0; i < posZeros.GetLength(0); i++)
            {
                int pos0X = posZeros[i, 0];
                int pos0Y = posZeros[i, 1];
                if (gameBoard[pos0Y - 1, pos0X] != ' ' && gameBoard[pos0Y - 1, pos0X] != 'P' &&
                    gameBoard[pos0Y + 1, pos0X] != ' ' && gameBoard[pos0Y + 1, pos0X] != 'P' &&
                    gameBoard[pos0Y, pos0X - 1] != ' ' && gameBoard[pos0Y, pos0X - 1] != 'P' &&
                    gameBoard[pos0Y, pos0X + 1] != ' ' && gameBoard[pos0Y, pos0X + 1] != 'P')
                {
                    continue;
                }
                PosUpdater(ref gameBoard, ' ', pos0X, pos0Y);
                bool moveMade = false;
                Random random = new Random();
                int dirX;
                int dirY;
                do
                {
                    dirX = 0;
                    dirY = 0;
                    switch (random.Next(1, 5))
                    {
                        case 1: dirY = -1; break;
                        case 2: dirY = 1; break;
                        case 3: dirX = -1; break;
                        case 4: dirX = 1; break;
                    }
                    if (gameBoard[pos0Y + dirY, pos0X + dirX] == ' ')
                    {
                        pos0X += dirX;
                        pos0Y += dirY;
                        moveMade = true;
                    }
                    else if (gameBoard[pos0Y + dirY, pos0X + dirX] == 'P')
                    {
                        if (!protection)
                        {
                            Console.Beep();
                            life--;
                            protection = true;
                        }
                        moveMade = true;
                    }
                } while (!moveMade);
                posZeros[i, 0] = pos0X;
                posZeros[i, 1] = pos0Y;
                Console.Write("\x1b[31m");
                PosUpdater(ref gameBoard, '0', pos0X, pos0Y);
                Console.Write("\x1b[39m");

            }
        }

        static void CountDown(ref char[,] gameBoard, ref int[,] zeroPos)
        {
            Random random = new Random();
            for (int i = 0; i < gameBoard.GetLength(0); i++)
            {
                for (int j = 0; j < gameBoard.GetLength(1); j++)
                {
                    if (49 < gameBoard[i, j] && gameBoard[i, j] < 58)
                    {
                        gameBoard[i, j] = Convert.ToChar(gameBoard[i, j] - 1);
                    }
                    else if (gameBoard[i, j] == '1')
                    {
                        if (random.Next(0, 100) < 3)
                        {
                            gameBoard[i, j] = '0';
                        }
                    }
                }
            }
            zeroPos = UpdateZerosPosition(gameBoard);
            GameBoardUpdater(gameBoard);
        }
    }
}
