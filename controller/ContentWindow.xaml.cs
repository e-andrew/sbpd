using System;
using System.Windows;
using Cerberus.view;

namespace Cerberus.controller
{
    public partial class ContentWindow : Window
    {
        private Operation _operation;
        
        public ContentWindow(Operation operation)
        {
            InitializeComponent();
            _operation = operation;
            OperationType operationType = _operation.GetOperationType();
            switch (operationType)
            {
                case OperationType.Read:
                    ContentArea.IsReadOnly = true;
                    ContentArea.Text = _operation.GetValue("content");
                    break;
                case OperationType.Append:
                    ContentArea.IsReadOnly = false;
                    break;
                case OperationType.Rewrite:
                    ContentArea.IsReadOnly = false;
                    ContentArea.Text = _operation.GetValue("content");
                    break;
            }
        }
        private void Done(object sender, EventArgs e)
        {
            _operation.SetValue("content", ContentArea.Text);
            Session.DoneOperation(_operation);
            Close();
        }
    }
}