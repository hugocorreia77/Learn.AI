using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Learn.Core.Shared.App.Pages
{
    public class BaseComponentViewModel
    {
        public bool IsBusy { get; set; } = false;
        public IServiceProvider ServiceProvider { get; internal set; }
        public NavigationManager Navigation { get; internal set; }
        public EditContext EditContext { get; set; }
        public ValidationMessageStore MessageStore { get; set; }


        public BaseComponentViewModel(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            Navigation = serviceProvider.GetRequiredService<NavigationManager>();
        }

        public virtual void SetContext<T>(T model)
        {
            EditContext = new EditContext(model);
            MessageStore = new ValidationMessageStore(EditContext);
        }

    }
}
