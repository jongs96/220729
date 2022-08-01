using System;
using System.Collections.Generic;
using System.Threading;

namespace GameFrameWork
{
    //유한상태기계(FSM)
    //상태에 따라 동작하도록 하는 것
    //상태는 중복되지 않아야 한다. ex)normal, battle의 상태가 중복될 수 없음.
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

        enum ITEM
        {
            //빗자루 - 1몇초마다 숫자 하나씩(뒤에서부터) 자동으로 제거(5초동안)
            //청소기 - 숫자 올클리어
            //또또또 - 같은 숫자만 여러개나오기 (5개)
            //보약 - 라이프 증가(+1)
            //피버 - 점수2배 (5초)
            //타임머신 레벨 다운 (속도도 같이 내려가기)
            //달팽이 - 5초간 속도 감소
            NONE, BROOM, CLEANER, TOTO, MEDI, FEVER, TIMEMACHINE, SNAIL
        }

        ITEM[] itemSlot = new ITEM[3] { ITEM.NONE, ITEM.NONE, ITEM.NONE };

        STATE myState = STATE.CTEATE;// 생성되는 상태로 초기화.

        public bool IsPlaying = true;
        double playTime = 0.0f;
        List<int> numList = new List<int>();
        static Random myRandom = new Random();
        bool reDraw = true;
        int score = 0;
        int LevelCount = 0, Level =1;
        int ItemCount = 0;
        double speed = 2.0;
        int Life = 3;
        double itemTime = 0.0;//item유지시간
        double ItemPlayTime = 0;//playtime처럼 itemtime check 변수.
        ITEM usedItem = ITEM.NONE;
        
        void ChangeState(STATE s)//함수를 통해서만 상태를 바꿔줘야함
        {
            if (myState == s) return;
            myState = s;
            switch(myState)//각각의 상태가 변환되면서 해야할 일 분기문처리.
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

                    if (usedItem != ITEM.NONE)
                    {
                        itemTime -= Time.deltaTime;
                        ItemPlayTime += Time.deltaTime;

                        Console.SetCursorPosition(25, 1);
                        Console.Write($"{itemTime.ToString("00.00")}");

                        if (itemTime <= 0.0f)
                        {
                            usedItem = ITEM.NONE;
                            reDraw = true;
                        }
                        else
                        {
                            switch(usedItem)
                            {
                                case ITEM.BROOM://1초가 지났는지를 셀수있는 변수로 지날때마다 numlist.count -1 숫자제거
                                    if (ItemPlayTime >= 1.0f)
                                    {
                                        ItemPlayTime -= 1;
                                        if (numList.Count > 0) numList.RemoveAt(numList.Count - 1);
                                    }                                    
                                    break;
                            }
                        }
                    }

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
                    case ConsoleKey.Q:
                        UseItem(0);
                        break;
                    case ConsoleKey.W:
                        UseItem(1);
                        break;
                    case ConsoleKey.E:
                        UseItem(2);
                        break;

                    default:
                        if (numList.Count > 0)//list내부에 아무것도 없을경우 실행하는 에러방지
                        {
                            int num = pressedKey - ConsoleKey.NumPad0;
                            if (num == numList[0])
                            {
                                reDraw = true;
                                numList.RemoveAt(0);
                                score += 10;
                                if (++LevelCount == 3)
                                {
                                    LevelCount = 0;
                                    LevelUp();
                                }
                                if(++ItemCount==3)
                                {
                                    ItemCount = 0;
                                    AddItem();
                                }
                            }
                            else
                            {
                                if (num > 0 && num < 6) 
                                {
                                    if (--Life == 0)//1~5사이의 값 잘못입력한경우
                                    {
                                        ChangeState(STATE.GAMEOVER);
                                    }
                                    else
                                    {
                                        reDraw = true;
                                    }
                                }
                                
                            }
                        }
                        
                        break;
                        
                }
            }
        }
        void UseItem(int i)// i = Slot number
        {
            usedItem = itemSlot[i];
            itemSlot[i] = ITEM.NONE;
            switch(usedItem)
            {
                case ITEM.NONE:
                    break;
                case ITEM.BROOM:
                    itemTime = 5.0;
                    break;
                case ITEM.CLEANER:
                    itemTime = 1.0;
                    numList.Clear();
                    break;
            }
            ItemPlayTime = 0.0f;
            reDraw = true;
        }
        void AddItem()
        {
            ITEM item = ITEM.CLEANER;
            for(int i = 0; i < itemSlot.Length; ++i)
            {
                if(itemSlot[i] == ITEM.NONE)
                {
                    itemSlot[i] = item;
                    reDraw = true;
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
            Console.SetCursorPosition(10, 0);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Score:{score.ToString("00000")}");
            Console.ForegroundColor = old;
            Console.SetCursorPosition(25, 1);
            Console.WriteLine($"Life :{Life}");

            Console.SetCursorPosition(0, 1);
            if (usedItem != ITEM.NONE)
            {
                Console.WriteLine($"[{GetItemName(usedItem)}]");
            }
            else
            {
                Console.Write("                                          ");
            }

            Console.SetCursorPosition(0, 3);
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
            Console.WriteLine();

            Console.SetCursorPosition(0, 5);
            Console.Write("                                      ");// 잔상제거
            Console.SetCursorPosition(0, 5);
            foreach (ITEM item in itemSlot)
            {
                Console.Write($"[{GetItemName(item)}]");
            }

            reDraw = false;
        }
        string GetItemName(ITEM item)
        {
            string name = "";
            switch(item)
            {
                case ITEM.NONE:
                    name = "없음";
                    break;
                case ITEM.BROOM:
                    name = "빗자루";
                    break;
                case ITEM.CLEANER:
                    name = "청소기";
                    break;
            }
            return name;
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
            
            //아이템은 20개를 지울때마다 얻을 수 있다.
            //아이템은 3개까지 모을 수 있다. ( 3슬롯 )
            //Q,W,E 키로 해당 슬롯의 아이템을 사용할 수있다.

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
