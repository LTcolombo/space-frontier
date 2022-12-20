﻿using Avatar;
using CityBuilding;
using xNode_1._8._0.Scripts;

namespace Nodes
{
    [NodeWidth(150)]
    [NodeTint(0.4f, 0.2f, 0.2f)]
    public class KarmaNode : Node, IDialogueNode
    {
        [Input(ShowBackingValue.Never)] 
        public string @in;

        public int value;

        public void Trigger()
        {
            AvatarModel.Instance.UpdateAttributeById("karma", value);
        }
    }
}