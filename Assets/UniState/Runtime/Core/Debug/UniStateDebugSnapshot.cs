using System;
using System.Collections.Generic;

namespace UniState
{
    public sealed class UniStateDebugSnapshot
    {
        public long Sequence { get; set; }
        public IReadOnlyList<DebugMachineNode> Machines { get; set; } = Array.Empty<DebugMachineNode>();

        public static UniStateDebugSnapshot Empty() =>
            new UniStateDebugSnapshot
            {
                Sequence = 0,
                Machines = Array.Empty<DebugMachineNode>()
            };
    }
}
