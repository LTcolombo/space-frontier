using Avatar;
using CityBuilding;
using xNode_1._8._0.Scripts;

namespace Nodes
{
    [NodeWidth(150)]
    [NodeTint(0.2f, 0.2f, 0.4f)]
    public class DimeNode : Node, IDialogueNode
    {
        [Input(ShowBackingValue.Never)] 
        public string @in;

        public int value;

        public void Trigger()
        {
            FTModel.Instance.Apply(value);
        }
    }
}