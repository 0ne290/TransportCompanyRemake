using Domain.Interfaces;
using Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Domain;

// В моей системе EF может создавать доменные сущности, но он не имеет полноценного DI-контейнера. Манипулируя с самим EF, эту проблему можно решить только двумя способами. Первый - внедрять DbContext в каждую сущность через ее конструктор и уже из этого DbContext доставать нужные зависимости, уже выданные ранее DbContext'у настоящим DI-контейнером. Очевидно, это антипаттерн, да и вообще нарушение принципа зависимостей Clean Architecture и DDD. Второй способ - создать интерфейс с нужной зависимостью в качестве свойства, наследоваться от этого интерфейса сущностями, требующими эту зависимость, создать перехватчик создания EF-сущностей (класс, наследующийся от IMaterializationInterceptor), в этом перехватчике реализовать метод InitializedInstance, в этом методе проверять тип создаваемой сущности на соответствие интерфейса с зависимостью, если соответствует - инициализировать свойство зависимости значением, полученным от настоящего DI-контейнера. Этот класс - дополнение ко второму способу, гарантирующее, что если не установить явно значение свойству зависимости, оно будет значением по умолчанию
public static class DefaultServiceProviderSingleton
{
    static DefaultServiceProviderSingleton()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IGeolocationService, DefaultGeolocationService>();
        Instance = serviceCollection.BuildServiceProvider();
    }
    
    public static readonly IServiceProvider Instance;
}