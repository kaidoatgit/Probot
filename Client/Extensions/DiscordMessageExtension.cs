using DSharpPlus.Entities;

namespace ProPayments.Client.Extensions
{
    public static class DiscordMessageExtension
    {
        public static IDiscordMessageBuilder ReplaceComponents(this DiscordMessage message, List<DiscordActionRowComponent> discordComponents)
        {
            DiscordMessageBuilder messageBuilder = new(message);
            messageBuilder.ClearComponents();
            messageBuilder.AddComponents(discordComponents);
            return messageBuilder;
        }

        public static List<DiscordActionRowComponent> SetDropdownDefaultValue(this DiscordMessage message, string componentId, string selectedOption, DiscordSelectComponent? dropdown = null)
        {
            var components = message.Components;

            var currentDropdown = components.OfType<DiscordActionRowComponent>()
                .SelectMany(row => row.Components)
                .OfType<DiscordSelectComponent>()
                .FirstOrDefault(c => c.CustomId.StartsWith(componentId));

            var updatedDropDown = currentDropdown!.SetDefaultValue(selectedOption);
            List<DiscordActionRowComponent> discordComponents = message.Components.Replace(currentDropdown!, updatedDropDown)!.ToList();
            if (dropdown == null)
            {
                return discordComponents;
            }

            var durationDropdown = components.OfType<DiscordActionRowComponent>()
                .SelectMany(row => row.Components)
                .OfType<DiscordSelectComponent>()
                .FirstOrDefault(c => c.CustomId.StartsWith(dropdown.CustomId));

            if (durationDropdown == null)
            {
                List<DiscordActionRowComponent> discordComponentsWithNewDropdown = new()
                    {
                        discordComponents[0],
                        new DiscordActionRowComponent(new List<DiscordComponent>(){ dropdown }),
                        discordComponents[1]
                    };
                return discordComponentsWithNewDropdown;
            }
            discordComponents = message.Components.Replace(durationDropdown!, dropdown)!.ToList();
            return discordComponents;
        }

        public static (DiscordSelectComponent? productSelect, DiscordSelectComponent? durationSelect) ParseComponentSelections(this DiscordMessage message)
        {
            var components = message.Components;

            var productSelect = components.OfType<DiscordActionRowComponent>()
                .SelectMany(row => row.Components)
                .OfType<DiscordSelectComponent>()
                .FirstOrDefault(c => c.CustomId.StartsWith("product_selection_menu"));

            var durationSelect = components.OfType<DiscordActionRowComponent>()
                .SelectMany(row => row.Components)
                .OfType<DiscordSelectComponent>()
                .FirstOrDefault(c => c.CustomId.StartsWith("duration_selection_menu"));

            return (productSelect, durationSelect);
        }

        private static DiscordSelectComponent SetDefaultValue(this DiscordSelectComponent dropdown, string selectedValue)
        {
            IEnumerable<DiscordSelectComponentOption> options = dropdown.Options
                .Select(option => new DiscordSelectComponentOption(option.Label, option.Value, option.Description, option.Value == selectedValue, option.Emoji))
                .AsEnumerable();

            return new DiscordSelectComponent(dropdown.CustomId, dropdown.Placeholder, options);
        }

        private static IEnumerable<DiscordActionRowComponent> Replace<T>(this IEnumerable<DiscordActionRowComponent> rowComponents, T oldComponent, T newComponent) where T : DiscordComponent
        {
            foreach (var row in rowComponents)
            {
                List<DiscordComponent> discordComponents = row.Components.ToList();
                var index = discordComponents.IndexOf(oldComponent);
                if (index != -1)
                {
                    discordComponents[index] = newComponent;
                    row.Components = discordComponents;
                }
            }

            return rowComponents;
        }
    }
}
