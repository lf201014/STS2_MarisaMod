using BaseLib.Abstracts;
using Godot;

namespace MarisaMod.Scripts.PatchesNModels
{
    public class MarisaCardPool : CustomCardPoolModel
    {
        public override string Title => "marisa";

        public override string EnergyColorName => "defect";

        public override Color DeckEntryCardColor => new(0, 10, 125);

        public override bool IsColorless => false;
    }
}