using System;
using Lab02_rpks_.Models;
namespace Lab02_rpks_.Services
{
    class HumanFactory
    {
        #region Fields
        static Random _RandomSource;
        static String[] _ManNames =
        {
            "Oliver", "Jack", "Harry", "Jacob", "Charlie",
            "Thomas", "Samuel", "Oscar", "James", "Max",
            "Adam", "Theo", "Arthur", "Tony", "Freddie",
            "Lewis", "David", "Michael", "Dexter", "Elliott"
        };
        static String[] _WomanNames =
        {
            "Olivia", "Emily", "Jessica", "Isabella", "Sophie",
            "Mia", "Grace", "Scarlett", "Charlotte", "Lucy",
            "Lilly", "Matilda", "Evelyn", "Jasmine", "Annabelle",
            "Rose", "Anna", "Violet", "Madison", "Elizabeth"
        };
        static String[] _Surnames =
        {
            "Smith", "Johnson", "Williams", "Jones", "Brown",
            "Davis", "Miller", "Wilson", "Moore", "Taylor",
            "Anderson", "White", "Jackson", "Harris", "Martin",
            "Thompson", "Wood", "Lewis", "Scott", "Cooper"
        };
        #endregion
        #region Constructor
        static HumanFactory()
        {
            _RandomSource = new Random();
        }
        #endregion
        #region Methods
        public static Patient SpawnPatient()
        {
            var Name = "";
            var Surname = "";
            var IsIll = false;
            lock (_RandomSource)
            {
                var IsMan = _RandomSource.Next(2) == 0;
                if (IsMan)
                {
                    Name = _ManNames[_RandomSource.Next(_ManNames.Length)];
                    Surname = _Surnames[_RandomSource.Next(_Surnames.Length)];
                }
                else
                {
                    Name = _WomanNames[_RandomSource.Next(_WomanNames.Length)];
                    Surname = _Surnames[_RandomSource.Next(_Surnames.Length)];
                }
                IsIll = _RandomSource.Next(2) == 0;
            }
            return new Patient(Name, Surname, IsIll);
        }

        public static Doctor SpawnDoctor()
        {
            var Name = "";
            var Surname = "";
            lock (_RandomSource)
            {
                var IsMan = _RandomSource.Next(2) == 0;
                if (IsMan)
                {
                    Name = _ManNames[_RandomSource.Next(_ManNames.Length)];
                    Surname = _Surnames[_RandomSource.Next(_Surnames.Length)];
                }
                else
                {
                    Name = _WomanNames[_RandomSource.Next(_WomanNames.Length)];
                    Surname = _Surnames[_RandomSource.Next(_Surnames.Length)];
                }
            }
            return new Doctor(Name, Surname, _RandomSource);
        }
        #endregion
    }
}
