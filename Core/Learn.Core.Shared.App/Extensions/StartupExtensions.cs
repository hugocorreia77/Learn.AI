using Learn.Core.Shared.App.Pages;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Reflection;

namespace Learn.Core.Shared.App.Extensions
{
    public static class StartupExtensions
    {

        public static IServiceCollection AddViewModelPages(this IServiceCollection services, Assembly? assembly)
        {
            // Obtém o assembly atual
            assembly ??= Assembly.GetExecutingAssembly();

            // Encontra todas as classes que herdam de BaseComponentViewModel
            var viewModelTypes = assembly.GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(BaseComponentViewModel)));

            // Registra cada ViewModel no container de DI
            foreach (var viewModelType in viewModelTypes)
            {
                services.AddTransient(viewModelType);
            }

            return services;
        }

    }
}
