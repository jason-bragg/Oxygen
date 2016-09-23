
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Orleans.Providers.Abstractions.Extensions
{
    public static class Extensions
    {
        public static T GetProvider<T>(this IProviderGroup providerGroup, string name, char separator = '.') 
            where T : class
        {
            if (providerGroup == null) throw new ArgumentNullException(nameof(providerGroup));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            IProviderGroup group = providerGroup;

            string[] groups = name.Split(separator);
            if (groups.Length < 1 || !MatchName(groups[0],providerGroup))
            {
                return default(T);
            }

            for (int i = 1; group!=null && i < groups.Length - 1; i++)
            {
                string groupName = groups[i];
                group = group.Providers
                             .Select(kvp => kvp.Value as IProviderGroup)
                             .FirstOrDefault(provider => provider != null && MatchName(groupName,provider));
            }

            string providerName = groups.Last();
            return group?.Providers.Select(kvp => kvp.Value)
                                   .FirstOrDefault(provider => MatchName(providerName,provider) && provider is T) as T;
        }

        public static T GetProvider<T>(this IServiceProvider serviceProvider, string name, char separator = '.')
            where T : class
        {
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));

            string[] groups = name.Split(separator);

            switch (groups.Length)
            {
                case 0:
                    return default(T);
                case 1:
                    string providerName = groups[0];
                    return serviceProvider.GetServices<IProvider>()
                                          .FirstOrDefault(provider => provider is T && MatchName(providerName,provider)) as T;
                default:
                    string groupName = groups[0];
                    IProviderGroup group = serviceProvider.GetServices<IProvider>()
                                                          .FirstOrDefault(provider => provider is IProviderGroup && MatchName(groupName,provider)) as IProviderGroup;
                    return group?.GetProvider<T>(name, separator);
            }
        }

        private static bool MatchName(string name, IProvider provider)
        {
            return string.Equals(provider.Name, name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
