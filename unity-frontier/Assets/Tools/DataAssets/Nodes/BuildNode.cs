using CityBuilding;
using xNode_1._8._0.Scripts;

namespace Nodes
{
    public class BuildNode : Node, IDialogueNode
    {
        [Input(ShowBackingValue.Never)] 
        public string @in;

        public BuildingType building;

        public void Trigger()
        {
            BuilderModel.Instance.StartPlacement(building);
        }
    }
}