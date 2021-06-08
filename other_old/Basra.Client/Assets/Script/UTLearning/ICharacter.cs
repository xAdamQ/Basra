namespace Basra.Client
{
    public interface ICharacter
    {
        int Health { get; set; }
        Inventory Inventory { get; set; }
        int Level { get; set; }

        void OnItemEquipped(Item item);
    }
}