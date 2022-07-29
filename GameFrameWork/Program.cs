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
        //public double sec_check = 0; //game frame work 내에 멤버변수 생성하여 일처리
        //public int sec = 0;
        double playTime = 0.0;
        int s, m, h = 0;
        bool Pause = true;//토글방식 
        public void Start()//반복전에 처음 시작할 때만 하는 초기작업.
        {
            //Console.WriteLine("게임을 시작 합니다.");
        }
        void InputProcess()
        {
            if (Console.KeyAvailable)//key를 누르지 않으면 false
            {//key입력이 들어왔을 때만 해당하게 안해주면 응답 대기상태에 빠져버림.
                ConsoleKey pressedKey = Console.ReadKey(true).Key;
                //사용한 키를 제거하지 않게 하기위해
                /*밑의 Console.ReadKey(true).Key 방식은 queue에서 해당값을 빼서 다음에 사용못함.
                if (Console.ReadKey(true).Key == ConsoleKey.Escape)//key값을 가지고 어떤 키를 입력받았는지 확인
                {//readkey default 값 false >> 입력한 키 알려줌.
                 //esc 키를 눌렀으면 false
                    IsPlaying = false;
                }
                else if(Console.ReadKey(true).Key == ConsoleKey.Spacebar)
                {

                }
                */
                switch (pressedKey)
                {
                    case ConsoleKey.Escape:
                        IsPlaying = false;
                        break;
                    case ConsoleKey.Spacebar:
                        Pause = !Pause;
                        break;
                    case ConsoleKey.Enter:
                        Pause = true;
                        h =  m = s = 0;// 우 > 좌 순서로 0 초기화.
                        break;
                }
            }            
        }
        public void Update()//매 프레임마다 반복해야하는 일.
        {//1.입력을 받음
            InputProcess();
            //Console.WriteLine(DateTime.Now.Ticks);//Datetime.Now : 현재시간
            //Ticks : 컴퓨터 내의 1/천만 의 초단위인 clock을 측정
            // 2.입력받은 값을 처리/계산
            //컴퓨터의 사양마다 다른 진행을 없애기 위해
            //*Time.deltaTime 을 해주면 프레임 차이가 나도 초당 게임의 진행이 같다.
            //프레임 간격시간을 곱해주는 순간 초당 행동이 되는 것이다.
            //ex) playTime += 3.0f * Time.deltaTime;
            //매초마다 1초 2초 3초가 표시되게하기
            //누적된 시간을 저장하는 변수 필요.
            //선생님코드
            if (!Pause)//Pause false일경우만 실행.
            {
                playTime += Time.deltaTime;
                //playTime *= 2;
                //Console.SetCursorPosition(0, 1); //(행, 열)해당 위치로 이동해서 출력
                //Console.Write(h.ToString("00:"));
                //Console.Write(m.ToString("00:"));
                //Console.Write((s).ToString("00\n"));

                if (playTime >= 1.0)
                {
                    //Console.Clear();
                    playTime -= 0.0f;//오차를 줄이기위해 -, 0으로 초기화시 자투리시간만큼 오차가생김
                    ++s;
                    if (s > 59)
                    {
                        s = 0;
                        ++m;
                    }
                    if (m > 59)
                    {
                        m = 0;
                        ++h;
                    }
                    //Console.WriteLine(s.ToString("00"));//"00"형태로 출력하라는 포맷 제공
                }
            }
            //sec_check += Time.deltaTime;
            /*
            if (sec_check > 1)
            {
                ++sec;
                sec_check = 0;
                Console.WriteLine($"{sec}초");
            }
            */
        //3.계산결과 그림
            Draw();
            //위의 1,2,3 논리 순서 중요!! 
        }
        void Draw()//07.29 ++
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine($"{h.ToString("00")} : {m.ToString("00")} : {s.ToString("00")}");
        }
        public void Destory()//종료하기 전에 해야하는 일
        {         //ex)서버에 저장.
            Console.WriteLine("게임을 종료 합니다.");
        }
    }
    class Program
    {
        static void Main(string[] args)
        {//Game Frame Work
         //Frame Work : 연속된 그림(Frame)을 보여줘 움직이는 것처럼 보이는 것.
         //ex)fps(Frame Per Secend): 1초에 몇장의 그림이 바뀌는가.
         //게임은 실시간으로 그림을 그리며 연속적으로 보여준다.
         //일반적으로 이벤트가 일어났을 때 동작하도록 Frame Work를 만든다.
         //하지만 게임은 계속 돌아가야한다.유저가 아무행동도 안하거나, 인터넷이 끊겼더라도.
            //실습
            //타이머를 만들자
            //스페이스바를 누르면 시간이 증가하고 다시 스페이스바를 누르면 멈춘다. 엔터를 누르면 초기화.
            // 00:00:00 시:분:초
            //1 frame work 시간을 재야함.
            GameFrameWork gfw = new GameFrameWork();
            long startTick = DateTime.Now.Ticks;
            
            gfw.Start();
            while (gfw.IsPlaying)//매우 간단한 Frame work
            {
                long deltaTick = DateTime.Now.Ticks - startTick;
                //deltaTick: Update가 1frame도는데 걸린 clock수 > 천만이 되면 1초. 
                startTick = DateTime.Now.Ticks;
                Time.deltaTime = deltaTick / 10000000.0;//1.0을 1초로 변환해 저장.
                //Frame Work
                
                gfw.Update();
                Thread.Sleep(1);// 1/1000초 지연
            }            
            gfw.Destory();
        }
    }
}
