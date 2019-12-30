using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using BepInEx;
using MonoMod.Cil;
using R2API.Utils;
using RoR2;
using RoR2.ConVar;
using RoR2Pro.Modules;
using UnityEngine;
using Console = System.Console;
using Reflection = RoR2Pro.Utilities.Reflection;

namespace RoR2Pro
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("RoR2Pro.RoR2Pro", "Risk of Rain 2 Pro", "0.0.1")]
    public class RoR2Pro : BaseUnityPlugin
    {
        public RoR2Pro()
        {
            // Hack to enable access to the instance from console commands
            RoR2Pro.Instance = this;
        }

        private static RoR2Pro Instance { get; set; }

        private List<Action> _moduleCleanUpActions = new List<Action>();

        public void Awake()
        {
            On.RoR2.Console.Awake += (orig, self) =>
            {
                orig(self);

                if (!TryRegisterConsoleVariables(self)) return;
                if (!TryRegisterConsoleCommands(self)) return;

                LoadModules();

                Chat.AddMessage("Loaded Risk of Rain 2 Pro");
            };
        }

        private bool TryRegisterConsoleCommands(RoR2.Console self)
        {
            const string commandCatalogField = "concommandCatalog";
            var types = typeof(RoR2Pro).Assembly.GetTypes();
            var catalog = self.GetFieldValue<IDictionary>(commandCatalogField);
            if (catalog == null)
            {
                Chat.AddMessage($"Could not find command catalog {commandCatalogField}. Unloading RoR2Pro.");
                return false;
            }

            foreach (var methodInfo in types.SelectMany(x => x.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)))
            {
                var customAttributes = methodInfo.GetCustomAttributes(false);
                foreach (var attribute in customAttributes.OfType<ConCommandAttribute>())
                {
                    var conCommand = R2API.Utils.Reflection.GetNestedType<RoR2.Console>("ConCommand").Instantiate();

                    conCommand.SetFieldValue("flags", attribute.flags);
                    conCommand.SetFieldValue("helpText", attribute.helpText);
                    conCommand.SetFieldValue("action", (RoR2.Console.ConCommandDelegate)Delegate.CreateDelegate(typeof(RoR2.Console.ConCommandDelegate), methodInfo));

                    catalog[attribute.commandName.ToLower()] = conCommand;
                    Chat.AddMessage($"Registered command {attribute.commandName}");
                }
            }

            return true;
        }

        private static bool TryRegisterConsoleVariables(RoR2.Console self)
        {
            const string registerConVarMethod = "RegisterConVarInternal";
            var registerConVarInternal = typeof(RoR2.Console).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(m => m.Name == registerConVarMethod);
            if (registerConVarInternal == null)
            {
                Chat.AddMessage($"Could not find {registerConVarMethod}. Unloading RoR2Pro.");
                return false;
            }
            else
            {
                var consoleVariables = Reflection.GetImplementingTypes<RoR2Pro, BaseConVar>().ToList();

                Chat.AddMessage($"Registering console variables [{string.Join(", ", consoleVariables.Select(v => v.Name))}]");
                foreach (var conVar in consoleVariables)
                {
                    var instance = (BaseConVar) Activator.CreateInstance(conVar);

                    registerConVarInternal.Invoke(self, new object[] {instance});
                }

                Chat.AddMessage($"Registering console variables [{string.Join(", ", consoleVariables.Select(v => v.Name))}]");
            }

            return true;
        }

        public void LoadModules()
        {
            foreach (var action in _moduleCleanUpActions)
            {
                action();
            }
            _moduleCleanUpActions.Clear();

            var modules = Reflection.GetImplementingTypes<RoR2Pro, IModule>().ToList();

            Chat.AddMessage($"Loading Risk of Rain 2 Pro modules: [{string.Join(", ", modules.Select(m => m.Name))}]");

            foreach (var module in modules)
            {
                var instance = (IModule)Activator.CreateInstance(module);

                _moduleCleanUpActions.Add(instance.Initialize());
            }
        }

        [ConCommand(commandName = "ror2pro_reload", flags = ConVarFlags.ExecuteOnServer, helpText = "Reload all Risk of Rain 2 Pro modules")]
        public static void ReloadModules(ConCommandArgs args)
        {
            Chat.AddMessage("Reloading modules");

            Instance.LoadModules();
        }
    }
}
