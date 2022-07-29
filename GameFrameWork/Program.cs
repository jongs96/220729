using System;
using System.Collections.Generic;
using System.Threading;

namespace GameFrameWork
{
    //유한상태기계(FSM)
    //상태에 따라 동작하도록 하는 것
    //상태는 중복되지 않아야 한다. ex)normal, battle의 상태가 중복될 수 없음.
    //
    class Time
    {
        public static double deltaTime;
    }
    class GameFrameWork
    {
        enum STATE//일반적으로 상태는 열겨형으로 표현.
        {
            CTEATE, START, PLAY, GAMEOVER
        }

        STATE myState = STATE.CTEATE;// 생성되는 상태로 초기화.

        public bool IsPlaying = true;
        double playTime = 0.0f;
        List<int> numList = new List<int>();
        static Random myRandom = new Random();
        bool reDraw = true;
        int score = 0;
        int LevelCount = 0, Level =1;
        double speed = 2.0;
        int Life = 3;
        
        void ChangeState(STATE s)//함수를 통해서만 상태를 바꿔줘야함
        {
            if (myState == s) return;
            myState = s;
            switch(myState)//각각의 상태에서 해야할 일 분기문처리.
            {
                case STATE.CTEATE:
                    break;
                case STATE.START:
                    Console.Clear();

                    score = 0;
                    Level = 1;
                    LevelCount = 0;
                    speed = 2.0;
                    playTime = 0.0f;
                    Life = 3;
                    numList.Clear();

                    Console.WriteLine("숫자 디펜스");
                    Console.WriteLine("Press anykey to start");
                    break;
                case STATE.PLAY:
                    Console.Clear();//Play상태로 변환되면 이전글자들 삭제
                    reDraw = true;
                    break;
                case STATE.GAMEOVER:
                    Console.Clear();
                    GameOver();
                    break;
            }
        }
        void StateProcess()//매 프레임마다해야할 일.
        {//항상 호출될 수 있도록 Update문에 넣어줘야함.
            switch (myState)
            {
                case STATE.CTEATE:
                    break;
                case STATE.START:
                    if(Console.KeyAvailable)//아무키나 누르면
                    {
                        ChangeState(STATE.PLAY);//play 상태로 변환.
                    }
                    break;
                case STATE.PLAY://play상태에서만 게임 플레이가 진행이 되는것
                    PlayInputProcess();
                    playTime += Time.deltaTime;

                    if (playTime >= speed)
                    {
                        playTime = 0.0f;
                        if (numList.Count == 10)
                        {
                            //Game over
                            ChangeState(STATE.GAMEOVER);
                        }
                        else
                        {
                            numList.Add(myRandom.Next(1, 6));
                            reDraw = true;
                        }
                    }
                    if(Life == 0)
                    {
                        ChangeState(STATE.GAMEOVER);
                    }
                    Draw();
                    break;
                case STATE.GAMEOVER:
                    if(Console.KeyAvailable)
                    {
                        ConsoleKey pressedKey = Console.ReadKey(true).Key;
                        switch(pressedKey)
                        {
                            case ConsoleKey.Y:
                                ChangeState(STATE.START);
                                break;
                            case ConsoleKey.N:
                                IsPlaying = false;
                                break;
                        }
                    }
                    break;
            }
        }
        public void Start()
        {
            Console.CursorVisible = false;
            //Console.WriteLine("게임을 시작합니다.");
            ChangeState(STATE.START);
        }
        void PlayInputProcess()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKey pressedKey = Console.ReadKey(true).Key;
                switch (pressedKey)
                {
                    case ConsoleKey.Escape:
                        IsPlaying = false;
                        break;
                    
                    default:
                        if (numList.Count > 0)//list내부에 아무것도 없을경우 실행하는 에러방지
                        {
                            int num = pressedKey - ConsoleKey.NumPad0;
                            if (num == numList[0])
                            {
                                numList.RemoveAt(0);
                                reDraw = true;
                                score += 10;
                                if (++LevelCount == 3)
                                {
                                    LevelCount = 0;
                                    LevelUp();
                                }
                            }
                            else
                            {
                                Life--;
                                reDraw = true;
                            }
                        }
                        
                        break;
                        
                }
            }
        }
        void LevelUp()
        {
            Level++;
            speed *= 0.9;
        }
        public void Update()
        {
            StateProcess();
        }
        void GameOver()
        {
            Console.Clear();
            Console.WriteLine("Game Over!");
            Console.WriteLine($"Your Score:{score}");
            Console.WriteLine("Retry? y/n");
        }
        void Draw()
        {
            if (!reDraw) return;
            ConsoleColor old = Console.ForegroundColor;
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Level: {Level}");
            Console.SetCursorPosition(20, 0);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Score:{score.ToString("00000")}");
            Console.ForegroundColor = old;
            Console.SetCursorPosition(20, 1);
            Console.WriteLine($"Life :{Life}");
            Console.SetCursorPosition(0, 1);
            for (int i = numList.Count - 1; i >= 0; --i)
            {
                Console.Write(numList[i]);
            }
            old = Console.ForegroundColor;  //이전색 old에 저장
            Console.ForegroundColor = ConsoleColor.Red;//색변환
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
            //Console.WriteLine("게임을 종료합니다.");
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
            //숫자가 10를 초과하면 게임오버
            //숫자를 지울 때마다 점수를 얻는다.(10점 추가)
            //15개의 숫자를 지울때 마다 레벨이 증가한다.
            //레벨이 증가하면 숫자가 추가 되는 속도가 10퍼센트 빨라진다.
            //라이프(초기값 3) - 잘못된 번호를 누르면 줄어든다. 라이프가 0이 되면 게임오버.

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
