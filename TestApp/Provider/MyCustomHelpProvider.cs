using CommandDotNet;
using CommandDotNet.Help;
using Microsoft.Extensions.Localization;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using TestApp.Extensions;
using TestApp.Configuration.Constants;
using TestApp.Shared;

namespace TestApp.Provider
{
    public class MyCustomHelpProvider : HelpTextProvider
    {

        private readonly IStringLocalizer _Localizer;

        private readonly IStringLocalizerFactory _LocalizerFactory;

        private readonly AppHelpSettings _appHelpSettings;

        private string? _appName;

        public MyCustomHelpProvider(IStringLocalizerFactory stringLocalizerFactory, AppSettings appSettings, string appName = null) : base(appSettings, appName)
        {
            _appName = appName;
            _appHelpSettings = appSettings.Help;
            _LocalizerFactory = stringLocalizerFactory;
            _Localizer = _LocalizerFactory.Create(typeof(CommandDotNetLocalization));
        }

        public override string GetHelpText(Command command) =>
            JoinSections(
                (null, CommandDescription(command)),
                (CheckForLocalisation(CommandDotNetConsts.Usage), SectionUsage(command)),
                (CheckForLocalisation(CommandDotNetConsts.Arguments), SectionOperands(command)),
                (CheckForLocalisation(CommandDotNetConsts.Options), SectionOptions(command, false)),
                (CheckForLocalisation(CommandDotNetConsts.OptionsAvailable), SectionOptions(command, true)),
                (CheckForLocalisation(CommandDotNetConsts.Commands), SectionSubcommands(command)),
                (null, ExtendedHelpText(command)));



        /// <summary>How operands are shown in the usage example</summary>
        protected override string? UsageOperand(Command command)
        {
            if (!command.Operands.Any())
            {
                return null;
            }

            if (!_appHelpSettings.ExpandArgumentsInUsage)
            {
                return CommandDotNetConsts.ArgumentsField;
            }

            if (command.Operands.Last().Arity.Minimum > 0)
            {
                return command.Operands.Select(o => $"<{o.Name}>").ToCsv(" ");
            }

            var sb = new StringBuilder("]");
            bool inOptionalRegion = true;
            foreach (var operand in command.Operands.Reverse())
            {
                if (inOptionalRegion && operand.Arity.Minimum > 0)
                {
                    // remove leading space
                    sb.Remove(0, 1);
                    sb.Insert(0, " [");
                    inOptionalRegion = false;
                }
                sb.Insert(0, $" <{operand.Name}>");
            }
            sb.Remove(0, 1);
            if (inOptionalRegion)
            {
                sb.Insert(0, "[");
            }
            return sb.ToString();
        }

        /// <summary>How options are shown in the usage example</summary>
        protected override string? UsageOption(Command command) =>
            command.Options.Any(o => !o.Hidden)
                ? CommandDotNetConsts.OptionsField
                : null;

        /// <summary>How subcommands are shown in the usage example</summary>
        protected override string? UsageSubcommand(Command command) =>
            command.Subcommands.Any()
                ? CommandDotNetConsts.CommandField
                : null;

        protected override string? ExtendedHelpText(Command command)
        {
            return CommandReplacements(command, CheckForLocalisation(command.ExtendedHelpText)); 
        }
 
        /// <summary>Hint displayed in the subcommands section for getting help for a subcommand.</summary>
        protected override string? SubcommandHelpHint(Command command) =>
            String.Format(_Localizer.GetString(CommandDotNetConsts.SubcommandHelpHint), AppName(command),PadFront(CommandPath(command)), Constants.HelpOptionName);

        private static string? PadFront(string? value) =>
           value.IsNullOrWhitespace() ? null : " " + value;

        protected override string? CommandDescription(Command command)
        {
            return CheckForLocalisation(command.Description.UnlessNullOrWhitespace());
        }

        protected override string? ArgumentDescription<IArgument>(IArgument argument) 
        {
            return CheckForLocalisation(argument.Description.UnlessNullOrWhitespace());
        }

        protected override string? ArgumentArity<IArgument>(IArgument argument)
        {
            return (argument.Arity.AllowsMany() ? " (" + CheckForLocalisation(CommandDotNetConsts.Multiple) + ")" : "");
        }

        /// <summary>Returns a comma-separated list of the allowed values</summary>
        protected override string? ArgumentAllowedValues<IArgument>(IArgument argument)
        {
            return argument.AllowedValues?.ToCsv(", ").UnlessNullOrWhitespace(v => $"{CheckForLocalisation(CommandDotNetConsts.AllowedValues)}: {v}");
        }

        private class CommandHelpValues
        {
            public readonly string Name;
            public readonly string? Description;

            public CommandHelpValues(string name, string? description)
            {
                Name = name;
                Description = description;
            }
        }

        private string? CommandReplacements(Command command, string? text) => text?
            .Replace("%UsageAppName%", AppName(command))
            .Replace("%AppName%", AppName(command))
            .Replace("%CmdPath%", command.GetPath());

        private class ArgumentHelpValues
        {
            public readonly string Template;
            public readonly string? TypeName;
            public readonly string? DefaultValue;
            public readonly string? Description;
            public readonly string? AllowedValues;

            public ArgumentHelpValues(
                string template,
                string? typeName,
                string? defaultValue,
                string? description,
                string? allowedValues)
            {
                Template = template;
                TypeName = typeName;
                DefaultValue = defaultValue;
                Description = description;
                AllowedValues = allowedValues;
            }
        }

        private ICollection<ArgumentHelpValues> BuildArgumentHelpValues<T>(IEnumerable<T> arguments) where T : IArgument =>
            arguments.Select(a => new ArgumentHelpValues
            ($"{ArgumentName(a)}{ArgumentArity(a)}",
                ArgumentTypeName(a),
                ArgumentDefaultValue(a),
                ArgumentDescription(a),
                ArgumentAllowedValues(a)
            )).ToCollection();

        private string CheckForLocalisation(string stringToCheck) 
        {
            if (!stringToCheck.IsNullOrEmpty() && _Localizer != null) 
            {
                return _Localizer.GetString(stringToCheck);
            }
            return stringToCheck;  
        }
    }

}

