using System;
using System.Collections.Generic;
using System.Text;

namespace UniState
{
    internal static class UniStateDebugTreeRenderer
    {
        public static string Render(UniStateDebugSnapshot snapshot)
        {
            var machines = snapshot.Machines ?? Array.Empty<DebugMachineNode>();
            var activeCount = 0;

            for (var i = 0; i < machines.Count; i++)
            {
                if (machines[i].Status == DebugMachineStatus.Running)
                {
                    activeCount++;
                }
            }

            var builder = new StringBuilder();
            builder.Append("UniState Debug Snapshot seq=");
            builder.Append(snapshot.Sequence);
            builder.Append(" machines=");
            builder.Append(machines.Count);
            builder.Append(" active=");
            builder.Append(activeCount);

            if (machines.Count == 0)
            {
                return builder.ToString();
            }

            var childMachines = BuildChildMachineLookup(machines);

            for (var i = 0; i < machines.Count; i++)
            {
                if (machines[i].ParentStateId.HasValue)
                {
                    continue;
                }

                builder.AppendLine();
                AppendMachine(builder, machines[i], childMachines, 0);
            }

            return builder.ToString();
        }

        private static Dictionary<int, List<DebugMachineNode>> BuildChildMachineLookup(IReadOnlyList<DebugMachineNode> machines)
        {
            var result = new Dictionary<int, List<DebugMachineNode>>();

            for (var i = 0; i < machines.Count; i++)
            {
                var parentStateId = machines[i].ParentStateId;
                if (!parentStateId.HasValue)
                {
                    continue;
                }

                if (!result.TryGetValue(parentStateId.Value, out var children))
                {
                    children = new List<DebugMachineNode>();
                    result[parentStateId.Value] = children;
                }

                children.Add(machines[i]);
            }

            return result;
        }

        private static void AppendMachine(
            StringBuilder builder,
            DebugMachineNode machine,
            Dictionary<int, List<DebugMachineNode>> childMachines,
            int indent)
        {
            AppendIndent(builder, indent);
            builder.Append("[M");
            builder.Append(machine.Id);
            builder.Append("] ");
            builder.Append(ShortName(machine.RuntimeType));
            builder.Append("#");
            builder.Append(machine.Id);
            builder.Append(" ");
            builder.Append(machine.Status);
            builder.Append(" parent=");
            builder.Append(machine.ParentStateId.HasValue ? "S" + machine.ParentStateId.Value : "<root>");
            builder.AppendLine();

            AppendIndent(builder, indent + 2);
            builder.Append("active: ");
            var activeState = FindState(machine, machine.ActiveStateId);
            builder.Append(activeState == null ? "<none>" : FormatState(activeState));
            builder.AppendLine();

            AppendIndent(builder, indent + 2);
            builder.Append("history: ");
            AppendHistory(builder, machine.History);
            builder.AppendLine();

            if (machine.LastError != null)
            {
                AppendIndent(builder, indent + 2);
                builder.Append("last error: ");
                builder.Append(machine.LastError.ErrorType);
                builder.Append(" ");
                builder.Append(ShortName(machine.LastError.ExceptionType));
                builder.Append(": ");
                builder.Append(machine.LastError.Message);
                builder.AppendLine();
            }

            if (machine.RecentTransitions != null && machine.RecentTransitions.Count > 0)
            {
                AppendIndent(builder, indent + 2);
                builder.AppendLine("recent:");

                for (var i = 0; i < machine.RecentTransitions.Count; i++)
                {
                    AppendIndent(builder, indent + 4);
                    AppendTransition(builder, machine.RecentTransitions[i]);
                    builder.AppendLine();
                }
            }

            if (machine.States == null || machine.States.Count == 0)
            {
                return;
            }

            AppendIndent(builder, indent + 2);
            builder.AppendLine("states:");

            for (var i = 0; i < machine.States.Count; i++)
            {
                var state = machine.States[i];
                if (state.ParentCompositeStateId.HasValue)
                {
                    continue;
                }

                AppendState(builder, machine, state, childMachines, indent + 4);
            }
        }

        private static void AppendState(
            StringBuilder builder,
            DebugMachineNode machine,
            DebugStateNode state,
            Dictionary<int, List<DebugMachineNode>> childMachines,
            int indent)
        {
            AppendIndent(builder, indent);
            builder.Append(FormatState(state));
            builder.Append(" ");
            builder.Append(state.Role);
            builder.Append(" ");
            builder.Append(state.Status);

            if (state.Phase != DebugStatePhase.None)
            {
                builder.Append(" phase=");
                builder.Append(state.Phase);
            }

            builder.AppendLine();

            if (childMachines.TryGetValue(state.Id, out var machines))
            {
                for (var i = 0; i < machines.Count; i++)
                {
                    AppendIndent(builder, indent + 2);
                    builder.AppendLine("child machine:");
                    AppendMachine(builder, machines[i], childMachines, indent + 4);
                }
            }

            if (state.SubStateIds == null)
            {
                return;
            }

            for (var i = 0; i < state.SubStateIds.Count; i++)
            {
                var subState = FindState(machine, state.SubStateIds[i]);
                if (subState == null)
                {
                    continue;
                }

                AppendState(builder, machine, subState, childMachines, indent + 2);
            }
        }

        private static void AppendHistory(StringBuilder builder, IReadOnlyList<DebugStateRef> history)
        {
            if (history == null || history.Count == 0)
            {
                builder.Append("<empty>");
                return;
            }

            for (var i = 0; i < history.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(" -> ");
                }

                builder.Append(ShortName(history[i].RuntimeType));
                builder.Append("#");
                builder.Append(history[i].Id);
            }
        }

        private static void AppendTransition(StringBuilder builder, DebugTransitionEvent transition)
        {
            builder.Append(ShortName(transition.FromStateType) ?? "<none>");
            builder.Append(" --");
            builder.Append(transition.Transition);
            builder.Append("--> ");
            builder.Append(ShortName(transition.ToStateType) ?? "<exit>");
        }

        private static DebugStateNode FindState(DebugMachineNode machine, int? id)
        {
            if (!id.HasValue || machine.States == null)
            {
                return null;
            }

            for (var i = 0; i < machine.States.Count; i++)
            {
                if (machine.States[i].Id == id.Value)
                {
                    return machine.States[i];
                }
            }

            return null;
        }

        private static string FormatState(DebugStateNode state) => ShortName(state.RuntimeType) + "#" + state.Id;

        private static string ShortName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return null;
            }

            var index = typeName.LastIndexOf('.');
            return index >= 0 ? typeName.Substring(index + 1) : typeName;
        }

        private static void AppendIndent(StringBuilder builder, int indent)
        {
            for (var i = 0; i < indent; i++)
            {
                builder.Append(' ');
            }
        }
    }
}
