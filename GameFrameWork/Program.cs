using System;
using System.Collections.Generic;

namespace GameFrameWork
{
    class GameFrameWork
    {
        public bool IsPlaying = true;
        public void Start()//반복전에 처음 시작할 때만 하는 초기작업.
        {
            Console.WriteLine("게임을 시작 합니다.");
        }
        void InputProcess()
        {
            if (Console.KeyAvailable)//key를 누르지 않으면 false
            {//key입력이 들어왔을 때만 해당하게 안해주면 응답 대기상태에 빠져버림.
                if (Console.ReadKey(true).Key == ConsoleKey.Escape)//key값을 가지고 어떤 키를 입력받았는지 확인
                {//readkey default 값 false >> 입력한 키 알려줌.
                    //esc 키를 눌렀으면 false
                    IsPlaying = false;
                }
            }
        }
        public void Update()//매 프레임마다 반복해야하는 일.
        {
            InputProcess();//1.입력을 받음            
            Console.WriteLine(DateTime.Now.Ticks);//Datetime.Now : 현재시간
            //Ticks : 컴퓨터 내의 1/천만 의 초단위인 clock을 측정
            // 2.입력받은 값을 처리/계산
            Draw();//3.계산결과 그림
            //위의 1,2,3 논리 순서 매우중요!! 
        }
        void Draw()
        {

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

            //타이머를 만들자
            //스페이스바를 누르면 시간이 증가하고 다시 스페이스바를 누르면 멈춘다.
            //1 frame work 시간을 재야함.
            GameFrameWork gfw = new GameFrameWork();
            long startTick = DateTime.Now.Ticks;

            gfw.Start();
            while (gfw.IsPlaying)//매우 간단한 Frame work
            {
                long deltaTick = DateTime.Now.Ticks - startTick;
                startTick = DateTime.Now.Ticks;
                //Frame Work
                gfw.Update();
            }            
            gfw.Destory();
        }
    }
}
