using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace English_App.Model
{
    public class Sentence : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string text;
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                OnPropertyChanged();
            }
        }

        private string vnText;
        public string VNText
        {
            get { return vnText; }
            set
            {
                vnText = value;
                OnPropertyChanged();
            }
        }

        private bool isTranslating;
        public bool IsTranslating
        {
            get { return isTranslating; }
            set
            {
                isTranslating = value;
                OnPropertyChanged();
            }
        }

        public Sentence(string text)
        {
            Text = text;
        }

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
