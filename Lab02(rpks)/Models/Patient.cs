using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab02_rpks_.Models
{
    class Patient : Human
    {
        #region Fields
        Boolean _IsIll;
        List<Doctor> _Doctors;
        #endregion
        #region Constructors
        public Patient(String Name, String Surname, Boolean IsIll) : base(Name, Surname)
		{
            this.IsIll = IsIll;
            _Doctors = new List<Doctor>();
        }
        #endregion
        #region Properties
        public Boolean IsIll
        {
            get
            {
                return _IsIll;
            }
            set
            {
                _IsIll = value;
            }
        }

        public Boolean IsAppointing
        {
            get
            {
                return DoctorsCount != 0;
            }
        }

        public Int32 DoctorsCount
        {
            get
            {
                return _Doctors.Count;
            }
        }

        public Doctor FirstDoctor
        {
            get
            {
                if (_Doctors.Count == 0)
                {
                    throw new ArgumentNullException("FirstDoctor");
                }
                return _Doctors[0];
            }
        }

        public Doctor SecondDoctor
        {
            get
            {
                if (_Doctors.Count != 2)
                {
                    throw new ArgumentNullException("SecondDoctor");
                }
                return _Doctors[1];
            }
        }
        #endregion
        #region Methods
        public void AddDoctor(Doctor Doc)
        {
            if (Doc == null)
            {
                throw new ArgumentNullException("Doc");
            }
            _Doctors.Add(Doc);
        }

        public override String ToString()
        {
            return String.Format("{0} {1}", Name, Surname);
        }
        #endregion
    }
}
