using System;
using System.Collections.Generic;
using System.Text;

namespace UniState
{
    internal static class UniStateDebugJsonRenderer
    {
        public static string Render(UniStateDebugSnapshot snapshot, bool isEnabled, string message, bool indented)
        {
            var builder = new StringBuilder();
            AppendObjectStart(builder, 0, indented);
            AppendProperty(builder, "isEnabled", isEnabled ? "true" : "false", 1, true, indented);

            if (!string.IsNullOrEmpty(message))
            {
                AppendProperty(builder, "message", Escape(message), 1, true, indented);
            }

            AppendProperty(builder, "sequence", snapshot.Sequence.ToString(), 1, true, indented);
            AppendPropertyName(builder, "machines", 1, indented);
            AppendMachineArray(builder, snapshot.Machines ?? Array.Empty<DebugMachineNode>(), 1, indented);
            NewLine(builder, indented);
            AppendObjectEnd(builder, 0, indented);

            return builder.ToString();
        }

        private static void AppendMachineArray(
            StringBuilder builder,
            IReadOnlyList<DebugMachineNode> machines,
            int indent,
            bool indented)
        {
            builder.Append("[");

            for (var i = 0; i < machines.Count; i++)
            {
                NewLine(builder, indented);
                AppendMachine(builder, machines[i], indent + 1, indented);

                if (i < machines.Count - 1)
                {
                    builder.Append(",");
                }
            }

            if (machines.Count > 0)
            {
                NewLine(builder, indented);
                AppendIndent(builder, indent, indented);
            }

            builder.Append("]");
        }

        private static void AppendMachine(StringBuilder builder, DebugMachineNode machine, int indent, bool indented)
        {
            AppendObjectStart(builder, indent, indented);
            AppendProperty(builder, "id", machine.Id.ToString(), indent + 1, true, indented);
            AppendProperty(builder, "runtimeType", Escape(machine.RuntimeType), indent + 1, true, indented);
            AppendProperty(builder, "resolverType", Escape(machine.ResolverType), indent + 1, true, indented);
            AppendProperty(builder, "status", Escape(machine.Status.ToString()), indent + 1, true, indented);
            AppendProperty(builder, "parentStateId", NullableInt(machine.ParentStateId), indent + 1, true, indented);
            AppendProperty(builder, "activeStateId", NullableInt(machine.ActiveStateId), indent + 1, true, indented);

            AppendPropertyName(builder, "history", indent + 1, indented);
            AppendStateRefArray(builder, machine.History ?? Array.Empty<DebugStateRef>(), indent + 1, indented);
            builder.Append(",");
            NewLine(builder, indented);

            AppendPropertyName(builder, "states", indent + 1, indented);
            AppendStateArray(builder, machine.States ?? Array.Empty<DebugStateNode>(), indent + 1, indented);
            builder.Append(",");
            NewLine(builder, indented);

            AppendPropertyName(builder, "recentTransitions", indent + 1, indented);
            AppendTransitionArray(builder, machine.RecentTransitions ?? Array.Empty<DebugTransitionEvent>(), indent + 1, indented);
            builder.Append(",");
            NewLine(builder, indented);

            AppendPropertyName(builder, "lastError", indent + 1, indented);
            AppendError(builder, machine.LastError, indent + 1, indented);
            NewLine(builder, indented);
            AppendObjectEnd(builder, indent, indented);
        }

        private static void AppendStateArray(
            StringBuilder builder,
            IReadOnlyList<DebugStateNode> states,
            int indent,
            bool indented)
        {
            builder.Append("[");

            for (var i = 0; i < states.Count; i++)
            {
                NewLine(builder, indented);
                AppendState(builder, states[i], indent + 1, indented);

                if (i < states.Count - 1)
                {
                    builder.Append(",");
                }
            }

            if (states.Count > 0)
            {
                NewLine(builder, indented);
                AppendIndent(builder, indent, indented);
            }

            builder.Append("]");
        }

        private static void AppendState(StringBuilder builder, DebugStateNode state, int indent, bool indented)
        {
            AppendObjectStart(builder, indent, indented);
            AppendProperty(builder, "id", state.Id.ToString(), indent + 1, true, indented);
            AppendProperty(builder, "declaredType", Escape(state.DeclaredType), indent + 1, true, indented);
            AppendProperty(builder, "runtimeType", Escape(state.RuntimeType), indent + 1, true, indented);
            AppendProperty(builder, "payloadType", Escape(state.PayloadType), indent + 1, true, indented);
            AppendProperty(builder, "role", Escape(state.Role.ToString()), indent + 1, true, indented);
            AppendProperty(builder, "status", Escape(state.Status.ToString()), indent + 1, true, indented);
            AppendProperty(builder, "phase", Escape(state.Phase.ToString()), indent + 1, true, indented);
            AppendProperty(builder, "parentCompositeStateId", NullableInt(state.ParentCompositeStateId), indent + 1, true, indented);

            AppendPropertyName(builder, "subStateIds", indent + 1, indented);
            AppendIntArray(builder, state.SubStateIds ?? Array.Empty<int>(), indent + 1, indented);
            builder.Append(",");
            NewLine(builder, indented);

            AppendPropertyName(builder, "lastTransition", indent + 1, indented);
            AppendTransition(builder, state.LastTransition, indent + 1, indented);
            builder.Append(",");
            NewLine(builder, indented);

            AppendPropertyName(builder, "lastError", indent + 1, indented);
            AppendError(builder, state.LastError, indent + 1, indented);
            NewLine(builder, indented);
            AppendObjectEnd(builder, indent, indented);
        }

        private static void AppendStateRefArray(
            StringBuilder builder,
            IReadOnlyList<DebugStateRef> states,
            int indent,
            bool indented)
        {
            builder.Append("[");

            for (var i = 0; i < states.Count; i++)
            {
                NewLine(builder, indented);
                AppendObjectStart(builder, indent + 1, indented);
                AppendProperty(builder, "id", states[i].Id.ToString(), indent + 2, true, indented);
                AppendProperty(builder, "declaredType", Escape(states[i].DeclaredType), indent + 2, true, indented);
                AppendProperty(builder, "runtimeType", Escape(states[i].RuntimeType), indent + 2, false, indented);
                NewLine(builder, indented);
                AppendObjectEnd(builder, indent + 1, indented);

                if (i < states.Count - 1)
                {
                    builder.Append(",");
                }
            }

            if (states.Count > 0)
            {
                NewLine(builder, indented);
                AppendIndent(builder, indent, indented);
            }

            builder.Append("]");
        }

        private static void AppendTransitionArray(
            StringBuilder builder,
            IReadOnlyList<DebugTransitionEvent> transitions,
            int indent,
            bool indented)
        {
            builder.Append("[");

            for (var i = 0; i < transitions.Count; i++)
            {
                NewLine(builder, indented);
                AppendTransition(builder, transitions[i], indent + 1, indented);

                if (i < transitions.Count - 1)
                {
                    builder.Append(",");
                }
            }

            if (transitions.Count > 0)
            {
                NewLine(builder, indented);
                AppendIndent(builder, indent, indented);
            }

            builder.Append("]");
        }

        private static void AppendTransition(
            StringBuilder builder,
            DebugTransitionEvent transition,
            int indent,
            bool indented)
        {
            if (transition == null)
            {
                builder.Append("null");
                return;
            }

            AppendObjectStart(builder, indent, indented);
            AppendProperty(builder, "sequence", transition.Sequence.ToString(), indent + 1, true, indented);
            AppendProperty(builder, "transition", Escape(transition.Transition.ToString()), indent + 1, true, indented);
            AppendProperty(builder, "fromStateId", NullableInt(transition.FromStateId), indent + 1, true, indented);
            AppendProperty(builder, "toStateId", NullableInt(transition.ToStateId), indent + 1, true, indented);
            AppendProperty(builder, "fromStateType", Escape(transition.FromStateType), indent + 1, true, indented);
            AppendProperty(builder, "toStateType", Escape(transition.ToStateType), indent + 1, true, indented);
            AppendProperty(builder, "goBackToType", Escape(transition.GoBackToType), indent + 1, false, indented);
            NewLine(builder, indented);
            AppendObjectEnd(builder, indent, indented);
        }

        private static void AppendError(StringBuilder builder, DebugErrorInfo error, int indent, bool indented)
        {
            if (error == null)
            {
                builder.Append("null");
                return;
            }

            AppendObjectStart(builder, indent, indented);
            AppendProperty(builder, "sequence", error.Sequence.ToString(), indent + 1, true, indented);
            AppendProperty(builder, "errorType", Escape(error.ErrorType), indent + 1, true, indented);
            AppendProperty(builder, "exceptionType", Escape(error.ExceptionType), indent + 1, true, indented);
            AppendProperty(builder, "message", Escape(error.Message), indent + 1, true, indented);
            AppendProperty(builder, "stateId", NullableInt(error.StateId), indent + 1, true, indented);
            AppendProperty(builder, "stateType", Escape(error.StateType), indent + 1, true, indented);
            AppendProperty(builder, "phase", Escape(error.Phase.ToString()), indent + 1, false, indented);
            NewLine(builder, indented);
            AppendObjectEnd(builder, indent, indented);
        }

        private static void AppendIntArray(
            StringBuilder builder,
            IReadOnlyList<int> items,
            int indent,
            bool indented)
        {
            builder.Append("[");

            for (var i = 0; i < items.Count; i++)
            {
                if (i > 0)
                {
                    builder.Append(",");
                    if (indented)
                    {
                        builder.Append(" ");
                    }
                }

                builder.Append(items[i]);
            }

            builder.Append("]");
        }

        private static void AppendProperty(
            StringBuilder builder,
            string name,
            string value,
            int indent,
            bool comma,
            bool indented)
        {
            AppendPropertyName(builder, name, indent, indented);
            builder.Append(value);

            if (comma)
            {
                builder.Append(",");
            }

            NewLine(builder, indented);
        }

        private static void AppendPropertyName(StringBuilder builder, string name, int indent, bool indented)
        {
            AppendIndent(builder, indent, indented);
            builder.Append("\"");
            builder.Append(name);
            builder.Append("\":");

            if (indented)
            {
                builder.Append(" ");
            }
        }

        private static void AppendObjectStart(StringBuilder builder, int indent, bool indented)
        {
            AppendIndent(builder, indent, indented);
            builder.Append("{");
            NewLine(builder, indented);
        }

        private static void AppendObjectEnd(StringBuilder builder, int indent, bool indented)
        {
            AppendIndent(builder, indent, indented);
            builder.Append("}");
        }

        private static void AppendIndent(StringBuilder builder, int indent, bool indented)
        {
            if (!indented)
            {
                return;
            }

            for (var i = 0; i < indent * 2; i++)
            {
                builder.Append(' ');
            }
        }

        private static void NewLine(StringBuilder builder, bool indented)
        {
            if (indented)
            {
                builder.AppendLine();
            }
        }

        private static string NullableInt(int? value) => value.HasValue ? value.Value.ToString() : "null";

        private static string Escape(string value)
        {
            if (value == null)
            {
                return "null";
            }

            var builder = new StringBuilder();
            builder.Append("\"");

            for (var i = 0; i < value.Length; i++)
            {
                switch (value[i])
                {
                    case '\\':
                        builder.Append("\\\\");
                        break;
                    case '"':
                        builder.Append("\\\"");
                        break;
                    case '\n':
                        builder.Append("\\n");
                        break;
                    case '\r':
                        builder.Append("\\r");
                        break;
                    case '\t':
                        builder.Append("\\t");
                        break;
                    default:
                        builder.Append(value[i]);
                        break;
                }
            }

            builder.Append("\"");
            return builder.ToString();
        }
    }
}
