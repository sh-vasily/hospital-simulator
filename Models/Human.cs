using System;
using System.Collections.Generic;
using Lab02_rpks_.Interfaces;
using System.Text;
using System.Threading.Tasks;

namespace Lab02_rpks_.Models
{
    class Human 
    {
        #region Fields
        String _Name;
        String _Surname;
        #endregion
        #region Constructors
        public Human(String Name, String Surname)
        {
            this.Name = Name;
            this.Surname = Surname;
        }
        #endregion
        #region Properties
        public String Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value ?? throw new ArgumentNullException("Name");
            }
        }

        public String Surname
        {
            get
            {
                return _Surname;
            }
            set
            {
                _Surname = value ?? throw new ArgumentNullException("Surname");
            }
        }
        #endregion
    }
}
