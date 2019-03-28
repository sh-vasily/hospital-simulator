using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Интерпретатор
{
    class MyCommand : INotifyPropertyChanged
    {
        public static uint GetCommandFromStream(System.IO.MemoryStream memoryStream)
        {
            var byteArr = new byte[4];
            memoryStream.Read(byteArr, 0, 4);
            return System.BitConverter.ToUInt32(byteArr, 0);
        }

        public static MyCommand GetCommand(System.IO.MemoryStream memoryStream) =>
            GetCommand(GetCommandFromStream(memoryStream));

        public static explicit operator uint(MyCommand myCommand)=>
          0 | (uint)myCommand.OperationCode | (uint)myCommand.FirstOperand << 5 | (uint)myCommand.SecondOperand << 14 | (uint)myCommand.ThirdOperand << 23;
      
        public static uint GetCommand(short thirdOperand = 0, short secondfOperand = 0, short firstOperand = 0, short operationCode = 0) =>
           0 | (uint)operationCode | (uint)firstOperand << 5  | (uint)secondfOperand << 14 | (uint)thirdOperand << 23;

        public static MyCommand GetCommand(uint command)=>
          new MyCommand
            {
                OperationCode = (short) (~(-1 << 5) & command),
                FirstOperand  = (short) (~(-1 << 9) & (command >> 5)),
                SecondOperand = (short) (~(-1 << 9) & (command >> 14)),
                ThirdOperand  = (short) (~(-1 << 9) & (command >> 23))
            };

        private short firstOperand;
        private short secondOperand;
        private short thirdOperand;
        private short operationCode;
        public short FirstOperand
        {
            get => firstOperand;
            set
            {
                firstOperand = value;
                OnPropertyChanged("FirstOperand");
            }
        }
        public short SecondOperand
        {
            get => secondOperand;
            set
            {
                secondOperand = value;
                OnPropertyChanged("SecondOperand");
            }
        }
        public short ThirdOperand
        {
            get => thirdOperand;
            set
            {
                thirdOperand = value;
                OnPropertyChanged("ThirdOperand");
            }
        }
        public short OperationCode
        {
            get => operationCode;
            set
            {
                operationCode = value;
                OnPropertyChanged("OperationCode");
            }
        }

        public override string ToString()
        {
            var res = "";
            var temp = System.Convert.ToString(thirdOperand, 2);
            for (int i = 0; i < 9 - temp.Length; ++i) res += "0";
            res += temp;
            temp = System.Convert.ToString(secondOperand, 2);
            for (int i = 0; i < 9 - temp.Length; ++i) res += "0";
            res += temp;
            temp = System.Convert.ToString(firstOperand, 2);
            for (int i = 0; i < 9 - temp.Length; ++i) res += "0";
            res += temp;
            temp = System.Convert.ToString(operationCode, 2);
            for (int i = 0; i < 5 - temp.Length; ++i) res += "0";
            res += temp;
            return res; 
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
