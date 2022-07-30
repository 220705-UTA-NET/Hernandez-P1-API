namespace SSBD.API.Models{
public class Item{
        public string Name {get;set;}
        public decimal Price {get;set;}
        public Item(string name, decimal p){
            this.Name = name;
            this.Price = p;
        }
    }
}