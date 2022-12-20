using UnityEngine;
using xNode_1._8._0.Scripts;

namespace Nodes
{
    [NodeWidth(150)]
    [NodeTint(0.2f, 0.4f, 0.4f)]
    public class LinkNode : Node, IDialogueNode
    {
        [Input(ShowBackingValue.Never)] 
        public string @in;

        public string value;

        public void Trigger()
        {
            Application.OpenURL(value);
        }
    }
}