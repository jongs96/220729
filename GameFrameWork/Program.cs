using System;
using System.Collections.Generic;
using System.Threading;

namespace GameFrameWork
{
    class Time
    {
        public static double deltaTime;
    }
    class GameFrameWork
    {        
        public bool IsPlaying = true;
        double playTime = 0.0f;
        List<int> numList = new List<int>();
        static Random myRandom = new Random();
        bool reDraw = true;
        int score = 0;
        
        public void Start()
        {
            Console.CursorVisible = false;
            //Console.WriteLine("게임을 시작합니다.");
        }
        void InputProcess()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKey pressedKey = Console.ReadKey(true).Key;
                switch (pressedKey)
                {
                    case ConsoleKey.Escape:
                        IsPlaying = false;
                        break;
                    /*
                    case ConsoleKey.NumPad1:
                        if (numList[0] == 1)
                        {
                            numList.RemoveAt(0);
                            reDraw = true;
                        }
                        break;
                    case ConsoleKey.NumPad2:
                        if (numList[0] == 2)
                        {
                            numList.RemoveAt(0);
                            reDraw = true;
                        }
                        break;
                    case ConsoleKey.NumPad3:
                        if (numList[0] == 3)
                        {
                            numList.RemoveAt(0);
                            reDraw = true;
                        }
                        break;
                    case ConsoleKey.NumPad4:
                        if (numList[0] == 1)
                        {
                            numList.RemoveAt(4);
                            reDraw = true;
                        }
                        break;
                    case ConsoleKey.NumPad5:
                        if (numList[0] == 5)
                        {
                            numList.RemoveAt(0);
                            reDraw = true;
                        }
                        break;
                    */
                    default:
                        int num = pressedKey - ConsoleKey.NumPad0;
                        if(num == numList[0])
                        {
                            numList.RemoveAt(0);
                            reDraw = true;
                        }
                        break;
                }
            }
        }
        public void Update()
        {
            InputProcess();
            
            playTime += Time.deltaTime;
            
            if (playTime >= 2 )
            {
                playTime = 0.0f;
                numList.Add(myRandom.Next(1,6));
                reDraw = true;
            }

            if(numList.Count >=10)
            {
                IsPlaying = false;
            }

            Draw();//최적화 : 화면에 그려야하는 상황에만 그리도록 하는 것이 좋다.
        }
        void Draw()
        {
            if (!reDraw) return;
            Console.SetCursorPosition(0, 0);
            for(int i = numList.Count - 1; i >= 0; --i)
            {
                Console.Write(numList[i]);
            }
            ConsoleColor old = Console.ForegroundColor;  //이전색 old에 저장
            Console.ForegroundColor = ConsoleColor.Yellow;//색변환
            for(int i =0; i < 10 - numList.Count; ++i)
            {
                Console.Write("*");
            }
            Console.ForegroundColor = old;
            Console.WriteLine();
            reDraw = false;
        }
        public void Destory()
        {
            Console.WriteLine("게임을 종료합니다.");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            //숫자디펜스 게임. 
            //일정시간(2초)마다 랜덤한(1~5) 숫자가 추가된다.
            //=>시간을 재는 변수, 랜덤숫자 저장 동적배열
            //가장 먼저 추가된 숫자를 눌러서 지운다. - 키패드 숫자이용.
            //10개이상의 숫자가 쌓이면 게임오버
            //숫자를 지울 때마다 점수를 얻는다.(10점 추가)
            //15개의 숫자를 지울때 마다 레벨이 증가한다.
            //레벨이 증가하면 숫자가 추가 되는 속도가 10퍼센트 빨라진다.

            GameFrameWork gfw = new GameFrameWork();
            long startTick = DateTime.Now.Ticks;
            gfw.Start();
            while (gfw.IsPlaying)
            {
                long deltaTick = DateTime.Now.Ticks - startTick;
                startTick = DateTime.Now.Ticks;
                Time.deltaTime = deltaTick / 10000000.0;
                //Frame Work
                gfw.Update();
                Thread.Sleep(1);
            }            
            gfw.Destory();
        }
    }
}
