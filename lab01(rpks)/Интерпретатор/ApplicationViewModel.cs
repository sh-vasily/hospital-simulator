using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Collections.Generic;


namespace Интерпретатор
{
    class ApplicationViewModel : INotifyPropertyChanged
    {
        private string selectedOperation = "~x";
        public string SelectedOperation
        {
            get => selectedOperation;
            set
            {
                selectedOperation = value;
                OnPropertyChanged("SelectedOperation");
            }
        }


        private List<string> operations = new List<string>(new string[] 
        { "z in base (x)=","~x","x|y","x&y","x^y","x->y","~(x->y)","x<->y",
          "↓","~(x&y)","x+y","x-y","x*y","x/y","x%y","swap(x,y)",
          "16","17","18","19","x<<y","x>>y","x<<<y", "x>>>y","x:=y"});

        public List<string> Operations
        {
            get => operations;
            set
            {
                operations = value;
                OnPropertyChanged("Operations");
            }
        }

       
        MyCommand command = new MyCommand
        {
            FirstOperand = 0,
            SecondOperand = 0,
            ThirdOperand = 0,
            OperationCode = 1
        };
        public MyCommand Command
        {
            get => command;
            set
            {
                Command.SecondOperand = 0;
                command = value;
                command.OperationCode = (short)Operations.IndexOf(SelectedOperation);
                OnPropertyChanged("Command");
            }
        }
        public uint BinaryCommand;
        public RelayCommand ExecuteCommand =>
            new RelayCommand(() =>
            {
                BinaryCommand = (uint)Command;
                var buffer = System.BitConverter.GetBytes(BinaryCommand);
                var stream = new System.IO.MemoryStream(buffer);
                BinaryCommand = Interpretator.Execute(stream);
                Command = MyCommand.GetCommand(BinaryCommand);
             });
        public RelayCommand SaveToFileCommand =>
            new RelayCommand(() =>
            {
                using (var fs = new System.IO.StreamWriter("output.txt"))
                {
                    fs.WriteLine(Command);
                }
            });


        public ApplicationViewModel()
        {
            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
