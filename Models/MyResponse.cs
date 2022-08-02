namespace SSBD.API{
    public class MyResponse{
        public string root_path {get;set;}= "/api/";
        //public string AllOrderPath = "/api/orders";
        public string order_search_path {get;set;}= "/api/getorder/{Id}";
        public string submit_order_path {get;set;}= "/api/submitorder";
        public string update_order {get;set;} = "/api/updateorder";

        public string search_orders {get;set;} = "/api/searchorder/name=name?&completed=completed";
        //public string status
        public Models.MyOrder? order {get;set;}
        public decimal total {get;set;}
        public string msg {get;set;}
        public MyResponse(Models.MyOrder order, decimal total, string msg){
            this.order = order;
            this.total = total;
            this.msg = msg;
        }
    }
}