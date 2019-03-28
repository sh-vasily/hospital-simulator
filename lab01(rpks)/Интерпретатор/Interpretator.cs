using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace Интерпретатор
{
    class Interpretator
    {
       public static UInt32 Execute(MemoryStream memoryStream)
        {
            var Command = MyCommand.GetCommand(memoryStream);

            var Operations = new Action<MyCommand>[]
            {
                (command)=> //0
                {
                    MessageBox.Show(OvertedGorner(command.ThirdOperand,command.FirstOperand));
                },
                (command)=> //1
                {
                    command.ThirdOperand = (short) ~ command.FirstOperand;
                },
                (command)=>//2
                {
                    command.ThirdOperand = (short)(command.FirstOperand | command.SecondOperand);
                },
                (command)=>//3
                {
                    command.ThirdOperand = (short)(command.FirstOperand & command.SecondOperand);
                },
                (command)=>//4
                {
                    command.ThirdOperand = (short)(command.FirstOperand ^ command.SecondOperand);
                },
                (command)=>//5
                {
                    command.ThirdOperand = (short)(~command.FirstOperand | command.SecondOperand);
                },
                (command)=>//6
                {
                    command.ThirdOperand = (short)~(~command.FirstOperand | command.SecondOperand);
                },
                (command)=>//7
                {
                    command.ThirdOperand =(short)((~command.FirstOperand | Command.SecondOperand)&(~Command.SecondOperand | Command.FirstOperand));
                },
                (command) =>//8
                {
                    command.ThirdOperand = (short)~(command.FirstOperand|command.SecondOperand);
                },
                (command)=> //9
                {
                    command.ThirdOperand= (short)~(command.FirstOperand&command.SecondOperand);
                },
                (command)=> //10
                {
                    command.ThirdOperand=(short)(command.FirstOperand+command.SecondOperand);
                },
                (command)=>//11
                {
                    command.ThirdOperand=(short)(command.FirstOperand-command.SecondOperand);
                },
                (command)=>//12
                {
                    command.ThirdOperand =(short)(command.FirstOperand*command.SecondOperand);
                },
                (command)=>//13
                {
                    command.ThirdOperand = (short)(command.FirstOperand/command.SecondOperand);
                },
                (command)=>//14
                {
                    command.ThirdOperand = (short)(command.FirstOperand % command.SecondOperand);
                },
                (command)=>//15
                {
                    command.FirstOperand  ^= command.SecondOperand;
                    command.SecondOperand ^= command.FirstOperand;
                    command.FirstOperand  ^= command.SecondOperand;
                },

            };

            Operations[Command.OperationCode](Command);

            return (uint)Command;
        }

        static string OvertedGorner(int num, int osn)
        {
            string res = "";
            while (num != 0)
            {
                if (osn < 10) res += num % osn;
                else res += Convert.ToChar(num % osn + 55);
                num /= osn;
            }

            return new string(res.Reverse().ToArray());
        }


    }
}
