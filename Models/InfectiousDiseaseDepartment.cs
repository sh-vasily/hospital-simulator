using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading;

using Lab02_rpks_.Services;

namespace Lab02_rpks_.Models
{
    #region Delegates
    delegate void QueueChangedEvent(List<Patient> Queue);
    delegate void LookoutChangedEvent(List<Patient> Lookout);
    public delegate void HistoryUpdatedEvent(String LastAction);
    #endregion
    class InfectiousDiseaseDepartment : IDisposable
    {
        #region Events
        public event QueueChangedEvent QueueChanged;
        public event LookoutChangedEvent LookoutChanged;
        public event HistoryUpdatedEvent HistoryUpdated;
        #endregion
        #region Fields
        Random _RandomSource;
        UInt32 _LookoutCapacity;
        UInt32 _PatientsLeftCount;
        List<Patient> _Lookout;
        List<Patient> _Queue;
        Doctor[] _Doctors;
        #region Timers
        System.Timers.Timer _EnqueueTimer;
        System.Timers.Timer _AddToLookoutTimer;
        System.Timers.Timer _InfectQueueTimer;
        #endregion
        #region Mutexes
        Mutex _QueueLocker;
        Mutex _LookoutLocker;
        Mutex _HistoryLocker;
        Mutex _RandomSourceLocker;
        #endregion
        #endregion
        #region Constructor
        public InfectiousDiseaseDepartment(UInt32 LookoutCapacity, UInt32 DoctorsCount, UInt32 TotalCountOfPacients, UInt32 TimeSpeed)
        {
            this._RandomSource = new Random();
            this.LookoutCapacity = LookoutCapacity;
            this.PatientsLeftCount = TotalCountOfPacients;
            this._Lookout = new List<Patient>();
            this._Queue = new List<Patient>();
            this._Doctors = new Doctor[DoctorsCount];
            for (var Index = 0; Index < DoctorsCount; Index++)
            {
                _Doctors[Index] = HumanFactory.SpawnDoctor();
                _Doctors[Index].FinishedAppointment += OnDoctorFinishedAppointment;
                _Doctors[Index].RequiredAssistance += OnDoctorRequiresAssistance;
            }
            #region Initializing timers
            _EnqueueTimer = new System.Timers.Timer(TimeSpeed);
            _EnqueueTimer.Elapsed += new ElapsedEventHandler(OnEnqueueTimerElapsed);
            _AddToLookoutTimer = new System.Timers.Timer(TimeSpeed);
            _AddToLookoutTimer.Elapsed += new ElapsedEventHandler(OnAddToLookoutTimerElapsed);
            _InfectQueueTimer = new System.Timers.Timer(TimeSpeed);
            _InfectQueueTimer.Elapsed += new ElapsedEventHandler(OnInfectQueueTimerElapsed);
            #endregion
            #region Initializing mutexes
            _QueueLocker = new Mutex();
            _LookoutLocker = new Mutex();
            _HistoryLocker = new Mutex();
            _RandomSourceLocker = new Mutex();
            #endregion
        }
        #endregion
        #region Properties
        public UInt32 LookoutCapacity
        {
            get
            {
                return _LookoutCapacity;
            }
            private set
            {
                _LookoutCapacity = value;
            }
        }

        public UInt32 PatientsLeftCount
        {
            get
            {
                return _PatientsLeftCount;
            }
            private set
            {
                _PatientsLeftCount = value;
            }
        }

        public UInt32 DoctorsCount
        {
            get
            {
                return (UInt32)_Doctors.Length;
            }
        }
        #endregion
        #region Event handlers
        void OnEnqueueTimerElapsed(Object O, ElapsedEventArgs EventArguments)
        {
            if (PatientsLeftCount == 0)
            {
                _EnqueueTimer.Elapsed -= new ElapsedEventHandler(OnEnqueueTimerElapsed);
                return;
            }
            _RandomSourceLocker.WaitOne();
            var AddingPatient = _RandomSource.Next(10) < 4;
            _RandomSourceLocker.ReleaseMutex();
            if (AddingPatient)
            {
                var NewPatientInQueue = HumanFactory.SpawnPatient();
                PatientsLeftCount--;
                _QueueLocker.WaitOne();
                _Queue.Add(NewPatientInQueue);
                QueueChanged(_Queue);
                _QueueLocker.ReleaseMutex();
                _HistoryLocker.WaitOne();
                HistoryUpdated($"Patien {NewPatientInQueue} got in queue.");
                _HistoryLocker.ReleaseMutex();
                Thread.Sleep(10);
            }
        }

        void OnAddToLookoutTimerElapsed(Object O, ElapsedEventArgs EventArguments)
        {
            _QueueLocker.WaitOne();
            if (_Queue.Count == 0)
            {
                if (PatientsLeftCount == 0)
                {
                    _AddToLookoutTimer.Elapsed -= new ElapsedEventHandler(OnAddToLookoutTimerElapsed);
                }
                _QueueLocker.ReleaseMutex();
                return;
            }
            _LookoutLocker.WaitOne();
            if (_Lookout.Count == 0 || _Lookout.Count < _LookoutCapacity && _Lookout[0].IsIll == _Queue[0].IsIll)
            {
                var NewPatientInLookout = _Queue[0];
                _Lookout.Add(NewPatientInLookout);
                LookoutChanged(_Lookout);
                _Queue.RemoveAt(0);
                QueueChanged(_Queue);
                _HistoryLocker.WaitOne();
                HistoryUpdated(String.Format("Patient {0} got in lookout.", NewPatientInLookout));
                _HistoryLocker.ReleaseMutex();
                SendPatientsToDoctors();
                Thread.Sleep(10);
            }
            _LookoutLocker.ReleaseMutex();
            _QueueLocker.ReleaseMutex();
        }

        void OnInfectQueueTimerElapsed(Object O, ElapsedEventArgs EventArguments)
        {
            _QueueLocker.WaitOne();
            if (PatientsLeftCount == 0 && _Queue.Count == 0)
            {
                _InfectQueueTimer.Elapsed -= new ElapsedEventHandler(OnInfectQueueTimerElapsed);
                _QueueLocker.ReleaseMutex();
                return;
            }
            _RandomSourceLocker.WaitOne();
            var InfectingQueue = _RandomSource.Next(10) < 2;
            _RandomSourceLocker.ReleaseMutex();
            if (_Queue.Any(Patient => !Patient.IsIll) && _Queue.Any(Patient => Patient.IsIll) && InfectingQueue)
            {
                _HistoryLocker.WaitOne();
                HistoryUpdated("Queue was infected:");
                for (var Index = 0; Index < _Queue.Count; Index++)
                {
                    if (!_Queue[Index].IsIll)
                    {
                        _Queue[Index].IsIll = true;
                        HistoryUpdated(String.Format("    {0} become ill.", _Queue[Index]));
                        Thread.Sleep(10);
                    }
                }
                _HistoryLocker.ReleaseMutex();
                QueueChanged(_Queue);
            }
            _QueueLocker.ReleaseMutex();
        }

        void OnDoctorFinishedAppointment(Doctor Who)
        {
            var HealedPatient = Who.Patient;
            _HistoryLocker.WaitOne();
            HistoryUpdated(String.Format("{0} {1} patient {2}.", Who, HealedPatient.IsIll ? "healed" : "consulted", HealedPatient));
            _HistoryLocker.ReleaseMutex();
            _LookoutLocker.WaitOne();
            _Lookout.Remove(HealedPatient);
            LookoutChanged(_Lookout);
            _LookoutLocker.ReleaseMutex();
            if (Who.Patient.DoctorsCount == 2)
            {
                Who.Patient.SecondDoctor.Patient = null;
            }
            Who.Patient = null;
            SendPatientsToDoctors();
        }

        void OnDoctorRequiresAssistance(Doctor Who)
        {
            var ProblemPatient = Who.Patient;
            _HistoryLocker.WaitOne();
            HistoryUpdated(String.Format("{0} needs help.", Who));
            _HistoryLocker.ReleaseMutex();
            while (_Doctors.All(Doctor => Doctor.IsWorking))
            {

            }
            Doctor NotWorkingDoctor = null;
            lock (_Doctors)
            {
                NotWorkingDoctor = _Doctors.Where(Doctor => !Doctor.IsWorking).ToArray()[0];
            }
            NotWorkingDoctor.StartWorkWithPatient(ProblemPatient);
            _HistoryLocker.WaitOne();
            HistoryUpdated(String.Format("{0} started HEEEEELP {1}.", NotWorkingDoctor, Who));
            _HistoryLocker.ReleaseMutex();
        }
        #endregion
        #region Methods
        public void Start()
        {
            try
            {
                if (!_EnqueueTimer.Enabled)
                {
                    _EnqueueTimer.Start();
                    _AddToLookoutTimer.Start();
                    _InfectQueueTimer.Start();
                    _HistoryLocker.WaitOne();
                     HistoryUpdated("Infectious disease department started working.");
                    _HistoryLocker.ReleaseMutex();
                }
            }
            catch(Exception ex)
            {
                System.Console.WriteLine(ex.StackTrace);
            }
        }

        public void Stop()
        {
            if (_EnqueueTimer.Enabled)
            {
                _EnqueueTimer.Stop();
                _AddToLookoutTimer.Stop();
                _InfectQueueTimer.Stop();
                _HistoryLocker.WaitOne();
                HistoryUpdated("Infectious disease department stopped working.");
                _HistoryLocker.ReleaseMutex();
            }
        }

        void SendPatientsToDoctors()
        {
            var NotWorkingDoctors = _Doctors.Where(Doctor => !Doctor.IsWorking).ToList();
            var WaitingPatients = _Lookout.Where(Patient => Patient.DoctorsCount == 0).ToList();
            while (NotWorkingDoctors.Count > 0 && WaitingPatients.Count > 0)
            {
                var CurrentDoctor = NotWorkingDoctors[0];
                var CurrentPatient = WaitingPatients[0];
                CurrentDoctor.StartWorkWithPatient(CurrentPatient);
                _HistoryLocker.WaitOne();
                HistoryUpdated(String.Format("{0} started {1} patient {2}.", CurrentDoctor, CurrentPatient.IsIll ?
                    "healing" : "consulting", CurrentPatient));
                _HistoryLocker.ReleaseMutex();
                NotWorkingDoctors.RemoveAt(0);
                WaitingPatients.RemoveAt(0);
            }
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
                _EnqueueTimer.Dispose();
                _AddToLookoutTimer.Dispose();
                _InfectQueueTimer.Dispose();
                _QueueLocker.Dispose();
                _LookoutLocker.Dispose();
                _HistoryLocker.Dispose();
                _RandomSourceLocker.Dispose();
                for (var Index = 0; Index < _Doctors.Length; Index++)
                {
                    _Doctors[Index].Dispose();
                }
            }
        }
        #endregion
    }
}
