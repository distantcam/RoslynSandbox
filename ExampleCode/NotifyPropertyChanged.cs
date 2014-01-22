using System;
using System.ComponentModel;
using System.Linq;

namespace ExampleCode
{
    public class BaseClass : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Person : BaseClass
    {
        public string GivenName { get; set; }
        public string FamilyName { get; set; }

        public int Age { get; set; }
    }
}