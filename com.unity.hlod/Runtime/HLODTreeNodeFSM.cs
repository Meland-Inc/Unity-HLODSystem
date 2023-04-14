using State = Unity.HLODSystem.HLODTreeNode.State;

namespace Unity.HLODSystem
{
    class HLODTreeNodeFSM : FSM<State>
    {
        protected override bool Compare(State lhs, State rhs)
        {
            return lhs == rhs;
        }
    }
}