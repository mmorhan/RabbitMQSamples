using PRC.RPCClient;

namespace RPC.RPCClient{
    public static class Rpc{
        public static void Main(string[] args){
            var rpcClient=new RPCClientClass();
            Console.WriteLine(" [x] Requesting fib(20)");
            var response=rpcClient.Call("20");
            Console.WriteLine( " [.] Got {0} ",response);
            rpcClient.Close();
            
        }
    }
}