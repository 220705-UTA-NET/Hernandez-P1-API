namespace SSBD.Models{
    public class MyOrder{
        public enum ITEM{
        ClassicSingleBurger,
        ClassicDoubleBurger,
        ClassicTripleBurger,
        SingleCheeseBurger,
        DoubleCheeseBurger,
        TripleCheeseBurger,
        VeggieBurger,
        DoubleBurger,
        ChickenBurger,
        ChickenFriedSteak,
        Fries,
        OnionRings,
        FriedChickenPieces,
        FriedCheeseSticks,
        FriedZucchini,
        CocaCola,
        Pepsi,
        Sprite,
        DrPepper,
        Lemonade
        }

        public int Id {get;set;}
        public string Name {get;set;}
        //public ITEM[] Item {get;set;}
        public string[] Items{get;set;}
        public string[] AddOn {get;set;}
        public MyOrder(){

        }
        public MyOrder(int id, string name, string[] items, string[] addons){
            this.Id = id;
            this.Name = name;
            this.Items = items;
            this.AddOn = addons;
        }
    }
}
