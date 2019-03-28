using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Forms;
using System.Collections.Generic;
using Lab02_rpks_.Interfaces;
using System.Windows;
using System.Collections.ObjectModel;

using Lab02_rpks_.Models;
using System;

namespace Lab02_rpks_
{
    class IDDPresenter : INotifyPropertyChanged
    {
        #region Fields
        private ObservableCollection<string> _Queue;
        private ObservableCollection<string> _Lookout;
        private ObservableCollection<string> _History;
        private InfectiousDiseaseDepartment IDD;
        #endregion

        #region Properties
        public ObservableCollection<string> Queue
        {
            get => _Queue;
            set
            {
                _Queue = value;
                OnPropertyChanged("Queue");
            }
        }

        public ObservableCollection<string> Lookout
        {
            get => _Lookout;
            set
            {
                _Lookout = value;
                OnPropertyChanged("Lookout");
            }
        }

        public ObservableCollection<string> History
        {
            get => _History;
            set
            {
                _History = value;
                OnPropertyChanged("History");
            }
        }
        #endregion

        #region Actions

        public ICommand Start =>
            new RelayCommand(() =>
            {
                IDD.Start(); 
            });

        public ICommand Stop =>
            new RelayCommand(() =>
            {
                IDD.Stop();
            });
        #endregion

        #region Event handlers
        public void OnHistoryUpdated(String LastAction)
        {

           // lock (_History)
            {
                //var inv2 = new Action(() =>
                {
                    _History.Add(LastAction);
                    OnPropertyChanged("History");
                }//);

                //inv2.DynamicInvoke();

                /*  _HistoryListBox.BeginInvoke((MethodInvoker)(() =>
                  {
                      _HistoryListBox.Items.Add(LastAction);
                      _HistoryListBox.SelectedIndex = _HistoryListBox.Items.Count - 1;
                  }));*/
            }
        }
        
        public void OnQueueChanged(List<Patient> Queue)
        {
            var QueueArray = Queue.ToArray();
            //lock (_Queue)
            {
                //var inv = new Action(() =>
                {

                    _Queue.Clear();

                    foreach (var patient in QueueArray)
                    {
                        var CurentItem = String.Format("{0} - {1}", patient, patient.IsIll ? "ill" : "healthy");
                        _Queue.Add(CurentItem);
                        OnPropertyChanged("Queue");
                    }
                }//);

                //inv.DynamicInvoke();
                /*_Queue.BeginInvoke((MethodInvoker)(() =>
                {
                    _QueueListBox.Items.Clear();
                    foreach (var Patient in QueueArray)
                    {
                        var CurrentItem = String.Format("{0} - {1}", Patient, Patient.IsIll ? "ill" : "healthy");
                        _QueueListBox.Items.Add(CurrentItem);
                    }
                    if (_QueueListBox.Items.Count != 0)
                    {
                        _QueueListBox.SelectedIndex = _QueueListBox.Items.Count - 1;
                    }
                }));*/
            }
        }
        
        public void OnLookoutChanged(List<Patient> Lookout)
        {
            //System.Windows.Forms.MessageBox.Show("Sad but true!");



            var LookoutArray = Lookout.ToArray();
            //lock (_Lookout)
            {
                //var inv = new Action(() =>
                {
                     _Lookout.Clear();
                    foreach (var patient in LookoutArray)
                    {
                        var CurrentItem = String.Format("{0} - {1}", patient, patient.IsIll ? "ill" : "healthy");
                        _Lookout.Add(CurrentItem);
                        Console.WriteLine(CurrentItem);
                        
                    }
                    OnPropertyChanged("Lookout");
                }//);

                //inv.DynamicInvoke();

                /*
                _LookoutListBox.BeginInvoke((MethodInvoker)(() =>
                {
                    _LookoutListBox.Items.Clear();
                    foreach (var Patient in LookoutArray)
                    {
                        var CurrentItem = String.Format("{0} - {1}", Patient, Patient.IsIll ? "ill" : "healthy");
                        _LookoutListBox.Items.Add(CurrentItem);
                    }
                    if (_LookoutListBox.Items.Count != 0)
                    {
                        _LookoutListBox.SelectedIndex = _LookoutListBox.Items.Count - 1;
                    }
                }));*/
            }
        }
        #endregion
        public IDDPresenter()
        {
            _Queue = new ObservableCollection<string>();
            _Lookout = new ObservableCollection<string>();
            _History = new ObservableCollection<string>();

            IDD = new InfectiousDiseaseDepartment(15, 10, (UInt32)((new Random()).Next(100, 151)), 250);

            IDD.QueueChanged += new QueueChangedEvent(OnQueueChanged);
            IDD.LookoutChanged += new LookoutChangedEvent(OnLookoutChanged);
            IDD.HistoryUpdated += new HistoryUpdatedEvent(OnHistoryUpdated);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
