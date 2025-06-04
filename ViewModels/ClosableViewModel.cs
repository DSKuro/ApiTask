using System;

namespace ApiTask.ViewModels
{
    public abstract class ClosableViewModel : ViewModelBase
    {
        public event EventHandler? ClosingRequest;

        protected void OnClosingRequest()
        {
            if (this.ClosingRequest != null)
            {
                this.ClosingRequest(this, EventArgs.Empty);
            }
        }
    }
}
