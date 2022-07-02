using System;
using System.Timers;

namespace Lab02_rpks_.Models
{
    #region Delegates
    delegate void FinishedAppointmentEvent(Doctor Who);
    delegate void RequiredAssistanceEvent(Doctor Who);
    #endregion
    class Doctor : Human , IDisposable
    {
        #region Events
        public event FinishedAppointmentEvent FinishedAppointment;
        public event RequiredAssistanceEvent RequiredAssistance;
        #endregion
        #region Fields
        Patient _Patient;
        Timer _WorkingTimer;
        Random _RandomSource;
        #endregion
        #region Constructors
        public Doctor(String Name, String Surname, Random RandomSource)
            : base(Name, Surname)
        {
            this._RandomSource = RandomSource;
            this._WorkingTimer = new Timer();
            _WorkingTimer.Elapsed += new ElapsedEventHandler(OnWorkingTimerElapsed);
            this._Patient = null;
        }
        #endregion
        #region Properties
        public Boolean IsWorking
        {
            get
            {
                return Patient != null;
            }
        }

        public Patient Patient
        {
            get
            {
                return _Patient;
            }
            set
            {
                _Patient = value;
            }
        }
        #endregion
        #region Event Handlers
        void OnWorkingTimerElapsed(Object O, ElapsedEventArgs EventArguments)
        {
            _WorkingTimer.Stop();
            Boolean AssistanceRequired = false;
            lock (_RandomSource)
            {
                AssistanceRequired = _RandomSource.Next(10) == 0 && Patient.DoctorsCount < 2;
            }
            if (AssistanceRequired)
            {
                RequiredAssistance(this);
            }
            else if (Patient.DoctorsCount == 1 || Patient.DoctorsCount == 2 && Patient.SecondDoctor == this)
            {
                FinishedAppointment(this);
            }
        }
        #endregion
        #region Methods
        public void StartWorkWithPatient(Patient Patient)
        {
            this.Patient = Patient;
            _Patient.AddDoctor(this);
            lock (_RandomSource)
            {
                _WorkingTimer.Interval = _RandomSource.Next(1, 11) * 250;
            }
            _WorkingTimer.Start();
        }

        public override String ToString()
        {
            return String.Format("Dr. {0} {1}", Name, Surname);
        }
        #endregion
        #region IDisposable methods
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(Boolean Disposing)
        {
            if (Disposing)
            {
                _WorkingTimer.Dispose();
            }
        }
        #endregion
    }
}
